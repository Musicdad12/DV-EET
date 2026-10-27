# DV EET

A Derail Valley end-of-train device with a flashing red light and a directional world-map marker. The player attaches it to a vehicle using the gadget system; no mounting plate is required. It can be purchased for a base price of 25,000 (career settings may change the final price).

## Requirements and installation

Install Unity Mod Manager and custom_item_mod, then install the DV_EET release ZIP with Unity Mod Manager and restart the game. To operate, mount the device on the last car, then switch it on. Removing it turns it off. At rest before motion is detected, the map arrow uses car orientation. The mod supports the game's non-VR and VR use interactions.

## Build

- Derail Valley with Unity Mod Manager and custom_item_mod installed.
- Unity 2019.4.40f1 for the bundled C# compiler and asset-bundle rebuilding.
- Run `Source/build.ps1`, passing `-GameDir` and `-UnityDir` if installed outside the defaults. Game libraries are referenced from the local installation and are not distributed here.
- To rebuild the asset bundle, open `UnityProject` in Unity 2019.4.40f1, select **DV_EET > Build Windows VR Assets**, then copy `UnityProject/Build/dv_eet_assets` to `Item/dv_eet_assets`.

## Releases and updates

`Prepare-Release.ps1 -GitHubOwner <account>` builds the DLL, configures the Unity Mod Manager repository and home-page URLs, writes `repository.json`, and prepares a ZIP under `dist`. The repository must be public for unauthenticated update checks. Copy the generated ZIP into the repository's `Releases` folder and commit it together with the updated metadata. Increment the version and repeat these steps for later releases. Update notifications require the user to install a package containing the correct `Repository` URL; creating a GitHub repository alone does not update an existing installation. Unity Mod Manager reports available updates; installation behavior depends on its version.

The item displays as `DV EET` while retaining the earlier `custom_item_mod/DV_EET/prefab` identifier for existing saves.

## Credits

Gadget factory adapted from ZG2 by Fuggschen under MIT; see `Licenses/ZG2-LICENSE.txt`. The Unity Mod Manager template license is in `Licenses/TEMPLATE-LICENSE.txt`.
