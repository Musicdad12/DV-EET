param([string]$GameDir = 'C:\Program Files (x86)\Steam\steamapps\common\Derail Valley', [string]$UnityDir = 'C:\Program Files\Unity\Hub\Editor\2019.4.40f1\Editor')
$managed=Join-Path $GameDir 'DerailValley_Data\Managed'
$compiler=Join-Path $UnityDir 'Data\Tools\Roslyn\csc.exe'
$mod=Join-Path $GameDir 'Mods\custom_item_mod'
$names=@('mscorlib.dll','netstandard.dll','System.dll','System.Core.dll','Assembly-CSharp.dll','DV.Common.dll','DV.Interaction.dll','VRTK.dll','DV.CabControls.Spec.dll','DV.Inventory.dll','DV.ThingTypes.dll','DV.Utils.dll','UnityEngine.dll','UnityEngine.CoreModule.dll','UnityEngine.PhysicsModule.dll','UnityEngine.ImageConversionModule.dll','Newtonsoft.Json.dll','Unity.TextMeshPro.dll')
$refs=$names | ForEach-Object { '-r:' + (Join-Path $managed $_) }
& $compiler -target:library "-out:$PSScriptRoot\DV_EET.dll" @refs "-r:$managed\UnityModManager\UnityModManager.dll" "-r:$managed\UnityModManager\0Harmony.dll" "-r:$mod\custom_item_mod.dll" "$PSScriptRoot\Main.cs" "$PSScriptRoot\DeviceState.cs" "$PSScriptRoot\EetModel.cs"
if ($LASTEXITCODE -ne 0) { throw 'Compilation failed' }






