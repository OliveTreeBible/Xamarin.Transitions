using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace OliveTree.Transitions
{
    public class TransitionCollection : Collection<TransitionBase>
    {
        private VisualElement _element;

        public VisualElement Element
        {
            // ReSharper disable once MemberCanBePrivate.Global
            get { return _element; }
            internal set
            {
                if (ReferenceEquals(_element, value)) return;

                _element = value;
                foreach (var t in this) t.Element = value;
            }
        }

        protected override void InsertItem(int index, TransitionBase item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            item.Element = Element;
            base.InsertItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            base.RemoveItem(index);
            this[index].Element = null;
        }

        protected override void SetItem(int index, TransitionBase item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            item.Element = Element;
            base.SetItem(index, item);
            this[index].Element = null;
        }

        protected override void ClearItems()
        {
            base.ClearItems();
            foreach (var t in this) t.Element = null;
        }
    }
}
