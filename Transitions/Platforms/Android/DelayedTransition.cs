using Android.Animation;
using Android.Runtime;
using System;

namespace OliveTree.Transitions.Droid
{
    public abstract class DelayedTransition : Android.Transitions.Transition
    {
        private TransitionBase _transition;

        protected DelayedTransition(IntPtr ptr, JniHandleOwnership own) : base(ptr, own) { }
        protected DelayedTransition(TransitionBase transition)
        {
            _transition = transition ?? throw new ArgumentNullException(nameof(transition));
        }

        public override ITimeInterpolator Interpolator => _transition.GetInterpolator();
        public override long Duration => _transition.CalculateDuration();

        public override Android.Transitions.Transition Clone()
        {
            var clone = (DelayedTransition)base.Clone();
            clone._transition = _transition;
            return clone;
        }
    }
}