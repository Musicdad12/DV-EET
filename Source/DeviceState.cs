using System;
using System.Reflection;
using DV.Customization.Gadgets;
using DV.Interaction;
using DV.VRTK_Extensions;
using VRTK;
using HarmonyLib;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace DV_EET
{
    // Stores power on the item, so its state follows it between held and mounted forms.
    public sealed class EetPowerState : MonoBehaviour
    {
        const string Key = "dv_eet_power";
        public bool IsOn { get; private set; }
        ItemSaveData save;
        void Awake()
        {
            save = GetComponent<ItemSaveData>();
            if (save != null) { save.ItemSaveDataRequested += Save; save.ItemSaveDataLoaded += Load; }
        }
        void OnDestroy()
        {
            if (save != null) { save.ItemSaveDataRequested -= Save; save.ItemSaveDataLoaded -= Load; }
        }
        JObject Save(JObject data) { data[Key] = IsOn; return data; }
        void Load(JObject data) { JToken token; IsOn = data.TryGetValue(Key, out token) && token.Value<bool>(); }
        public void SetPower(bool on) { IsOn = on; }
        public void Toggle() { IsOn = !IsOn; }
    }

    // Prototype marker and lamp driver for the mounted gadget. The switch connection
    // and lifecycle still require in-game verification.
    public sealed class MountedEet : MonoBehaviour
    {
        GadgetBase gadget;
        Light lamp;
        Material lensMaterial;
        Transform toggleLever;
        VRTK_InteractableObject_DV vrSwitch;
        GameObject arrow;
        Material arrowMaterial;
        Mesh arrowMesh;
        MapMarkersController map;
        Vector3 lastAbsolute;
        Vector3 travel = Vector3.forward;
        bool hasLast;
        float nextSample;
        static MethodInfo mapPosition = AccessTools.Method(typeof(MapMarkersController), "GetMapPosition");
        static MethodInfo absolutePosition = AccessTools.Method(AccessTools.TypeByName("DV.OriginShift.OriginShift"), "AbsolutePosition", new[] { typeof(Transform) });
        void Awake()
        {
            gadget = GetComponent<GadgetBase>();
            var holder = new GameObject("EET_MountedLight");
            holder.transform.SetParent(transform, false);
            // Gadget's visual child holds the original model coordinates.
            var visual = transform.Find("DV_EET_Visual");
            if (visual != null) holder.transform.SetParent(visual, false);
            holder.transform.localPosition = new Vector3(0f, .325f, -.11f);
            holder.transform.localRotation = Quaternion.Euler(0, 180, 0);
            lamp = holder.AddComponent<Light>(); lamp.type = LightType.Spot;
            lamp.color = Color.red; lamp.range = 18f; lamp.spotAngle = 85f; lamp.intensity = 2f;
            lamp.renderMode = LightRenderMode.ForcePixel;
            lamp.enabled = false;
            // The original OBJ is one combined mesh. EetModel supplies a separate
            // lens so its brightness can be controlled independently of the case.
            if (visual != null) foreach (var r in visual.GetComponentsInChildren<Renderer>(true))
            {
                if (r.name != "EET_RedLens") continue;
                var lensShader = Shader.Find("Unlit/Color");
                if (lensShader != null)
                {
                    lensMaterial = new Material(lensShader);
                    lensMaterial.color = new Color(.27f, .002f, .002f);
                    r.sharedMaterial = lensMaterial;
                }
                else
                {
                    lensMaterial = r.material;
                    lensMaterial.EnableKeyword("_EMISSION");
                }
                break;
            }
            // Use the same static interaction area approach as the game's non-VR
            // toggle switches. The trigger extends beyond the removal collider on
            // the front and right side so it can be hit from a useful angle.
            var parent = visual != null ? visual : transform;
            var switchObject = new GameObject("EET_ToggleSwitch");
            switchObject.transform.SetParent(parent, false);
            switchObject.transform.localPosition = new Vector3(.092f, .23f, 0f);
            int layer = LayerMask.NameToLayer("Interactable");
            if (layer >= 0) switchObject.layer = layer;
            var areaObject = new GameObject("EET_ToggleInteractionArea");
            areaObject.transform.SetParent(switchObject.transform, false);
            areaObject.transform.localPosition = new Vector3(.02f, .02f, -.065f);
            var areaCollider = areaObject.AddComponent<BoxCollider>();
            areaCollider.size = new Vector3(.14f, .12f, .23f);
            areaCollider.isTrigger = true;
            var handler = AGrabHandler.AddGrabHandler<GrabHandlerButton>(switchObject, new[] { areaObject });
            handler.Pressed += OnSwitchPressed;
            var area = areaObject.AddComponent<StaticInteractionArea>();
            area.Initialize(handler, layer >= 0 ? layer : switchObject.layer);
            // The desktop grab handler above is not used by VR. Give the same
            // switch its own VRTK use target, with priority above gadget removal.
            if (VRManager.IsVREnabled())
            {
                var vrTarget = new GameObject("EET_VRToggleTarget");
                vrTarget.transform.SetParent(switchObject.transform, false);
                vrTarget.transform.localPosition = new Vector3(.015f, 0f, -.025f);
                if (layer >= 0) vrTarget.layer = layer;
                var vrCollider = vrTarget.AddComponent<BoxCollider>();
                vrCollider.size = new Vector3(.075f, .10f, .10f);
                vrSwitch = vrTarget.AddComponent<VRTK_InteractableObject_DV>();
                vrSwitch.isGrabbable = false;
                vrSwitch.isUsable = true;
                vrSwitch.useOnlyIfGrabbed = false;
                vrSwitch.holdButtonToUse = false;
                vrSwitch.priority = 1;
                vrSwitch.pipaExclusiveInteraction = true;
                vrSwitch.interactionHandPoses = new InteractionHandPoses(HandPose.Point, HandPose.Point, HandPose.Point);
                vrSwitch.InteractableObjectUsed += OnVrSwitchUsed;
            }

            // Chrome lever with round mounting collar; up is on and down is off.
            EetModel.AddToggleAppearance(switchObject, false);
            toggleLever = switchObject.transform.Find("EET_ToggleLeverPivot");
        }
        void OnVrSwitchUsed(object sender, InteractableObjectEventArgs args)
        {
            Main.Entry.Logger.Log("DV_EET: VR switch used");
            ToggleSwitch();
        }
        void OnSwitchPressed(GrabHandlerButton handler)
        {
            ToggleSwitch();
        }
        void ToggleSwitch()
        {
            if (gadget == null || gadget.TrainCar == null) return;
            var item = gadget.GadgetItem;
            var power = item != null ? item.GetComponent<EetPowerState>() : null;
            if (power != null) power.Toggle();
        }
        void Update()
        {
            if (vrSwitch != null) vrSwitch.useOverrideButton = SetupDeviceSpecificControls.useOverrideButtonForButtonComponent;
            var item = gadget != null ? gadget.GadgetItem : null;
            var power = item != null ? item.GetComponent<EetPowerState>() : null;
            bool active = power != null && power.IsOn && gadget.TrainCar != null;
            if (toggleLever != null) toggleLever.localRotation = Quaternion.Euler(0f, 0f, active ? 35f : -35f);
            bool lit = active && Mathf.Repeat(Time.time, 1f) < .35f;
            if (lamp != null) lamp.enabled = lit;
            if (lensMaterial != null)
            {
                if (lensMaterial.HasProperty("_Color")) lensMaterial.color = lit ? Color.red : new Color(.27f, .002f, .002f);
                if (lensMaterial.HasProperty("_EmissionColor")) lensMaterial.SetColor("_EmissionColor", lit ? Color.red * 2f : Color.black);
            }
            if (!active) { HideMarker(); hasLast = false; return; }
            if (mapPosition == null || absolutePosition == null) { HideMarker(); return; }
            if (map == null) map = FindObjectOfType<MapMarkersController>();
            if (map == null) return;
            if (arrow == null && arrowMesh != null) HideMarker();
            if (arrow == null) CreateArrow();
            if (arrow == null) return;
            var position = (Vector3)absolutePosition.Invoke(null, new object[] { transform });
            if (Time.unscaledTime >= nextSample)
            {
                if (hasLast)
                {
                    Vector3 delta = position - lastAbsolute; delta.y = 0;
                    if (delta.sqrMagnitude > .0001f) travel = delta.normalized;
                }
                else { var f = gadget.TrainCar.transform.forward; f.y = 0; if (f.sqrMagnitude > .001f) travel = f.normalized; }
                lastAbsolute = position; hasLast = true; nextSample = Time.unscaledTime + .25f;
            }
            arrow.transform.localPosition = (Vector3)mapPosition.Invoke(map, new object[] { position, true });
            arrow.transform.localRotation = Quaternion.LookRotation(travel, Vector3.up);
        }
        void CreateArrow()
        {
            var boundsField = AccessTools.Field(typeof(MapMarkersController), "mapBounds");
            if (boundsField == null) return;
            var bounds = (Vector2)boundsField.GetValue(map);
            float size = Mathf.Max(.001f, Mathf.Min(Mathf.Abs(bounds.x), Mathf.Abs(bounds.y)) * .018f);
            arrow = new GameObject("DV_EET map arrow");
            arrow.transform.SetParent(map.transform, false);
            arrow.layer = map.gameObject.layer;
            arrowMesh = new Mesh();
            arrowMesh.vertices = new[] { new Vector3(0, 0, size), new Vector3(-size*.65f, 0, -size*.7f), new Vector3(size*.65f, 0, -size*.7f) };
            // Double-sided to work with either side of the in-game map.
            arrowMesh.triangles = new[] { 0, 1, 2, 2, 1, 0 };
            arrowMesh.RecalculateBounds();
            arrow.AddComponent<MeshFilter>().sharedMesh = arrowMesh;
            var shader = Shader.Find("Unlit/Color");
            if (shader == null) { HideMarker(); return; }
            arrowMaterial = new Material(shader); arrowMaterial.color = Color.red;
            arrow.AddComponent<MeshRenderer>().sharedMaterial = arrowMaterial;
        }
        void HideMarker()
        {
            if (arrow != null) Destroy(arrow);
            if (arrowMesh != null) Destroy(arrowMesh);
            if (arrowMaterial != null) Destroy(arrowMaterial);
            arrow = null; arrowMesh = null; arrowMaterial = null;
        }
        void OnDisable() { if (lamp != null) lamp.enabled = false; if (lensMaterial != null) { if (lensMaterial.HasProperty("_Color")) lensMaterial.color = new Color(.27f, .002f, .002f); if (lensMaterial.HasProperty("_EmissionColor")) lensMaterial.SetColor("_EmissionColor", Color.black); } HideMarker(); hasLast = false; }
        void OnDestroy() { if (vrSwitch != null) vrSwitch.InteractableObjectUsed -= OnVrSwitchUsed; HideMarker(); if (lensMaterial != null) Destroy(lensMaterial); }
    }
}











