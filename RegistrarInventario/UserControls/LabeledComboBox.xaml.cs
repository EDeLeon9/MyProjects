using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;

namespace RegistrarInventario
{
    public partial class LabeledComboBox : UserControl
    {
        public string LblContent { get { return (string)GetValue(LblContentProperty); } set { SetValue(LblContentProperty, value); } }
        public static readonly DependencyProperty LblContentProperty = DependencyProperty.Register("LblContent", typeof(string), typeof(LabeledComboBox), new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public double LblWidth { get { return (double)GetValue(LblWidthProperty); } set { SetValue(LblWidthProperty, value); } }
        public static readonly DependencyProperty LblWidthProperty = DependencyProperty.Register("LblWidth", typeof(double), typeof(LabeledComboBox), new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public IEnumerable CboItemsSource { get { return (IEnumerable)GetValue(CboItemsSourceProperty); } set { SetValue(CboItemsSourceProperty, value); } }
        public static readonly DependencyProperty CboItemsSourceProperty = DependencyProperty.Register("CboItemsSource", typeof(IEnumerable), typeof(LabeledComboBox), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnCboItemsSourceChanged)));

        public string CboSelectedValuePath { get { return (string)GetValue(CboSelectedValuePathProperty); } set { SetValue(CboSelectedValuePathProperty, value); } }
        public static readonly DependencyProperty CboSelectedValuePathProperty = DependencyProperty.Register("CboSelectedValuePath", typeof(string), typeof(LabeledComboBox), new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string CboDisplayMemberPath { get { return (string)GetValue(CboDisplayMemberPathProperty); } set { SetValue(CboDisplayMemberPathProperty, value); } }
        public static readonly DependencyProperty CboDisplayMemberPathProperty = DependencyProperty.Register("CboDisplayMemberPath", typeof(string), typeof(LabeledComboBox), new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public object CboSelectedValue { get { return (object)GetValue(CboSelectedValueProperty); } set { SetValue(CboSelectedValueProperty, value); } }
        public static readonly DependencyProperty CboSelectedValueProperty = DependencyProperty.Register("CboSelectedValue", typeof(object), typeof(LabeledComboBox), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public object CboSelectedItem { get { return (object)GetValue(CboSelectedItemProperty); } set { SetValue(CboSelectedItemProperty, value); } }
        public static readonly DependencyProperty CboSelectedItemProperty = DependencyProperty.Register("CboSelectedItem", typeof(object), typeof(LabeledComboBox), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string CboText { get { return (string)GetValue(CboTextProperty); } set { SetValue(CboTextProperty, value); } }
        public static readonly DependencyProperty CboTextProperty = DependencyProperty.Register("CboText", typeof(string), typeof(LabeledComboBox), new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public DataTemplate CboItemTemplate { get { return (DataTemplate)GetValue(CboItemTemplateProperty); } set { SetValue(CboItemTemplateProperty, value); } }
        public static readonly DependencyProperty CboItemTemplateProperty = DependencyProperty.Register("CboItemTemplate", typeof(DataTemplate), typeof(LabeledComboBox), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string CboColumnNames { get { return (string)GetValue(CboColumnNamesProperty); } set { SetValue(CboColumnNamesProperty, value); } }
        public static readonly DependencyProperty CboColumnNamesProperty = DependencyProperty.Register("CboColumnNames", typeof(string), typeof(LabeledComboBox), new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnCboColumnNamesChanged)));

        public ControlTemplate CboControlTemplateItems { get { return (ControlTemplate)GetValue(CboControlTemplateItemsProperty); } set { SetValue(CboControlTemplateItemsProperty, value); } }
        public static readonly DependencyProperty CboControlTemplateItemsProperty = DependencyProperty.Register("CboControlTemplateItems", typeof(ControlTemplate), typeof(LabeledComboBox), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public bool CboIsEditable { get { return (bool)GetValue(CboIsEditableProperty); } set { SetValue(CboIsEditableProperty, value); } }
        public static readonly DependencyProperty CboIsEditableProperty = DependencyProperty.Register("CboIsEditable", typeof(bool), typeof(LabeledComboBox), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));



        public LabeledComboBox()
        {
            InitializeComponent();
            this.Dispatcher.BeginInvoke(() => SetTextBoxEvents(cboComboBox), DispatcherPriority.Background);  //Background asegura que esté todo cargado ya que es un nivel bajo de priority suficiente para que se ejecute bien y no tenga tanto delay
        }

        private static void OnCboColumnNamesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LabeledComboBox labeledComboBox = (LabeledComboBox)d;
            if (!DesignerProperties.GetIsInDesignMode(d) && !string.IsNullOrWhiteSpace(labeledComboBox.CboColumnNames))
            {
                List<string> campos = new List<string>(labeledComboBox.CboColumnNames.Split(","));
                //Para que funcionen mejor los triggers de IsHighlighted y IsMouseOver del style en los resources se deben usar controles que se estiren por todo el Grid y 
                //también que se les pueda poner un borde a la izquierda, por ello se usan Labels en vez de TextBlocks.
                List<Label> labels = new List<Label>();
                Grid grid = new Grid();

                for (int i = 0; i < campos.Count; i++)
                {
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { SharedSizeGroup = campos[i].Trim() + "Group" });
                    if (i == 0)
                        //No se asigna Label.SetBinding() ya que XamlWriter.Save() no lo genera, por eso se asigna como string. Pero si se usa Content para el string de binding éste se 
                        //pone entre etiquetas <Label></Label>, por eso se usa Tag (propiedad auxiliar) para luego reemplazarlo por Content en el string de XamlWriter.Save().
                        labels.Add(new Label() { Padding = new Thickness(5, 2, 5, 2), Tag = "{Binding " + campos[i].Trim() + "}" });
                    else
                        labels.Add(new Label() { Padding = new Thickness(5, 2, 5, 2), Tag = "{Binding " + campos[i].Trim() + "}", BorderBrush = new SolidColorBrush(Colors.DarkGray), BorderThickness = new Thickness(1, 0, 0, 0) });
                    Grid.SetColumn(labels[^1], i * 2);
                    grid.Children.Add(labels[^1]);

                    //Se agregan columnas con Labels vacíos para que se distribuyan bien los tamaños en el Grid ya que los labels de las otras columnas se les asigna 
                    //SharedSizeGroup y por ello no se expanden. Y con esto ya no es necesario asignar Width = Auto a las columnas.
                    grid.ColumnDefinitions.Add(new ColumnDefinition());
                    labels.Add(new Label());
                    Grid.SetColumn(labels[^1], (i * 2) + 1);
                    grid.Children.Add(labels[^1]);
                }

                string xmlString = XamlWriter.Save(new ControlTemplate(typeof(Control))).Replace("/>", "> {0} </ControlTemplate>");
                xmlString = string.Format(xmlString, XamlWriter.Save(grid)).Replace("Tag=\"{}", "Content=\"");  //cambia Tag por Content y elimina es escape {} que le agrega a los Labels.
                labeledComboBox.CboControlTemplateItems = (ControlTemplate)XamlReader.Parse(xmlString);
                labeledComboBox.cboComboBox.ItemContainerStyle = (Style)labeledComboBox.FindResource("multipleColumnsStyle");
            }
        }

        private static void OnCboItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ComboBox comboBox = ((LabeledComboBox)d).cboComboBox;
            ScrollViewer scrollViewer = (ScrollViewer)(comboBox.Template.FindName("DropDownScrollViewer", comboBox));
            scrollViewer?.ScrollToTop();
        }


        #region ScrollTextBoxToHome
        private bool mouseIsPressingTextBox;  //Usado en TextBox_MouseButton() para saber si se esta seleccionando manualmente el texto

        private void SetTextBoxEvents(ComboBox comboBox)
        {
            if (comboBox.IsEditable)
            {
                TextBox textBox = (TextBox)comboBox.Template.FindName("PART_EditableTextBox", comboBox);
                if (textBox != null)
                {
                    textBox.SelectionChanged += ScrollTextBoxToHome;
                    textBox.PreviewMouseDown += TextBox_MouseButton;
                    textBox.PreviewMouseUp += TextBox_MouseButton;
                }
            }
        }

        private void TextBox_MouseButton(object sender, MouseButtonEventArgs e) => mouseIsPressingTextBox = e.ButtonState == MouseButtonState.Pressed;

        private void ScrollTextBoxToHome(object sender, RoutedEventArgs e)  //Corrige que se pueda ver el inicio del texto del ComboBox editable cuando se selecciona un nuevo elemento o al abrir el DropDown
        {
            TextBox textBox = (TextBox)sender;
            if (textBox != null
                && Keyboard.IsKeyUp(Key.LeftShift)
                && Keyboard.IsKeyUp(Key.RightShift)
                && (Mouse.LeftButton == MouseButtonState.Released || (Mouse.LeftButton == MouseButtonState.Pressed && !mouseIsPressingTextBox))
                && (Mouse.RightButton == MouseButtonState.Released || (Mouse.RightButton == MouseButtonState.Pressed && !mouseIsPressingTextBox))
                && textBox.SelectionStart == 0
                && textBox.SelectionLength == textBox.Text.Length
                && textBox.SelectionLength != 0
            )
            {
                //textBox.ScrollToHome();  Comentado porque no funciona como se quiere
                textBox.SelectionLength = 0;  //Solo se asigna SelectionLength = 0 ya que si entra en este if ya tiene SelectionStart == 0
            }
        }
        #endregion
    }
}
