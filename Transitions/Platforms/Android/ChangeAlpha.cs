using System;
using Android.Animation;
using Android.Runtime;
using Android.Transitions;
using Android.Views;

namespace OliveTree.Transitions.Droid
{
    public class ChangeAlpha : DelayedTransition
    {
        private static readonly string PropertyName = $"com.olivetree:{nameof(ChangeAlpha)}:alpha";

        public ChangeAlpha(TransitionBase transition) : base(transition) { }
        public ChangeAlpha(IntPtr handle, JniHandleOwnership own) : base(handle, own) { }

        public override void CaptureStartValues(TransitionValues transitionValues)
        {
            if (transitionValues is null) throw new ArgumentNullException(nameof(transitionValues));
            CaptureValues(transitionValues);
        }

        public override void CaptureEndValues(TransitionValues transitionValues)
        {
            if (transitionValues is null) throw new ArgumentNullException(nameof(transitionValues));
            CaptureValues(transitionValues);
        }

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
            anim.Update += (_, args) =>
            {
                if (view.IsDisposed()) return;
                view.Alpha = (float)args.Animation.AnimatedValue;
            };

            return anim;
        }
    }
}