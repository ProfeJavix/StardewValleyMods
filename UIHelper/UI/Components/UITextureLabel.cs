﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UIHelper
{
    internal class UITextureLabel : UILabel
    {
        private Texture2D icon;
        private Rectangle? srcRect;
        internal UITextureLabel(Texture2D icon, Rectangle initialBounds, bool hasBox, Color? borderColor, Rectangle? sourceRect, EAlign align) : base(null, initialBounds, hasBox, null, null, borderColor, align)
        {  
            this.icon = icon;
            this.srcRect = sourceRect;
        }

        public override void draw(SpriteBatch b)
        {
            if(hasBox)
                UIUtils.DrawBox(b, Bounds, borderColor, 0);
            if(icon != null)
                b.Draw(icon, GetBoundsWithBorder(hasBox), srcRect, Color.White);
        }
    }
}
