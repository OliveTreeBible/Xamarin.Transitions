using OliveTree.Transitions.iOS.Animations.Interpolators;
using UIKit;

namespace OliveTree.Transitions.iOS
{
    [TransitionHandler(typeof(Transitions.OpacityTransition))]
    public class OpacityTransition : TransitionBase
    {
        private float? _start;
        protected override void BeganAnimation(UIView target)
        {
            _start = target.Layer.Opacity;
        }

        protected override void EndingAnimation(UIView target)
        {
            target.Layer.Opacity = (float) (Transition?.Element?.Opacity ?? 0); //ensure it's set rather than delayed by VisualElementTracker
            AnimateLayer(new DoubleInterpolator
            {
                From = _start ?? 0,
                To = target.Layer.Opacity,
            }, "opacity");
        }
    }
}