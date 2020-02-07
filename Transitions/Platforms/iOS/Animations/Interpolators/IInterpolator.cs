using Foundation;

namespace OliveTree.Transitions.iOS.Animations.Interpolators
{
    public interface IInterpolator
    {
        NSObject ProvideValue(double delta);
    }

#pragma warning disable CA1716 // Identifiers should not match keywords
    public interface IInterpolator<T> : IInterpolator
    {
        T From { get; set; }
        T To { get; set; }
    }
#pragma warning restore CA1716 // Identifiers should not match keywords
}