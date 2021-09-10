using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
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

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            var harmony = new Harmony(this.ModManifest.UniqueID);


            harmony.Patch(
                                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.localSoundAt)),
                                prefix: new HarmonyMethod(typeof(SoundPatches), nameof(SoundPatches.localSoundAt_prefix))
                            );

        }

        /*private IEnumerable<Horse> GetHorsesIn(GameLocation location)
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
        }*/

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

            public static void localSoundAt_prefix(GameLocation __instance, ref string audioName, Vector2 position)
            {
                foreach (Horse horse1 in GetHorsesIn(__instance))
                {
                    //if (audioName.EndsWith("step", StringComparison.InvariantCultureIgnoreCase) && __instance.characters.OfType<Horse>().Union(new[] { Game1.player.mount }).FirstOrDefault(h => h != null && h.getTileLocation() == position) is Horse horse && horse != null && horse.Name == "Ducati")
                    if (audioName.EndsWith("step", StringComparison.InvariantCultureIgnoreCase) && horse1.getTileLocation() == position && horse1.rider != null && horse1.Name == "Ducati")
                    {
                        audioName = "vroom";
                    }
                }

            }
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>

    }
}