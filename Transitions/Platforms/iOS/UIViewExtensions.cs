using System;
using UIKit;

namespace OliveTree.Transitions.iOS
{
    internal static class UIViewExtensions
    {
        public static bool IsDisposed(this UIView uiView) => uiView.Handle == IntPtr.Zero;
    }
}