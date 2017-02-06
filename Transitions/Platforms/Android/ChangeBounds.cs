using System;
using Android.Animation;
using Android.Runtime;
using Android.Transitions;
using Android.Views;

namespace OliveTree.Transitions.Droid
{
    public class ChangeBounds : Android.Transitions.ChangeBounds
    {
        private TransitionBase _transition;

        public ChangeBounds(TransitionBase transition)
        {
            if (transition == null) throw new ArgumentNullException(nameof(transition));
            _transition = transition;
        }

        public ChangeBounds(IntPtr ptr, JniHandleOwnership own) : base(ptr, own) { }
        public override Android.Transitions.Transition Clone()
        {
            var clone = (ChangeBounds)base.Clone();
            clone._transition = _transition;
            return clone;
        }

        public override ITimeInterpolator Interpolator => _transition.GetInterpolator();
        public override long Duration => _transition.CalculateDuration();
    }
}