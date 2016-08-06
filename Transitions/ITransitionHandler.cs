using System;
using OliveTree.Transitions.Transitions;

namespace OliveTree.Transitions
{
    public interface ITransitionHandler
    {
        event EventHandler Completed;
        void Attach(TransitionBase transition);
    }
}
