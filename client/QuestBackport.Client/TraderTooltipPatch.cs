using System.Reflection;
using EFT;
using EFT.UI;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace QuestBackport.Client;

public class TraderTooltipPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
        => AccessTools.Method(typeof(TraderTooltip), nameof(TraderTooltip.Show));

    [PatchPostfix]
    private static void PatchPostfix(TraderTooltip __instance, Profile.TraderInfo traderInfo)
    {
        if (!ClientPlugin.DisableSalesVolumeRequirement)
        {
            return;
        }

        var show = !VanillaTraders.Contains(traderInfo?.Id);
        __instance._moneySpent.gameObject.SetActive(show);
        __instance._moneySpentRequired.gameObject.SetActive(show);
        if (!show)
        {
            __instance._moneySpentMet.SetActive(false);
        }
    }
}
