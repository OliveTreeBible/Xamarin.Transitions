using System;

namespace OliveTree.Transitions
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class TransitionHandler : Attribute
    {
        public Type Handler { get; }

        public TransitionHandler(Type handler)
        {
            Handler = handler;
        }
    }
}
