using System;
using Android.Animation;
using Android.Runtime;
using Android.Transitions;
using Android.Views;

namespace OliveTree.Transitions.Droid
{
    public class ChangeAlpha : Transition
    {
        private static readonly string PropertyName = $"com.olivetree:{nameof(ChangeAlpha)}:alpha";

        // ReSharper disable UnusedMember.Global
        public ChangeAlpha() { }
        public ChangeAlpha(IntPtr ptr, JniHandleOwnership own) : base(ptr, own) { }
        // ReSharper restore UnusedMember.Global

        public override void CaptureStartValues(TransitionValues transitionValues) => CaptureValues(transitionValues);
        public override void CaptureEndValues(TransitionValues transitionValues) => CaptureValues(transitionValues);

        private static void CaptureValues(TransitionValues values) => values.Values[PropertyName] = values.View.Alpha;

        public override Animator CreateAnimator(ViewGroup sceneRoot, TransitionValues startValues, TransitionValues endValues)
        {
            if (startValues == null || endValues == null) return null;

            var view = endValues.View;
            float start = (float)startValues.Values[PropertyName],
                end = (float)endValues.Values[PropertyName];

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (start == end) return null;

            var anim = ValueAnimator.OfFloat(start, end);
            anim.Update += (_, args) => view.Alpha = (float)args.Animation.AnimatedValue;

            return anim;
        }
    }
}