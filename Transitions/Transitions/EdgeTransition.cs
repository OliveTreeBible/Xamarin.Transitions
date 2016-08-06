using OliveTree.Animations;
using OliveTree.Animations.Curves;
using System;
using Xamarin.Forms;

namespace OliveTree.Transitions.Transitions
{
    public class EdgeTransition : LayoutTransition
    {
        private static readonly AnimationCurve Enter = new Spring { Friction = 5 };
        private static readonly AnimationCurve Exit = new BackCurve { Amplitude = 0.25d, Mode = EasingMode.In };

        public override TimeSpan Duration { get; set; } = TimeSpan.FromMilliseconds(400);
        public override AnimationCurve Curve
        {
            get
            {
                var e = Element;
                var p = (VisualElement)Element?.Parent;
                if (e == null || p == null) return new EasingCurve();

                return e.Bounds.IntersectsWith(p.Bounds) ? Enter : Exit;
            }
            set { }
        }
    }
}