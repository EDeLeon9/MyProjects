using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AttachedBehaviors
{
    public enum EventTypeOptions
    {
        None,
        Click,
        MouseLeftDoubleClick,
        MouseRightButtonUp,
        GotFocus,
        LostFocus,
        SelectedCellsChanged
    }

    public static class AttachedBehavior
    {
        //--- Typear propa y dar TAB dos veces si desea crear un nuevo Attached Property ---//

        public static EventTypeOptions GetEventType(DependencyObject obj) { return (EventTypeOptions)obj.GetValue(EventTypeProperty); }
        public static void SetEventType(DependencyObject obj, EventTypeOptions value) { obj.SetValue(EventTypeProperty, value); }
        public static readonly DependencyProperty EventTypeProperty = DependencyProperty.RegisterAttached("EventType", typeof(EventTypeOptions), typeof(AttachedBehavior), new PropertyMetadata(EventTypeOptions.None, new PropertyChangedCallback(OnEventTypeChanged)));

        public static ICommand GetCommand(DependencyObject obj) { return (ICommand)obj.GetValue(CommandProperty); }
        public static void SetCommand(DependencyObject obj, ICommand value) { obj.SetValue(CommandProperty, value); }
        public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(AttachedBehavior), new PropertyMetadata(null));

        public static object GetCommandParameter(DependencyObject obj) { return (object)obj.GetValue(CommandParameterProperty); }
        public static void SetCommandParameter(DependencyObject obj, object value) { obj.SetValue(CommandParameterProperty, value); }
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.RegisterAttached("CommandParameter", typeof(object), typeof(AttachedBehavior), new PropertyMetadata(null));

        public static bool GetParamIsSender(DependencyObject obj) { return (bool)obj.GetValue(ParamIsSenderProperty); }
        public static void SetParamIsSender(DependencyObject obj, bool value) { obj.SetValue(ParamIsSenderProperty, value); }
        public static readonly DependencyProperty ParamIsSenderProperty = DependencyProperty.RegisterAttached("ParamIsSender", typeof(bool), typeof(AttachedBehavior), new PropertyMetadata(false));



        private static void OnEventTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Control control = (Control)d;
            if (control != null)
            {
                switch ((EventTypeOptions)e.OldValue)
                {
                    case EventTypeOptions.Click:
                        control.MouseLeftButtonUp -= OnEvent;
                        break;
                    case EventTypeOptions.MouseLeftDoubleClick:
                        control.MouseDoubleClick -= OnMouseLeftDoubleClick;
                        break;
                    case EventTypeOptions.MouseRightButtonUp:
                        control.MouseRightButtonUp -= OnMouseEvent;
                        break;
                    case EventTypeOptions.GotFocus:
                        control.GotFocus -= OnEvent;
                        break;
                    case EventTypeOptions.LostFocus:
                        control.LostFocus -= OnEvent;
                        break;
                    case EventTypeOptions.SelectedCellsChanged:
                        (control as DataGrid).SelectedCellsChanged -= OnSelectedCellsChanged;
                        break;
                }

                switch ((EventTypeOptions)e.NewValue)
                {
                    case EventTypeOptions.Click:
                        control.MouseLeftButtonUp += OnEvent;
                        break;
                    case EventTypeOptions.MouseLeftDoubleClick:
                        control.MouseDoubleClick += OnMouseLeftDoubleClick;
                        break;
                    case EventTypeOptions.MouseRightButtonUp:
                        control.MouseRightButtonUp += OnMouseEvent;
                        break;
                    case EventTypeOptions.GotFocus:
                        control.GotFocus += OnEvent;
                        break;
                    case EventTypeOptions.LostFocus:
                        control.LostFocus += OnEvent;
                        break;
                    case EventTypeOptions.SelectedCellsChanged:
                        (control as DataGrid).SelectedCellsChanged += OnSelectedCellsChanged;
                        break;
                }
            }
        }


        private static void OnEvent(object sender, RoutedEventArgs e)
        {
            Control control = (Control)sender;
            if (control != null)
            {
                ICommand command = GetCommand(control);
                object commandParameter;

                if (GetParamIsSender(control))
                    commandParameter = sender;
                else
                    commandParameter = GetCommandParameter(control);

                command?.Execute(commandParameter);
            }
        }

        private static void OnMouseEvent(object sender, MouseButtonEventArgs e)
        {
            OnEvent(sender, null);
        }

        private static void OnMouseLeftDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                OnEvent(sender, null);
        }

        private static void OnSelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            OnEvent(sender, null);
        }
    }
}
