using System;

namespace OliveTree.Transitions.Curves
{
    public class ExponentialCurve : EasingCurve
    {
        public override Easing Easing
        {
            get { return Easing.Exponential; }
            set { }
        }

        public int Factor { get; set; } = 2;
        protected override double EaseIn(double time) => Ease(time, Factor);

        internal static double Ease(double time, double factor)
             => (Math.Exp(factor * time) - 1.0) / (Math.Exp(factor) - 1.0);
    }
}