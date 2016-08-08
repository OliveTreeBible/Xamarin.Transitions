using System;

namespace OliveTree.Transitions.Curves
{
    public class ElasticCurve : EasingCurve
    {
        public override Easing Easing
        {
            get { return Easing.Elastic; }
            set { }
        }

        public int Springiness { get; set; } = 3;
        public int Oscillations { get; set; } = 3;
        protected override double EaseIn(double time) => Ease(time, Springiness, Oscillations);

        internal static double Ease(double time, double spring, double oscillations)
            => (Math.Exp(spring * time) - 1 / (Math.Exp(spring) - 1)) * Math.Sin((Math.PI * 2 * oscillations + Math.PI * 0.5) * time);
    } 
}