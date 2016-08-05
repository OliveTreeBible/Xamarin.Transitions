using Foundation;
using OliveTree.Animations.Curves;
using OliveTree.Animations.iOS.Interpolators;
using System;
using System.Collections.Generic;
using System.Linq;
using CoreAnimation;

namespace OliveTree.Animations.iOS
{
    public class SpringAnimation : CAKeyFrameAnimation
    {
        private Lazy<double[]> _steps;
        private Spring _spring;
        private IInterpolator _interpolator;

        public override NSObject Copy(NSZone zone)
        {
            var copy = (SpringAnimation)base.Copy(zone);

            copy._steps = _steps;
            copy._spring = _spring;
            copy._interpolator = _interpolator;
            copy.Speed = Speed * (float)(1.0 / EasingAnimation.DragCoefficient());
            return copy;
        }

        public SpringAnimation(Spring spring, IInterpolator interpolator)
        {
            _steps = new Lazy<double[]>(() => spring.Steps(EasingAnimation.FramesPerSecond).ToArray());
            _spring = spring;
            _interpolator = interpolator;
        }

        protected internal SpringAnimation(IntPtr handle)
            : base(handle)
        { }

        public override NSObject[] Values
        {
            get { return _steps.Value.Select(_interpolator.ProvideValue).ToArray(); }
            set { }
        }

        public override double Duration
        {
            get { return (double)_steps.Value.Length / EasingAnimation.FramesPerSecond; }
            set { }
        }
    }
}