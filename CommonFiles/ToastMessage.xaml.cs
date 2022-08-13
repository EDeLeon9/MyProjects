using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace ToastMessageNotification
{
    public partial class ToastMessage : Window
    {
        private Window activeWindow;

        private static List<ToastMessage> toastMessageList = new List<ToastMessage>();

        public static void Show(string message)
        {
            ToastMessage toastMessage = new ToastMessage(message);
            toastMessage.Show();
        }

        public ToastMessage(string message)
        {
            activeWindow = Application.Current.Windows.OfType<Window>().FirstOrDefault(x => x.IsActive);
            InitializeComponent();
            this.Opacity = 0;
            txtMessage.Text = message;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                if (activeWindow != null)
                {
                    this.Left = activeWindow.Left + (activeWindow.ActualWidth / 2) - (this.ActualWidth / 2);
                    this.Top = activeWindow.Top + activeWindow.Height - this.ActualHeight - 40;
                }
                else
                {
                    this.Left = (SystemParameters.WorkArea.Width / 2) - (this.ActualWidth / 2);
                    this.Top = SystemParameters.WorkArea.Height - 65;
                }

                toastMessageList.Add(this);

                if (toastMessageList.Count > 1)
                {
                    this.Top = toastMessageList[^2].Top - this.ActualHeight - 3;
                }

                this.Opacity = 1;  //Se activa el Opacity luego de haber dado la posición correcta a la ventana.
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void DoubleAnimation_Completed(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            toastMessageList.Remove(this);
        }
    }
}
