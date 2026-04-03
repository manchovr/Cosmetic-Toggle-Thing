using BepInEx;
using BepInEx.Configuration;
using GorillaNetworking;
using Photon.Pun;
using UnityEngine.XR;

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
            if (InputDevices.GetDeviceAtXRNode(configIsLeft.Value ? XRNode.LeftHand : XRNode.RightHand).TryGetFeatureValue(CommonUsages.primary2DAxisClick, out bool current))
            {
                if (current && !lastClick)
                    Wear(configCosmeticID.Value);

                lastClick = current;
            }
        }
        public void Wear(string cosmeticName)
        {
            CosmeticsController.instance.ApplyCosmeticItemToSet(CosmeticsController.instance.currentWornSet, CosmeticsController.instance.GetItemFromDict(cosmeticName), true, false);
            CosmeticsController.instance.ApplyCosmeticItemToSet(VRRig.LocalRig.tryOnSet, CosmeticsController.instance.GetItemFromDict(cosmeticName), true, false);
            CosmeticsController.instance.UpdateWornCosmetics(PhotonNetwork.InRoom);
        }
    }
}
