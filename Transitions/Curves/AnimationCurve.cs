using System;
using Xamarin.Forms.Xaml;

namespace OliveTree.Transitions.Curves
{
    public abstract class AnimationCurve : IMarkupExtension<AnimationCurve>
    {
        public AnimationCurve ProvideValue(IServiceProvider serviceProvider) => this;
        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) => ProvideValue(serviceProvider);
    }
}