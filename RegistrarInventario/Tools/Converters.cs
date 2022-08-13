using System;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace RegistrarInventario
{
    //Usar DateTimeConverter solo con DataGrids ya que ellos hacen las conversiones solo al salir de la fila (y no al escribir cada caracter como en un TextBox separado)
    //Convierte los DateTime con Ticks en 0 a un string vacío
    public class DateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                DateTime dateTime = (DateTime)value;
                if (dateTime.Ticks == 0)
                    return "";
                else
                    return dateTime.ToString(App.DATETIME_FORMAT, CultureInfo.InvariantCulture);
            }
            else return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!string.IsNullOrWhiteSpace((string)value) && DateTime.TryParseExact((string)value, App.DATETIME_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime))
                return dateTime;
            else
                return new DateTime();  //Evita error para no asignar null a un DateTime que no es nullable (que no es DateTime?), pero este new DateTime() devolverá los Ticks en 0 y es lo que hay que validar luego si se quiere nulls
        }
    }


    //Inverte un boolean
    public class InvertBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => !(bool)value;
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => !(bool)value;
    }


    //Suma una cantidad a un double. La cantidad introducida como parámetro desde el XAML entra como string, por ello se usa double.Parse((string)parameter)
    public class AddStringToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (double)((double)value + double.Parse((string)parameter));
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => (double)((double)value - double.Parse((string)parameter));
    }


    //Devuelve true si el valor es mayor a 1
    public class IntegerIsGreaterThanOneToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (int)value > 1;
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }


    //Combina dos strings usando como % a donde se va a colocar value
    public class JoinStringsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (parameter as string).Replace("%", (string)value);
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
