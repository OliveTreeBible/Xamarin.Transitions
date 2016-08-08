using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Android.Animation;
using Android.Runtime;
using Android.Transitions;
using Android.Views;

namespace OliveTree.Transitions.Droid
{
    public class ChangeRenderTransform : Transition
    {
        private const string Prefix = "com.olivetree:" + nameof(ChangeRenderTransform);
        private const string PivotX = "pivotX";
        private const string PivotY = "pivotY";
        private const string Rotation = "rotation";
        private const string RotationX = "rotationX";
        private const string RotationY = "rotationY";
        private const string ScaleX = "scaleX";
        private const string ScaleY = "scaleY";
        private const string TranslationX = "translationX";
        private const string TranslationY = "translationY";

        // ReSharper disable UnusedMember.Global
        public ChangeRenderTransform() { }
        public ChangeRenderTransform(IntPtr ptr, JniHandleOwnership own) : base(ptr, own) { }
        // ReSharper restore UnusedMember.Global

        public override void CaptureStartValues(TransitionValues transitionValues) => CaptureValues(transitionValues);
        public override void CaptureEndValues(TransitionValues transitionValues) => CaptureValues(transitionValues);

        private static void Capture(IDictionary values, string property, object value) => values[$"{Prefix}:{property}"] = value;
        private static void CaptureValues(TransitionValues values)
        {
            var view = values.View;
            var vals = values.Values;

            Capture(vals, PivotX, view.PivotX);
            Capture(vals, PivotY, view.PivotY);
            Capture(vals, Rotation, view.Rotation);
            Capture(vals, RotationX, view.RotationX);
            Capture(vals, RotationY, view.RotationY);
            Capture(vals, ScaleX, view.ScaleX);
            Capture(vals, ScaleY, view.ScaleY);
            Capture(vals, TranslationX, view.TranslationX);
            Capture(vals, TranslationY, view.TranslationY);
        }

        public override Animator CreateAnimator(ViewGroup sceneRoot, TransitionValues startValues, TransitionValues endValues)
        {
            if (startValues == null || endValues == null) return null;

            var view = endValues.View;

            var props = CreatePropertyValues(startValues, endValues).ToArray();
            return !props.Any() ? null : ObjectAnimator.OfPropertyValuesHolder(view, props);
        }

        private static IEnumerable<PropertyValuesHolder> CreatePropertyValues(TransitionValues startValues, TransitionValues endValues)
        {
            foreach (var propertyName in AnimateProperties())
            {
                var fullName = $"{Prefix}:{propertyName}";
                float start = (float)startValues.Values[fullName],
                    end = (float)endValues.Values[fullName];

                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (start == end) continue;

                yield return PropertyValuesHolder.OfFloat(propertyName, start, end);
            }
        }

        private static IEnumerable<string> AnimateProperties()
        {
            yield return PivotX;
            yield return PivotY;
            yield return Rotation;
            yield return RotationX;
            yield return RotationY;
            yield return ScaleX;
            yield return ScaleY;
            yield return TranslationX;
            yield return TranslationY;
        }
    }
}