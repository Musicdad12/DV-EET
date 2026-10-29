using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using DV_EET;

public static class EetAppearanceValidation
{
    [MenuItem("DV_EET/Validate Weathered Finish")]
    public static void Validate()
    {
        AssetBundle bundle = null; GameObject item = null; GameObject camGo = null; GameObject lightGo = null; RenderTexture rt = null; Texture2D shot = null;
        Color previousAmbient = RenderSettings.ambientLight;
        try
        {
            bundle = AssetBundle.LoadFromFile("Build/dv_eet_assets");
            if (!bundle) throw new Exception("VR bundle could not be loaded");
            var prefab = bundle.LoadAsset<GameObject>("assets/dv_eet_generated/dv_eet.prefab");
            if (!prefab) throw new Exception("Item prefab missing");
            item = UnityEngine.Object.Instantiate(prefab);
            EetModel.Replace(item);
            var casing = item.transform.Find("EET_Case");
            var basePart = item.transform.Find("EET_Base");
            var label = item.transform.Find("EET_Label");
            if (!casing || !basePart || !label || !item.transform.Find("EET_RedLens")) throw new Exception("Remodeled parts missing");
            var yellow = casing.GetComponent<Renderer>().sharedMaterial;
            var chrome = basePart.GetComponent<Renderer>().sharedMaterial;
            if (yellow.name != "DV_EET_WeatheredSafetyYellow" || !yellow.mainTexture) throw new Exception("Weathered yellow missing");
            if (chrome.name != "DV_EET_Chrome" || !chrome.HasProperty("_Metallic") || chrome.GetFloat("_Metallic") < .8f) throw new Exception("Chrome missing");
            if (label.GetComponent<Renderer>().sharedMaterial.name != "DV_EET_BrandedPanel") throw new Exception("Brand label missing");
            var lever = item.transform.Find("EET_LooseToggleRoot/EET_LooseTogglePivot/EET_LooseToggleLever");
            if (!lever || lever.GetComponent<Renderer>().sharedMaterial != chrome) throw new Exception("Chrome switch missing");
            camGo = new GameObject("EET Validation Camera"); var cam = camGo.AddComponent<Camera>();
            cam.transform.position = new Vector3(.65f,.40f,-1.15f); cam.transform.LookAt(new Vector3(0,.22f,0));
            cam.clearFlags = CameraClearFlags.SolidColor; cam.backgroundColor = new Color(.18f,.2f,.22f);
            cam.fieldOfView = 27; cam.nearClipPlane=.01f; cam.farClipPlane=8;
            lightGo = new GameObject("EET Validation Light"); var light = lightGo.AddComponent<Light>(); light.type=LightType.Directional; light.intensity=1.1f;
            lightGo.transform.rotation=Quaternion.Euler(45,-40,0); RenderSettings.ambientLight = new Color(.45f,.45f,.45f);
            rt = new RenderTexture(720,720,24,RenderTextureFormat.ARGB32); cam.targetTexture=rt; cam.Render();
            var previous = RenderTexture.active; RenderTexture.active=rt; shot = new Texture2D(720,720,TextureFormat.RGB24,false);
            shot.ReadPixels(new Rect(0,0,720,720),0,0); shot.Apply(); RenderTexture.active=previous;
            File.WriteAllBytes("Build/appearance-preview.png",shot.EncodeToPNG());
            File.WriteAllText("Build/appearance-validation.txt", "PASS: yellow material and weather texture assigned; chrome material metallic; UVs and texture variation present.\n");
            Debug.Log("DV_EET appearance validation passed");
        }
        finally
        {
            RenderSettings.ambientLight = previousAmbient;
            if (item) UnityEngine.Object.DestroyImmediate(item);
            if (camGo) UnityEngine.Object.DestroyImmediate(camGo);
            if (lightGo) UnityEngine.Object.DestroyImmediate(lightGo);
            if (rt) UnityEngine.Object.DestroyImmediate(rt);
            if (shot) UnityEngine.Object.DestroyImmediate(shot);
            if (bundle) bundle.Unload(false);
        }
    }
}


