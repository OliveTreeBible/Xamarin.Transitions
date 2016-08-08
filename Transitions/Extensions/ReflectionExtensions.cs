using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
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

        private static Delegate CreateDelegate(object target, string method) => target.GetType()
            .GetMethod(method, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)?
            .CreateDelegate(BatchHandlerType.Value, target);

        #endregion Batched Reflection access

        public static bool IsBatched(this VisualElement element) => (bool)(BatchedProperty?.GetValue(element) ?? false);

        public static void AddBatchCommittedHandler(this VisualElement element, object target, string method)
            => BatchCommittedEvent.AddMethod.Invoke(element, new object[] { CreateDelegate(target, method) });

        public static void RemoveBatchCommittedHandler(this VisualElement element, object target, string method)
            => BatchCommittedEvent.RemoveMethod.Invoke(element, new object[] { CreateDelegate(target, method) });


        private static readonly MethodInfo GetAssemblies = typeof(Device).GetMethod(nameof(GetAssemblies), BindingFlags.Static | BindingFlags.NonPublic);
        public static Assembly[] GetHandlerAssemblies(this Element element) => (Assembly[])GetAssemblies.Invoke(null, null);
    }
}
