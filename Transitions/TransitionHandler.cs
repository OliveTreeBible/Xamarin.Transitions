using System;

namespace OliveTree.Transitions
{
    public interface ITransitionHandler
    {
        event EventHandler Completed;
        void Attach(TransitionBase transition);
    }

    public interface ITransitionProvider
    {
        ITransitionHandler? Resolve<T>() where T : TransitionBase;
        ITransitionHandler? Resolve(Type transitionType);
    }
}
