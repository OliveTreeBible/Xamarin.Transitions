using System;
using OliveTree.Transitions.Curves;
using Xamarin.Forms;

namespace OliveTree.Transitions
{
    public class EdgeTransition : LayoutTransition
    {
        private static readonly AnimationCurve Enter = new Spring { Friction = 5 };
        private static readonly AnimationCurve Exit = new EasingCurve { Easing = Easing.Cubic, Mode = EasingMode.In };

        public override TimeSpan Duration { get; set; } = TimeSpan.FromMilliseconds(400);
        public override AnimationCurve Curve
        {
            get
            {
                var e = Element;
                var p = (VisualElement?)Element?.Parent;
                if (e is null || p is null) 
                    return new EasingCurve();

                return e.Bounds.IntersectsWith(p.Bounds) ? Enter : Exit;
            }
            set { }
        }
    }
}