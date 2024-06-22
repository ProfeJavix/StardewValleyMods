
namespace OSTPlayer
{
    public class Song : IComparable<Song>
    {
        public string Id{get;set;}
        public string Name{get; set;}
        public bool isPlaying;
        public bool HasName;

        public Song(string id, string name, bool isPlaying = false){
            Id = id;
            Name = name ?? id;
            HasName = name != null;
            this.isPlaying = isPlaying;
        }

        public int CompareTo(Song other){
            return LogicUtils.CompareSongsByPos(this, other);
        }
    }
}
