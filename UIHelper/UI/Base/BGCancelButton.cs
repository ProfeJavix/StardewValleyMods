using Microsoft.Xna.Framework;

namespace UIHelper
{
    internal class BGCancelButton : BGButton
    {
        internal BGCancelButton(Rectangle bounds, Action closeAction, Action retAction) : base(bounds, closeAction, retAction)
        {
            btnSrc = new(192, 256, 64, 64);
        }
    }
}
