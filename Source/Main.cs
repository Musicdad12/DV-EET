using System;
using System.Linq;
using System.Reflection;
using custom_item_mod;
using DV.Customization;
using DV.Customization.Gadgets;
using HarmonyLib;
using UnityEngine;
using UnityModManagerNet;

[assembly: AssemblyVersion("1.1.1.0")]
[assembly: AssemblyFileVersion("1.1.1.0")]

namespace DV_EET
{
    public static class Main
    {
        internal static UnityModManager.ModEntry Entry;
        internal static Harmony Patches;
        public static bool Load(UnityModManager.ModEntry entry)
        {
            Entry = entry;
            try { Patches = new Harmony(entry.Info.Id); Patches.PatchAll(Assembly.GetExecutingAssembly()); UnloadWatcher.UnloadRequested += AttachGadgetPatch.Reset; return true; }
            catch (Exception e) { entry.Logger.LogException("DV_EET failed to load", e); return false; }
        }
    }

    // A successful removal returns the item. Shut down power only when removal
    // succeeded, not when the game refuses to remove an obstructed gadget.
    [HarmonyPatch(typeof(GadgetBase), "Remove")]
    internal static class PowerOffOnRemoval
    {
        [HarmonyPostfix]
        static void Postfix(GadgetItem __result)
        {
            if (__result == null) return;
            var power = __result.GetComponent<EetPowerState>();
            if (power != null) power.SetPower(false);
        }
    }

