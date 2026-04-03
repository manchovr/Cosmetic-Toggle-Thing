using BepInEx;
using BepInEx.Configuration;
using GorillaNetworking;
using Photon.Pun;
using UnityEngine.XR;
using Valve.VR;

namespace CosmeticToggleThing
{
	[BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
	public class PluginBepinex : BaseUnityPlugin
	{
        private ConfigEntry<string> configCosmeticID;
        private ConfigEntry<bool> configIsLeft;
        bool lastClick = false; //stupid joystick click running every frame

        private void Awake()
        {
            configCosmeticID = Config.Bind("General","CosmeticID","LBANI.","The cosmetic that you want to toggle");
            configIsLeft = Config.Bind("General","Left Hand",false,"Which hand to toggle");
        }
        void Update()
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
