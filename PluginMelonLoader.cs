using CosmeticToggleThing;
using GorillaNetworking;
using MelonLoader;
using Photon.Pun;
using UnityEngine.XR;
using Valve.VR;

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

        public override void OnInitializeMelon()
        {
            category = MelonPreferences.CreateCategory("CosmeticToggleThing");

            configCosmeticID = category.CreateEntry<string>("Cosmetic ID", "LBANI.");
            configIsLeft = category.CreateEntry<bool>("Use Left Hand", false);
        }

        public override void OnUpdate()
        {
            var controllerOculus = configIsLeft.Value ? ControllerInputPoller.instance.leftControllerDevice : ControllerInputPoller.instance.rightControllerDevice;
            var controllerSteam = configIsLeft.Value ? SteamVR_Actions.gorillaTag_LeftJoystickClick : SteamVR_Actions.gorillaTag_RightJoystickClick;

            bool oculusPressed = controllerOculus.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out bool currentOculus) && currentOculus;
            bool steamPressed = controllerSteam.state;

            bool pressed = oculusPressed || steamPressed; // why does steam and meta link have to be different :sob:

            if (pressed && !lastClick)
            {
                Wear(configCosmeticID.Value);
            }

            lastClick = pressed;
        }
        public void Wear(string cosmeticName)
        {
            CosmeticsController.instance.ApplyCosmeticItemToSet(CosmeticsController.instance.currentWornSet, CosmeticsController.instance.GetItemFromDict(cosmeticName), true, false);
            CosmeticsController.instance.ApplyCosmeticItemToSet(VRRig.LocalRig.tryOnSet, CosmeticsController.instance.GetItemFromDict(cosmeticName), true, false);
            CosmeticsController.instance.UpdateWornCosmetics(PhotonNetwork.InRoom);
        }
    }
}
