using CoreGraphics;
using Foundation;

namespace OliveTree.Animations.iOS.Interpolators
{
    public class PointInterpolator : IInterpolator<CGPoint>
    {
        public CGPoint From { get; set; }
        public CGPoint To { get; set; }

        NSObject IInterpolator.ProvideValue(double delta)
        {
            var tdx = To.X - From.X;
            var tdy = To.Y - From.Y;
            return NSValue.FromCGPoint(new CGPoint(tdx*delta + From.X, tdy*delta + From.Y));
        }
    }
}