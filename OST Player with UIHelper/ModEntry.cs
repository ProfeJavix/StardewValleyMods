using GenericModConfigMenu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData;
using UIHelper;

namespace OSTPlayer
{
    internal sealed class ModEntry : Mod
    {

        public static ModEntry? context;

        private ModConfig config;

        private static IMobilePhoneApi? mobileApi;
        private static IUIHelperApi uiApi;

        private IModHelper helper;

        public static List<Song> songs = new List<Song>();
        private bool changingSong = false;
        private int songsHeardCount = 0;

        private int playingIndex = -1;

        public override void Entry(IModHelper helper)
        {
            context = this;
            this.helper = helper;

            config = helper.ReadConfig<ModConfig>();

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.DayStarted += OnDayStarted;

            if (!config.ModEnabled)
                return;

            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.GameLoop.OneSecondUpdateTicked += OnOneSecondUpdateTicked;

        }

        private void OnOneSecondUpdateTicked(object? sender, OneSecondUpdateTickedEventArgs e)
        {
            if(!Context.IsWorldReady || changingSong)
                return;
            
            if(Game1.currentSong != null && Game1.currentSong.IsStopped){
                PlayNextTrack();
            }

            if(Game1.player.songsHeard.Count != songsHeardCount){
                songsHeardCount = Game1.player.songsHeard.Count;
                LoadSongs(config.ProgressiveMode);
            }
        }

        private void PlayNextTrack()
        {
            if(playingIndex != -1){
                int index = playingIndex;
                if(!config.RandomSkip){
                    index ++;
                    if(songs.Count <= index)
                        index = 0;
                }else{
                    List<int> indexes = Enumerable.Range(0, songs.Count).Where(i => i != playingIndex).ToList();
                    Random randomIdx = new Random();
                    index = indexes[randomIdx.Next(indexes.Count)];
                }

                PlaySelectedOST(index);
            }
        }

        private void OnDayStarted(object? sender, DayStartedEventArgs e)
        {
            changingSong = true;
            songsHeardCount = Game1.player.songsHeard.Count;
            changingSong = false;
        }

        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            LoadUiApi();
            LoadConfig();
            AddAppToMobile();
        }

        private void LoadUiApi()
        {
            uiApi = Helper.ModRegistry.GetApi<IUIHelperApi>("ProfeJavix.UIHelper");
            if(uiApi == null)
                throw new Exception("UI does not exist.");
        }