    // Adapted from ZG2's item/gadget factory (Fuggschen, MIT; see ZG2-LICENSE.txt).
    [HarmonyPatch]
    internal static class AttachGadgetPatch
    {
        static readonly System.Collections.Generic.List<GameObject> roots = new System.Collections.Generic.List<GameObject>();
        internal static void Reset() { foreach (var root in roots) if (root != null) UnityEngine.Object.Destroy(root); roots.Clear(); }
        static MethodBase TargetMethod() { return AccessTools.Method(typeof(DV.Shops.GlobalShopController), "Awake"); }
        [HarmonyPrefix, HarmonyPriority(200), HarmonyAfter("custom_item_mod")]
        static void Prefix(DV.Shops.GlobalShopController __instance)
        {
            foreach (var item in ItemModsFinder.CustomItems)
            {
                // Keep the original prefab identity so purchased items and saved
                // gadgets continue to resolve; only the localized display name changes.
                if (item.ItemSpec.ItemPrefabName != "custom_item_mod/DV_EET/prefab") continue;
                item.Name = "DV EOTD";
                var harbor = __instance.globalShopList.FirstOrDefault(shop => shop != null && shop.name == "[ItemShop] Harbor");
                item.ShopData.soldOnlyAt = new System.Collections.Generic.List<Shop> { harbor };
                if (harbor == null) Main.Entry.Logger.Warning("DV_EET: Harbor shop not found; item will not be stocked anywhere");
                try
                {
                    var standard = Shader.Find("Standard");
                    Main.Entry.Logger.Log("DV_EET: game Standard shader " + (standard == null ? "not found" : (standard.name + ", supported=" + standard.isSupported)));
                    AdjustShelf(item); Setup(item.ItemPrefab);
                }
                catch (Exception e) { Main.Entry.Logger.LogException("DV_EET attachment setup failed", e); }
            }
        }
        static void AdjustShelf(CustomItem item)
        {
            // custom_item_mod creates a display sample as the first shelf-item child.
            // Move the sample, not the scan tag or the item prefab.
            var shelf = item.ShopData.shelfItem.transform;
            if (shelf.childCount == 0) { Main.Entry.Logger.Warning("DV_EET shelf sample not found"); return; }
            var sample = shelf.GetChild(0);
            EetModel.Replace(sample.gameObject);
            sample.localPosition += new Vector3(0f, 0f, -0.15f);
        }
        static void Setup(GameObject item)
        {
            if (item.GetComponent<GadgetItem>() != null) return;
            EetModel.Replace(item);
            if (item.GetComponent<EetPowerState>() == null) item.AddComponent<EetPowerState>();
            // Keep the template activeSelf=true under an inactive parent. Unity then
            // invokes Awake on the unparented clone before GadgetItem assigns it.
            // In the previous build the template itself was inactive; the clone's
            // Awake was delayed, leaving its save and placement internals uninitialized.
            var templateHolder = new GameObject("[DV_EET Template Holder]");
            roots.Add(templateHolder);
            UnityEngine.Object.DontDestroyOnLoad(templateHolder);
            templateHolder.SetActive(false);
            var root = new GameObject("DV_EET_Gadget");
            root.transform.SetParent(templateHolder.transform, false);
            var gadget = root.AddComponent<GadgetBase>();
            root.AddComponent<MountedEet>();
            Set(gadget, "removalMethod", GadgetBase.GadgetRemovalMethod.EmptyHand);
            Set(gadget, "requiredMountPoints", 0);
            Set(gadget, "vrIgnoreColliders", new Collider[0]);
            Set(gadget, "reservationGroupID", "DV_EET");
            Set(gadget, "requireSoldering", false);
            var requirements = new TrainCarCustomization.TrainCarCustomizerBase.TrainCarRequirements
            {
                electricsFuse = false,
                baseControls = false,
                trainCarPresence = TrainCarCustomization.TrainCarCustomizerBase.CustomizerTrainCarRequirements.None,
                simPorts = new TrainCarCustomization.TrainCarCustomizerBase.TrainCarRequirements.PortRequirement[0]
            };
            Set(gadget, "requirements", requirements);
            Set(gadget, "boundsCenter", new Vector3(0f, 0.23f, 0f));
            Set(gadget, "boundsSize", new Vector3(.22f, .47f, .19f));
            var visual = new GameObject("DV_EET_Visual");
            visual.transform.SetParent(root.transform, false);
            // The game maps gadget +Z into the surface; the model back is +Z.
            // Identity keeps +Y up and the front lamp facing away from the car.
            visual.transform.localRotation = Quaternion.identity;
            var filters = item.GetComponentsInChildren<MeshFilter>(true);
            var highlights = new System.Collections.Generic.List<MeshFilter>();
            foreach (var f in filters)
            {
                // Replace the old fixed switch with the animated toggle lever.
                if (string.Equals(f.name, "side_switch", StringComparison.OrdinalIgnoreCase) ||
                    f.name.StartsWith("EET_LooseToggle", StringComparison.Ordinal)) continue;
                var renderer = f.GetComponent<MeshRenderer>();
                if (renderer == null || f.sharedMesh == null) continue;
                var go = new GameObject(f.name);
                go.transform.SetParent(visual.transform, false);
                // The imported model has a hierarchy. Preserve transforms relative to item root.
                go.transform.localPosition = item.transform.InverseTransformPoint(f.transform.position);
                go.transform.localRotation = Quaternion.Inverse(item.transform.rotation) * f.transform.rotation;
                go.transform.localScale = f.transform.lossyScale;
                var mf = go.AddComponent<MeshFilter>(); mf.sharedMesh = f.sharedMesh;
                go.AddComponent<MeshRenderer>().sharedMaterials = renderer.sharedMaterials;
                highlights.Add(mf);
            }
            Set(gadget, "highlightMeshes", highlights.ToArray());
            var collider = visual.AddComponent<BoxCollider>();
            collider.center = new Vector3(0f,.23f,0f); collider.size = new Vector3(.22f,.47f,.19f);
            var layer = LayerMask.NameToLayer("Interactable");
            if (layer >= 0) SetLayer(root, layer);
            Set(item.AddComponent<GadgetItem>(), "gadgetPrefab", gadget);
        }
        static void Set(object target, string fieldName, object value)
        {
            var field = AccessTools.Field(target.GetType(), fieldName);
            if (field == null) throw new MissingFieldException(target.GetType().FullName, fieldName);
            field.SetValue(target, value);
        }
        static void SetLayer(GameObject go, int layer)
        {
            go.layer = layer;
            foreach (Transform child in go.transform) SetLayer(child.gameObject, layer);
        }
    }
}











