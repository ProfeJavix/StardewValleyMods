using StardewModdingAPI;

namespace UIHelper
{
    internal class Console
    {
        internal static void Log(string message, LogLevel level = LogLevel.Warn){
            ModEntry.context.Monitor.Log(message, level);
        }
    }
}
