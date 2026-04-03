using CosmeticToggleThing;
using GorillaNetworking;
using MelonLoader;
using Photon.Pun;
using UnityEngine.XR;

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
