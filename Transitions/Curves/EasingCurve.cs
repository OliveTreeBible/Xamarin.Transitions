using System;

namespace OliveTree.Transitions.Curves
{
    public class EasingCurve : AnimationCurve
    {
        public virtual Easing Easing { get; set; } = Easing.Linear;
        public EasingMode Mode { get; set; } = EasingMode.Out;

        //http://referencesource.microsoft.com/#PresentationCore/Core/CSharp/System/Windows/Media/Animation/EasingFunctionBase.cs
        public double Ease(double time) => Mode switch
        {
            EasingMode.In => EaseIn(time),
            EasingMode.Out => 1.0 - EaseIn(1.0 - time),
            EasingMode.InOut => time < 0.5 ? EaseIn(time * 2) * 0.5 : (1.0 - EaseIn((1.0 - time) * 2.0)) * 0.5 + 0.5,
            _ => throw new InvalidOperationException(),
        };

        protected virtual double EaseIn(double time) => Easing switch
        {
            Easing.Quadratic => Math.Pow(time, 2),
            Easing.Cubic => Math.Pow(time, 3),
            Easing.Quartic => Math.Pow(time, 4),
            Easing.Quintic => Math.Pow(time, 5),
            Easing.Sine => 1.0 - Math.Sin(Math.PI * 0.5 * (1.0 - time)),
            Easing.Circle => 1.0 - Math.Sqrt(1.0 - time * time),
            Easing.Power => PowerCurve.Ease(time, 2.0),
            Easing.Exponential => ExponentialCurve.Ease(time, 2.0),
            Easing.Back => BackCurve.Ease(time, 1.0),
            Easing.Elastic => ElasticCurve.Ease(time, 3.0, 3.0),
            _ => time,
        };
    }   
}