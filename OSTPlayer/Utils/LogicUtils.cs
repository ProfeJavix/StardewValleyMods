using Microsoft.Xna.Framework.Audio;
using StardewValley;
using StardewValley.GameData;

namespace OSTPlayer
{
    public class LogicUtils
    {
        private static HashSet<string> songNames = new HashSet<string>(){
            "50s",
            "AbigailFlute",
            "AbigailFluteDuet",
            "aerobics",
            "archaeo",
            "bigDrums",
            "breezy",
            "caldera",
            "Cavern",
            "christmasTheme",
            "CloudCountry",
            "ClubLoop",
            "cowboy_boss",
            "cowboy_outlawsong",
            "Cowboy_OVERWORLD",
            "Cowboy_singing",
            "Cowboy_undead",
            "crane_game",
            "crane_game_fast",
            "Crystal Bells",
            "desolate",
            "distantBanjo",
            "echos",
            "elliottPiano",
            "EmilyDance",
            "EmilyDream",
            "EmilyTheme",
            "end_credits",
            "event1",
            "event2",
            "fall1",
            "fall2",
            "fall3",
            "fallFest",
            "fieldofficeTentMusic",
            "FlowerDance",
            "FrogCave",
            "Ghost Synth",
            "grandpas_theme",
            "gusviolin",
            "harveys_theme_jazz",
            "heavy",
            "honkytonky",
            "Icicles",
            "IslandMusic",
            "jaunty",
            "junimoKart",
            "junimoKart_ghostMusic",
            "junimoKart_mushroomMusic",
            "junimoKart_slimeMusic",
            "junimoKart_whaleMusic",
            "junimoStarSong",
            "kidadumbautumn",
            "libraryTheme",
            "MainTheme",
            "Majestic",
            "MarlonsTheme",
            "marnieShop",
            "mermaidSong",
            "moonlightJellies",
            "movie_classic",
            "movie_nature",
            "movie_wumbus",
            "movieTheater",
            "movieTheaterAfter",
            "musicboxsong",
            "Near The Planet Core",
            "night_market",
            "Of Dwarves",
            "Overcast",
            "PIRATE_THEME",
            "playful",
            "Plums",
            "poppy",
            "ragtime",
            "sad_kid",
            "sadpiano",
            "Saloon1",
            "sam_acoustic1",
            "sam_acoustic2",
            "sampractice",
            "sappypiano",
            "Secret Gnomes",
            "SettlingIn",
            "shaneTheme",
            "shimmeringbastion",
            "spaceMusic",
            "spirits_eve",
            "spring1",
            "spring2",
            "spring3",
            "springtown",
            "Stadium_ambient",
            "starshoot",
            "submarine_song",
            "summer1",
            "summer2",
            "summer3",
            "SunRoom",
            "sweet",
            "tickTock",
            "tinymusicbox",
            "title_night",
            "tribal",
            "Tropical Jam",
            "VolcanoMines1",
            "VolcanoMines2",
            "wavy",
            "wedding",
            "winter1",
            "winter2",
            "winter3",
            "WizardSong",
            "woodsTheme",
            "XOR"
        };
        public static List<Song> GetAllSongs()
        {
            List<Song> songs = new List<Song>();
            foreach(string name in songNames){
                songs.Add(new Song(name));
            }
            return songs;
        }

        // Removes song added by default while playing a track
        public static void removeHeardSong(string songName)
        {

            HashSet<string> songsHeard = Game1.player.songsHeard;
            switch (songName)
            {
                case "EarthMine":
                    songsHeard.Remove("Crystal Bells");
                    songsHeard.Remove("Cavern");
                    songsHeard.Remove("Secret Gnomes");
                    break;
                case "FrostMine":
                    songsHeard.Remove("Cloth");
                    songsHeard.Remove("Icicles");
                    songsHeard.Remove("XOR");
                    break;
                case "LavaMine":
                    songsHeard.Remove("Of Dwarves");
                    songsHeard.Remove("Near The Planet Core");
                    songsHeard.Remove("Overcast");
                    songsHeard.Remove("tribal");
                    break;
                case "VolcanoMines":
                    songsHeard.Remove("VolcanoMines1");
                    songsHeard.Remove("VolcanoMines2");
                    break;
                default:
                    songsHeard.Remove(songName);

                    break;
            }
        }
    }
}
