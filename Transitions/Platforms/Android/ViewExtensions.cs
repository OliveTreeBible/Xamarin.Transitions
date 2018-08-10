using Android.Views;

namespace OliveTree.Transitions.Droid
{
    internal static class ViewExtensions
    {
        public static bool IsDisposed(this View view) => !view.PeerReference.IsValid;
    }
}