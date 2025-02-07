﻿using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace FontAwesome.WPF
{
    /// <summary>
    /// Control extensions
    /// </summary>
    public static class ControlExtensions
    {
        /// <summary>
        /// The key used for storing the spinner Storyboard.
        /// </summary>
        private static readonly string SpinnerStoryBoardName = string.Format("{0}Spinner", typeof(FontAwesome).Name);

        /// <summary>
        /// Start the spinning animation
        /// </summary>
        /// <typeparam name="T">FrameworkElement and ISpinable</typeparam>
        /// <param name="control">Control to apply the rotation </param>
        public static void BeginSpin<T>(this T control)
            where T : FrameworkElement, ISpinable
        {
            var transformGroup = control.RenderTransform as TransformGroup ?? new TransformGroup();

            var rotateTransform = transformGroup.Children.OfType<RotateTransform>().FirstOrDefault();

            if (rotateTransform != null)
            {
                rotateTransform.Angle = 0;
            }
            else
            {
                transformGroup.Children.Add(new RotateTransform(0.0));
                control.RenderTransform = transformGroup;
                control.RenderTransformOrigin = new Point(0.5, 0.5);
            }

            var storyboard = new Storyboard();

            var animation = new DoubleAnimation
            {
                From = 0,
                To = 360,
                AutoReverse = false,
                RepeatBehavior = RepeatBehavior.Forever,
                Duration = new Duration(TimeSpan.FromSeconds(control.SpinDuration))
            };
            storyboard.Children.Add(animation);

            Storyboard.SetTarget(animation, control);
            Storyboard.SetTargetProperty(animation,
                new PropertyPath("(0).(1)[0].(2)", UIElement.RenderTransformProperty,
                    TransformGroup.ChildrenProperty, RotateTransform.AngleProperty));

            storyboard.Begin();
            control.Resources.Add(SpinnerStoryBoardName, storyboard);
        }

        /// <summary>
        /// Stop the spinning animation 
        /// </summary>
        /// <typeparam name="T">FrameworkElement and ISpinable</typeparam>
        /// <param name="control">Control to stop the rotation.</param>
        public static void StopSpin<T>(this T control)
            where T : FrameworkElement, ISpinable
        {
            if (!(control.Resources[SpinnerStoryBoardName] is Storyboard storyboard)) 
                return;

            storyboard.Stop();

            control.Resources.Remove(SpinnerStoryBoardName);
        }

        /// <summary>
        /// Sets the rotation for the control
        /// </summary>
        /// <typeparam name="T">FrameworkElement and IRotatable</typeparam>
        /// <param name="control">Control to apply the rotation</param>
        public static void SetRotation<T>(this T control)
            where T : FrameworkElement, IRotatable
        {
            var transformGroup = control.RenderTransform as TransformGroup ?? new TransformGroup();

            var rotateTransform = transformGroup.Children.OfType<RotateTransform>().FirstOrDefault();

            if (rotateTransform != null)
            {
                rotateTransform.Angle = control.Rotation;
            }
            else
            {
                transformGroup.Children.Add(new RotateTransform(control.Rotation));
                control.RenderTransform = transformGroup;
                control.RenderTransformOrigin = new Point(0.5, 0.5);
            }
        }

        /// <summary>
        /// Sets the flip orientation for the control
        /// </summary>
        /// <typeparam name="T">FrameworkElement and IRotatable</typeparam>
        /// <param name="control">Control to apply the rotation</param>
        public static void SetFlipOrientation<T>(this T control)
            where T : FrameworkElement, IFlippable
        {
            var transformGroup = control.RenderTransform as TransformGroup ?? new TransformGroup();

            int scaleX = control.FlipOrientation == FlipOrientation.Normal || control.FlipOrientation == FlipOrientation.Vertical ? 1 : -1;
            int scaleY = control.FlipOrientation == FlipOrientation.Normal || control.FlipOrientation == FlipOrientation.Horizontal ? 1 : -1;

            var scaleTransform = transformGroup.Children.OfType<ScaleTransform>().FirstOrDefault();

            if (scaleTransform != null)
            {
                scaleTransform.ScaleX = scaleX;
                scaleTransform.ScaleY = scaleY;
            }
            else
            {
                transformGroup.Children.Add(new ScaleTransform(scaleX, scaleY));
                control.RenderTransform = transformGroup;
                control.RenderTransformOrigin = new Point(0.5, 0.5);
            }
        }
    }
}
