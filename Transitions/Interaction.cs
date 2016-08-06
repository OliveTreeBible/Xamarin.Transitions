using System;
using System.Linq;
using OliveTree.Transitions.Transitions;
using Xamarin.Forms;

namespace OliveTree.Transitions
{
    public static class Interaction
    {
        public static readonly BindableProperty TransitionsProperty = BindableProperty.CreateAttached("Transitions", typeof(TransitionCollection), typeof(Interaction),
            default(TransitionCollection), propertyChanged: PropertyChanged);

        public static TransitionCollection GetTransitions(BindableObject @object)
        {
            if (@object == null) throw new ArgumentNullException(nameof(@object));

            var collection = (TransitionCollection)@object.GetValue(TransitionsProperty);
            if (collection == null)
            {
                collection = new TransitionCollection();
                @object.SetValue(TransitionsProperty, collection);
            }

            return collection;
        }

        public static void SetTransitions(BindableObject @object, TransitionCollection collection)
        {
            if (@object == null) throw new ArgumentNullException(nameof(@object));
            @object.SetValue(TransitionsProperty, collection);
        }

        private static void PropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (ReferenceEquals(oldValue, newValue)) return;

            var oldColl = (TransitionCollection)oldValue;
            var newColl = (TransitionCollection)newValue;

            if (oldColl != null) oldColl.Element = null;
            if (newColl != null) newColl.Element = bindable as VisualElement;
        }

        public static void SetImmediate(this VisualElement element, Action setterBlock)
        {
            var transitions = (TransitionCollection)element.GetValue(TransitionsProperty);
            try
            {
                foreach (var t in transitions ?? Enumerable.Empty<TransitionBase>()) t.IsDisabled = true;
                element.BatchBegin();

                setterBlock();
            }
            finally
            {
                element.BatchCommit();
                foreach (var t in transitions ?? Enumerable.Empty<TransitionBase>()) t.IsDisabled = false;
            }
        }
    }
}
