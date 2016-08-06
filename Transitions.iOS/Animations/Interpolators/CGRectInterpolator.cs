using CoreGraphics;
using Foundation;

namespace OliveTree.Animations.iOS.Interpolators
{
    public class RectInterpolator : IInterpolator<CGRect>
    {
        public CGRect From { get; set; }
        public CGRect To { get; set; }

        NSObject IInterpolator.ProvideValue(double delta)
        {
            var tdx = To.X - From.X;
            var tdy = To.Y - From.Y;
            var tdw = To.Width - From.Width;
            var tdh = To.Height - From.Height;
            return NSValue.FromCGRect(new CGRect(
                tdx * delta + From.X,
                tdy * delta + From.Y,
                tdw * delta + From.Width,
                tdh * delta + From.Height
            ));
        }
    }
}