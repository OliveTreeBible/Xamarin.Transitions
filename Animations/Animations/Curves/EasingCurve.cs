using System;

namespace OliveTree.Animations.Curves
{
    public class EasingCurve : AnimationCurve
    {
        public virtual Easing Easing { get; set; } = Easing.Linear;
        public EasingMode Mode { get; set; } = EasingMode.Out;

        public double Ease(double time)
        {
            //http://referencesource.microsoft.com/#PresentationCore/Core/CSharp/System/Windows/Media/Animation/EasingFunctionBase.cs
            switch (Mode)
            {
                case EasingMode.In:
                    return EaseIn(time);
                case EasingMode.Out:
                    return 1.0 - EaseIn(1.0 - time);
                case EasingMode.InOut:
                    return time < 0.5 ? EaseIn(time * 2) * 0.5 : (1.0 - EaseIn((1.0 - time) * 2.0)) * 0.5 + 0.5;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected virtual double EaseIn(double time)
        {
            switch (Easing)
            {
                default:
                case Easing.Linear:
                    return time;

                case Easing.Quadratic: return Math.Pow(time, 2);
                case Easing.Cubic: return Math.Pow(time, 3);
                case Easing.Quartic: return Math.Pow(time, 4);
                case Easing.Quintic: return Math.Pow(time, 5);

                case Easing.Sine: return 1.0 - Math.Sin(Math.PI * 0.5 * (1.0 - time));
                case Easing.Circle: return 1.0 - Math.Sqrt(1.0 - time * time);

                case Easing.Power: return PowerCurve.Ease(time, 2.0);
                case Easing.Exponential: return ExponentialCurve.Ease(time, 2.0);
                case Easing.Back: return BackCurve.Ease(time, 1.0);
                case Easing.Elastic: return ElasticCurve.Ease(time, 3.0, 3.0);
            }
        }
    }   
}