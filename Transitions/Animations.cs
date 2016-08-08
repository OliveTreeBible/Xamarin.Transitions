using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OliveTree.Transitions.Curves;
using Xamarin.Forms;

namespace OliveTree.Transitions
{
    public static class Animations
    {
        public static Task FadeToAsync(this VisualElement element, double value, AnimationCurve curve = null)
            => element.AnimateAsync<OpacityTransition>(() => element.Opacity = value);

        private static Task AnimateAsync<TTransition>(this VisualElement element, Action setter)
            where TTransition : TransitionBase, new()
        {
            var trans = new TTransition { Element = element };
            return trans.Animate(setter)
                .ContinueWith(t => trans.Element = null);
        }
    }
}
