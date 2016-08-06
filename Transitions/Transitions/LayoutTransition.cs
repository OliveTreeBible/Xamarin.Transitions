using Xamarin.Forms;

namespace OliveTree.Transitions.Transitions
{
    public class LayoutTransition : TransitionBase
    {
        protected override bool ShouldTransition(string propertyName)
        {
            var element = Element;
            if (element == null || element.Width < 0 || element.Height < 0) return false;

            switch (propertyName)
            {
                case nameof(VisualElement.X):
                case nameof(VisualElement.Y):
                case nameof(VisualElement.Width):
                case nameof(VisualElement.Height):
                    return true;

                default:
                    return false;
            }
        }
    }
}