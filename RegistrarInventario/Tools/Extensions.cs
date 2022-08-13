using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace RegistrarInventario
{
    public static class WindowExtension
    {
        public static Window WindowsOwner = null;

        public static void DimAndShow(this Window window)
        {
            window.Owner = WindowsOwner;
            window.Owner.Opacity = 0.4;
            //DefaultOwner.Effect = new BlurEffect() { Radius = 3 };
            window.ShowDialog();
            window.Owner.Effect = null;
            window.Owner.Opacity = 1;
        }
    }
}
