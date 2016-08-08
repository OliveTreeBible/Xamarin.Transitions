using Xamarin.Forms;

namespace OliveTree.Transitions
{
    public static class TransitionsLibrary
    {
        public static void Register<TProvider>() where TProvider : class, ITransitionProvider
            => DependencyService.Register<ITransitionProvider, TProvider>();
    }
}
