using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using CoreAnimation;
using OliveTree.Transitions.Curves;
using OliveTree.Transitions.iOS.Animations.Interpolators;

namespace OliveTree.Animations.iOS
{
    /// <summary>
    /// Inspired by Objective-C examples with looking at leveraging C# concepts to simplify use.
    ///     https://github.com/bryanoltman/CAAnimation-EasingEquations
    ///     http://www.dzone.com/snippets/robert-penner-easing-equations
    ///     https://github.com/robb/RBBAnimation
    /// </summary>
    public class EasingAnimation : CAKeyFrameAnimation
    {
#if DEBUG
        [System.Runtime.InteropServices.DllImport(ObjCRuntime.Constants.UIKitLibrary, EntryPoint = "UIAnimationDragCoefficient")]
        protected internal static extern float DragCoefficient();
#else
        protected internal static nfloat DragCoefficient() => 1.0f;
#endif


        protected internal const int FramesPerSecond = 60 * 4; //Probably need more than the 60 refresh rate, but 4 per frame may be excessive

        private EasingCurve _curve;
        private IInterpolator _interpolator;

        public override NSObject Copy(NSZone zone)
        {
            var copy = (EasingAnimation)base.Copy(zone);

            copy._curve = _curve;
            copy._interpolator = _interpolator;

            var drag = (float)(1.0 / DragCoefficient());
            copy.Speed = Speed * drag;
            return copy;
        }

        public EasingAnimation(EasingCurve curve, IInterpolator interpolator)
        {
            _curve = curve ?? new EasingCurve();
            _interpolator = interpolator;
        }

        // ReSharper disable once UnusedMember.Global - animation system creates this one
        protected internal EasingAnimation(IntPtr handle)
            : base(handle)
        { }

        public override NSObject[] Values
        {
            get
            {
                return Steps(TimeSpan.FromSeconds(Duration))
                    .Select(_curve.Ease)
                    .Select(_interpolator.ProvideValue)
                    .ToArray();
            }
            set { }
        }

        private static IEnumerable<double> Steps(TimeSpan duration)
        {
            var stepCount = duration.TotalSeconds * FramesPerSecond;
            var stepSize = 1 / stepCount;

            yield return 0;

            var step = 0d;
            while ((step += stepSize) < 1)
                yield return step;

            yield return 1;
        }
    }
}