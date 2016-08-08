using System;
using System.Collections.Generic;
using System.Linq;

namespace OliveTree.Transitions.iOS
{
    public class Provider : ITransitionProvider
    {
        private static readonly Dictionary<Type, Type> Handlers = new Dictionary<Type, Type>
        {
            [typeof(Transitions.LayoutTransition)] = typeof(LayoutTransition),
            [typeof(Transitions.OpacityTransition)] = typeof(OpacityTransition),
            [typeof(Transitions.TransformTransition)] = typeof(TransformTransition)
        };

        public ITransitionHandler Resolve<T>() where T : Transitions.TransitionBase => Resolve(typeof(T));

        public ITransitionHandler Resolve(Type transitionType)
        {
            Type handlerType;
            if (!Handlers.TryGetValue(transitionType, out handlerType))
            {
                handlerType = Handlers.Where(kv => kv.Key.IsAssignableFrom(transitionType))
                    .Select(kv => kv.Value)
                    .FirstOrDefault();
            }

            if (handlerType == null) return null;
            return Activator.CreateInstance(handlerType) as ITransitionHandler;
        }
    }
}
