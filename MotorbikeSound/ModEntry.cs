using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using GenericModConfigMenu;
using HarmonyLib;
using StardewValley;
using StardewValley.Characters;

namespace MotorbikeSound
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        /*********
        ** Fields
        *********/
        /// <summary>The in-game event detected on the last update tick.</summary>
        private ModConfig Config;


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>

        public override void Entry(IModHelper helper)
        {
            var harmony = new Harmony(this.ModManifest.UniqueID);
            this.Config = this.Helper.ReadConfig<ModConfig>();
            string bikeName = this.Config.BikeName;

            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;

            

            harmony.Patch(
                                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.localSoundAt)),
                                prefix: new HarmonyMethod(typeof(SoundPatches), nameof(SoundPatches.localSoundAt_prefix))
                            );

        }




        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            var api = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (api is null) return;

            api.RegisterModConfig(
                mod: this.ModManifest,
                revertToDefault: () => this.Config = new ModConfig(),
                saveToFile: () => this.Helper.WriteConfig(this.Config)
                );

            api.SetDefaultIngameOptinValue(this.ModManifest, true);

            api.RegisterSimpleOption(
                mod: this.ModManifest,
                optionName: "Motorbike Name",
                optionDesc: "The name of the horse that should sound like a motorcycle",
                optionGet: () => this.Config.BikeName,
                optionSet: value => this.Config.BikeName = value
                );
        }

    }
    class ModConfig
    {
        public string BikeName { get; set; }

        public ModConfig()
        {
            this.BikeName = "Ducati";
        }
    }
    [HarmonyPatch(typeof(GameLocation), "localSoundAt")]
    public class SoundPatches
    {
        public static IEnumerable<Horse> GetHorsesIn(GameLocation location)
        {
            if (!Context.IsMultiplayer)
            {
                return from h in ((IEnumerable)location.characters).OfType<Horse>()
                       select h;
            }
            return (from h in ((IEnumerable)location.characters).OfType<Horse>()
                    select h).Concat(from player in (IEnumerable<Farmer>)location.farmers
                                     where player.mount != null
                                     select player.mount).Distinct();
        }

        private static IMonitor Monitor;

        // call this method from your Entry class
        public static void Initialize(IMonitor monitor)
        {
            Monitor = monitor;
        }
        static ModConfig config = new ModConfig();
        static string bikeName = config.BikeName;

        public static void localSoundAt_prefix(GameLocation __instance, ref string audioName, Vector2 position)
        {
            foreach (Horse horse1 in GetHorsesIn(__instance))
            {
                if (audioName.EndsWith("step", StringComparison.InvariantCultureIgnoreCase) && horse1.getTileLocation() == position && horse1.rider != null && horse1.Name == bikeName)
                {
                    audioName = "vroom";
                }
            }

        }
    }
}