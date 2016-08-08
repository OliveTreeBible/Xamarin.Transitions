using Xamarin.Forms;

namespace OliveTree.Transitions
{
    public class TransformTransition : TransitionBase
    {
        protected override bool ShouldTransition(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(VisualElement.Rotation):
                case nameof(VisualElement.RotationX):
                case nameof(VisualElement.AnchorX):
                case nameof(VisualElement.AnchorY):
                case nameof(VisualElement.Scale):
                case nameof(VisualElement.TranslationX):
                case nameof(VisualElement.TranslationY):
                    return true;

                default:
                    return false;
            }
        }
    }
}