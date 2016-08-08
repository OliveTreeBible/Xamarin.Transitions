using System;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;

namespace OliveTree.Transitions.Extensions
{
    public static class ReflectionExtensions
    {
        #region BatchCommitted event access

        private static readonly EventInfo BatchCommittedEvent = typeof(VisualElement).GetEvent("BatchCommitted", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly Lazy<Type> BatchHandlerType = new Lazy<Type>(() =>
        {
            //This whole thing is because EventArg<T> is internal to Forms
            // Otherwise we could just create EventHandler<EventArg<VisualElement>>
            var eventArgsType = typeof(EventArgs);

            var assem = typeof(VisualElement).GetTypeInfo().Assembly;
            var argType = assem.DefinedTypes.FirstOrDefault(t => t.FullName == "Xamarin.Forms.EventArg`1" && eventArgsType.IsAssignableFrom(t.AsType()));

            if (argType == null) return null;
            return typeof(EventHandler<>).MakeGenericType(argType.MakeGenericType(typeof(VisualElement)));
        });

        private static readonly PropertyInfo BatchedProperty = typeof(VisualElement).GetProperty("Batched", BindingFlags.Instance | BindingFlags.NonPublic);

        private static Delegate TransformDelegate(Delegate @delegate) => @delegate.GetMethodInfo().CreateDelegate(BatchHandlerType.Value, @delegate.Target);

        #endregion Batched Reflection access

        public static bool IsBatched(this VisualElement element) => (bool)(BatchedProperty?.GetValue(element) ?? false);

        public static void AddBatchCommittedHandler(this VisualElement element, EventHandler @delegate)
            => BatchCommittedEvent.AddMethod.Invoke(element, new object[] { TransformDelegate(@delegate) });

        public static void RemoveBatchCommittedHandler(this VisualElement element, EventHandler @delegate)
            => BatchCommittedEvent.RemoveMethod.Invoke(element, new object[] { TransformDelegate(@delegate) });
    }
}
