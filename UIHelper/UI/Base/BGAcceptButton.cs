using Microsoft.Xna.Framework;

namespace UIHelper
{
    internal class BGAcceptButton : BGButton
    {
        internal BGAcceptButton(Rectangle bounds, Action closeAction, Action retAction) : base(bounds, closeAction, retAction)
        {
            btnSrc = new(128, 256, 64, 64);
        }
    }
}
