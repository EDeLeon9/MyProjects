using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace RegistrarInventario
{
    public static class AttachedProperty
    {
        //--- Typear propa y dar TAB dos veces si desea crear un nuevo Attached Property ---//

        public static Page GetFramePage(DependencyObject obj) { return (Page)obj.GetValue(FramePageProperty); }
        public static void SetFramePage(DependencyObject obj, Page value) { obj.SetValue(FramePageProperty, value); }
        public static readonly DependencyProperty FramePageProperty = DependencyProperty.RegisterAttached("FramePage", typeof(Page), typeof(AttachedProperty), new PropertyMetadata(null, new PropertyChangedCallback(OnFramePageChanged)));

        private static void OnFramePageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Frame frame = (Frame)d;
            if (frame != null)
            {
                frame.NavigationUIVisibility = NavigationUIVisibility.Hidden;
                if (e.NewValue != null)
                    frame.NavigationService.Navigate((Page)e.NewValue);
                else
                {
                    frame.NavigationService.RemoveBackEntry();
                    frame.NavigationService.Content = null;
                }
            }
        }
    }
}
