using OliveTree.Transitions.Curves;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using PropertyChangingEventArgs = Xamarin.Forms.PropertyChangingEventArgs;

namespace OliveTree.Transitions
{
    public abstract class TransitionBase
    {
        private VisualElement _element;
        private bool _animating;

        private readonly ITransitionHandler _handler;
        public event EventHandler<bool> Transitioning;

        protected TransitionBase()
        {
            _handler = DependencyService.Get<ITransitionProvider>()?.Resolve(GetType());
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
            element.BatchCommitted -= BatchCommitted;
            element.PropertyChanging -= ElementOnPropertyChanging;
            element.PropertyChanged -= ElementOnPropertyChanged;
        }
        private void RegisterHandlers(VisualElement element)
        {
            if (element == null) return;
            element.BatchCommitted += BatchCommitted;
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
                else if (Element == null || !Element.Batched)          //_animating is implied, already started a batched animation
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

        private void BatchCommitted(object sender, EventArg<VisualElement> e)
        {
            IsAnimating = false;
        }

        public async Task Animate(Action action)
        {
            if (action is null) throw new ArgumentNullException(nameof(action));

            if (_handler == null)
            {
                action();
                return;
            }

            var tcs = new TaskCompletionSource<bool>();

            _handler.Completed += Handler;
            _handler.Attach(this);

            action();
            await tcs.Task.ConfigureAwait(false);

            void Handler(object _, EventArgs __)
            {
                _handler.Completed -= Handler;
                tcs.TrySetResult(true);
            }
        }
    }
}