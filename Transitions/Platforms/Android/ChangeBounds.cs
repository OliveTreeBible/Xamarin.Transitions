using System;
using Android.Animation;
using Android.Runtime;

namespace OliveTree.Transitions.Droid
{
    public class ChangeBounds : Android.Transitions.ChangeBounds
    {
        private TransitionBase _transition;

        public ChangeBounds(TransitionBase transition)
        {
            _transition = transition ?? throw new ArgumentNullException(nameof(transition));
        }

        public ChangeBounds(IntPtr handle, JniHandleOwnership own) : base(handle, own) { }
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