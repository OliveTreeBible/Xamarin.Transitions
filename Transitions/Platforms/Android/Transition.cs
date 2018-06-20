using System;
using System.Collections.Generic;
using System.Linq;
using Android.Animation;
using Android.Transitions;
using Android.Views;
using OliveTree.Transitions.Curves;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Object = Java.Lang.Object;
using View = Android.Views.View;

namespace OliveTree.Transitions.Droid
{
    public class Transition : ITransitionHandler
    {
        private TransitionBase _transition;

        public event EventHandler Completed;

        void ITransitionHandler.Attach(TransitionBase transition)
        {
            if (ReferenceEquals(_transition, transition)) return;

            if (_transition != null)
                _transition.Transitioning -= OnTransitioning;

            _transition = transition;

            if (_transition != null)
                _transition.Transitioning += OnTransitioning;
        }

        protected ViewGroup Container
        {
            get
            {
                var element = _transition?.Element;
                return element != null
                    ? Platform.GetRenderer(element)?.View as ViewGroup
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

            foreach (var t in BuildTransitions(_transition?.Element))
                trans.AddTransition(t);

            TransitionManager.BeginDelayedTransition(container, trans);
        }

        private static IEnumerable<Android.Transitions.Transition> BuildTransitions(VisualElement element)
        {
            if (element == null) yield break;

            foreach (var tb in Interaction.GetTransitions(element) ?? Enumerable.Empty<TransitionBase>())
            {
                Android.Transitions.Transition transition;
                if (tb is LayoutTransition)
                    transition = new ChangeBounds(tb);
                else if (tb is OpacityTransition)
                    transition = new ChangeAlpha(tb);
                else if (tb is TransformTransition)
                    transition = new ChangeRenderTransform(tb);
                else
                    continue;

                yield return transition;
            }
        }

        private class TransitionCompletion : Object, Android.Transitions.Transition.ITransitionListener
        {
            private readonly Transition _transition;
            private readonly View _container;
            private LayerType _startingType;

            public TransitionCompletion(Transition transition, View container)
            {
                if (transition == null) throw new ArgumentNullException(nameof(transition));
                if (container == null) throw new ArgumentNullException(nameof(container));
                _transition = transition;
                _container = container;
            }

            public void OnTransitionStart(Android.Transitions.Transition transition)
            {
                _startingType = _container.LayerType;
                if (_startingType != LayerType.Hardware)
                    _container.SetLayerType(LayerType.Hardware, null);
            }

            public void OnTransitionPause(Android.Transitions.Transition transition) { }
            public void OnTransitionResume(Android.Transitions.Transition transition) { }

            public void OnTransitionCancel(Android.Transitions.Transition transition) => Cleanup();
            public void OnTransitionEnd(Android.Transitions.Transition transition) => Cleanup();
            private void Cleanup()
            {
                if (_startingType != LayerType.Hardware)
                    _container.SetLayerType(_startingType, null);
                _transition.Completed?.Invoke(_transition, EventArgs.Empty);
            }
        }
    }
}