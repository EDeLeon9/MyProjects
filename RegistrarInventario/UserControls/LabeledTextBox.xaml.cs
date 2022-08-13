using System;
using System.Windows.Data;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System.Windows.Media;
using System.Text.RegularExpressions;
using System.Media;
using System.Globalization;
using System.Windows.Input;

namespace RegistrarInventario
{
    public partial class LabeledTextBox : UserControl
    {
        //--- Typear propdp y dar TAB dos veces si desea crear una nueva DependencyProperty ---//

        //Explicación de FrameworkPropertyMetadata (antes solo se usaba PropertyMetadata para el valor por defecto): El primer parámetro de FrameworkPropertyMetadata
        //es el valor que toma por defecto si no se usa esta propiedad, el segundo es para tener que colocar Mode=TwoWay en la vista. Nota: el último constructor de 
        //FrameworkPropertyMetadata supuestamente permite activar UpdateSourceTrigger=PropertyChanged para no tener que colocarlo en la vista, pero no funciona.

        private string lastGoodDateTimeText;

        public string LblContent { get { return (string)GetValue(LblContentProperty); } set { SetValue(LblContentProperty, value); } }
        public static readonly DependencyProperty LblContentProperty = DependencyProperty.Register("LblContent", typeof(string), typeof(LabeledTextBox), new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public double LblWidth { get { return (double)GetValue(LblWidthProperty); } set { SetValue(LblWidthProperty, value); } }
        public static readonly DependencyProperty LblWidthProperty = DependencyProperty.Register("LblWidth", typeof(double), typeof(LabeledTextBox), new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));  //Se usa double.NaN ya que es el valor por defecto de Width cuando no se usa en un control

        public double TxtWidth { get { return (double)GetValue(TxtWidthProperty); } set { SetValue(TxtWidthProperty, value); } }
        public static readonly DependencyProperty TxtWidthProperty = DependencyProperty.Register("TxtWidth", typeof(double), typeof(LabeledTextBox), new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));  //Se usa double.NaN ya que es el valor por defecto de Width cuando no se usa en un control

