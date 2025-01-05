using StardewModdingAPI.Utilities;

namespace TreeOrganizer
{
    public sealed class ModConfig
    {
        public bool ModEnabled { get; set; }
        public KeybindList GrabTreeKey { get; set; }

        public ModConfig() 
        { 
            ModEnabled = true;
            GrabTreeKey = KeybindList.Parse("LeftShift + O");
        }

    }
}
