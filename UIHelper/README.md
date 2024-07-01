**UI Helper**: A dependency to help you save time and effort making your SMAPI UI

To use it, you'll need to ***copy the IUIHelperApi.cs to your SMAPI project folder***. Then, in ModEntry you have to ***create an instance and initialize it*** like any other API:
>IUIHelperApi api = Helper.ModRegistry.GetApi\<IUIHelperApi\>("ProfeJavix.UIHelper");

(it's recommended to do this on Gameloop.GameLaunched event or later)

When starting StardewValley.exe, make sure you have the Nexus's file in Mods folder.
And that's it! You'll have access to a set of functionalities that will make easier the UI building :grin:. Every method and param its explained in the file.

You have an example of use in "OST Player with UIHelper" folder.
