using Foundation;

namespace OliveTree.Animations.iOS.Interpolators
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