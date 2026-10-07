using System.Reflection;
using EFT;
using EFT.UI;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace QuestBackport.Client;

public class TradingPlayerPanelPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
        => AccessTools.Method(typeof(TradingPlayerPanel), nameof(TradingPlayerPanel.UpdateStats));

    [PatchPostfix]
    private static void PatchPostfix(TradingPlayerPanel __instance, Profile.TraderInfo traderInfo)
    {
        if (!ClientPlugin.DisableSalesVolumeRequirement)
        {
            return;
        }

        var show = !VanillaTraders.Contains(traderInfo?.Id);
        __instance._currentMoney.gameObject.SetActive(show);
        __instance._nextMoney.gameObject.SetActive(show);
    }
}
