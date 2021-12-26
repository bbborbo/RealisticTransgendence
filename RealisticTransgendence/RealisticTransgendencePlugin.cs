using BepInEx;
using BepInEx.Configuration;
using R2API;
using R2API.Utils;
using RoR2;
using System;
using static R2API.RecalculateStatsAPI.StatHookEventArgs;

namespace RealisticTransgendence
{
    [BepInDependency(R2API.R2API.PluginGUID)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [BepInDependency("com.Borbo.Transgendence", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin(guid, modName, version)]
    public class RealisticTransgendencePlugin : BaseUnityPlugin
    {
        public const string guid = "com.TransRights.RealisticTransgendence";
        public const string modName = "RealisticTransgendence";
        public const string version = "1.0.0";
        internal static ConfigFile CustomConfigFile { get; set; }
        public static ConfigEntry<bool> OnlyOneExtraJump { get; set; }

        void Awake()
        {
            InitializeConfig();
            LanguageAPI.Add("ITEM_SHIELDONLY_PICKUP", "Convert all your health into shield. Increase maximum health. Gain an extra jump.");
            LanguageAPI.Add("ITEM_SHIELDONLY_DESC", 
                "<style=cIsHealing>Convert</style> all but <style=cIsHealing>1 health</style> into <style=cIsHealing>regenerating shields</style>. " +
                "<style=cIsHealing>Gain 50% <style=cStack>(+25% per stack)</style> maximum health</style>. " + 
                (OnlyOneExtraJump.Value ? "Gain an additional jump" : "Gain 1 additional jump <style=cStack>(+1 per stack)</style>."));

            On.RoR2.CharacterBody.RecalculateStats += TransgendenceDoubleJump;
        }

        private void InitializeConfig()
        {
            CustomConfigFile = new ConfigFile(Paths.ConfigPath + $"\\{modName}.cfg", true);

            OnlyOneExtraJump = CustomConfigFile.Bind<bool>(
                "Only One Extra Jump",
                "Should TRANSGENDENCE only provide one additional jump",
                false,
                "Set this to FALSE if you want TRANSGENDENCE's realistic double jumps to stack."
                );
        }

        private void TransgendenceDoubleJump(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            orig(self);
            int transCount = self.inventory.GetItemCount(RoR2Content.Items.ShieldOnly);
            if (!OnlyOneExtraJump.Value)
            {
                self.maxJumpCount += transCount;
            }
            else if (transCount > 0)
            {
                self.maxJumpCount += 1;
            }
        }
    }
}
