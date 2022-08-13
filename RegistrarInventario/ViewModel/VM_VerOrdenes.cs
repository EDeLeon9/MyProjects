using System;
using System.Windows;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using ToastMessageNotification;

namespace RegistrarInventario
{
    public class VM_VerOrdenes : NotifyPropertyChangedBase
    {
        #region MEMBERS
        public Action<object> ScrollToOrden;
        public Action<int> ReturnIdOrden;
        public Action UnselectAllCells;

        private ObservableCollection<Orden> _colOrdenes;
        private ObservableCollection<Gasto> _colGastos;
        private ObservableCollection<Proveedor> _colProveedores;
        private ObservableCollection<Tienda> _colTiendas;
        private string _descripcionOrden;
        private string _fechaOrden;
        private string _idTienda;
        private string _idProveedor;
        private string _comentariosOrden;
        private bool _terminado;
        private Orden _selectedOrden;
        private bool _agregandoNuevaOrden;
        #endregion

        #region PROPERTIES
        public RelayCommand CommandNuevaOrden { get; set; }
        public RelayCommand CommandGuardarCambios { get; set; }
        public RelayCommand CommandElegirOrdenSeleccionada { get; set; }
        public RelayCommand CommandVerDetalleGastos { get; set; }
        public RelayCommand CommandEliminarOrden { get; set; }
        public RelayCommand CommandRegresar { get; set; }
        
        public int IdOrdenInicial { get; set; }
        public ObservableCollection<Orden> ColOrdenes { get => _colOrdenes; set { _colOrdenes = value; NotifyPropertyChanged(); } }
        public ObservableCollection<Gasto> ColGastos { get => _colGastos; set { _colGastos = value; NotifyPropertyChanged(); } }
        public ObservableCollection<Proveedor> ColProveedores { get => _colProveedores; set { _colProveedores = value; NotifyPropertyChanged(); } }
        public ObservableCollection<Tienda> ColTiendas { get => _colTiendas; set { _colTiendas = value; NotifyPropertyChanged(); } }
        public string DescripcionOrden { get => _descripcionOrden; set { _descripcionOrden = value; NotifyPropertyChanged(); } }
        public string FechaOrden { get => _fechaOrden; set { _fechaOrden = value; NotifyPropertyChanged(); } }
        public string IdProveedor { get => _idProveedor; set { _idProveedor = value; NotifyPropertyChanged(); } }
        public string ComentariosOrden { get => _comentariosOrden; set { _comentariosOrden = value; NotifyPropertyChanged(); } }
        public bool Terminado { get => _terminado; set { _terminado = value; NotifyPropertyChanged(); } }
        public bool AgregandoNuevaOrden { get => _agregandoNuevaOrden; set { _agregandoNuevaOrden = value; NotifyPropertyChanged(); } }

        public string IdTienda 
        { 
            get => _idTienda; 
            set 
            { 
                _idTienda = value; 
                NotifyPropertyChanged();
                ColProveedores = DataLayer.GetProveedores(_idTienda);
                IdProveedor = ColProveedores.Count == 1 ? ColProveedores[0].IdProveedor.ToString() : null;
            } 
        }