        private void LoadSongs(bool progressiveMode)
        {
            songs = LogicUtils.GetAllSongs();
            if (progressiveMode)
            {
                HashSet<string> heardSongs = Game1.player.songsHeard;
                songs = songs.Where(s => heardSongs.Contains(s.Id)).ToList();
                songs.Sort();
            }
            InitUI();
        }

        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)//
        {
            if (!Context.IsWorldReady)
                return;

            if (config.ToggleKey.JustPressed())
                OpenOSTPlayer();
            if(config.SkipKey.JustPressed()){
                if(playingIndex != -1)
                    Game1.playSound("select");
                PlayNextTrack();
            
            }
            if (config.StopKey.JustPressed() && playingIndex != -1){
                Game1.playSound("drumkit6");
                PlayDefaultOST();
            }
            
        }

        private void LoadConfig()
        {
            IGenericModConfigMenuApi configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu == null)
                return;

            configMenu.Register(
                ModManifest,
                () => config = new ModConfig(),
                () => Helper.WriteConfig(config)
            );

            configMenu.AddBoolOption(
                ModManifest,
                () => config.ModEnabled,
                value => config.ModEnabled = value,
                () => Helper.Translation.Get("config.enabled"),
                () => Helper.Translation.Get("config.enabled.tooltip"),
                "modEnabled"
            );

            configMenu.AddKeybindList(
                ModManifest,
                () => config.ToggleKey,
                value => config.ToggleKey = value,
                () => Helper.Translation.Get("config.toggleKey"),
                () => Helper.Translation.Get("config.togglekey.tooltip")
            );

            configMenu.AddBoolOption(
                ModManifest,
                () => config.ProgressiveMode,
                value => config.ProgressiveMode = value,
                () => Helper.Translation.Get("config.progressivemode"),
                () => Helper.Translation.Get("config.progressivemode.tooltip"),
                "progressiveMode"
            );

            configMenu.AddKeybindList(
                ModManifest,
                () => config.SkipKey,
                value => config.SkipKey = value,
                () => Helper.Translation.Get("config.skipkey"),
                () => Helper.Translation.Get("config.skipkey.tooltip")
            );

            configMenu.AddBoolOption(
                ModManifest,
                () => config.RandomSkip,
                value => config.RandomSkip = value,
                () => Helper.Translation.Get("config.randomskip"),
                () => Helper.Translation.Get("config.randomskip.tooltip")
            );

            configMenu.AddKeybindList(
                ModManifest,
                () => config.StopKey,
                value => config.StopKey = value,
                () => Helper.Translation.Get("config.stopkey"),
                () => Helper.Translation.Get("config.stopkey.tooltip")
            );

            configMenu.OnFieldChanged(ModManifest, OnOptionChanged);

        }

        private void AddAppToMobile()
        {
            mobileApi = helper.ModRegistry.GetApi<IMobilePhoneApi>("JoXW.MobilePhone");

            if (mobileApi != null)
            {
                Texture2D icon = helper.ModContent.Load<Texture2D>(Path.Combine("assets", "appIcon.png"));
                bool ok = mobileApi.AddApp(helper.ModRegistry.ModID, "OST Player", OpenOSTPlayer, icon);
                Monitor.Log(ok ? "app cargada con exito" : "app no cargada", LogLevel.Debug);
            }
        }

        private void OpenOSTPlayer()
        {
            uiApi.OpenUI();
        }

        private void InitUI(){
            uiApi.InitUIFeatures(1300, 600, Color.SaddleBrown, Color.SaddleBrown, true, 100);
            uiApi.AddTitleToPage("OST Player", 0, null, true, Color.Wheat, Color.SaddleBrown);
            uiApi.ConfigPageScrollArea(true, true, 0, Color.Wheat, Color.SaddleBrown);
            InitSongLabels();
            uiApi.SetValueChangedAction(PlayingToggled);
        }



        private void PlayingToggled(string id, object value)
        {
            bool playing = (bool)value;
            if(playing){
                Monitor.Log("Entered PlayingToggled", LogLevel.Debug);
                int.TryParse(id, out int index);
                int i = 0;
                foreach(Song s in songs){
                    if(i != index){
                        s.isPlaying = false;
                        uiApi.SetComponentValue(i.ToString(), false);
                    }
                }
            }
        }

        private void InitSongLabels(){

            int id = 0;
            foreach (Song song in songs)
            {
                SongLabel songlbl = new SongLabel(id, song, SongClicked);
                uiApi.AddCustomComponent(songlbl, song.isPlaying, 0, 'C', id.ToString());
                uiApi.SetCustomLeftClickAction(id.ToString(), songlbl.receiveLeftClick);
                uiApi.SetCustomLeftClickReleasedAction(id.ToString(), songlbl.releaseLeftClick);
                if(song.isPlaying)
                    playingIndex = id;
                id++;
            }
        }

        private void SongClicked(SongLabel sl, bool playing){
            if(playing){
                PlayDefaultOST();
            }else{
                PlaySelectedOST(sl.index);
            }
            uiApi.SetComponentValue(sl.index.ToString(), sl.index != playingIndex);
        }

        private void PlayDefaultOST()
        {
            changingSong = true;
            if(playingIndex != -1){
                Game1.stopMusicTrack(MusicContext.MusicPlayer);
                Game1.addHUDMessage(new OSTHUDMessage($"{Helper.Translation.Get("ost.stopped")}.", false));
                songs.ElementAt(playingIndex).isPlaying = false;
                playingIndex = -1;
            }
            changingSong = false;
        }
        private void PlaySelectedOST(int index){

            Song song = songs.ElementAt(index);
            bool songHeard = Game1.player.songsHeard.Contains(song.Id);

            changingSong = true;
            Game1.changeMusicTrack(song.Id, false, MusicContext.MusicPlayer);

            if(playingIndex != -1)
                songs.ElementAt(playingIndex).isPlaying = false;

            songs.ElementAt(index).isPlaying = true;
            playingIndex = index;

            if(!config.ProgressiveMode && !songHeard){
                LogicUtils.removeHeardSong(song.Id);
            }
            Game1.addHUDMessage(new OSTHUDMessage($"{Helper.Translation.Get("ost.playing")} {song.Name}.", true));
            changingSong = false;
        }

        private void OnOptionChanged(string fieldId, object newValue)
        {
            if (fieldId == "modEnabled")
            {
                PlayDefaultOST();

                bool on = (bool)newValue;

                if (on)
                {
                    helper.Events.Input.ButtonPressed += OnButtonPressed;
                    helper.Events.GameLoop.OneSecondUpdateTicked += OnOneSecondUpdateTicked;
                }
                else
                {
                    helper.Events.Input.ButtonPressed -= OnButtonPressed;
                    helper.Events.GameLoop.OneSecondUpdateTicked -= OnOneSecondUpdateTicked;
                }
            }
            else if (fieldId == "progressiveMode")
            {
                PlayDefaultOST();
                LoadSongs((bool)newValue);
            }
        }


    }
}
