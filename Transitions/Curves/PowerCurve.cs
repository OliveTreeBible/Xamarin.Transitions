using System;

namespace OliveTree.Transitions.Curves
{
    public class PowerCurve : EasingCurve
    {
        public override Easing Easing
        {
            get { return Easing.Power; }
            set { }
        }

        public int Power { get; set; } = 2;
        protected override double EaseIn(double time) => Ease(time, Power);

        internal static double Ease(double time, double power) => Math.Pow(time, power);
    }
}