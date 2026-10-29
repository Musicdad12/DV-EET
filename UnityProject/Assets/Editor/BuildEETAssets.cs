using System.IO;
using UnityEditor;
using UnityEngine;

public static class BuildEETAssets
{
    [MenuItem("DV_EET/Build Windows VR Assets")]
    public static void BuildVR()
    {
        PlayerSettings.virtualRealitySupported = true;
        PlayerSettings.stereoRenderingPath = StereoRenderingPath.SinglePass;
        PlayerSettings.SetVirtualRealitySDKs(BuildTargetGroup.Standalone, new[] { "OpenVR" });
        PlayerSettings.colorSpace = ColorSpace.Linear;
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        Build();
        EetAppearanceValidation.Validate();
        Debug.Log("DV_EET VR asset build completed: VR enabled, single-pass stereo, OpenVR, linear color.");
    }
    [MenuItem("DV_EET/Build Windows Assets")]
    public static void Build()
    {
        const string root = "Assets/DV_EET_Generated";
        Directory.CreateDirectory(root);
        AssetDatabase.Refresh();
        var model = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/DV_EET.obj");
        if (!model) throw new System.Exception("OBJ did not import");
        var instance = (GameObject)PrefabUtility.InstantiatePrefab(model);
        instance.name = "DV_EET";
        var collider = instance.AddComponent<BoxCollider>();
        collider.center = new Vector3(0f, 0.23f, 0f);
        collider.size = new Vector3(0.22f, 0.47f, 0.19f);
        // The runtime mod must supply the interaction and power-state logic.
        var lightObject = new GameObject("EET_RedLight");
        lightObject.transform.SetParent(instance.transform, false);
        lightObject.transform.localPosition = new Vector3(0f, 0.25f, -0.105f);
        lightObject.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
        var lamp = lightObject.AddComponent<Light>();
        lamp.type = LightType.Spot;
        lamp.color = Color.red;
        lamp.range = 18f;
        lamp.spotAngle = 85f;
        lamp.intensity = 2f;
        lamp.enabled = false;
        var switchTarget = new GameObject("EET_SwitchTarget");
        switchTarget.transform.SetParent(instance.transform, false);
        switchTarget.transform.localPosition = new Vector3(0.145f, 0.197f, 0f);
        var switchCollider = switchTarget.AddComponent<BoxCollider>();
        switchCollider.size = new Vector3(0.045f, 0.07f, 0.05f);
        var prefab = PrefabUtility.SaveAsPrefabAsset(instance, root + "/DV_EET.prefab");
        Object.DestroyImmediate(instance);
        if (!prefab) throw new System.Exception("Prefab creation failed");
        CreateIcon(root + "/EET_Icon.png", false);
        CreateIcon(root + "/EET_Dropped.png", true);
        AssetDatabase.Refresh();
        foreach (var name in new[] {"EET_Icon.png", "EET_Dropped.png"})
        {
            var importer = (TextureImporter)AssetImporter.GetAtPath(root + "/" + name);
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.SaveAndReimport();
        }
        foreach (var path in new[] {root + "/DV_EET.prefab", root + "/EET_Icon.png", root + "/EET_Dropped.png"})
        {
            var importer = AssetImporter.GetAtPath(path);
            importer.assetBundleName = "dv_eet_assets";
        }
        AssetDatabase.SaveAssets();
        Directory.CreateDirectory("Build");
        var manifest = BuildPipeline.BuildAssetBundles("Build", BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);
        if (!manifest || !File.Exists("Build/dv_eet_assets")) throw new System.Exception("Asset bundle build failed");
        Debug.Log("DV_EET bundle built successfully");
    }
    static void CreateIcon(string path, bool dropped)
    {
        const int n = 256;
        var t = new Texture2D(n,n,TextureFormat.RGBA32,false);
        var clear = new Color(0,0,0,0);
        var body = dropped ? new Color(.42f,.64f,.7f) : new Color(.91f,.72f,.075f);
        var dark = dropped ? body : new Color(.65f,.68f,.72f);
        var red = dropped ? body : new Color(.8f,.015f,.015f);
        for (int y=0;y<n;y++) for(int x=0;x<n;x++) {
            Color c=clear;
            if(x>=91 && x<165 && y>=51 && y<209) c=body;
            if(x>=84 && x<172 && y>=200 && y<213) c=body;
            if(x>=98 && x<158 && y>=24 && y<51) c=dark;
            if(x>=120 && x<136 && y>=213 && y<231) c=dark;
            if(x>=82 && x<174 && y>=150 && y<195) c=body;
            if(x>=96 && x<160 && y>=160 && y<186) c=red;
            if(x>=100 && x<156 && y>=75 && y<132) c=red;
            if(x>=165 && x<189 && y>=108 && y<114) c=dark;
            t.SetPixel(x,y,c);
        }
        t.Apply(); File.WriteAllBytes(path,t.EncodeToPNG()); Object.DestroyImmediate(t);
    }
}




