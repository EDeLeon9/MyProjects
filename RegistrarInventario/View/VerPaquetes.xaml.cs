using System;
using System.Text;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows;

namespace RegistrarInventario
{
    public partial class VerPaquetes : Page, IUniqueTabPage
    {
        public string TabTitle { get; set; } = "Ver Paquetes";
        public int InitialIdItem { get; set; }
        public Action<int> UpdateForeignItem { get; set; }

        public VerPaquetes(int idPaqueteSeleccionado, Action<int> setReturnedIdPaquete)
        {
            this.InitialIdItem = idPaqueteSeleccionado;
            this.UpdateForeignItem = setReturnedIdPaquete;
        }

        public void InitializePage()
        {
            InitializeComponent();
            this.Title = TabTitle;
            dtcVM_VerPaquetes.IdPaqueteInicial = this.InitialIdItem;
            dtcVM_VerPaquetes.ReturnIdPaquete = this.UpdateForeignItem;
            dtcVM_VerPaquetes.ScrollToPaquete = SCut.GetScrollGridToObjectDelegate(grdPaquetes);
            dtcVM_VerPaquetes.UnselectAllCells = () => grdPaquetes.UnselectAllCells();  //Requerido para que se quite el efecto de selección del registro
        }

        public void ReselectItem(int idItem)
        {
            dtcVM_VerPaquetes.SetIdPaqueteSeleccionado(idItem);
        }
    }
}
