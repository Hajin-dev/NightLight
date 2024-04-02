using StardewModdingAPI.Utilities;

namespace NightLight
{
    internal class ModConfig
    {
        public bool NightLightEnabled { get; set; } = true;
        public bool NightLightOutdoors {  get; set; } = true;
        public bool NightLightMines { get; set; } = true;
        public KeybindList NightLightToggleAllKey { get; set; } = KeybindList.Parse("LeftAlt + L");
        public KeybindList NightLightToggleOutdoorsKey { get; set; } = KeybindList.Parse("LeftAlt + O");
        public KeybindList NightLightToggleMinesKey { get; set; } = KeybindList.Parse("LeftAlt + U");
    }
}
