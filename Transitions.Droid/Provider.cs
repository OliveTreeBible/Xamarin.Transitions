using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace OliveTree.Transitions.Droid
{
    public class Provider : ITransitionProvider
    {
        public ITransitionHandler Resolve<T>() where T : TransitionBase => Resolve(typeof(T));
        public ITransitionHandler Resolve(Type transitionType) => new Transition();
    }
}