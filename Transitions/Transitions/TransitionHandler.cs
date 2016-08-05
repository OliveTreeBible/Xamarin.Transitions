using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
