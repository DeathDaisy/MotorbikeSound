using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace GenericModConfigMenu
{
    public interface IGenericModConfigMenuApi
    {
        void RegisterModConfig(IManifest mod, Action revertToDefault, Action saveToFile);
        void UnregisterModConfig(IManifest mod);

        void SetDefaultIngameOptinValue(IManifest mod, bool optedIn);

        void RegisterSimpleOption(IManifest mod, string optionName, string optionDesc, Func<string> optionGet, Action<string> optionSet);

        /// <summary>Get the currently-displayed mod config menu, if any.</summary>
        /// <param name="mod">The manifest of the mod whose config menu is being shown, or <c>null</c> if not applicable.</param>
        /// <param name="page">The page ID being shown for the current config menu, or <c>null</c> if not applicable. This may be <c>null</c> even if a mod config menu is shown (e.g. because the mod doesn't have pages).</param>
        /// <returns>Returns whether a mod config menu is being shown.</returns>
        bool TryGetCurrentMenu(out IManifest mod, out string page);
    }
}