        public string TxtText { get { return (string)GetValue(TxtTextProperty); } set { SetValue(TxtTextProperty, value); } }
        public static readonly DependencyProperty TxtTextProperty = DependencyProperty.Register("TxtText", typeof(string), typeof(LabeledTextBox), new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnTxtTextChanged)));

        public VerticalAlignment TxtVerticalContentAlignment { get { return (VerticalAlignment)GetValue(TxtVerticalContentAlignmentProperty); } set { SetValue(TxtVerticalContentAlignmentProperty, value); } }
        public static readonly DependencyProperty TxtVerticalContentAlignmentProperty = DependencyProperty.Register("TxtVerticalContentAlignment", typeof(VerticalAlignment), typeof(LabeledTextBox), new FrameworkPropertyMetadata(VerticalAlignment.Center, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string NumsNChars { get { return (string)GetValue(NumsNCharsProperty); } set { SetValue(NumsNCharsProperty, value); } }
        public static readonly DependencyProperty NumsNCharsProperty = DependencyProperty.Register("NumsNChars", typeof(string), typeof(LabeledTextBox), new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public bool TxtIsReadOnly { get { return (bool)GetValue(TxtIsReadOnlyProperty); } set { SetValue(TxtIsReadOnlyProperty, value); } }
        public static readonly DependencyProperty TxtIsReadOnlyProperty = DependencyProperty.Register("TxtIsReadOnly", typeof(bool), typeof(LabeledTextBox), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public Brush TxtBackground { get { return (Brush)GetValue(TxtBackgroundProperty); } set { SetValue(TxtBackgroundProperty, value); } }
        public static readonly DependencyProperty TxtBackgroundProperty = DependencyProperty.Register("TxtBackground", typeof(Brush), typeof(LabeledTextBox), new FrameworkPropertyMetadata((Brush)Application.Current.Resources["mainBackground"], FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public TextWrapping TxtTextWrapping { get { return (TextWrapping)GetValue(TxtTextWrappingProperty); } set { SetValue(TxtTextWrappingProperty, value); } }
        public static readonly DependencyProperty TxtTextWrappingProperty = DependencyProperty.Register("TxtTextWrapping", typeof(TextWrapping), typeof(LabeledTextBox), new FrameworkPropertyMetadata(TextWrapping.NoWrap, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));



        public LabeledTextBox()
        {
            InitializeComponent();

            //En lugar de usar esta línea se usa ElementName en la vista del user control para no asignar un DataContext sobre el DataContext de la ventana donde se creará el control
            //this.DataContext = this;  //Asigna la clase y propiedades del objeto construido al DataContext del objeto construido
        }


        //Se valida el cambio en el dependency property TxtTextChanged ya que con eventos no reconoce cuando hacen Copy&Paste
        private static void OnTxtTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LabeledTextBox labeledTextBox = (LabeledTextBox)d;
            string numsNChars = labeledTextBox.NumsNChars;
            string newValue = (string)e.NewValue ?? "";
            string oldValue = (string)e.OldValue ?? "";

            if (numsNChars.Contains("0"))
            {
                if (Regex.IsMatch(newValue, @"[^\d.-]") || (!numsNChars.Contains(".") && newValue.Contains(".")) || (numsNChars.Contains("+") && newValue.Contains("-")))
                {
                    SystemSounds.Beep.Play();
                    labeledTextBox.TxtText = oldValue;  //Si es null entonces es ""
                }
                else if (newValue != "" && newValue != "-")
                {
                    if (!decimal.TryParse(newValue, out _))  // _ es descarte
                    {
                        SystemSounds.Beep.Play();
                        labeledTextBox.TxtText = oldValue;  //Si es null entonces es ""
                    }
                }
            }
            else if (numsNChars.Contains("/") && numsNChars.Contains(":"))  //Fecha(/) y hora(:)
            {
                newValue = newValue.Trim();

                //El pattern [^AaPpMm\d/: ] indica que IsMatch() es true si encuentra algún ([]) caracter que no (^) sea A, a, P, p, M, m, número (\d), /, : ni espacio ( )
                //El pipe (|) es un OR entre las dos expresiones
                //El pattern [/:]{2,} indica que IsMatch() es true si encuentra una combinación entre los caracteres / y : unidos 2 o más veces ({2,})
                //docs.microsoft.com/en-us/dotnet/standard/base-types/regular-expression-language-quick-reference
                //if (!string.IsNullOrWhiteSpace(newValue) && Regex.IsMatch(newValue, @"[^AaPpMm\d/: ]|[/:]{2,}"))
                if (!string.IsNullOrWhiteSpace(newValue) && Regex.IsMatch(newValue, @"[^AaPpMm\d/: ]"))
                {
                    SystemSounds.Beep.Play();
                    labeledTextBox.TxtText = oldValue;
                }
                //Primero se inserta un espacio antes de AM/PM para asegurar que esté el espacio,
                //luego con Regex.Replace() se indica que reemplace todo lo que tenga el pattern [ ]{2,} (que es el caracter espacio 2 o más veces) por un solo espacio
                else if (newValue.Length > 10 && DateTime.TryParseExact(Regex.Replace(newValue.Insert(newValue.Length - 2, " "), "[ ]{2,}", " "), App.DATETIME_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDateTime))
                    labeledTextBox.lastGoodDateTimeText = parsedDateTime.ToString(App.DATETIME_FORMAT, CultureInfo.InvariantCulture);
            }
        }

        private void txtTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (NumsNChars.Contains("0"))
            {
                if (this.TxtText == "-")
                {
                    this.TxtText = "0";
                }
                else if (!string.IsNullOrWhiteSpace(this.TxtText))
                {
                    //Porción de código para mantener los ceros escritos en los decimales (por ejemplo que si se escribió 15.10 no se cambie a 15.1)
                    int parteDecimal = 0;
                    decimal.TryParse(this.TxtText, out decimal num);
                    string[] partes = num.ToString().Split(".");
                    if (partes.Count() == 2)
                    {
                        parteDecimal = partes[1].Length;
                    }
                    string newValue = num.ToString(string.Format("f{0}", parteDecimal.ToString()));
                    if (newValue != this.TxtText)
                    {
                        this.TxtText = newValue;
                    }
                }
            }
            else if (NumsNChars.Contains("/") && NumsNChars.Contains(":"))  //Fecha(/) y hora(:)
            {
                if (!string.IsNullOrWhiteSpace(this.TxtText))
                {
                    //Primero se inserta un espacio antes de AM/PM para asegurar que esté el espacio,
                    //luego con Regex.Replace() se indica que reemplace todo lo que tenga el pattern [ ]{2,} (que es el caracter espacio 2 o más veces) por un solo espacio
                    string trimmedText = this.TxtText.Trim();
                    if (trimmedText.Length > 10 && DateTime.TryParseExact(Regex.Replace(trimmedText.Insert(trimmedText.Length - 2, " "), "[ ]{2,}", " "), App.DATETIME_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDateTime))
                        this.TxtText = parsedDateTime.ToString(App.DATETIME_FORMAT, CultureInfo.InvariantCulture);
                    else
                    {
                        SystemSounds.Beep.Play();
                        this.TxtText = lastGoodDateTimeText;
                    }
                }
                else if (!string.IsNullOrEmpty(this.TxtText))
                {
                    SystemSounds.Beep.Play();
                    this.TxtText = "";
                }
            }
        }

        private void txtTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (NumsNChars.Contains("0") && this.TxtTextWrapping == TextWrapping.NoWrap && !this.TxtIsReadOnly && sender != null)
            {
                TextBox textBox = (TextBox)sender;
                textBox.SelectAll();
            }
            else if (!this.TxtIsReadOnly && string.IsNullOrWhiteSpace(this.TxtText) && NumsNChars.Contains("/") && NumsNChars.Contains(":"))  //Fecha(/) y hora(:)
            {
                this.TxtText = DateTime.Now.ToString(App.DATETIME_FORMAT, CultureInfo.InvariantCulture);
            }
        }

        private void txtTextBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (NumsNChars.Contains("0") && this.TxtTextWrapping == TextWrapping.NoWrap && !this.TxtIsReadOnly && sender != null)
            {
                TextBox textBox = (TextBox)sender;
                if (!textBox.IsFocused)
                {
                    textBox.Focus();
                    e.Handled = true;  //Evita que se haga el clic que hace que se quite lo seleccionado con SelectAll en txtTextBox_GotFocus
                }
            }
        }
    }
}
