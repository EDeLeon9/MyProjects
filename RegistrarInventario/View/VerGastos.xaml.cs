using System;
using System.Text;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows;

namespace RegistrarInventario
{
    public partial class VerGastos : Page, IUniqueTabPage
    {
        public string TabTitle { get; set; } = "Ver Gastos de {0}";
        public int InitialIdItem { get; set; }
        public Action<int> UpdateForeignItem { get; set; }
        public string TablaAsociada { get; set; }

        public VerGastos(int idTablaAsociada, string tablaAsociada)
        {
            this.TabTitle = string.Format(this.TabTitle, tablaAsociada);
            this.InitialIdItem = idTablaAsociada;
            this.TablaAsociada = tablaAsociada;
        }

        public void InitializePage()
        {
            InitializeComponent();
            this.Title = this.TabTitle;
            dtcVM_VerGastos.TablaAsociada = this.TablaAsociada;
            dtcVM_VerGastos.IdTablaAsociada = this.InitialIdItem.ToString();
            dtcVM_VerGastos.ScrollToGasto = SCut.GetScrollGridToObjectDelegate(grdGastos);
            dtcVM_VerGastos.ScrollToOrdenOPaquete = SCut.GetScrollGridToObjectDelegate(grdOrdenesOPaquetes);
            dtcVM_VerGastos.UnselectAllCells = () => grdGastos.UnselectAllCells();  //Requerido para que se quite el efecto de selección del registro
        }

        public void ReselectItem(int idItem)
        {
            dtcVM_VerGastos.SetIdOrdenOPaqueteSeleccionado(idItem);
        }
    }
}
