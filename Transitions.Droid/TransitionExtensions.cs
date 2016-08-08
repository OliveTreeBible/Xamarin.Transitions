using System;
using System.Linq;
using Android.Animation;
using OliveTree.Transitions.Curves;
using Object = Java.Lang.Object;

namespace OliveTree.Transitions.Droid
{
    public static class TransitionExtensions
    {
        private const int FramesPerSecond = 60 * 4; //Probably need more than the 60 refresh rate, but 4 per frame may be excessive


        public static ITimeInterpolator GetInterpolator(this TransitionBase transition)
        {
            var spring = transition.Curve as Spring;
            if (spring != null) return new SpringInterpolator(spring);

            var easing = transition.Curve as EasingCurve ?? new EasingCurve();
            return new EasingInterpolator(easing);
        }

        public static long CalculateDuration(this TransitionBase transition)
        {
            var spring = transition.Curve as Spring;
            if (spring != null)
            {
                const int framesPerSecond = FramesPerSecond;
                var seconds = spring.Steps(framesPerSecond).Count() / (double)framesPerSecond;

                return (long)(seconds * 1000);
            }

            return (long)transition.Duration.TotalMilliseconds;
        }


        private class EasingInterpolator : Object, ITimeInterpolator
        {
            private readonly EasingCurve _curve;

            public EasingInterpolator(EasingCurve curve)
            {
                _curve = curve;
            }

            public float GetInterpolation(float input) => (float)_curve.Ease(input);
        }

        private class SpringInterpolator : Object, ITimeInterpolator
        {
            private readonly double[] _steps;

            public SpringInterpolator(Spring curve)
            {
                _steps = curve.Steps(FramesPerSecond).ToArray();
            }

            public float GetInterpolation(float input)
            {
                var rawFrame = input * _steps.Length;

                var idxA = Math.Max(Math.Min((int)rawFrame, _steps.Length - 1), 0);
                return (float)_steps[idxA];
            }
        }
    }
}