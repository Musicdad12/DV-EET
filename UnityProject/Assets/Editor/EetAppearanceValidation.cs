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
            var mesh = item.GetComponentInChildren<MeshFilter>();
            var mats = mesh.GetComponent<MeshRenderer>().sharedMaterials;
            if (mats.Length != 4 || mats[0].name != "DV_EET_WeatheredSafetyYellow" || mats[1].name != "DV_EET_Chrome") throw new Exception("Incorrect material assignments");
            if (mesh.sharedMesh.uv.Length != mesh.sharedMesh.vertexCount) throw new Exception("Missing casing UV coordinates");
            if (!mats[0].mainTexture || mats[0].mainTexture.name != "DV_EET_WeatheredYellow") throw new Exception("Weathering texture missing");
            if (!mats[1].HasProperty("_Metallic") || mats[1].GetFloat("_Metallic") < .8f) throw new Exception("Chrome metallic setting missing");
            bool foundLooseLever = false;
            foreach (var r in item.GetComponentsInChildren<Renderer>(true))
                if (r.name == "EET_LooseToggleLever") { foundLooseLever = true; if (r.sharedMaterial != mats[1]) throw new Exception("Loose toggle is not chrome"); }
            if (!foundLooseLever) throw new Exception("Loose toggle lever missing");
            if (mats[3].name != "DV_EET_BrandedPanel" || !mats[3].mainTexture || mats[3].mainTexture.name != "DV_EET_DerailValleyGadgets_Label") throw new Exception("Panel lettering texture missing");
            var panelUV = mesh.sharedMesh.uv;
            if (panelUV[56].x < .99f || panelUV[57].x > .01f || panelUV[58].y < .99f) throw new Exception("Panel UV mapping incorrect");
            var texture = mats[0].mainTexture as Texture2D;
            if (texture.GetPixel(24,32) == texture.GetPixel(80,200)) throw new Exception("Weather texture lacks variation");
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


