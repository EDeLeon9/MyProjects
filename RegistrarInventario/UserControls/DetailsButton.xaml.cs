using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RegistrarInventario
{
    public partial class DetailsButton : UserControl
    {
        private Transform renderTransform1;
        private Transform renderTransform2;
        public event EventHandler ButtonClick;  //EventHandler es un delegado (en vez de declarar un delegado (un delegado se declara fuera de una clase) se usa EventHandler);

        public bool Expanded { get { return (bool)GetValue(ExpandidoProperty); } set { SetValue(ExpandidoProperty, value); } }
        public static readonly DependencyProperty ExpandidoProperty = DependencyProperty.Register("Expandido", typeof(bool), typeof(DetailsButton), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string ContentText { get { return (string)GetValue(ContentTextProperty); } set { SetValue(ContentTextProperty, value); } }
        public static readonly DependencyProperty ContentTextProperty = DependencyProperty.Register("ContentText", typeof(string), typeof(DetailsButton), new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));



        public DetailsButton()
        {
            InitializeComponent();
            renderTransform1 = path1.RenderTransform;
            renderTransform2 = path2.RenderTransform;
        }

        private void btnDetalles_Click(object sender, RoutedEventArgs e)
        {
            if (Expanded)
            {
                path1.RenderTransform = renderTransform1;
                path2.RenderTransform = renderTransform2;
                Expanded = false;
            }
            else
            {
                path1.RenderTransform = null;
                path2.RenderTransform = null;
                Expanded = true;
            }

            ButtonClick?.Invoke(sender, e);  //Se usa sender en vez de this para que el sender sea el botón y no todo el User Control
        }
    }
}
