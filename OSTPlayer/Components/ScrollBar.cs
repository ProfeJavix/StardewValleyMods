using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace OSTPlayer
{
    public class ScrollBar: ClickableTextureComponent
    {

        public bool IsScrolling{get; set;}
        public int CurrentPos{
            get{return bounds.Y;}
            set{bounds.Y = value;}
        }


        public ScrollBar(Rectangle bounds, float scale = 5f) : base(bounds, Game1.mouseCursors, new Rectangle(435, 463, 6, 10), scale){
            IsScrolling = false;
        }
    }
}
