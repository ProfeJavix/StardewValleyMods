using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace OSTPlayer
{
    public class SongLabel: ClickableTextureComponent
    {
        private static Texture2D playIcon = null, stopIcon = null;
        private Rectangle playArea, nameArea;
        private Song song;
        private bool highlighted = false;
        private bool clicked = false;

        public Song Song{
            get{return song;}
        }
        public bool IsPlaying{
            get{return Song.isPlaying;}
        }
        public bool Highlighted{
            get{return highlighted;}
            set{highlighted = value;}
        }

        public bool Clicked{
            get{return clicked;}
            set{clicked = value;}
        }
        public int index;

        public Action<SongLabel, bool> OnClickAction;

        public SongLabel(int index, Song song, Action<SongLabel, bool> action): base(new(0, 0, 1000, 150), null, Rectangle.Empty, 1)
        {
            this.index = index;
            this.song = song;
            OnClickAction += action;
            if (playIcon == null || stopIcon == null)
                loadIcons();
        }

        private void loadIcons()
        {
            if (playIcon == null)
            {
                playIcon = ModEntry.context.Helper.ModContent.Load<Texture2D>("assets/playIcon.png");
            }

            if (stopIcon == null)
            {
                stopIcon = ModEntry.context.Helper.ModContent.Load<Texture2D>("assets/stopIcon.png");
            }
        }

        private Texture2D getPlayBoxIcon(bool isPlaying)
        {
            return isPlaying ? stopIcon : playIcon;
        }

        private void setAreas()
        {
            playArea = new Rectangle(
                bounds.X+16,
                bounds.Y+16,
                bounds.Height-32,
                bounds.Height-32
            );

            nameArea = new Rectangle(
                playArea.Right + 16,
                playArea.Y,
                bounds.Width - bounds.Height - 32 - 16,
                bounds.Height - 32
            );
        }

        private Color getIconColor(){
            return song.isPlaying? Color.Red : Color.MediumSeaGreen;
        }

        private Color getHighlightColor(Color color){
            if(Highlighted){
                if(Clicked)
                    return UIUtils.getHighLightColor(color, 1.6f);
                return UIUtils.getHighLightColor(color);
            }

            return color;
        }

        public override void draw(SpriteBatch b)
        {
            setAreas();
            UIUtils.DrawBox(b, playArea, getHighlightColor(Color.SaddleBrown));
            UIUtils.DrawBox(b, nameArea, getHighlightColor(Color.SaddleBrown));
            b.Draw(getPlayBoxIcon(song.isPlaying), playArea, getHighlightColor(getIconColor()));
            b.DrawString(Game1.dialogueFont, song.Name, UIUtils.getCenteredText(nameArea, song.Name), getHighlightColor(Color.Black));
        }

        public override bool containsPoint(int x, int y)
        {
            return bounds.Contains(x, y);
        }

        public override void tryHover(int x, int y, float maxScaleIncrease = 0.1F)
        {
            Highlighted = containsPoint(x,y);
        }

        public void receiveLeftClick(int x, int y){
            Game1.playSound("select");
            Clicked = containsPoint(x,y);
        }

        public void releaseLeftClick(int x, int y){
            Clicked = false;
            if(containsPoint(x, y)){
                song.isPlaying = !song.isPlaying;
                OnClickAction?.Invoke(this, song.isPlaying);
            }
        }
    }
}
