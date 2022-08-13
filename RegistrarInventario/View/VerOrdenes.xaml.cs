using System;
using System.Text;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows;

namespace RegistrarInventario
{
    public partial class VerOrdenes : Page, IUniqueTabPage
    {
        public string TabTitle { get; set; } = "Ver Órdenes";
        public int InitialIdItem { get; set; }
        public Action<int> UpdateForeignItem { get; set; }

        public VerOrdenes(int idOrdenSeleccionada, Action<int> setReturnedIdOrden)
        {
            this.InitialIdItem = idOrdenSeleccionada;
            this.UpdateForeignItem = setReturnedIdOrden;
        }

        public void InitializePage()
        {
            InitializeComponent();
            this.Title = TabTitle;
            dtcVM_VerOrdenes.IdOrdenInicial = this.InitialIdItem;
            dtcVM_VerOrdenes.ReturnIdOrden = this.UpdateForeignItem;
            dtcVM_VerOrdenes.ScrollToOrden = SCut.GetScrollGridToObjectDelegate(grdOrdenes);
            dtcVM_VerOrdenes.UnselectAllCells = () => grdOrdenes.UnselectAllCells();  //Requerido para que se quite el efecto de selección del registro
        }

        public void ReselectItem(int idItem)
        {
            dtcVM_VerOrdenes.SetIdOrdenSeleccionada(idItem);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
