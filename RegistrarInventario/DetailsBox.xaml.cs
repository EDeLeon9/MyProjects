using System;
using System.Collections.Generic;
using System.Media;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RegistrarInventario
{
    /// <summary>
    /// Lógica de interacción para DetailsBox.xaml
    /// </summary>
    public partial class DetailsBox : Window
    {

        public string WinTitle { get { return (string)GetValue(WinTitleProperty); } set { SetValue(WinTitleProperty, value); } }
        public static readonly DependencyProperty WinTitleProperty = DependencyProperty.Register("WinTitle", typeof(string), typeof(DetailsBox), new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        
        public string TxtContent { get { return (string)GetValue(TxtContentProperty); } set { SetValue(TxtContentProperty, value); } }
        public static readonly DependencyProperty TxtContentProperty = DependencyProperty.Register("TxtContent", typeof(string), typeof(DetailsBox), new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public String TxtDetails { get { return (String)GetValue(TxtDetailsProperty); } set { SetValue(TxtDetailsProperty, value); } }
        public static readonly DependencyProperty TxtDetailsProperty = DependencyProperty.Register("TxtDetails", typeof(String), typeof(DetailsBox), new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string TxtDetailsButtonText { get { return (string)GetValue(TxtDetailsButtonTextProperty); } set { SetValue(TxtDetailsButtonTextProperty, value); } }
        public static readonly DependencyProperty TxtDetailsButtonTextProperty = DependencyProperty.Register("TxtDetailsButtonText", typeof(string), typeof(DetailsBox), new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string TxtCloseButtonText { get { return (string)GetValue(TxtCloseButtonTextProperty); } set { SetValue(TxtCloseButtonTextProperty, value); } }
        public static readonly DependencyProperty TxtCloseButtonTextProperty = DependencyProperty.Register("TxtCloseButtonText", typeof(string), typeof(DetailsBox), new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));


        private double winMinHeight = 0;

        public DetailsBox()
        {
            InitializeComponent();
            this.DataContext = this;
            this.Owner = App.Current.MainWindow;  //Para que quede centrada la ventana.

            winMinHeight = this.MinHeight;

            bdrDetails.Visibility = Visibility.Hidden;
            grdWindow.RowDefinitions[2].Height = new GridLength(0, GridUnitType.Pixel);
        }

        public static void Show(string content, string details, string title = null, string detailsButtonText = "Details", string closeButtonText = "Close")
        {
            DetailsBox box = new DetailsBox();
            box.WinTitle = title == null ? App.Current.MainWindow.Title : title;
            box.TxtContent = content;
            box.TxtDetails = details;
            box.TxtDetailsButtonText = detailsButtonText;
            box.TxtCloseButtonText = closeButtonText;
            box.ShowDialog();
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            SystemSounds.Hand.Play();
        }

        private void btnDetalles_ButtonClick(object sender, EventArgs e)
        {
            this.SizeToContent = SizeToContent.Manual;

            if (bdrDetails.Visibility == Visibility.Hidden)
            {
                grdWindow.RowDefinitions[0].Height = new GridLength(grdWindow.RowDefinitions[0].ActualHeight, GridUnitType.Pixel);
                bdrDetails.Visibility = Visibility.Visible;
                grdWindow.RowDefinitions[2].Height = new GridLength(1, GridUnitType.Star);
                this.MinHeight = this.ActualHeight + 70;
                this.Height = this.ActualHeight + 180;
            }
            else
            {
                this.MinHeight = winMinHeight;
                this.Height = this.ActualHeight - grdWindow.RowDefinitions[2].ActualHeight;
                grdWindow.RowDefinitions[0].Height = new GridLength(1, GridUnitType.Star);
                bdrDetails.Visibility = Visibility.Hidden;
                grdWindow.RowDefinitions[2].Height = new GridLength(0, GridUnitType.Pixel);
            }
        }

        private void btnCerrar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
