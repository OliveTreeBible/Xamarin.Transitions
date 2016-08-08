using Xamarin.Forms;

namespace OliveTree.Transitions
{
    public class OpacityTransition : TransitionBase
    {
        protected override bool ShouldTransition(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(VisualElement.Opacity):
                    return true;

                default:
                    return false;
            }
        }
    }
}