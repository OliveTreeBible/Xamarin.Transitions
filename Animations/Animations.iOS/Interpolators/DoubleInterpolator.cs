using Foundation;

namespace OliveTree.Animations.iOS.Interpolators
{
    public class DoubleInterpolator : IInterpolator<double>
    {
        public double From { get; set; }
        public double To { get; set; }

        NSObject IInterpolator.ProvideValue(double delta)
        {
            var td = To - From;
            return NSNumber.FromDouble(td * delta + From);
        }
    }
}