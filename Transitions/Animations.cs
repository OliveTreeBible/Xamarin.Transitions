using OliveTree.Transitions.Curves;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace OliveTree.Transitions
{
    public static class Animations
    {
        public static async Task FadeToAsync(this VisualElement element, double value, AnimationCurve curve = null)
            => await element.AnimateAsync<OpacityTransition>(() => element.Opacity = value).ConfigureAwait(false);

        private static async Task AnimateAsync<TTransition>(this VisualElement element, Action setter)
            where TTransition : TransitionBase, new()
        {
            var trans = new TTransition { Element = element };
            await trans.Animate(setter).ConfigureAwait(true);
            trans.Element = null;
        }
    }
}
