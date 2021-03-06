﻿using System;
using Avalonia.Controls;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom
{
    /// <summary>
    /// Binds AssociatedObject object Tag property to root visual DataContext.
    /// </summary>
    public sealed class BindTagToVisualRootDataContextBehavior : Behavior<Control>
    {
        private IDisposable? _disposable;

        /// <inheritdoc/>
        protected override void OnAttached()
        {
            base.OnAttached();
            if (AssociatedObject != null)
            {
                AssociatedObject.AttachedToVisualTree += AttachedToVisualTree; 
            }
        }

        /// <inheritdoc/>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            if (AssociatedObject != null)
            {
                AssociatedObject.AttachedToVisualTree -= AttachedToVisualTree; 
            }
            _disposable?.Dispose();
        }

        private void AttachedToVisualTree(object sender, VisualTreeAttachmentEventArgs e)
        {
            _disposable = BindDataContextToTag((IControl)AssociatedObject.GetVisualRoot(), AssociatedObject);
        }

        private static IDisposable? BindDataContextToTag(IControl source, IControl? target)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (target == null)
                throw new ArgumentNullException(nameof(target));

            var data = source.GetObservable(Control.DataContextProperty);
            if (data != null)
            {
                return target.Bind(Control.TagProperty, data);
            }
            return null;
        }
    }
}
