using System;
using System.Linq;
using Xamarin.Forms;

namespace OliveTree.Transitions
{
    public static class Interaction
    {
        public static readonly BindableProperty TransitionsProperty = BindableProperty.CreateAttached("Transitions", typeof(TransitionCollection), typeof(Interaction),
            default(TransitionCollection), propertyChanged: PropertyChanged);

        public static TransitionCollection GetTransitions(BindableObject bindable)
        {
            if (bindable == null)
                throw new ArgumentNullException(nameof(bindable));

            var collection = (TransitionCollection)bindable.GetValue(TransitionsProperty);
            if (collection == null)
            {
                collection = new TransitionCollection();
                bindable.SetValue(TransitionsProperty, collection);
            }

            return collection;
        }

        public static void SetTransitions(BindableObject bindable, TransitionCollection collection)
        {
            if (bindable == null)
                throw new ArgumentNullException(nameof(bindable));
            bindable.SetValue(TransitionsProperty, collection);
        }

        private static void PropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (ReferenceEquals(oldValue, newValue))
                return;

            var oldColl = (TransitionCollection)oldValue;
            var newColl = (TransitionCollection)newValue;

            if (oldColl != null)
                oldColl.Element = null;
            if (newColl != null)
                newColl.Element = bindable as VisualElement;
        }

        public static void SetImmediate(this VisualElement element, Action setterBlock)
        {
            if (element is null) throw new ArgumentNullException(nameof(element));
            if (setterBlock is null) throw new ArgumentNullException(nameof(setterBlock));


            var transitions = (TransitionCollection)element.GetValue(TransitionsProperty);
            try
            {
                foreach (var t in transitions ?? Enumerable.Empty<TransitionBase>())
                    t.IsDisabled = true;
                element.BatchBegin();

                setterBlock();
            }
            finally
            {
                element.BatchCommit();
                foreach (var t in transitions ?? Enumerable.Empty<TransitionBase>())
                    t.IsDisabled = false;
            }
        }
    }
}
