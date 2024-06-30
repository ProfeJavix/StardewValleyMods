using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace UIHelper
{
    public class ModEntry : Mod
    {
        public static ModEntry context;
        public IModHelper helper;
        private readonly Dictionary<string, ModUIFeatures> modsFeatures = new();

        public override void Entry(IModHelper helper)
        {
            context = this;
            this.helper = helper;

            helper.Events.Display.WindowResized += OnWindowResized;
        }

        private void OnWindowResized(object? sender, WindowResizedEventArgs e)
        {
            foreach (KeyValuePair<string, ModUIFeatures> item in modsFeatures)
            {
                item.Value?.ReadjustMenu();
            }
        }

        public override object? GetApi(IModInfo mod)
        {
            return new ModApi(mod.Manifest.UniqueID, modsFeatures);
        }
    }
}
