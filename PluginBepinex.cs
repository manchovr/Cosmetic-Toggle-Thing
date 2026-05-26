using BepInEx;
using BepInEx.Configuration;
using GorillaNetworking;
using HarmonyLib;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;
using Valve.VR;
using System.Collections;

namespace CosmeticToggleThing
{
	[BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
	public class PluginBepinex : BaseUnityPlugin
	{
        private ConfigEntry<string> configCosmeticID;
        private ConfigEntry<bool> configIsLeft;
        bool lastClick = false; //stupid joystick click running every frame
        bool isSteamVR;

        private void Awake()
        {
            configCosmeticID = Config.Bind("General","CosmeticID","LBANI.","The cosmetic that you want to toggle");
            configIsLeft = Config.Bind("General","Left Hand",false,"Which hand to toggle");
        }
        void Update()
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
            CosmeticsController.instance.OnCosmeticsUpdated?.Invoke();
        }
        void Start()
        {
            StartCoroutine(WaitForPlayFab());
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
