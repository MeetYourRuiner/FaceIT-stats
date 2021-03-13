using System;
using System.Windows;

namespace FaceitStats.WPF.Views
{
    public static class FocusExtension
    {
        public static readonly DependencyProperty IsFocusedProperty = DependencyProperty.RegisterAttached(
            "IsFocused",
            typeof(bool?),
            typeof(FocusExtension),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, OnIsFocusedPropertyChanged) { BindsTwoWayByDefault = true }
        );
        public static void SetIsFocused(UIElement element, Boolean value)
        {
            element.SetValue(IsFocusedProperty, value);
        }
        public static Boolean GetIsFocused(UIElement element)
        {
            return (Boolean)element.GetValue(IsFocusedProperty);
        }

        private static void OnIsFocusedPropertyChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var element = (UIElement)d;

            if (e.OldValue == null)
            {
                element.LostFocus += Element_LostFocus;
                element.LostKeyboardFocus += Element_LostFocus;
            }

            if (e.NewValue != null && (bool)e.NewValue)
            {
                element.Focus();
            }
        }

        private static void Element_LostFocus(object sender, RoutedEventArgs e)
        {
            var element = (UIElement)sender;
            element.SetValue(IsFocusedProperty, false);
        }
    }
}
