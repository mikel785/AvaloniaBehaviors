﻿using System;
using System.Collections.Generic;
using Avalonia.Controls;

namespace Avalonia.Xaml.Interactivity
{
    /// <summary>
    /// Defines a <see cref="BehaviorCollection"/> attached property and provides a method for executing an <seealso cref="ActionCollection"/>.
    /// </summary>
    public sealed class Interaction
    {
        static Interaction()
        {
            BehaviorsProperty.Changed.Subscribe(e =>
            {
                var oldCollection = (BehaviorCollection?)e.OldValue;
                var newCollection = (BehaviorCollection?)e.NewValue;

                if (oldCollection == newCollection)
                {
                    return;
                }

                if (oldCollection != null && oldCollection.AssociatedObject != null)
                {
                    oldCollection.Detach();
                }

                if (newCollection != null && e.Sender != null)
                {
                    newCollection.Attach(e.Sender);
                }
            });
        }

        /// <summary>
        /// Gets or sets the <see cref="BehaviorCollection"/> associated with a specified object.
        /// </summary>
        public static readonly AttachedProperty<BehaviorCollection?> BehaviorsProperty =
            AvaloniaProperty.RegisterAttached<Interaction, IAvaloniaObject, BehaviorCollection?>("Behaviors");

        /// <summary>
        /// Gets the <see cref="BehaviorCollection"/> associated with a specified object.
        /// </summary>
        /// <param name="obj">The <see cref="IAvaloniaObject"/> from which to retrieve the <see cref="BehaviorCollection"/>.</param>
        /// <returns>A <see cref="BehaviorCollection"/> containing the behaviors associated with the specified object.</returns>
        public static BehaviorCollection? GetBehaviors(IAvaloniaObject obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            BehaviorCollection? behaviorCollection = (BehaviorCollection?)obj.GetValue(BehaviorsProperty);
            if (behaviorCollection == null)
            {
                behaviorCollection = new BehaviorCollection();
                obj.SetValue(BehaviorsProperty, behaviorCollection);

                if (obj is Control control)
                {
                    control.AttachedToVisualTree -= Control_Loaded;
                    control.AttachedToVisualTree += Control_Loaded;
                    control.DetachedFromVisualTree -= Control_Unloaded;
                    control.DetachedFromVisualTree += Control_Unloaded;
                }
            }

            return behaviorCollection;
        }

        /// <summary>
        /// Sets the <see cref="BehaviorCollection"/> associated with a specified object.
        /// </summary>
        /// <param name="obj">The <see cref="IAvaloniaObject"/> on which to set the <see cref="BehaviorCollection"/>.</param>
        /// <param name="value">The <see cref="BehaviorCollection"/> associated with the object.</param>
        public static void SetBehaviors(IAvaloniaObject obj, BehaviorCollection? value)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }
            obj.SetValue(BehaviorsProperty, value);
        }

        /// <summary>
        /// Executes all actions in the <see cref="ActionCollection"/> and returns their results.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> which will be passed on to the action.</param>
        /// <param name="actions">The set of actions to execute.</param>
        /// <param name="parameter">The value of this parameter is determined by the calling behavior.</param>
        /// <returns>Returns the results of the actions.</returns>
        public static IEnumerable<object> ExecuteActions(object? sender, ActionCollection? actions, object? parameter)
        {
            List<object> results = new List<object>();

            if (actions == null)
            {
                return results;
            }

            foreach (var avaloniaObject in actions)
            {
                IAction action = (IAction)avaloniaObject;
                object? result = action.Execute(sender, parameter);
                if (result != null)
                {
                    results.Add(result);
                }
            }

            return results;
        }

        private static void Control_Loaded(object sender, VisualTreeAttachmentEventArgs e)
        {
            if (sender is IAvaloniaObject d)
            {
                GetBehaviors(d)?.Attach(d);
            }
        }

        private static void Control_Unloaded(object sender, VisualTreeAttachmentEventArgs e)
        {
            if (sender is IAvaloniaObject d)
            {
                GetBehaviors(d)?.Detach();
            }
        }
    }
}
