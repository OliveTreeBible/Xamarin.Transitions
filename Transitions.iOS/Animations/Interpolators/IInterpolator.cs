using Foundation;

namespace OliveTree.Transitions.iOS.Animations.Interpolators
{
    public interface IInterpolator
    {
        NSObject ProvideValue(double delta);
    }

    public interface IInterpolator<T> : IInterpolator
    {
        T From { get; set; }
        T To { get; set; }
    }
}