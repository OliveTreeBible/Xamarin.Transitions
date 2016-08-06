using System;
using CoreAnimation;
using Foundation;
using OliveTree.Animations.iOS.Interpolators;
using UIKit;
using Xamarin.Forms;

namespace OliveTree.Transitions.iOS
{
    [TransitionHandler(typeof(Transitions.TransformTransition))]
    public class TransformTransition : TransitionBase
    {
        private Transform _start;

        protected override void BeganAnimation(UIView target)
        {
            _start = default(Transform);
            var ve = Transition?.Element;
            if (ve == null) return;

            _start = new Transform(ve);
        }

        protected override void EndingAnimation(UIView target)
        {
            var ve = Transition?.Element;
            if (ve == null) return;

            var transform = new Transform(ve);

            target.Layer.Transform = transform;
            AnimateLayer(new TransformInterpolator
            {
                From = _start,
                To = transform
            }, "transform");
        }

        private struct Transform
        {
            private VisualElement _element;
            private double _translationX;
            private double _translationY;
            private double _scale;
            private double _rotation;
            private double _rotationX;
            private double _rotationY;

            public Transform(VisualElement element)
            {
                _element = element;
                _translationX = element.TranslationX;
                _translationY = element.TranslationY;
                _scale = element.Scale;
                _rotation = element.Rotation;
                _rotationX = element.RotationX;
                _rotationY = element.RotationY;
            }

            public static implicit operator CATransform3D(Transform tr)
            {
                var ve = tr._element;

                double anchorX = ve.AnchorX,
                    anchorY = ve.AnchorY,
                    width = ve.Width,
                    height = ve.Height,
                    translationX = tr._translationX,
                    translationY = tr._translationY,
                    scale = tr._scale,
                    rotation = tr._rotation,
                    rotationX = tr._rotationX,
                    rotationY = tr._rotationY;

                var caTransform3D = CATransform3D.Identity;
                if (Math.Abs(anchorX - 0.5) > 0.001)
                    caTransform3D = caTransform3D.Translate((nfloat)((anchorX - 0.5f) * width), 0, 0);
                if (Math.Abs(anchorY - 0.5) > 0.001)
                    caTransform3D = caTransform3D.Translate(0, (nfloat)((anchorY - 0.5f) * height), 0);
                if (Math.Abs(translationX) > 0.001 || Math.Abs(translationY) > 0.001)
                    caTransform3D = caTransform3D.Translate((nfloat)translationX, (nfloat)translationY, 0);
                if (Math.Abs(scale - 1f) > 0.001)
                    caTransform3D = caTransform3D.Scale((nfloat)scale);
                if (Math.Abs(rotationY % 180f) > 0.001 || Math.Abs(rotationX % 180f) > 0.001)
                    caTransform3D.m34 = -1f / 400f;
                if (Math.Abs(rotationX % 360f) > 0.001)
                    caTransform3D = caTransform3D.Rotate((float)(rotationX * Math.PI / 180.0), 1f, 0.0f, 0.0f);
                if (Math.Abs(rotationY % 360f) > 0.001)
                    caTransform3D = caTransform3D.Rotate((float)(rotationY * Math.PI / 180.0), 0.0f, 1f, 0.0f);

                return caTransform3D.Rotate((float)(rotation * Math.PI / 180.0), 0.0f, 0.0f, 1f);
            }

            public static Transform operator -(Transform l, Transform r) => new Transform
            {
                _element = l._element,
                _translationX = l._translationX - r._translationX,
                _translationY = l._translationY - r._translationY,
                _scale = l._scale - r._scale,
                _rotation = l._rotation - r._rotation,
                _rotationX = l._rotationX - r._rotationX,
                _rotationY = l._rotationY - r._rotationY,
            };

            public static Transform operator *(Transform l, double f) => new Transform
            {
                _element = l._element,
                _translationX = l._translationX * f,
                _translationY = l._translationY * f,
                _scale = l._scale * f,
                _rotation = l._rotation * f,
                _rotationX = l._rotationX * f,
                _rotationY = l._rotationY * f,
            };

            public static Transform operator +(Transform l, Transform r) => new Transform
            {
                _element = l._element,
                _translationX = l._translationX + r._translationX,
                _translationY = l._translationY + r._translationY,
                _scale = l._scale + r._scale,
                _rotation = l._rotation + r._rotation,
                _rotationX = l._rotationX + r._rotationX,
                _rotationY = l._rotationY + r._rotationY,
            };
        }

        private class TransformInterpolator : IInterpolator<Transform>
        {
            public NSObject ProvideValue(double delta)
            {
                var td = To - From;
                return NSValue.FromCATransform3D(td * delta + From);
            }

            public Transform From { get; set; }
            public Transform To { get; set; }
        }
    }
}