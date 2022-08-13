using System;
using System.Windows.Data;
using System.Windows;
using SingleInstanceManager;
using System.Windows.Controls;
using DBConnections;

namespace RegistrarInventario
{
    public partial class App : Application
    {
        public const string APP_TITLE = "Registro de Inventario";
        public const string DATETIME_FORMAT = "dd/MM/yyyy h:mm:ss tt";

        public App()
        {
            SingleInstanceApplicationManager.Validate("RegistrarInventario");

            DBConnection.GlobalConnections.Add(new DBConnection("127.0.0.1", "inventarioventas", "root", "4533"));
            DBConnection.GlobalConnections[0].UseOfCALL = false;
        }

        private void ListBoxItem_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void ListBoxItem_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender != null)
                (sender as ListBoxItem).IsSelected = true;
        }
    }
}
