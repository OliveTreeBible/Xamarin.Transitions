using System;

namespace OliveTree.Animations.Curves
{
    public class BackCurve : EasingCurve
    {
        public override Easing Easing
        {
            get { return Easing.Back; }
            set { }
        }

        public double Amplitude { get; set; } = 1.0d;
        protected override double EaseIn(double time) => Ease(time, Amplitude);

        internal static double Ease(double time, double amplitude)
            => Math.Pow(time, 3) - time * amplitude * Math.Sin(Math.PI * time);
    }
}