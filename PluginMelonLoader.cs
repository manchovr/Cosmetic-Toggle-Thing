using CosmeticToggleThing;
using GorillaNetworking;
using MelonLoader;
using Photon.Pun;
using UnityEngine.XR;
using Valve.VR;
using System.Collections;

[assembly: MelonInfo(typeof(PluginMelonloader), PluginInfo.Name, PluginInfo.Version, "mancho")]
[assembly: MelonGame(null, null)]
namespace CosmeticToggleThing
{
    public class PluginMelonloader : MelonMod
    {
        private MelonPreferences_Category category;

        private MelonPreferences_Entry<string> configCosmeticID;
        private MelonPreferences_Entry<bool> configIsLeft;
        bool lastClick = false; //stupid joystick click running every frame
        bool isSteamVR;

        public override void OnInitializeMelon()
        {
            category = MelonPreferences.CreateCategory("CosmeticToggleThing");

            configCosmeticID = category.CreateEntry<string>("Cosmetic ID", "LBANI.");
            configIsLeft = category.CreateEntry<bool>("Use Left Hand", false);
        }

        public override void OnUpdate()
        {
            bool Click;
            if (configIsLeft.Value)
            {
                if (!isSteamVR)
                    ControllerInputPoller.instance.leftControllerDevice.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out Click);
                else
                    Click = SteamVR_Actions.gorillaTag_LeftJoystickClick.state;
            }
            else
            {
                if (!isSteamVR)
                    ControllerInputPoller.instance.rightControllerDevice.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out Click);
                else
                    Click = SteamVR_Actions.gorillaTag_RightJoystickClick.state;
            }
            if (Click && !lastClick)
                Wear(configCosmeticID.Value);

            lastClick = Click;
        }
        public void Wear(string cosmeticName)
        {
            CosmeticsController.instance.ApplyCosmeticItemToSet(CosmeticsController.instance.currentWornSet, CosmeticsController.instance.GetItemFromDict(cosmeticName), true, false);
            CosmeticsController.instance.ApplyCosmeticItemToSet(VRRig.LocalRig.tryOnSet, CosmeticsController.instance.GetItemFromDict(cosmeticName), true, false);
            CosmeticsController.instance.UpdateWornCosmetics(PhotonNetwork.InRoom);
        }
        public override void OnLateInitializeMelon()
        {
            MelonCoroutines.Start(WaitForPlayFab());
        }
        private IEnumerator WaitForPlayFab()
        {
            while (PlayFabAuthenticator.instance == null || PlayFabAuthenticator.instance.platform == null || string.IsNullOrEmpty(PlayFabAuthenticator.instance.platform.PlatformTag))
            {
                yield return null;
            }

            string tag = PlayFabAuthenticator.instance.platform.PlatformTag;

            isSteamVR = tag.ToLower().Contains("steam");
        }
    }
}
