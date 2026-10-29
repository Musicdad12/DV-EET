using System;
using UnityEngine;

namespace DV_EET
{
    // Original model with the old knob removed and rounded chrome hardware.
    internal static class EetModel
    {
        static Mesh body;
        static Mesh lens;
        static Material yellowMaterial;
        static Material reflectorMaterial;
        internal static Material Chrome { get; private set; }
        static Texture2D weatherTexture;
        static Material labelMaterial;
        static Texture2D labelTexture;
        // Embedded label: Derail Valley Gadgets.
        const string LabelPng = "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";
        static readonly Vector3[] Vertices = {
            new Vector3(0.1250000f,0.0400000f,-0.0650000f),
            new Vector3(-0.1250000f,0.0400000f,-0.0650000f),
            new Vector3(-0.1250000f,0.3600000f,-0.0650000f),
            new Vector3(0.1250000f,0.3600000f,-0.0650000f),
            new Vector3(0.1250000f,0.0400000f,0.0650000f),
            new Vector3(-0.1250000f,0.0400000f,0.0650000f),
            new Vector3(-0.1250000f,0.3600000f,0.0650000f),
            new Vector3(0.1250000f,0.3600000f,0.0650000f),
            new Vector3(0.1275000f,0.3500000f,-0.0700000f),
            new Vector3(-0.1275000f,0.3500000f,-0.0700000f),
            new Vector3(-0.1275000f,0.3900000f,-0.0700000f),
            new Vector3(0.1275000f,0.3900000f,-0.0700000f),
            new Vector3(0.1275000f,0.3500000f,0.0700000f),
            new Vector3(-0.1275000f,0.3500000f,0.0700000f),
            new Vector3(-0.1275000f,0.3900000f,0.0700000f),
            new Vector3(0.1275000f,0.3900000f,0.0700000f),
            new Vector3(0.1375000f,0.0000000f,-0.0700000f),
            new Vector3(-0.1375000f,0.0000000f,-0.0700000f),
            new Vector3(-0.1375000f,0.0700000f,-0.0700000f),
            new Vector3(0.1375000f,0.0700000f,-0.0700000f),
            new Vector3(0.1375000f,0.0000000f,0.0800000f),
            new Vector3(-0.1375000f,0.0000000f,0.0800000f),
            new Vector3(-0.1375000f,0.0700000f,0.0800000f),
            new Vector3(0.1375000f,0.0700000f,0.0800000f),
            new Vector3(0.0910000f,0.3800000f,-0.0150000f),
            new Vector3(0.0690000f,0.3800000f,-0.0150000f),
            new Vector3(0.0690000f,0.4500000f,-0.0150000f),
            new Vector3(0.0910000f,0.4500000f,-0.0150000f),
            new Vector3(0.0910000f,0.3800000f,0.0150000f),
            new Vector3(0.0690000f,0.3800000f,0.0150000f),
            new Vector3(0.0690000f,0.4500000f,0.0150000f),
            new Vector3(0.0910000f,0.4500000f,0.0150000f),
            new Vector3(-0.0690000f,0.3800000f,-0.0150000f),
            new Vector3(-0.0910000f,0.3800000f,-0.0150000f),
            new Vector3(-0.0910000f,0.4500000f,-0.0150000f),
            new Vector3(-0.0690000f,0.4500000f,-0.0150000f),
            new Vector3(-0.0690000f,0.3800000f,0.0150000f),
            new Vector3(-0.0910000f,0.3800000f,0.0150000f),
            new Vector3(-0.0910000f,0.4500000f,0.0150000f),
            new Vector3(-0.0690000f,0.4500000f,0.0150000f),
            new Vector3(0.0900000f,0.4375000f,-0.0150000f),
            new Vector3(-0.0900000f,0.4375000f,-0.0150000f),
            new Vector3(-0.0900000f,0.4625000f,-0.0150000f),
            new Vector3(0.0900000f,0.4625000f,-0.0150000f),
            new Vector3(0.0900000f,0.4375000f,0.0150000f),
            new Vector3(-0.0900000f,0.4375000f,0.0150000f),
            new Vector3(-0.0900000f,0.4625000f,0.0150000f),
            new Vector3(0.0900000f,0.4625000f,0.0150000f),
            new Vector3(0.0900000f,0.1000000f,0.0665000f),
            new Vector3(-0.0900000f,0.1000000f,0.0665000f),
            new Vector3(-0.0900000f,0.2400000f,0.0665000f),
            new Vector3(0.0900000f,0.2400000f,0.0665000f),
            new Vector3(0.0900000f,0.1000000f,0.0915000f),
            new Vector3(-0.0900000f,0.1000000f,0.0915000f),
            new Vector3(-0.0900000f,0.2400000f,0.0915000f),
            new Vector3(0.0900000f,0.2400000f,0.0915000f),
            new Vector3(0.1100000f,0.0875000f,-0.0700000f),
            new Vector3(-0.1100000f,0.0875000f,-0.0700000f),
            new Vector3(-0.1100000f,0.1125000f,-0.0700000f),
            new Vector3(0.1100000f,0.1125000f,-0.0700000f),
            new Vector3(0.1100000f,0.0875000f,-0.0660000f),
            new Vector3(-0.1100000f,0.0875000f,-0.0660000f),
            new Vector3(-0.1100000f,0.1125000f,-0.0660000f),
            new Vector3(0.1100000f,0.1125000f,-0.0660000f),
            new Vector3(-0.0750000f,0.2500000f,-0.0820000f),
            new Vector3(-0.0735590f,0.2646320f,-0.0820000f),
            new Vector3(-0.0692910f,0.2787010f,-0.0820000f),
            new Vector3(-0.0623600f,0.2916680f,-0.0820000f),
            new Vector3(-0.0530330f,0.3030330f,-0.0820000f),
            new Vector3(-0.0416680f,0.3123600f,-0.0820000f),
            new Vector3(-0.0287010f,0.3192910f,-0.0820000f),
            new Vector3(-0.0146320f,0.3235590f,-0.0820000f),
            new Vector3(-0.0000000f,0.3250000f,-0.0820000f),
            new Vector3(0.0146320f,0.3235590f,-0.0820000f),
            new Vector3(0.0287010f,0.3192910f,-0.0820000f),
            new Vector3(0.0416680f,0.3123600f,-0.0820000f),
            new Vector3(0.0530330f,0.3030330f,-0.0820000f),
            new Vector3(0.0623600f,0.2916680f,-0.0820000f),
            new Vector3(0.0692910f,0.2787010f,-0.0820000f),
            new Vector3(0.0735590f,0.2646320f,-0.0820000f),
            new Vector3(0.0750000f,0.2500000f,-0.0820000f),
            new Vector3(0.0735590f,0.2353680f,-0.0820000f),
            new Vector3(0.0692910f,0.2212990f,-0.0820000f),
            new Vector3(0.0623600f,0.2083320f,-0.0820000f),
            new Vector3(0.0530330f,0.1969670f,-0.0820000f),
            new Vector3(0.0416680f,0.1876400f,-0.0820000f),
            new Vector3(0.0287010f,0.1807090f,-0.0820000f),
            new Vector3(0.0146320f,0.1764410f,-0.0820000f),
            new Vector3(0.0000000f,0.1750000f,-0.0820000f),
            new Vector3(-0.0146320f,0.1764410f,-0.0820000f),
            new Vector3(-0.0287010f,0.1807090f,-0.0820000f),
            new Vector3(-0.0416680f,0.1876400f,-0.0820000f),
            new Vector3(-0.0530330f,0.1969670f,-0.0820000f),
            new Vector3(-0.0623600f,0.2083320f,-0.0820000f),
            new Vector3(-0.0692910f,0.2212990f,-0.0820000f),
            new Vector3(-0.0735590f,0.2353680f,-0.0820000f),
            new Vector3(-0.0750000f,0.2500000f,-0.0640000f),
            new Vector3(-0.0735590f,0.2646320f,-0.0640000f),
            new Vector3(-0.0692910f,0.2787010f,-0.0640000f),
            new Vector3(-0.0623600f,0.2916680f,-0.0640000f),
            new Vector3(-0.0530330f,0.3030330f,-0.0640000f),
            new Vector3(-0.0416680f,0.3123600f,-0.0640000f),
            new Vector3(-0.0287010f,0.3192910f,-0.0640000f),
            new Vector3(-0.0146320f,0.3235590f,-0.0640000f),
            new Vector3(-0.0000000f,0.3250000f,-0.0640000f),
            new Vector3(0.0146320f,0.3235590f,-0.0640000f),
            new Vector3(0.0287010f,0.3192910f,-0.0640000f),
            new Vector3(0.0416680f,0.3123600f,-0.0640000f),
            new Vector3(0.0530330f,0.3030330f,-0.0640000f),
            new Vector3(0.0623600f,0.2916680f,-0.0640000f),
            new Vector3(0.0692910f,0.2787010f,-0.0640000f),
            new Vector3(0.0735590f,0.2646320f,-0.0640000f),
            new Vector3(0.0750000f,0.2500000f,-0.0640000f),
            new Vector3(0.0735590f,0.2353680f,-0.0640000f),
            new Vector3(0.0692910f,0.2212990f,-0.0640000f),
            new Vector3(0.0623600f,0.2083320f,-0.0640000f),
            new Vector3(0.0530330f,0.1969670f,-0.0640000f),
            new Vector3(0.0416680f,0.1876400f,-0.0640000f),
            new Vector3(0.0287010f,0.1807090f,-0.0640000f),
            new Vector3(0.0146320f,0.1764410f,-0.0640000f),
            new Vector3(0.0000000f,0.1750000f,-0.0640000f),
            new Vector3(-0.0146320f,0.1764410f,-0.0640000f),
            new Vector3(-0.0287010f,0.1807090f,-0.0640000f),
            new Vector3(-0.0416680f,0.1876400f,-0.0640000f),
            new Vector3(-0.0530330f,0.1969670f,-0.0640000f),
            new Vector3(-0.0623600f,0.2083320f,-0.0640000f),
            new Vector3(-0.0692910f,0.2212990f,-0.0640000f),
            new Vector3(-0.0735590f,0.2353680f,-0.0640000f),
            new Vector3(-0.0610000f,0.2500000f,-0.0910000f),
            new Vector3(-0.0598280f,0.2619010f,-0.0910000f),
            new Vector3(-0.0563570f,0.2733440f,-0.0910000f),
            new Vector3(-0.0507200f,0.2838900f,-0.0910000f),
            new Vector3(-0.0431340f,0.2931340f,-0.0910000f),
            new Vector3(-0.0338900f,0.3007200f,-0.0910000f),
            new Vector3(-0.0233440f,0.3063570f,-0.0910000f),
            new Vector3(-0.0119010f,0.3098280f,-0.0910000f),
            new Vector3(-0.0000000f,0.3110000f,-0.0910000f),
            new Vector3(0.0119010f,0.3098280f,-0.0910000f),
            new Vector3(0.0233440f,0.3063570f,-0.0910000f),
            new Vector3(0.0338900f,0.3007200f,-0.0910000f),
            new Vector3(0.0431340f,0.2931340f,-0.0910000f),
            new Vector3(0.0507200f,0.2838900f,-0.0910000f),
            new Vector3(0.0563570f,0.2733440f,-0.0910000f),
            new Vector3(0.0598280f,0.2619010f,-0.0910000f),
            new Vector3(0.0610000f,0.2500000f,-0.0910000f),
            new Vector3(0.0598280f,0.2380990f,-0.0910000f),
            new Vector3(0.0563570f,0.2266560f,-0.0910000f),
            new Vector3(0.0507200f,0.2161100f,-0.0910000f),
            new Vector3(0.0431340f,0.2068660f,-0.0910000f),
            new Vector3(0.0338900f,0.1992800f,-0.0910000f),
            new Vector3(0.0233440f,0.1936430f,-0.0910000f),
            new Vector3(0.0119010f,0.1901720f,-0.0910000f),
            new Vector3(0.0000000f,0.1890000f,-0.0910000f),
            new Vector3(-0.0119010f,0.1901720f,-0.0910000f),
            new Vector3(-0.0233440f,0.1936430f,-0.0910000f),
            new Vector3(-0.0338900f,0.1992800f,-0.0910000f),
            new Vector3(-0.0431340f,0.2068660f,-0.0910000f),
            new Vector3(-0.0507200f,0.2161100f,-0.0910000f),
            new Vector3(-0.0563570f,0.2266560f,-0.0910000f),
            new Vector3(-0.0598280f,0.2380990f,-0.0910000f),
            new Vector3(-0.0610000f,0.2500000f,-0.0790000f),
            new Vector3(-0.0598280f,0.2619010f,-0.0790000f),
            new Vector3(-0.0563570f,0.2733440f,-0.0790000f),
            new Vector3(-0.0507200f,0.2838900f,-0.0790000f),
            new Vector3(-0.0431340f,0.2931340f,-0.0790000f),
            new Vector3(-0.0338900f,0.3007200f,-0.0790000f),
            new Vector3(-0.0233440f,0.3063570f,-0.0790000f),
            new Vector3(-0.0119010f,0.3098280f,-0.0790000f),
            new Vector3(-0.0000000f,0.3110000f,-0.0790000f),
            new Vector3(0.0119010f,0.3098280f,-0.0790000f),
            new Vector3(0.0233440f,0.3063570f,-0.0790000f),
            new Vector3(0.0338900f,0.3007200f,-0.0790000f),
            new Vector3(0.0431340f,0.2931340f,-0.0790000f),
            new Vector3(0.0507200f,0.2838900f,-0.0790000f),
            new Vector3(0.0563570f,0.2733440f,-0.0790000f),
            new Vector3(0.0598280f,0.2619010f,-0.0790000f),
            new Vector3(0.0610000f,0.2500000f,-0.0790000f),
            new Vector3(0.0598280f,0.2380990f,-0.0790000f),
            new Vector3(0.0563570f,0.2266560f,-0.0790000f),
            new Vector3(0.0507200f,0.2161100f,-0.0790000f),
            new Vector3(0.0431340f,0.2068660f,-0.0790000f),
            new Vector3(0.0338900f,0.1992800f,-0.0790000f),
            new Vector3(0.0233440f,0.1936430f,-0.0790000f),
            new Vector3(0.0119010f,0.1901720f,-0.0790000f),
            new Vector3(0.0000000f,0.1890000f,-0.0790000f),
            new Vector3(-0.0119010f,0.1901720f,-0.0790000f),
            new Vector3(-0.0233440f,0.1936430f,-0.0790000f),
            new Vector3(-0.0338900f,0.1992800f,-0.0790000f),
            new Vector3(-0.0431340f,0.2068660f,-0.0790000f),
            new Vector3(-0.0507200f,0.2161100f,-0.0790000f),
            new Vector3(-0.0563570f,0.2266560f,-0.0790000f),
            new Vector3(-0.0598280f,0.2380990f,-0.0790000f),
            new Vector3(-0.1215000f,0.2140000f,0.0000000f),
            new Vector3(-0.1215000f,0.2121730f,0.0091840f),
            new Vector3(-0.1215000f,0.2069710f,0.0169710f),
            new Vector3(-0.1215000f,0.1991840f,0.0221730f),
            new Vector3(-0.1215000f,0.1900000f,0.0240000f),
            new Vector3(-0.1215000f,0.1808160f,0.0221730f),
            new Vector3(-0.1215000f,0.1730290f,0.0169710f),
            new Vector3(-0.1215000f,0.1678270f,0.0091840f),
            new Vector3(-0.1215000f,0.1660000f,0.0000000f),
            new Vector3(-0.1215000f,0.1678270f,-0.0091840f),
            new Vector3(-0.1215000f,0.1730290f,-0.0169710f),
            new Vector3(-0.1215000f,0.1808160f,-0.0221730f),
            new Vector3(-0.1215000f,0.1900000f,-0.0240000f),
            new Vector3(-0.1215000f,0.1991840f,-0.0221730f),
            new Vector3(-0.1215000f,0.2069710f,-0.0169710f),
            new Vector3(-0.1215000f,0.2121730f,-0.0091840f),
            new Vector3(-0.1365000f,0.2140000f,0.0000000f),
            new Vector3(-0.1365000f,0.2121730f,0.0091840f),
            new Vector3(-0.1365000f,0.2069710f,0.0169710f),
            new Vector3(-0.1365000f,0.1991840f,0.0221730f),
            new Vector3(-0.1365000f,0.1900000f,0.0240000f),
            new Vector3(-0.1365000f,0.1808160f,0.0221730f),
            new Vector3(-0.1365000f,0.1730290f,0.0169710f),
            new Vector3(-0.1365000f,0.1678270f,0.0091840f),
            new Vector3(-0.1365000f,0.1660000f,0.0000000f),
            new Vector3(-0.1365000f,0.1678270f,-0.0091840f),
            new Vector3(-0.1365000f,0.1730290f,-0.0169710f),
            new Vector3(-0.1365000f,0.1808160f,-0.0221730f),
            new Vector3(-0.1365000f,0.1900000f,-0.0240000f),
            new Vector3(-0.1365000f,0.1991840f,-0.0221730f),
            new Vector3(-0.1365000f,0.2069710f,-0.0169710f),
            new Vector3(-0.1365000f,0.2121730f,-0.0091840f),
            new Vector3(-0.1340000f,0.1770000f,-0.0060000f),
            new Vector3(-0.1560000f,0.1770000f,-0.0060000f),
            new Vector3(-0.1560000f,0.2170000f,-0.0060000f),
            new Vector3(-0.1340000f,0.2170000f,-0.0060000f),
            new Vector3(-0.1340000f,0.1770000f,0.0060000f),
            new Vector3(-0.1560000f,0.1770000f,0.0060000f),
            new Vector3(-0.1560000f,0.2170000f,0.0060000f),
            new Vector3(-0.1340000f,0.2170000f,0.0060000f),
            new Vector3(-0.1241188f,0.3533812f,-0.0666188f),
            new Vector3(-0.1251569f,0.3523431f,-0.0620000f),
            new Vector3(-0.1251569f,0.3523431f,0.0620000f),
            new Vector3(-0.1241188f,0.3533812f,0.0666188f),
            new Vector3(-0.1251569f,0.3580000f,-0.0676569f),
            new Vector3(-0.1275000f,0.3580000f,-0.0620000f),
            new Vector3(-0.1275000f,0.3580000f,0.0620000f),
            new Vector3(-0.1251569f,0.3580000f,0.0676569f),
            new Vector3(-0.1251569f,0.3820000f,-0.0676569f),
            new Vector3(-0.1275000f,0.3820000f,-0.0620000f),
            new Vector3(-0.1275000f,0.3820000f,0.0620000f),
            new Vector3(-0.1251569f,0.3820000f,0.0676569f),
            new Vector3(-0.1241188f,0.3866188f,-0.0666188f),
            new Vector3(-0.1251569f,0.3876569f,-0.0620000f),
            new Vector3(-0.1251569f,0.3876569f,0.0620000f),
            new Vector3(-0.1241188f,0.3866188f,0.0666188f),
            new Vector3(0.1241188f,0.3533812f,-0.0666188f),
            new Vector3(0.1251569f,0.3523431f,-0.0620000f),
            new Vector3(0.1251569f,0.3523431f,0.0620000f),
            new Vector3(0.1241188f,0.3533812f,0.0666188f),
            new Vector3(0.1251569f,0.3580000f,-0.0676569f),
            new Vector3(0.1275000f,0.3580000f,-0.0620000f),
            new Vector3(0.1275000f,0.3580000f,0.0620000f),
            new Vector3(0.1251569f,0.3580000f,0.0676569f),
            new Vector3(0.1251569f,0.3820000f,-0.0676569f),
            new Vector3(0.1275000f,0.3820000f,-0.0620000f),
            new Vector3(0.1275000f,0.3820000f,0.0620000f),
            new Vector3(0.1251569f,0.3820000f,0.0676569f),
            new Vector3(0.1241188f,0.3866188f,-0.0666188f),
            new Vector3(0.1251569f,0.3876569f,-0.0620000f),
            new Vector3(0.1251569f,0.3876569f,0.0620000f),
            new Vector3(0.1241188f,0.3866188f,0.0666188f),
            new Vector3(-0.1195000f,0.3523431f,-0.0676569f),
            new Vector3(-0.1195000f,0.3500000f,-0.0620000f),
            new Vector3(-0.1195000f,0.3500000f,0.0620000f),
            new Vector3(-0.1195000f,0.3523431f,0.0676569f),
            new Vector3(0.1195000f,0.3523431f,-0.0676569f),
            new Vector3(0.1195000f,0.3500000f,-0.0620000f),
            new Vector3(0.1195000f,0.3500000f,0.0620000f),
            new Vector3(0.1195000f,0.3523431f,0.0676569f),
            new Vector3(-0.1195000f,0.3876569f,-0.0676569f),
            new Vector3(-0.1195000f,0.3900000f,-0.0620000f),
            new Vector3(-0.1195000f,0.3900000f,0.0620000f),
            new Vector3(-0.1195000f,0.3876569f,0.0676569f),
            new Vector3(0.1195000f,0.3876569f,-0.0676569f),
            new Vector3(0.1195000f,0.3900000f,-0.0620000f),
            new Vector3(0.1195000f,0.3900000f,0.0620000f),
            new Vector3(0.1195000f,0.3876569f,0.0676569f),
            new Vector3(-0.1195000f,0.3580000f,-0.0700000f),
            new Vector3(-0.1195000f,0.3820000f,-0.0700000f),
            new Vector3(0.1195000f,0.3580000f,-0.0700000f),
            new Vector3(0.1195000f,0.3820000f,-0.0700000f),
            new Vector3(-0.1195000f,0.3580000f,0.0700000f),
            new Vector3(-0.1195000f,0.3820000f,0.0700000f),
            new Vector3(0.1195000f,0.3580000f,0.0700000f),
            new Vector3(0.1195000f,0.3820000f,0.0700000f),
            new Vector3(-0.1336962f,0.0038038f,-0.0661962f),
            new Vector3(-0.1348640f,0.0026360f,-0.0610000f),
            new Vector3(-0.1348640f,0.0026360f,0.0710000f),
            new Vector3(-0.1336962f,0.0038038f,0.0761962f),
            new Vector3(-0.1348640f,0.0090000f,-0.0673640f),
            new Vector3(-0.1375000f,0.0090000f,-0.0610000f),
            new Vector3(-0.1375000f,0.0090000f,0.0710000f),
            new Vector3(-0.1348640f,0.0090000f,0.0773640f),
            new Vector3(-0.1348640f,0.0610000f,-0.0673640f),
            new Vector3(-0.1375000f,0.0610000f,-0.0610000f),
            new Vector3(-0.1375000f,0.0610000f,0.0710000f),
            new Vector3(-0.1348640f,0.0610000f,0.0773640f),
            new Vector3(-0.1336962f,0.0661962f,-0.0661962f),
            new Vector3(-0.1348640f,0.0673640f,-0.0610000f),
            new Vector3(-0.1348640f,0.0673640f,0.0710000f),
            new Vector3(-0.1336962f,0.0661962f,0.0761962f),
            new Vector3(0.1336962f,0.0038038f,-0.0661962f),
            new Vector3(0.1348640f,0.0026360f,-0.0610000f),
            new Vector3(0.1348640f,0.0026360f,0.0710000f),
            new Vector3(0.1336962f,0.0038038f,0.0761962f),
            new Vector3(0.1348640f,0.0090000f,-0.0673640f),
            new Vector3(0.1375000f,0.0090000f,-0.0610000f),
            new Vector3(0.1375000f,0.0090000f,0.0710000f),
            new Vector3(0.1348640f,0.0090000f,0.0773640f),
            new Vector3(0.1348640f,0.0610000f,-0.0673640f),
            new Vector3(0.1375000f,0.0610000f,-0.0610000f),
            new Vector3(0.1375000f,0.0610000f,0.0710000f),
            new Vector3(0.1348640f,0.0610000f,0.0773640f),
            new Vector3(0.1336962f,0.0661962f,-0.0661962f),
            new Vector3(0.1348640f,0.0673640f,-0.0610000f),
            new Vector3(0.1348640f,0.0673640f,0.0710000f),
            new Vector3(0.1336962f,0.0661962f,0.0761962f),
            new Vector3(-0.1285000f,0.0026360f,-0.0673640f),
            new Vector3(-0.1285000f,0.0000000f,-0.0610000f),
            new Vector3(-0.1285000f,0.0000000f,0.0710000f),
            new Vector3(-0.1285000f,0.0026360f,0.0773640f),
            new Vector3(0.1285000f,0.0026360f,-0.0673640f),
            new Vector3(0.1285000f,0.0000000f,-0.0610000f),
            new Vector3(0.1285000f,0.0000000f,0.0710000f),
            new Vector3(0.1285000f,0.0026360f,0.0773640f),
            new Vector3(-0.1285000f,0.0673640f,-0.0673640f),
            new Vector3(-0.1285000f,0.0700000f,-0.0610000f),
            new Vector3(-0.1285000f,0.0700000f,0.0710000f),
            new Vector3(-0.1285000f,0.0673640f,0.0773640f),
            new Vector3(0.1285000f,0.0673640f,-0.0673640f),
            new Vector3(0.1285000f,0.0700000f,-0.0610000f),
            new Vector3(0.1285000f,0.0700000f,0.0710000f),
            new Vector3(0.1285000f,0.0673640f,0.0773640f),
            new Vector3(-0.1285000f,0.0090000f,-0.0700000f),
            new Vector3(-0.1285000f,0.0610000f,-0.0700000f),
            new Vector3(0.1285000f,0.0090000f,-0.0700000f),
            new Vector3(0.1285000f,0.0610000f,-0.0700000f),
            new Vector3(-0.1285000f,0.0090000f,0.0800000f),
            new Vector3(-0.1285000f,0.0610000f,0.0800000f),
            new Vector3(0.1285000f,0.0090000f,0.0800000f),
            new Vector3(0.1285000f,0.0610000f,0.0800000f),
            new Vector3(0.0706906f,0.3816906f,-0.0133094f),
            new Vector3(0.0701716f,0.3811716f,-0.0110000f),
            new Vector3(0.0701716f,0.3811716f,0.0110000f),
            new Vector3(0.0706906f,0.3816906f,0.0133094f),
            new Vector3(0.0701716f,0.3840000f,-0.0138284f),
            new Vector3(0.0690000f,0.3840000f,-0.0110000f),
            new Vector3(0.0690000f,0.3840000f,0.0110000f),
            new Vector3(0.0701716f,0.3840000f,0.0138284f),
            new Vector3(0.0701716f,0.4460000f,-0.0138284f),
            new Vector3(0.0690000f,0.4460000f,-0.0110000f),
            new Vector3(0.0690000f,0.4460000f,0.0110000f),
            new Vector3(0.0701716f,0.4460000f,0.0138284f),
            new Vector3(0.0706906f,0.4483094f,-0.0133094f),
            new Vector3(0.0701716f,0.4488284f,-0.0110000f),
            new Vector3(0.0701716f,0.4488284f,0.0110000f),
            new Vector3(0.0706906f,0.4483094f,0.0133094f),
            new Vector3(0.0893094f,0.3816906f,-0.0133094f),
            new Vector3(0.0898284f,0.3811716f,-0.0110000f),
            new Vector3(0.0898284f,0.3811716f,0.0110000f),
            new Vector3(0.0893094f,0.3816906f,0.0133094f),
            new Vector3(0.0898284f,0.3840000f,-0.0138284f),
            new Vector3(0.0910000f,0.3840000f,-0.0110000f),
            new Vector3(0.0910000f,0.3840000f,0.0110000f),
            new Vector3(0.0898284f,0.3840000f,0.0138284f),
            new Vector3(0.0898284f,0.4460000f,-0.0138284f),
            new Vector3(0.0910000f,0.4460000f,-0.0110000f),
            new Vector3(0.0910000f,0.4460000f,0.0110000f),
            new Vector3(0.0898284f,0.4460000f,0.0138284f),
            new Vector3(0.0893094f,0.4483094f,-0.0133094f),
            new Vector3(0.0898284f,0.4488284f,-0.0110000f),
            new Vector3(0.0898284f,0.4488284f,0.0110000f),
            new Vector3(0.0893094f,0.4483094f,0.0133094f),
            new Vector3(0.0730000f,0.3811716f,-0.0138284f),
            new Vector3(0.0730000f,0.3800000f,-0.0110000f),
            new Vector3(0.0730000f,0.3800000f,0.0110000f),
            new Vector3(0.0730000f,0.3811716f,0.0138284f),
            new Vector3(0.0870000f,0.3811716f,-0.0138284f),
            new Vector3(0.0870000f,0.3800000f,-0.0110000f),
            new Vector3(0.0870000f,0.3800000f,0.0110000f),
            new Vector3(0.0870000f,0.3811716f,0.0138284f),
            new Vector3(0.0730000f,0.4488284f,-0.0138284f),
            new Vector3(0.0730000f,0.4500000f,-0.0110000f),
            new Vector3(0.0730000f,0.4500000f,0.0110000f),
            new Vector3(0.0730000f,0.4488284f,0.0138284f),
            new Vector3(0.0870000f,0.4488284f,-0.0138284f),
            new Vector3(0.0870000f,0.4500000f,-0.0110000f),
            new Vector3(0.0870000f,0.4500000f,0.0110000f),
            new Vector3(0.0870000f,0.4488284f,0.0138284f),
            new Vector3(0.0730000f,0.3840000f,-0.0150000f),
            new Vector3(0.0730000f,0.4460000f,-0.0150000f),
            new Vector3(0.0870000f,0.3840000f,-0.0150000f),
            new Vector3(0.0870000f,0.4460000f,-0.0150000f),
            new Vector3(0.0730000f,0.3840000f,0.0150000f),
            new Vector3(0.0730000f,0.4460000f,0.0150000f),
            new Vector3(0.0870000f,0.3840000f,0.0150000f),
            new Vector3(0.0870000f,0.4460000f,0.0150000f),
            new Vector3(-0.0893094f,0.3816906f,-0.0133094f),
            new Vector3(-0.0898284f,0.3811716f,-0.0110000f),
            new Vector3(-0.0898284f,0.3811716f,0.0110000f),
            new Vector3(-0.0893094f,0.3816906f,0.0133094f),
            new Vector3(-0.0898284f,0.3840000f,-0.0138284f),
            new Vector3(-0.0910000f,0.3840000f,-0.0110000f),
            new Vector3(-0.0910000f,0.3840000f,0.0110000f),
            new Vector3(-0.0898284f,0.3840000f,0.0138284f),
            new Vector3(-0.0898284f,0.4460000f,-0.0138284f),
            new Vector3(-0.0910000f,0.4460000f,-0.0110000f),
            new Vector3(-0.0910000f,0.4460000f,0.0110000f),
            new Vector3(-0.0898284f,0.4460000f,0.0138284f),
            new Vector3(-0.0893094f,0.4483094f,-0.0133094f),
            new Vector3(-0.0898284f,0.4488284f,-0.0110000f),
            new Vector3(-0.0898284f,0.4488284f,0.0110000f),
            new Vector3(-0.0893094f,0.4483094f,0.0133094f),
            new Vector3(-0.0706906f,0.3816906f,-0.0133094f),
            new Vector3(-0.0701716f,0.3811716f,-0.0110000f),
            new Vector3(-0.0701716f,0.3811716f,0.0110000f),
            new Vector3(-0.0706906f,0.3816906f,0.0133094f),
            new Vector3(-0.0701716f,0.3840000f,-0.0138284f),
            new Vector3(-0.0690000f,0.3840000f,-0.0110000f),
            new Vector3(-0.0690000f,0.3840000f,0.0110000f),
            new Vector3(-0.0701716f,0.3840000f,0.0138284f),
            new Vector3(-0.0701716f,0.4460000f,-0.0138284f),
            new Vector3(-0.0690000f,0.4460000f,-0.0110000f),
            new Vector3(-0.0690000f,0.4460000f,0.0110000f),
            new Vector3(-0.0701716f,0.4460000f,0.0138284f),
            new Vector3(-0.0706906f,0.4483094f,-0.0133094f),
            new Vector3(-0.0701716f,0.4488284f,-0.0110000f),
            new Vector3(-0.0701716f,0.4488284f,0.0110000f),
            new Vector3(-0.0706906f,0.4483094f,0.0133094f),
            new Vector3(-0.0870000f,0.3811716f,-0.0138284f),
            new Vector3(-0.0870000f,0.3800000f,-0.0110000f),
            new Vector3(-0.0870000f,0.3800000f,0.0110000f),
            new Vector3(-0.0870000f,0.3811716f,0.0138284f),
            new Vector3(-0.0730000f,0.3811716f,-0.0138284f),
            new Vector3(-0.0730000f,0.3800000f,-0.0110000f),
            new Vector3(-0.0730000f,0.3800000f,0.0110000f),
            new Vector3(-0.0730000f,0.3811716f,0.0138284f),
            new Vector3(-0.0870000f,0.4488284f,-0.0138284f),
            new Vector3(-0.0870000f,0.4500000f,-0.0110000f),
            new Vector3(-0.0870000f,0.4500000f,0.0110000f),
            new Vector3(-0.0870000f,0.4488284f,0.0138284f),
            new Vector3(-0.0730000f,0.4488284f,-0.0138284f),
            new Vector3(-0.0730000f,0.4500000f,-0.0110000f),
            new Vector3(-0.0730000f,0.4500000f,0.0110000f),
            new Vector3(-0.0730000f,0.4488284f,0.0138284f),
            new Vector3(-0.0870000f,0.3840000f,-0.0150000f),
            new Vector3(-0.0870000f,0.4460000f,-0.0150000f),
            new Vector3(-0.0730000f,0.3840000f,-0.0150000f),
            new Vector3(-0.0730000f,0.4460000f,-0.0150000f),
            new Vector3(-0.0870000f,0.3840000f,0.0150000f),
            new Vector3(-0.0870000f,0.4460000f,0.0150000f),
            new Vector3(-0.0730000f,0.3840000f,0.0150000f),
            new Vector3(-0.0730000f,0.4460000f,0.0150000f),
            new Vector3(-0.0883094f,0.4391906f,-0.0133094f),
            new Vector3(-0.0888284f,0.4386716f,-0.0110000f),
            new Vector3(-0.0888284f,0.4386716f,0.0110000f),
            new Vector3(-0.0883094f,0.4391906f,0.0133094f),
            new Vector3(-0.0888284f,0.4415000f,-0.0138284f),
            new Vector3(-0.0900000f,0.4415000f,-0.0110000f),
            new Vector3(-0.0900000f,0.4415000f,0.0110000f),
            new Vector3(-0.0888284f,0.4415000f,0.0138284f),
            new Vector3(-0.0888284f,0.4585000f,-0.0138284f),
            new Vector3(-0.0900000f,0.4585000f,-0.0110000f),
            new Vector3(-0.0900000f,0.4585000f,0.0110000f),
            new Vector3(-0.0888284f,0.4585000f,0.0138284f),
            new Vector3(-0.0883094f,0.4608094f,-0.0133094f),
            new Vector3(-0.0888284f,0.4613284f,-0.0110000f),
            new Vector3(-0.0888284f,0.4613284f,0.0110000f),
            new Vector3(-0.0883094f,0.4608094f,0.0133094f),
            new Vector3(0.0883094f,0.4391906f,-0.0133094f),
            new Vector3(0.0888284f,0.4386716f,-0.0110000f),
            new Vector3(0.0888284f,0.4386716f,0.0110000f),
            new Vector3(0.0883094f,0.4391906f,0.0133094f),
            new Vector3(0.0888284f,0.4415000f,-0.0138284f),
            new Vector3(0.0900000f,0.4415000f,-0.0110000f),
            new Vector3(0.0900000f,0.4415000f,0.0110000f),
            new Vector3(0.0888284f,0.4415000f,0.0138284f),
            new Vector3(0.0888284f,0.4585000f,-0.0138284f),
            new Vector3(0.0900000f,0.4585000f,-0.0110000f),
            new Vector3(0.0900000f,0.4585000f,0.0110000f),
            new Vector3(0.0888284f,0.4585000f,0.0138284f),
            new Vector3(0.0883094f,0.4608094f,-0.0133094f),
            new Vector3(0.0888284f,0.4613284f,-0.0110000f),
            new Vector3(0.0888284f,0.4613284f,0.0110000f),
            new Vector3(0.0883094f,0.4608094f,0.0133094f),
            new Vector3(-0.0860000f,0.4386716f,-0.0138284f),
            new Vector3(-0.0860000f,0.4375000f,-0.0110000f),
            new Vector3(-0.0860000f,0.4375000f,0.0110000f),
            new Vector3(-0.0860000f,0.4386716f,0.0138284f),
            new Vector3(0.0860000f,0.4386716f,-0.0138284f),
            new Vector3(0.0860000f,0.4375000f,-0.0110000f),
            new Vector3(0.0860000f,0.4375000f,0.0110000f),
            new Vector3(0.0860000f,0.4386716f,0.0138284f),
            new Vector3(-0.0860000f,0.4613284f,-0.0138284f),
            new Vector3(-0.0860000f,0.4625000f,-0.0110000f),
            new Vector3(-0.0860000f,0.4625000f,0.0110000f),
            new Vector3(-0.0860000f,0.4613284f,0.0138284f),
            new Vector3(0.0860000f,0.4613284f,-0.0138284f),
            new Vector3(0.0860000f,0.4625000f,-0.0110000f),
            new Vector3(0.0860000f,0.4625000f,0.0110000f),
            new Vector3(0.0860000f,0.4613284f,0.0138284f),
            new Vector3(-0.0860000f,0.4415000f,-0.0150000f),
            new Vector3(-0.0860000f,0.4585000f,-0.0150000f),
            new Vector3(0.0860000f,0.4415000f,-0.0150000f),
            new Vector3(0.0860000f,0.4585000f,-0.0150000f),
            new Vector3(-0.0860000f,0.4415000f,0.0150000f),
            new Vector3(-0.0860000f,0.4585000f,0.0150000f),
            new Vector3(0.0860000f,0.4415000f,0.0150000f),
            new Vector3(0.0860000f,0.4585000f,0.0150000f)
        };
        static readonly int[][] Faces = {
            new int[] {0,2,3,0,1,2,4,6,5,4,7,6,0,5,1,0,4,5,3,6,7,3,2,6,0,7,4,0,3,7,1,6,2,1,5,6},
            new int[] {95,93,94,95,92,93,95,91,92,95,90,91,95,89,90,95,88,89,95,87,88,95,86,87,95,85,86,95,84,85,95,83,84,95,82,83,95,81,82,95,80,81,95,79,80,95,78,79,95,77,78,95,76,77,95,75,76,95,74,75,95,73,74,95,72,73,95,71,72,95,70,71,95,69,70,95,68,69,95,67,68,95,66,67,95,65,66,95,64,65,96,98,97,96,99,98,96,100,99,96,101,100,96,102,101,96,103,102,96,104,103,96,105,104,96,106,105,96,107,106,96,108,107,96,109,108,96,110,109,96,111,110,96,112,111,96,113,112,96,114,113,96,115,114,96,116,115,96,117,116,96,118,117,96,119,118,96,120,119,96,121,120,96,122,121,96,123,122,96,124,123,96,125,124,96,126,125,96,127,126,64,97,65,64,96,97,65,98,66,65,97,98,66,99,67,66,98,99,67,100,68,67,99,100,68,101,69,68,100,101,69,102,70,69,101,102,70,103,71,70,102,103,71,104,72,71,103,104,72,105,73,72,104,105,73,106,74,73,105,106,74,107,75,74,106,107,75,108,76,75,107,108,76,109,77,76,108,109,77,110,78,77,109,110,78,111,79,78,110,111,79,112,80,79,111,112,80,113,81,80,112,113,81,114,82,81,113,114,82,115,83,82,114,115,83,116,84,83,115,116,84,117,85,84,116,117,85,118,86,85,117,118,86,119,87,86,118,119,87,120,88,87,119,120,88,121,89,88,120,121,89,122,90,89,121,122,90,123,91,90,122,123,91,124,92,91,123,124,92,125,93,92,124,125,93,126,94,93,125,126,94,127,95,94,126,127,95,96,64,95,127,96,232,237,236,232,233,237,233,238,237,233,234,238,234,239,238,234,235,239,236,241,240,236,237,241,237,242,241,237,238,242,238,243,242,238,239,243,240,245,244,240,241,245,241,246,245,241,242,246,242,247,246,242,243,247,248,252,253,248,253,249,249,253,254,249,254,250,250,254,255,250,255,251,252,256,257,252,257,253,253,257,258,253,258,254,254,258,259,254,259,255,256,260,261,256,261,257,257,261,262,257,262,258,258,262,263,258,263,259,232,264,265,232,265,233,233,265,266,233,266,234,234,266,267,234,267,235,264,268,269,264,269,265,265,269,270,265,270,266,266,270,271,266,271,267,268,248,249,268,249,269,269,249,250,269,250,270,270,250,251,270,251,271,244,273,272,244,245,273,245,274,273,245,246,274,246,275,274,246,247,275,272,277,276,272,273,277,273,278,277,273,274,278,274,279,278,274,275,279,276,261,260,276,277,261,277,262,261,277,278,262,278,263,262,278,279,263,232,280,264,232,236,280,236,281,280,236,240,281,240,272,281,240,244,272,264,282,268,264,280,282,280,283,282,280,281,283,281,276,283,281,272,276,268,252,248,268,282,252,282,256,252,282,283,256,283,260,256,283,276,260,235,267,284,235,284,239,239,284,285,239,285,243,243,285,275,243,275,247,267,271,286,267,286,284,284,286,287,284,287,285,285,287,279,285,279,275,271,251,255,271,255,286,286,255,259,286,259,287,287,259,263,287,263,279,288,293,292,288,289,293,289,294,293,289,290,294,290,295,294,290,291,295,292,297,296,292,293,297,293,298,297,293,294,298,294,299,298,294,295,299,296,301,300,296,297,301,297,302,301,297,298,302,298,303,302,298,299,303,304,308,309,304,309,305,305,309,310,305,310,306,306,310,311,306,311,307,308,312,313,308,313,309,309,313,314,309,314,310,310,314,315,310,315,311,312,316,317,312,317,313,313,317,318,313,318,314,314,318,319,314,319,315,288,320,321,288,321,289,289,321,322,289,322,290,290,322,323,290,323,291,320,324,325,320,325,321,321,325,326,321,326,322,322,326,327,322,327,323,324,304,305,324,305,325,325,305,306,325,306,326,326,306,307,326,307,327,300,329,328,300,301,329,301,330,329,301,302,330,302,331,330,302,303,331,328,333,332,328,329,333,329,334,333,329,330,334,330,335,334,330,331,335,332,317,316,332,333,317,333,318,317,333,334,318,334,319,318,334,335,319,288,336,320,288,292,336,292,337,336,292,296,337,296,328,337,296,300,328,320,338,324,320,336,338,336,339,338,336,337,339,337,332,339,337,328,332,324,308,304,324,338,308,338,312,308,338,339,312,339,316,312,339,332,316,291,323,340,291,340,295,295,340,341,295,341,299,299,341,331,299,331,303,323,327,342,323,342,340,340,342,343,340,343,341,341,343,335,341,335,331,327,307,311,327,311,342,342,311,315,342,315,343,343,315,319,343,319,335,344,349,348,344,345,349,345,350,349,345,346,350,346,351,350,346,347,351,348,353,352,348,349,353,349,354,353,349,350,354,350,355,354,350,351,355,352,357,356,352,353,357,353,358,357,353,354,358,354,359,358,354,355,359,360,364,365,360,365,361,361,365,366,361,366,362,362,366,367,362,367,363,364,368,369,364,369,365,365,369,370,365,370,366,366,370,371,366,371,367,368,372,373,368,373,369,369,373,374,369,374,370,370,374,375,370,375,371,344,376,377,344,377,345,345,377,378,345,378,346,346,378,379,346,379,347,376,380,381,376,381,377,377,381,382,377,382,378,378,382,383,378,383,379,380,360,361,380,361,381,381,361,362,381,362,382,382,362,363,382,363,383,356,385,384,356,357,385,357,386,385,357,358,386,358,387,386,358,359,387,384,389,388,384,385,389,385,390,389,385,386,390,386,391,390,386,387,391,388,373,372,388,389,373,389,374,373,389,390,374,390,375,374,390,391,375,344,392,376,344,348,392,348,393,392,348,352,393,352,384,393,352,356,384,376,394,380,376,392,394,392,395,394,392,393,395,393,388,395,393,384,388,380,364,360,380,394,364,394,368,364,394,395,368,395,372,368,395,388,372,347,379,396,347,396,351,351,396,397,351,397,355,355,397,387,355,387,359,379,383,398,379,398,396,396,398,399,396,399,397,397,399,391,397,391,387,383,363,367,383,367,398,398,367,371,398,371,399,399,371,375,399,375,391,400,405,404,400,401,405,401,406,405,401,402,406,402,407,406,402,403,407,404,409,408,404,405,409,405,410,409,405,406,410,406,411,410,406,407,411,408,413,412,408,409,413,409,414,413,409,410,414,410,415,414,410,411,415,416,420,421,416,421,417,417,421,422,417,422,418,418,422,423,418,423,419,420,424,425,420,425,421,421,425,426,421,426,422,422,426,427,422,427,423,424,428,429,424,429,425,425,429,430,425,430,426,426,430,431,426,431,427,400,432,433,400,433,401,401,433,434,401,434,402,402,434,435,402,435,403,432,436,437,432,437,433,433,437,438,433,438,434,434,438,439,434,439,435,436,416,417,436,417,437,437,417,418,437,418,438,438,418,419,438,419,439,412,441,440,412,413,441,413,442,441,413,414,442,414,443,442,414,415,443,440,445,444,440,441,445,441,446,445,441,442,446,442,447,446,442,443,447,444,429,428,444,445,429,445,430,429,445,446,430,446,431,430,446,447,431,400,448,432,400,404,448,404,449,448,404,408,449,408,440,449,408,412,440,432,450,436,432,448,450,448,451,450,448,449,451,449,444,451,449,440,444,436,420,416,436,450,420,450,424,420,450,451,424,451,428,424,451,444,428,403,435,452,403,452,407,407,452,453,407,453,411,411,453,443,411,443,415,435,439,454,435,454,452,452,454,455,452,455,453,453,455,447,453,447,443,439,419,423,439,423,454,454,423,427,454,427,455,455,427,431,455,431,447,456,461,460,456,457,461,457,462,461,457,458,462,458,463,462,458,459,463,460,465,464,460,461,465,461,466,465,461,462,466,462,467,466,462,463,467,464,469,468,464,465,469,465,470,469,465,466,470,466,471,470,466,467,471,472,476,477,472,477,473,473,477,478,473,478,474,474,478,479,474,479,475,476,480,481,476,481,477,477,481,482,477,482,478,478,482,483,478,483,479,480,484,485,480,485,481,481,485,486,481,486,482,482,486,487,482,487,483,456,488,489,456,489,457,457,489,490,457,490,458,458,490,491,458,491,459,488,492,493,488,493,489,489,493,494,489,494,490,490,494,495,490,495,491,492,472,473,492,473,493,493,473,474,493,474,494,494,474,475,494,475,495,468,497,496,468,469,497,469,498,497,469,470,498,470,499,498,470,471,499,496,501,500,496,497,501,497,502,501,497,498,502,498,503,502,498,499,503,500,485,484,500,501,485,501,486,485,501,502,486,502,487,486,502,503,487,456,504,488,456,460,504,460,505,504,460,464,505,464,496,505,464,468,496,488,506,492,488,504,506,504,507,506,504,505,507,505,500,507,505,496,500,492,476,472,492,506,476,506,480,476,506,507,480,507,484,480,507,500,484,459,491,508,459,508,463,463,508,509,463,509,467,467,509,499,467,499,471,491,495,510,491,510,508,508,510,511,508,511,509,509,511,503,509,503,499,495,475,479,495,479,510,510,479,483,510,483,511,511,483,487,511,487,503},
            new int[] {48,50,51,48,49,50,52,54,53,52,55,54,48,53,49,48,52,53,51,54,55,51,50,54,48,55,52,48,51,55,49,54,50,49,53,54},
            new int[] {56,58,59,56,57,58,60,62,61,60,63,62,56,61,57,56,60,61,59,62,63,59,58,62,56,63,60,56,59,63,57,62,58,57,61,62},
            new int[] {159,157,158,159,156,157,159,155,156,159,154,155,159,153,154,159,152,153,159,151,152,159,150,151,159,149,150,159,148,149,159,147,148,159,146,147,159,145,146,159,144,145,159,143,144,159,142,143,159,141,142,159,140,141,159,139,140,159,138,139,159,137,138,159,136,137,159,135,136,159,134,135,159,133,134,159,132,133,159,131,132,159,130,131,159,129,130,159,128,129,160,162,161,160,163,162,160,164,163,160,165,164,160,166,165,160,167,166,160,168,167,160,169,168,160,170,169,160,171,170,160,172,171,160,173,172,160,174,173,160,175,174,160,176,175,160,177,176,160,178,177,160,179,178,160,180,179,160,181,180,160,182,181,160,183,182,160,184,183,160,185,184,160,186,185,160,187,186,160,188,187,160,189,188,160,190,189,160,191,190,128,161,129,128,160,161,129,162,130,129,161,162,130,163,131,130,162,163,131,164,132,131,163,164,132,165,133,132,164,165,133,166,134,133,165,166,134,167,135,134,166,167,135,168,136,135,167,168,136,169,137,136,168,169,137,170,138,137,169,170,138,171,139,138,170,171,139,172,140,139,171,172,140,173,141,140,172,173,141,174,142,141,173,174,142,175,143,142,174,175,143,176,144,143,175,176,144,177,145,144,176,177,145,178,146,145,177,178,146,179,147,146,178,179,147,180,148,147,179,180,148,181,149,148,180,181,149,182,150,149,181,182,150,183,151,150,182,183,151,184,152,151,183,184,152,185,153,152,184,185,153,186,154,153,185,186,154,187,155,154,186,187,155,188,156,155,187,188,156,189,157,156,188,189,157,190,158,157,189,190,158,191,159,158,190,191,159,160,128,159,191,160}
        };
        static void EnsureMeshes()
        {
            if (body != null && lens != null) return;
            body = new Mesh { name = "DV_EET_Body_NoKnob" };
            body.vertices = Vertices;
            // Project the yellow casing texture across the front and back faces.
            var uv = new Vector2[Vertices.Length];
            for (int i = 0; i < Vertices.Length; i++)
                uv[i] = new Vector2(Mathf.Clamp01((Vertices[i].x + .125f) / .25f), Mathf.Clamp01(Vertices[i].y / .40f));
            // The white panel owns its vertices, so give it a full-panel UV map.
            foreach (int index in Faces[3])
                uv[index] = new Vector2(Mathf.Clamp01((Vertices[index].x + .11f) / .22f), Mathf.Clamp01((Vertices[index].y - .0875f) / .025f));
            body.uv = uv;
            body.subMeshCount = 4;
            for (int i = 0; i < 4; i++) body.SetTriangles(Faces[i], i);
            body.RecalculateNormals(); body.RecalculateBounds();
            lens = new Mesh { name = "DV_EET_SeparateLens" };
            lens.vertices = Vertices; lens.triangles = Faces[4];
            lens.RecalculateNormals(); lens.RecalculateBounds();
        }
        // Resolve a shader from the running game rather than relying on a
        // serialized bundle shader, which may render magenta in the player.
        static Material CompatibleMaterial(Material source)
        {
            var result = new Material(source);
            var gameShader = Shader.Find("Standard");
            if (gameShader != null && gameShader.isSupported) result.shader = gameShader;
            else if (result.shader == null || !result.shader.isSupported)
            {
                var fallback = Shader.Find("Unlit/Texture");
                if (fallback != null && fallback.isSupported) result.shader = fallback;
            }
            return result;
        }
        static void EnsureMaterials(Material[] original)
        {
            if (yellowMaterial != null && Chrome != null) return;
            // Stable procedural weathering: yellow paint with restrained grime and rust.
            const int size = 256;
            weatherTexture = new Texture2D(size, size, TextureFormat.RGBA32, true);
            weatherTexture.name = "DV_EET_WeatheredYellow";
            weatherTexture.wrapMode = TextureWrapMode.Clamp;
            var pixels = new Color32[size * size];
            var yellow = new Color(.91f, .72f, .075f);
            var rust = new Color(.37f, .16f, .045f);
            for (int y = 0; y < size; y++) for (int x = 0; x < size; x++)
            {
                float u = x / 255f, v = y / 255f;
                float cloud = Mathf.PerlinNoise(u * 8f + 12f, v * 8f + 31f);
                float fine = Mathf.PerlinNoise(u * 53f + 6f, v * 53f + 7f);
                float grime = Mathf.Clamp01((cloud - .35f) * .27f + (fine - .4f) * .11f + (1f-v) * .10f);
                Color c = yellow * (1f - grime);
                float corrosion = Mathf.PerlinNoise(u * 18f + 27f, v * 18f + 11f);
                float rustAmount = Mathf.Clamp01((corrosion - .62f) * 3f) * Mathf.Clamp01((fine - .43f) * 3f) * .75f;
                c = Color.Lerp(c, rust, rustAmount); c.a = 1f;
                pixels[y * size + x] = c;
            }
            weatherTexture.SetPixels32(pixels); weatherTexture.Apply(true, false);
            yellowMaterial = CompatibleMaterial(original[0]);
            yellowMaterial.name = "DV_EET_WeatheredSafetyYellow";
            if (yellowMaterial.HasProperty("_Color")) yellowMaterial.color = Color.white;
            if (yellowMaterial.HasProperty("_MainTex")) yellowMaterial.mainTexture = weatherTexture;
            if (yellowMaterial.HasProperty("_Metallic")) yellowMaterial.SetFloat("_Metallic", .12f);
            if (yellowMaterial.HasProperty("_Glossiness")) yellowMaterial.SetFloat("_Glossiness", .28f);
            labelTexture = new Texture2D(2, 2, TextureFormat.RGBA32, true);
            labelTexture.name = "DV_EET_DerailValleyGadgets_Label";
            if (!ImageConversion.LoadImage(labelTexture, Convert.FromBase64String(LabelPng)))
                throw new InvalidOperationException("Could not decode the DV_EET panel label");
            labelTexture.wrapMode = TextureWrapMode.Clamp;
            labelMaterial = CompatibleMaterial(original[3]);
            labelMaterial.name = "DV_EET_BrandedPanel";
            if (labelMaterial.HasProperty("_Color")) labelMaterial.color = Color.white;
            if (labelMaterial.HasProperty("_MainTex"))
            {
                labelMaterial.mainTexture = labelTexture;
                // Unity's built-in cube UVs face the camera upside down on the front.
                labelMaterial.mainTextureScale = new Vector2(-1f, -1f);
                labelMaterial.mainTextureOffset = new Vector2(1f, 1f);
            }
            Chrome = CompatibleMaterial(original[1]);
            Chrome.name = "DV_EET_Chrome";
            if (Chrome.HasProperty("_Color")) Chrome.color = new Color(.72f, .75f, .78f);
            if (Chrome.HasProperty("_Metallic")) Chrome.SetFloat("_Metallic", .88f);
            if (Chrome.HasProperty("_Glossiness")) Chrome.SetFloat("_Glossiness", .78f);
        }
        static GameObject Part(GameObject root, string name, PrimitiveType shape, Vector3 pos, Vector3 scale, Material material, Quaternion rotation)
        {
            var part = GameObject.CreatePrimitive(shape);
            part.name = name; part.layer = root.layer;
            part.transform.SetParent(root.transform, false);
            part.transform.localPosition = pos; part.transform.localScale = scale;
            part.transform.localRotation = rotation;
            var collider = part.GetComponent<Collider>();
            if (collider != null) { collider.enabled = false; UnityEngine.Object.Destroy(collider); }
            part.GetComponent<Renderer>().sharedMaterial = material;
            return part;
        }
        internal static void AddToggleAppearance(GameObject parent, bool loose)
        {
            var mount = Part(parent, loose ? "EET_LooseToggleMount" : "EET_ToggleMount", PrimitiveType.Cylinder,
                Vector3.zero, new Vector3(.025f,.004f,.025f), Chrome, Quaternion.Euler(0,0,90));
            var pivot = new GameObject(loose ? "EET_LooseTogglePivot" : "EET_ToggleLeverPivot");
            pivot.layer = parent.layer; pivot.transform.SetParent(parent.transform, false);
            pivot.transform.localPosition = new Vector3(.006f,0,0);
            pivot.transform.localRotation = Quaternion.Euler(0,0,loose ? -35f : 35f);
            Part(pivot, loose ? "EET_LooseToggleLever" : "EET_ToggleLever", PrimitiveType.Cylinder,
                new Vector3(.021f,0,0), new Vector3(.009f,.021f,.009f), Chrome, Quaternion.Euler(0,0,90));
            Part(pivot, loose ? "EET_LooseToggleTip" : "EET_ToggleTip", PrimitiveType.Sphere,
                new Vector3(.042f,0,0), new Vector3(.012f,.012f,.012f), Chrome, Quaternion.identity);
        }
        internal static void Replace(GameObject root)
        {
            if (root == null) return;
            foreach (var child in root.GetComponentsInChildren<Transform>(true)) if (child.name == "EET_RedLens") return;
            var original = root.GetComponentInChildren<MeshFilter>(true);
            if (original == null || original.sharedMesh == null) throw new InvalidOperationException("DV_EET original mesh not found");
            var renderer = original.GetComponent<MeshRenderer>();
            if (renderer == null || renderer.sharedMaterials.Length < 5) throw new InvalidOperationException("DV_EET material slots not found");
            var materials = renderer.sharedMaterials;
            EnsureMaterials(materials);
            if (reflectorMaterial == null)
            {
                reflectorMaterial = CompatibleMaterial(materials[4]);
                reflectorMaterial.name = "DV_EET_RedReflector";
                if (reflectorMaterial.HasProperty("_Color")) reflectorMaterial.color = new Color(.8f, .015f, .015f);
                if (reflectorMaterial.HasProperty("_Metallic")) reflectorMaterial.SetFloat("_Metallic", .55f);
                if (reflectorMaterial.HasProperty("_Glossiness")) reflectorMaterial.SetFloat("_Glossiness", .9f);
                reflectorMaterial.DisableKeyword("_EMISSION");
                if (reflectorMaterial.HasProperty("_EmissionColor")) reflectorMaterial.SetColor("_EmissionColor", Color.black);
            }
            original.sharedMesh = null; renderer.enabled = false;
            var q = Quaternion.identity;
            // Back remains +Z at the existing attachment origin. The lamp faces -Z.
            Part(root,"EET_Case",PrimitiveType.Cube,new Vector3(0,.245f,0),new Vector3(.18f,.31f,.14f),yellowMaterial,q);
            Part(root,"EET_Cap",PrimitiveType.Cube,new Vector3(0,.41f,0),new Vector3(.195f,.025f,.15f),yellowMaterial,q);
            Part(root,"EET_Base",PrimitiveType.Cube,new Vector3(0,.055f,0),new Vector3(.145f,.07f,.135f),Chrome,q);
            Part(root,"EET_BaseCollar",PrimitiveType.Cube,new Vector3(0,.095f,0),new Vector3(.17f,.015f,.14f),yellowMaterial,q);
            Part(root,"EET_LampHousing",PrimitiveType.Cube,new Vector3(0,.325f,-.083f),new Vector3(.14f,.115f,.032f),yellowMaterial,q);
            Part(root,"EET_RedLens",PrimitiveType.Cube,new Vector3(0,.325f,-.102f),new Vector3(.105f,.075f,.008f),CompatibleMaterial(materials[4]),q);
            Part(root,"EET_ReflectorFrame",PrimitiveType.Cube,new Vector3(0,.18f,-.074f),new Vector3(.12f,.125f,.012f),yellowMaterial,q);
            Part(root,"EET_Reflector",PrimitiveType.Cube,new Vector3(0,.18f,-.082f),new Vector3(.09f,.105f,.006f),reflectorMaterial,q);
            Part(root,"EET_Label",PrimitiveType.Cube,new Vector3(0,.105f,-.074f),new Vector3(.105f,.018f,.005f),labelMaterial,q);
            Part(root,"EET_TopPostBase",PrimitiveType.Cylinder,new Vector3(0,.426f,0),new Vector3(.043f,.005f,.043f),Chrome,q);
            Part(root,"EET_TopPost",PrimitiveType.Cylinder,new Vector3(0,.445f,0),new Vector3(.022f,.018f,.022f),Chrome,q);
            var sw = new GameObject("EET_LooseToggleRoot"); sw.layer=root.layer; sw.transform.SetParent(root.transform,false);
            sw.transform.localPosition = new Vector3(.092f,.23f,0);
            AddToggleAppearance(sw, true);
        }
    }
}