        public Orden SelectedOrden 
        { 
            get => _selectedOrden; 
            set
            {
                if (AgregandoNuevaOrden && value != null)
                {
                    if (MessageBox.Show("Actualmente está agregando una nueva orden ¿Desea descartar los datos ingresados?", App.APP_TITLE, MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                    {
                        _selectedOrden = null;
                        //NotifyPropertyChanged();
                        //Se usa DataGrid.UnselectAllCells() en lugar de NotifyPropertyChanged() para que la propiedad no afecte el proceso de selección que se está llevando a cabo y así 
                        //evitar una excepción en tiempo de ejecución, y también que NotifyPropertyChanged no quita visualmente la selección, y DataGrid.UnselectAllCells() si lo hace.
                        UnselectAllCells.Invoke();
                        return;
                    }
                    else AgregandoNuevaOrden = false;
                }

                _selectedOrden = value;
                NotifyPropertyChanged();

                if (_selectedOrden != null)
                {
                    DescripcionOrden = _selectedOrden.DescripcionOrden;
                    FechaOrden = (new DateTimeConverter().Convert(_selectedOrden.FechaOrden, null, null, null) as string);
                    IdTienda = _selectedOrden.IdTienda.ToString();
                    IdProveedor = _selectedOrden.IdProveedor.ToString();
                    ComentariosOrden = _selectedOrden.ComentariosOrden;
                    Terminado = _selectedOrden.Terminado;
                    ColGastos = DataLayer.GetGastos(_selectedOrden.IdOrden.ToString(), "Orden");
                }
                else
                {
                    DescripcionOrden = null;
                    FechaOrden = null;
                    IdTienda = null;
                    IdProveedor = null;
                    ComentariosOrden = null;
                    Terminado = false;
                    ColGastos = null;
                }

                NotifyPropertyChanged("SumMontoGasto");
            } 
        }

        public decimal SumMontoGasto
        {
            get => ColGastos?.Sum(x => x.MontoGasto) ?? 0;
        }
        #endregion

        public VM_VerOrdenes()
        {
            CommandNuevaOrden = new RelayCommand(NuevaOrden);
            CommandGuardarCambios = new RelayCommand(GuardarCambios);
            CommandElegirOrdenSeleccionada = new RelayCommand(ElegirOrdenSeleccionada);
            CommandVerDetalleGastos = new RelayCommand(VerDetalleGastos);
            CommandEliminarOrden = new RelayCommand(EliminarOrden);
            CommandRegresar = new RelayCommand(Regresar);
            SCut.DispatcherAux(InitGuiElements);
        }

        #region METHODS
        private void InitGuiElements()
        {
            ColTiendas = DataLayer.GetTiendas();
            ColOrdenes = DataLayer.GetOrdenes();

            if (IdOrdenInicial > 0)
                SetIdOrdenSeleccionada(IdOrdenInicial);
        }

        public void SetIdOrdenSeleccionada(int value)
        {
            SelectedOrden = ColOrdenes.Where(x => x.IdOrden == value).FirstOrDefault();
            ScrollToOrden.Invoke(SelectedOrden);
        }

        private void NuevaOrden(object parameter)
        {
            SelectedOrden = null;
        }

        private void GuardarCambios(object parameter)
        {
            int idOrdenSeleccionar = 0;

            if (AgregandoNuevaOrden)
            {
                idOrdenSeleccionar = DataLayer.InsertOrden(DescripcionOrden, FechaOrden, IdProveedor, ComentariosOrden, Terminado);
                if (idOrdenSeleccionar > 0)
                    //MessageBox.Show("Nueva orden creada.", App.APP_TITLE, MessageBoxButton.OK, MessageBoxImage.Information);
                    ToastMessage.Show("Nueva orden creada");
            }
            else
            {
                if (DataLayer.UpdateOrden(DescripcionOrden, FechaOrden, IdProveedor, ComentariosOrden, Terminado, SelectedOrden?.IdOrden.ToString()))
                {
                    idOrdenSeleccionar = SelectedOrden.IdOrden;
                    //MessageBox.Show("Orden actualizada.", App.APP_TITLE, MessageBoxButton.OK, MessageBoxImage.Information);
                    ToastMessage.Show("Orden actualizada");
                }
            }

            if (idOrdenSeleccionar > 0)
            {
                AgregandoNuevaOrden = false;
                ColOrdenes = DataLayer.GetOrdenes();
                SetIdOrdenSeleccionada(idOrdenSeleccionar);
            }
        }

        private void ElegirOrdenSeleccionada(object parameter)
        {
            if (SelectedOrden != null)
                ReturnIdOrden?.Invoke(SelectedOrden.IdOrden);
            else 
                MessageBox.Show("Por favor seleccione una orden.", App.APP_TITLE, MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        private void VerDetalleGastos(object parameter)
        {
            if (SelectedOrden != null)
                TabManager.ShowTab(new VerGastos(SelectedOrden.IdOrden, "Orden"));
        }

        private void EliminarOrden(object parameter)
        {
            if (MessageBox.Show("¿Desea eliminar esta orden sin productos? Si tiene gastos asociados también se eliminarán.", App.APP_TITLE, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if (DataLayer.DeleteOrdenYGastos(SelectedOrden?.IdOrden.ToString()))
                {
                    //MessageBox.Show("Orden eliminada.", App.APP_TITLE, MessageBoxButton.OK, MessageBoxImage.Information);
                    ToastMessage.Show("Orden eliminada");
                    ColOrdenes = DataLayer.GetOrdenes();
                    SelectedOrden = null;
                }
            }
        }

        private void Regresar(object parameter)
        {
            TabManager.CloseTab();
        }
        #endregion
    }
}
