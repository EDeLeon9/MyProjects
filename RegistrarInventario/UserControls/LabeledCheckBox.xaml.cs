using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace RegistrarInventario
{
    public partial class LabeledCheckBox : UserControl
    {
        public string LblContent { get { return (string)GetValue(LblContentProperty); } set { SetValue(LblContentProperty, value); } }
        public static readonly DependencyProperty LblContentProperty = DependencyProperty.Register("LblContent", typeof(string), typeof(LabeledCheckBox), new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public double LblWidth { get { return (double)GetValue(LblWidthProperty); } set { SetValue(LblWidthProperty, value); } }
        public static readonly DependencyProperty LblWidthProperty = DependencyProperty.Register("LblWidth", typeof(double), typeof(LabeledCheckBox), new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public bool ChkIsChecked { get { return (bool)GetValue(ChkIsCheckedProperty); } set { SetValue(ChkIsCheckedProperty, value); } }
        public static readonly DependencyProperty ChkIsCheckedProperty = DependencyProperty.Register("ChkIsChecked", typeof(bool), typeof(LabeledCheckBox), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));



        public LabeledCheckBox()
        {
            InitializeComponent();
        }
    }
}
