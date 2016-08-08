using System;
using CoreGraphics;
using OliveTree.Transitions.iOS.Animations.Interpolators;
using UIKit;

namespace OliveTree.Transitions.iOS
{
    public class LayoutTransition : TransitionBase
    {
        private CGRect _start;
        private CGPoint _startPosition;

        protected override void BeganAnimation(UIView target)
        {
            Console.WriteLine($"{target.GetHashCode()} - {target.Layer.Bounds}");
            _start = target.Layer.Bounds;
            _startPosition = target.Layer.Position;
        }

        protected override void EndingAnimation(UIView target)
        {
            target.LayoutIfNeeded();

            AnimateLayer(new RectInterpolator
            {
                From = _start,
                To = target.Layer.Bounds
            }, "bounds");

            AnimateLayer(new PointInterpolator
            {
                From = _startPosition,
                To = target.Layer.Position
            }, "position");
        }
    }
}

