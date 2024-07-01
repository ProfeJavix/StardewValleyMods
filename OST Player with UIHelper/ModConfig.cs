using StardewModdingAPI.Utilities;

namespace OSTPlayer
{
    public sealed class ModConfig
    {
        public bool ModEnabled { get; set; }
        public KeybindList ToggleKey { get; set; }
        public bool ProgressiveMode{get;set;}
        public KeybindList SkipKey {get; set;}
        public bool RandomSkip {get; set;}
        public KeybindList StopKey {get; set;}

        public ModConfig() 
        { 
            ModEnabled = true;
            ToggleKey = KeybindList.Parse("Insert");
            ProgressiveMode = true;
            SkipKey = KeybindList.Parse("P");
            RandomSkip = false;
            StopKey = KeybindList.Parse("O");
        }

    }
}
