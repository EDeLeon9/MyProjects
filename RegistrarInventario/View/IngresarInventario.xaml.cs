using System;
using System.Text;
using System.Collections.Generic;
using System.Windows.Controls;
using ToastMessageNotification;

namespace RegistrarInventario
{
    public partial class IngresarInventario : Page, IUniqueTabPage
    {
        public string TabTitle { get; set; } = "Ingresar Inventario";
        public int InitialIdItem { get; set; }
        public Action<int> UpdateForeignItem { get; set; }

        public IngresarInventario()
        {
        }

        public void InitializePage() 
        {
            InitializeComponent();
            this.Title = TabTitle;
            dtcVM_IngresarInventario.ScrollToInventario = SCut.GetScrollGridToObjectDelegate(grdInventario);
        }

        public void ReselectItem(int idItem)
        {
        }
    }
}
