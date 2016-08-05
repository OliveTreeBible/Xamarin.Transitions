using OliveTree.Animations.Curves;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xamarin.Forms;
using PropertyChangingEventArgs = Xamarin.Forms.PropertyChangingEventArgs;

namespace OliveTree.Transitions.Transitions
{
    public abstract class TransitionBase
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

        public static bool IsBatched(VisualElement element)
        {
            return (bool)(BatchedProperty?.GetValue(element) ?? false);
        }

        public static void AddBatchCommittedHandler(VisualElement element, object target, MethodInfo handler)
            => BatchCommittedEvent.AddMethod.Invoke(element, new[] { handler.CreateDelegate(BatchHandlerType.Value, target) });
        public static void RemoveBatchCommittedHandler(VisualElement element, object target, MethodInfo handler)
            => BatchCommittedEvent.RemoveMethod.Invoke(element, new[] { handler.CreateDelegate(BatchHandlerType.Value, target) });

        #endregion Batched Reflection access


        #region Handler Resolution

        private static readonly MethodInfo GetAssemblies = typeof(Device).GetMethod(nameof(GetAssemblies), BindingFlags.Static | BindingFlags.NonPublic);
        private static readonly Dictionary<Type, Type> ResolvedHandlers = new Dictionary<Type, Type>();

        private static Type ResolveType(TransitionBase forTransition)
        {
            var sourceType = forTransition.GetType();

            Type handlerType;
            if (ResolvedHandlers.TryGetValue(sourceType, out handlerType))
                return handlerType;

            var handlerInterface = typeof(ITransitionHandler);
            foreach (Type type in from assem in (Assembly[])GetAssemblies.Invoke(null, null)
                from ti in assem.DefinedTypes
                select ti.AsType())
            {
                if (!handlerInterface.IsAssignableFrom(type)) continue;

                var attr = type.GetTypeInfo().GetCustomAttributes<TransitionHandler>()
                    .FirstOrDefault(a => a.Handler.IsAssignableFrom(sourceType));
                if (attr == null) continue;

                return ResolvedHandlers[sourceType] = type;
            }

            return ResolvedHandlers[sourceType] = null;
        }

        private static ITransitionHandler Resolve(TransitionBase forTransition)
        {
            var type = ResolveType(forTransition);
            if (type == null) return null;

            var handler = Activator.CreateInstance(type);
            return handler as ITransitionHandler;
        }

        #endregion

        private static readonly MethodInfo BatchCommittedMethod = typeof(TransitionBase).GetMethod(nameof(BatchCommitted), BindingFlags.Instance | BindingFlags.NonPublic);

        private VisualElement _element;
        private bool _animating;

        private readonly ITransitionHandler _handler;
        public event EventHandler<bool> Transitioning;

        protected TransitionBase()
        {
            _handler = Resolve(this);
        }

        protected abstract bool ShouldTransition(string propertyName);

        public virtual TimeSpan Duration { get; set; } = TimeSpan.FromMilliseconds(250);
        public virtual AnimationCurve Curve { get; set; } = new EasingCurve();
        public bool IsDisabled { get; set; }

        public VisualElement Element
        {
            // ReSharper disable once MemberCanBePrivate.Global
            get { return _element; }
            internal set
            {
                if (ReferenceEquals(_element, value)) return;

                var old = _element;
                _element = value;
                UnregisterHandlers(old);
                RegisterHandlers(value);
            }
        }

        private void UnregisterHandlers(VisualElement element)
        {
            if (element == null) return;
            RemoveBatchCommittedHandler(element, this, BatchCommittedMethod);
            element.PropertyChanging -= ElementOnPropertyChanging;
            element.PropertyChanged -= ElementOnPropertyChanged;
        }
        private void RegisterHandlers(VisualElement element)
        {
            if (element == null) return;
            AddBatchCommittedHandler(element, this, BatchCommittedMethod);
            element.PropertyChanging += ElementOnPropertyChanging;
            element.PropertyChanged += ElementOnPropertyChanged;
        }


        private bool IsAnimating
        {
            set
            {
                if (IsDisabled || _animating == value) return;

                if (value)                              //!_animating is implied from the first check
                    Transitioning?.Invoke(this, _animating = true);
                else if (!IsBatched(Element))          //_animating is implied, already started a batched animation
                    Transitioning?.Invoke(this, _animating = false);
            }
        }

        private void ElementOnPropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            if (!ShouldTransition(e.PropertyName)) return;

            IsAnimating = true;
        }

        private void ElementOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Renderer":
                    var element = Element;
                    if (element == null) return;
                    
                    //We want our handlers after the renderer, so re-register
                    UnregisterHandlers(element);
                    RegisterHandlers(element);
                    _handler?.Attach(this);
                    break;

                default:
                    if (!ShouldTransition(e.PropertyName)) return;

                    IsAnimating = false; //won't end the animation right away if batched
                    break;
            }
        }

        [SuppressMessage("ReSharper", "UnusedParameter.Local")]
        private void BatchCommitted(object sender, EventArgs e)
        {
            IsAnimating = false;
        }

        public Task Animate(Action action)
        {
            if (_handler == null)
            {
                action();
                return Task.CompletedTask;
            }

            var tcs = new TaskCompletionSource<bool>();
            EventHandler handler = null;
            handler = (_, __) =>
            {
                _handler.Completed -= handler;
                tcs.TrySetResult(true);
            };

            _handler.Completed += handler;
            _handler.Attach(this);

            action();
            return tcs.Task;
        }
    }
}