using System;
using System.Collections.Generic;
using System.Linq;
using Android.Animation;
using Android.Transitions;
using Android.Views;
using OliveTree.Animations.Curves;
using OliveTree.Animations.Droid;
using OliveTree.Transitions.Transitions;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using LayoutTransition = OliveTree.Transitions.Transitions.LayoutTransition;
using View = Android.Views.View;

namespace OliveTree.Transitions.Droid
{
    [TransitionHandler(typeof(LayoutTransition))]
    [TransitionHandler(typeof(OpacityTransition))]
    [TransitionHandler(typeof(TransformTransition))]
    public class TransitionBase : ITransitionHandler
    {
        private Transitions.TransitionBase _transition;

        public event EventHandler Completed;

        void ITransitionHandler.Attach(Transitions.TransitionBase transition)
        {
            if (ReferenceEquals(_transition, transition)) return;

            if (_transition != null)
                _transition.Transitioning -= OnTransitioning;

            _transition = transition;

            if (_transition != null)
                _transition.Transitioning += OnTransitioning;
        }

        private Transitions.TransitionBase Transition => _transition;

        protected ViewGroup Container
        {
            get
            {
                var element = Transition?.Element;
                return element != null
                    ? Platform.GetRenderer(element)?.ViewGroup
                    : null;
            }
        }

        private void OnTransitioning(object sender, bool transitioning)
        {
            var container = Container;
            if (container == null) return;

            if (!transitioning) return;

            var trans = new TransitionSet();
            trans.SetOrdering(TransitionOrdering.Together);
            trans.AddListener(new TransitionCompletion(this, container));

            foreach (var t in BuildTransitions(Transition?.Element))
                trans.AddTransition(t);

            TransitionManager.BeginDelayedTransition(container, trans);
        }

        private static IEnumerable<Transition> BuildTransitions(VisualElement element)
        {
            if (element == null) yield break;

            foreach (var tb in Interaction.GetTransitions(element) ?? Enumerable.Empty<Transitions.TransitionBase>())
            {
                Transition transition;
                if (tb is LayoutTransition)
                    transition = new ChangeBounds();
                else if (tb is OpacityTransition)
                    transition = new ChangeAlpha();
                else if (tb is TransformTransition)
                    transition = new ChangeRenderTransform();
                else
                    continue;

                var spring = tb.Curve as Spring;
                if (spring != null)
                {
                    const int framesPerSecond = SpringInterpolator.FramesPerSecond;
                    var seconds = spring.Steps(framesPerSecond).Count() / (double)framesPerSecond;
                    transition.SetDuration((long)(seconds * 1000));
                    transition.SetInterpolator(new SpringInterpolator(spring));
                }
                else if (tb.Curve is EasingCurve)
                {
                    transition.SetDuration((long)tb.Duration.TotalMilliseconds);
                    transition.SetInterpolator(new CurveInterpolator((EasingCurve)tb.Curve));
                }

                yield return transition;
            }
        }

        private class TransitionCompletion : Java.Lang.Object, Transition.ITransitionListener
        {
            private readonly TransitionBase _transition;
            private readonly View _container;
            private LayerType _startingType;

            public TransitionCompletion(TransitionBase transition, View container)
            {
                if (transition == null) throw new ArgumentNullException(nameof(transition));
                if (container == null) throw new ArgumentNullException(nameof(container));
                _transition = transition;
                _container = container;
            }

            public void OnTransitionStart(Transition transition)
            {
                _startingType = _container.LayerType;
                if (_startingType != LayerType.Hardware)
                    _container.SetLayerType(LayerType.Hardware, null);
            }

            public void OnTransitionPause(Transition transition) { }
            public void OnTransitionResume(Transition transition) { }

            public void OnTransitionCancel(Transition transition) => Cleanup();
            public void OnTransitionEnd(Transition transition) => Cleanup();
            private void Cleanup()
            {
                if (_startingType != LayerType.Hardware)
                    _container.SetLayerType(_startingType, null);
                _transition.Completed?.Invoke(_transition, EventArgs.Empty);
            }
        }

        private class CurveInterpolator : Java.Lang.Object, ITimeInterpolator
        {
            private readonly EasingCurve _curve;

            public CurveInterpolator(EasingCurve curve)
            {
                _curve = curve;
            }

            public float GetInterpolation(float input) => (float)_curve.Ease(input);
        }

        private class SpringInterpolator : Java.Lang.Object, ITimeInterpolator
        {
            internal const int FramesPerSecond = 60 * 4; //Probably need more than the 60 refresh rate, but 4 per frame may be excessive
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