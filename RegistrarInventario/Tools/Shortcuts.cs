using System;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using DBConnections;

namespace RegistrarInventario
{
    public static class SCut
    {
        public static int ToInt(string sNum)
        {
            int.TryParse(sNum, out int num);  //Si sNum es null devolverá 0
            return num;
        }

        public static decimal ToDecimal(string sNum)
        {
            decimal.TryParse(sNum, out decimal num);  //Si sNum es null devolverá 0
            return num;
        }

        public static string NullIfUnassigned(string value, bool isText)
        {
            if (isText)
                return string.IsNullOrWhiteSpace(value) ? "NULL" : "\'" + value + "\'";
            else
                return SCut.ToDecimal(value) == 0 ? "NULL" : value;
        }

        public static bool IsValidId(string sNum)
        {
            return !Regex.IsMatch(sNum ?? "", "[+-., ]") && int.TryParse(sNum, out int intNum) && intNum > 0;
        }

        public static string DateTimeStringToDB(string dateTimeWithAppFormat)
        {
            DateTime dateTime = (new DateTimeConverter().ConvertBack(dateTimeWithAppFormat, null, null, null) as DateTime?).GetValueOrDefault();
            if (dateTime.Ticks > 0)
                return dateTime.ToString(DBConnection.DB_DATETIME_FORMAT, CultureInfo.InvariantCulture);
            else 
                return "";
        }

        public static void DispatcherAux(Action action)
        {
            DependencyObject auxObject = new DependencyObject();
            if (!DesignerProperties.GetIsInDesignMode(auxObject))
                auxObject.Dispatcher.BeginInvoke(action, DispatcherPriority.Background);  //Background asegura que esté todo cargado ya que es un nivel bajo de priority suficiente para que se ejecute bien y no tenga tanto delay
        }

        public static Action<object> GetScrollGridToObjectDelegate(DataGrid dataGrid)
        {
            return (obj) =>
            {
                if (dataGrid != null && obj != null)
                {
                    dataGrid.UpdateLayout();
                    dataGrid.ScrollIntoView(obj);
                    dataGrid.Focus();
                }
            };
        }
    }
}
