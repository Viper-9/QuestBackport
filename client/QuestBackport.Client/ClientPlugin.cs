using System;
using System.IO;
using BepInEx;
using BepInEx.Logging;
using Newtonsoft.Json;

namespace QuestBackport.Client;

[BepInPlugin("com.viper.questbackport.client", "QuestBackport Client", "1.0.4")]
public class ClientPlugin : BaseUnityPlugin
{
    private const string ServerConfigRelativePath = "SPT_Runtime/user/mods/QuestBackport/db/Config.json";

    internal static new ManualLogSource? Logger { get; private set; }

    internal static bool DisableSalesVolumeRequirement { get; private set; }

    internal static bool QuestContentEnabled { get; private set; } = true;

    private void Awake()
    {
        Logger = base.Logger;
        LoadServerConfig();
        new TraderTooltipPatch().Enable();
        new TradingPlayerPanelPatch().Enable();
        new HandoverItemCachePatch().Enable();

        if (QuestContentEnabled)
        {
            QuestAlternativeConditions.Load();
            if (QuestAlternativeConditions.Enabled)
            {
                new QuestAlternativeConditionTestAllPatch().Enable();
                new QuestAlternativeConditionCompletionPatch().Enable();
                new QuestObjectivesViewFilterPatch().Enable();
            }
        }
    }

    private void LoadServerConfig()
    {
        var configPath = Path.Combine(BepInEx.Paths.GameRootPath, ServerConfigRelativePath);

        if (!File.Exists(configPath))
        {
            Logger?.LogWarning($"[QuestBackport.Client] {configPath} 를 찾지 못해 거래량 UI를 그대로 둡니다.");
            return;
        }

        try
        {
            var json = File.ReadAllText(configPath);
            var config = JsonConvert.DeserializeObject<ModConfig>(json);
            DisableSalesVolumeRequirement = config?.DisableSalesVolumeRequirement ?? false;
            QuestContentEnabled = config?.QuestContentEnabled ?? true;
            Logger?.LogInfo($"[QuestBackport.Client] disableSalesVolumeRequirement = {DisableSalesVolumeRequirement}");
        }
        catch (Exception ex)
        {
            Logger?.LogError($"[QuestBackport.Client] {configPath} 읽기 실패: {ex.Message}");
        }
    }

    private class ModConfig
    {
        public bool DisableSalesVolumeRequirement { get; set; }

        public bool QuestContentEnabled { get; set; } = true;
    }
}
