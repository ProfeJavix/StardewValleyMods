
namespace OSTPlayer
{
    public class Song : IComparable<Song>
    {
        public string name{set;get;}
        public bool isPlaying;

        public Song(string name, bool isPlaying = false){
            this.name = name;
            this.isPlaying = isPlaying;
        }

        public int CompareTo(Song other){
            return string.Compare(name, other.name, StringComparison.OrdinalIgnoreCase);
        }
    }
}
