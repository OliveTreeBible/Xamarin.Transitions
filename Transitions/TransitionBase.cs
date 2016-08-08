using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using OliveTree.Transitions.Curves;
using OliveTree.Transitions.Extensions;
using Xamarin.Forms;
using PropertyChangingEventArgs = Xamarin.Forms.PropertyChangingEventArgs;

namespace OliveTree.Transitions
{
    public abstract class TransitionBase
    {
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
            element.RemoveBatchCommittedHandler(this, nameof(BatchCommitted));
            element.PropertyChanging -= ElementOnPropertyChanging;
            element.PropertyChanged -= ElementOnPropertyChanged;
        }
        private void RegisterHandlers(VisualElement element)
        {
            if (element == null) return;
            element.AddBatchCommittedHandler(this, nameof(BatchCommitted));
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
                else if (!(Element?.IsBatched() ?? false))          //_animating is implied, already started a batched animation
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