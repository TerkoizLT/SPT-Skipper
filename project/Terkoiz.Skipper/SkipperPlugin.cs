using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using EFT.UI;
using JetBrains.Annotations;
using UnityEngine;

namespace Terkoiz.Skipper
{
    [BepInPlugin("com.terkoiz.skipper", "Terkoiz.Skipper", "1.1.0")]
    public class SkipperPlugin : BaseUnityPlugin
    {
        internal const string SkipButtonName = "SkipButton";

        internal new static ManualLogSource Logger { get; private set; }

        private const string MainSectionName = "Main";
        internal static ConfigEntry<bool> ModEnabled;
        internal static ConfigEntry<bool> AlwaysDisplay;
        internal static ConfigEntry<bool> SkipConfirmation;
        internal static ConfigEntry<KeyboardShortcut> DisplayHotkey;

        [UsedImplicitly]
        internal void Start()
        {
            Logger = base.Logger;
            InitConfiguration();

            new QuestObjectiveViewPatch().Enable();
        }

        private void InitConfiguration()
        {
            ModEnabled = Config.Bind(
                MainSectionName,
                "1. Enabled",
                true,
                "Global mod toggle. Will need to re-open the quest window for the setting change to take effect.");

            AlwaysDisplay = Config.Bind(
                MainSectionName,
                "2. Always display Skip button",
                false,
                "If enabled, the Skip button will always be visible.");

            DisplayHotkey = Config.Bind(
                MainSectionName,
                "3. Display hotkey",
                new KeyboardShortcut(KeyCode.LeftControl),
                "Holding down this key will make the Skip buttons appear.");
            
            SkipConfirmation = Config.Bind(
                MainSectionName,
                "4. Skip confirmation",
                true,
                "If disabled, a confirmation dialog will appear when you press the Skip button.");
        }

        [UsedImplicitly]
        internal void Update()
        {
            if (!ModEnabled.Value || AlwaysDisplay.Value)
            {
                if (AlwaysDisplay.Value && QuestObjectiveViewPatch.LastSeenObjectivesBlock != null && !QuestObjectiveViewPatch.LastSeenObjectivesBlock.activeSelf)
                {
                    ChangeButtonVisibility(true);
                }
                
                return;
            }

            if (QuestObjectiveViewPatch.LastSeenObjectivesBlock == null || !QuestObjectiveViewPatch.LastSeenObjectivesBlock.activeSelf)
            {
                return;
            }

            if (DisplayHotkey.Value.IsDown())
            {
                ChangeButtonVisibility(true);
            }

            if (DisplayHotkey.Value.IsUp())
            {
                ChangeButtonVisibility(false);
            }
        }

        private static void ChangeButtonVisibility(bool setVisibilityTo)
        {
            foreach (var button in QuestObjectiveViewPatch.LastSeenObjectivesBlock.GetComponentsInChildren<DefaultUIButton>(includeInactive: true))
            {
                if (button.name != SkipButtonName) continue;

                button.gameObject.SetActive(setVisibilityTo);
            }
        }
    }
}
