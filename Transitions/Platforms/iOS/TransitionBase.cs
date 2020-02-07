using System;
using Foundation;
using CoreAnimation;
using OliveTree.Animations.iOS;
using OliveTree.Transitions.Curves;
using OliveTree.Transitions.iOS.Animations.Interpolators;
using UIKit;
using Xamarin.Forms.Platform.iOS;

namespace OliveTree.Transitions.iOS
{
    public abstract class TransitionBase : NSObject, ITransitionHandler
    {
        public event EventHandler Completed;
        protected Transitions.TransitionBase Transition { get; private set; }
        private UIView Renderer { get; set; }

        void ITransitionHandler.Attach(Transitions.TransitionBase transition)
        {
            if (ReferenceEquals(Transition, transition)) return;

            var t = Transition;
            if (t != null)
                t.Transitioning -= OnTransitioning;

            Transition = transition;

            if (transition != null)
                transition.Transitioning += OnTransitioning;

            var element = transition?.Element;
            Renderer = element != null ? Platform.GetRenderer(element)?.NativeView : null;
        }

        protected virtual void BeganAnimation(UIView target) { }
        protected virtual void EndingAnimation(UIView target) { }

        private void OnTransitioning(object sender, bool transitioning)
        {
            var r = Renderer;
            if (r == null || r.IsDisposed()) return;

            if (transitioning)
            {
                /* TODO - it would appear that to customize easing (and have it per-transition) we need to:
                 *      1. Implement Begin/Ending on each transition to configure a CAAnimation
                 *      2. Add animations to the layer in Began (before the property has changed)
                 *      3. Remove the UIView.Begin/Commit code from here.
                 */

                UIView.BeginAnimations(Guid.NewGuid().ToString());
                UIView.SetAnimationBeginsFromCurrentState(true);
                UIView.SetAnimationDuration(Transition?.Duration.TotalSeconds ?? 0.25f);
                UIView.SetAnimationCurve(Transition?.Curve is Spring ? UIViewAnimationCurve.EaseOut : UIViewAnimationCurve.Linear);
                BeganAnimation(r);
            }
            else
            {
                EndingAnimation(r);
                UIView.CommitAnimations();
            }
        }

        protected void AnimateLayer(IInterpolator interpolator, string keyPath)
        {
            var curve = Transition?.Curve ?? new EasingCurve();

#pragma warning disable CA2000 // Dispose objects before losing scope
            var animation = CreateAnimation(curve, interpolator);
#pragma warning restore CA2000 // Dispose objects before losing scope
            animation.KeyPath = keyPath;
            animation.Duration = Transition?.Duration.TotalSeconds ?? 0.25f;
            animation.AnimationStopped += AnimationStopped;

            Renderer?.Layer.AddAnimation(animation, keyPath);
        }

        private void AnimationStopped(object sender, CAAnimationStateEventArgs e)
        {
            if (e.Finished)
            {
                ((CAAnimation) sender).AnimationStopped -= AnimationStopped;
                Completed?.Invoke(this, EventArgs.Empty);
            }
        }

        private static CAPropertyAnimation CreateAnimation(AnimationCurve curve, IInterpolator interpolator)
        {
            var spring = curve as Spring;
            if (spring != null) return new SpringAnimation(spring, interpolator);

            var easing = curve as EasingCurve;
            if (easing != null) return new EasingAnimation(easing, interpolator);

            throw new NotSupportedException($"{nameof(curve)} doesn't have a supported animator");
        }
    }
}