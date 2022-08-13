using System;
using System.Windows;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Linq;
using ToastMessageNotification;

namespace RegistrarInventario
{
    public class VM_VerGastos : NotifyPropertyChangedBase
    {
        #region MEMBERS
        public Action<object> ScrollToGasto;
        public Action<object> ScrollToOrdenOPaquete;
        public Action UnselectAllCells;

        private string _idTablaAsociada;
        private string _tablaAsociada;
        private ObservableCollection<Gasto> _colGastos;
        private ObservableCollection<Orden> _colOrdenesOPaquetes;
        private Orden _selectedOrdenOPaquete;
        private string _fechaGasto;
        private string _montoGasto;
        private string _descripcionGasto;
        private string _comentariosGasto;
        private Gasto _selectedGasto;
        private bool _agregandoNuevoGasto;

        #endregion

        #region PROPERTIES
        public RelayCommand CommandNuevoGasto { get; set; }
        public RelayCommand CommandGuardarCambios { get; set; }
        public RelayCommand CommandEliminarGasto { get; set; }
        public RelayCommand CommandVerMasDatosOrdenesOPaquetes { get; set; }
        public RelayCommand CommandRegresar { get; set; }
        public string FechaGasto { get => _fechaGasto; set { _fechaGasto = value; NotifyPropertyChanged(); } }
        public string MontoGasto { get => _montoGasto; set { _montoGasto = value; NotifyPropertyChanged(); } }
        public string DescripcionGasto { get => _descripcionGasto; set { _descripcionGasto = value; NotifyPropertyChanged(); } }
        public string ComentariosGasto { get => _comentariosGasto; set { _comentariosGasto = value; NotifyPropertyChanged(); } }
        public bool AgregandoNuevoGasto { get => _agregandoNuevoGasto; set { _agregandoNuevoGasto = value; NotifyPropertyChanged(); } }

        public string TablaAsociada 
        { 
            get => _tablaAsociada; 
            set { _tablaAsociada = value; 
                NotifyPropertyChanged(); 
                NotifyPropertyChanged("TablaAsociadaPlural"); 
            } 
        }

        public string TablaAsociadaPlural
        {
            get => TablaAsociada == "Orden" ? "Órdenes" : "Paquetes";
        }

        public int IdGastoSeleccionado
        {
            get => SelectedGasto?.IdGasto ?? 0;
            set
            {
                SelectedGasto = ColGastos.Where(x => x.IdGasto == value).FirstOrDefault();
                ScrollToGasto.Invoke(SelectedGasto);
            }
        }

        public decimal SumMontoGasto
        {
            get => ColGastos?.Sum(x => x.MontoGasto) ?? 0;
        }

        public ObservableCollection<Gasto> ColGastos 
        { 
            get => _colGastos; 
            set 
            { 
                _colGastos = value; 
                NotifyPropertyChanged();
                NotifyPropertyChanged("SumMontoGasto");
            }
        }

        public ObservableCollection<Orden> ColOrdenesOPaquetes
        {
            get => _colOrdenesOPaquetes;
            set
            {
                _colOrdenesOPaquetes = value;
                NotifyPropertyChanged();
            }
        }

        public Orden SelectedOrdenOPaquete
        {
            get => _selectedOrdenOPaquete;
            set
            {
                _selectedOrdenOPaquete = value;
                NotifyPropertyChanged();

                IdTablaAsociada = _selectedOrdenOPaquete.IdOrden.ToString();
                ColGastos = DataLayer.GetGastos(IdTablaAsociada, TablaAsociada);
            }
        }

        public string IdTablaAsociada
        {
            get => _idTablaAsociada;
            set
            {
                _idTablaAsociada = value;
                NotifyPropertyChanged();
            }
        }

        public Gasto SelectedGasto
        {
            get => _selectedGasto;
            set
            {
                if (AgregandoNuevoGasto && value != null)
                {
                    if (MessageBox.Show("Actualmente está agregando un nuevo gasto ¿Desea descartar los datos ingresados?", App.APP_TITLE, MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                    {
                        _selectedGasto = null;
                        //NotifyPropertyChanged();
                        //Se usa DataGrid.UnselectAllCells() en lugar de NotifyPropertyChanged() para que la propiedad no afecte el proceso de selección que se está llevando a cabo y así 
                        //evitar una excepción en tiempo de ejecución, y también que NotifyPropertyChanged no quita visualmente la selección, y DataGrid.UnselectAllCells() si lo hace.
                        UnselectAllCells.Invoke();
                        return;
                    }
                    else AgregandoNuevoGasto = false;
                }

                _selectedGasto = value;
                NotifyPropertyChanged();

                if (_selectedGasto != null)
                {
                    FechaGasto = (new DateTimeConverter().Convert(_selectedGasto.FechaGasto, null, null, null) as string);
                    DescripcionGasto = _selectedGasto.DescripcionGasto;
                    MontoGasto = _selectedGasto.MontoGasto.ToString();
                    ComentariosGasto = _selectedGasto.ComentariosGasto;
                }
                else
                {
                    FechaGasto = null;
                    DescripcionGasto = null;
                    MontoGasto = null;
                    ComentariosGasto = null;
                }
            }
        }
        #endregion

        public VM_VerGastos()
        {
            CommandNuevoGasto = new RelayCommand(NuevoGasto);
            CommandGuardarCambios = new RelayCommand(GuardarCambios);
            CommandEliminarGasto = new RelayCommand(EliminarGasto);
            CommandVerMasDatosOrdenesOPaquetes = new RelayCommand(VerMasDatosOrdenesOPaquetes);
            CommandRegresar = new RelayCommand(Regresar);
            SCut.DispatcherAux(InitGuiElements);
        }

        #region METHODS

        private void InitGuiElements()
        {
            ColGastos = DataLayer.GetGastos(IdTablaAsociada, TablaAsociada);

            if (TablaAsociada == "Orden")
            {
                ColOrdenesOPaquetes = DataLayer.GetOrdenes();
            }
            else
            {
                ObservableCollection<Paquete> colPaquetesAux = DataLayer.GetPaquetes();
                ColOrdenesOPaquetes = new ObservableCollection<Orden>();
                foreach (Paquete paquete in colPaquetesAux)
                {
                    ColOrdenesOPaquetes.Add(new Orden()
                    {
                        IdOrden = paquete.IdPaquete,
                        CantProductosOrden = paquete.CantProductosPaquete,
                        TotalOrden = paquete.GastosPaquete,
                        FechaOrden = (paquete.FechaConsolidacion ?? paquete.FechaLlegadaPanama).GetValueOrDefault(),
                        DescripcionOrden = paquete.DescripcionPaquete
                    });
                }
            }

            if (SCut.ToInt(IdTablaAsociada) > 0)
                SetIdOrdenOPaqueteSeleccionado(SCut.ToInt(IdTablaAsociada));
        }

        public void SetIdOrdenOPaqueteSeleccionado(int value)
        {
            SelectedOrdenOPaquete = ColOrdenesOPaquetes.Where(x => x.IdOrden == value).FirstOrDefault();
            ScrollToOrdenOPaquete.Invoke(SelectedOrdenOPaquete);
            IdTablaAsociada = SelectedOrdenOPaquete?.IdOrden.ToString() ?? null;
        }

        private void NuevoGasto(object parameter)
        {
            SelectedGasto = null;
        }

        private void GuardarCambios(object parameter)
        {
            int idGastoSeleccionar = 0;

            if (AgregandoNuevoGasto)
            {
                idGastoSeleccionar = DataLayer.InsertGasto(IdTablaAsociada, TablaAsociada, FechaGasto, MontoGasto, DescripcionGasto, ComentariosGasto);
                if (idGastoSeleccionar > 0)
                    //MessageBox.Show("Nueva gasto creado.", App.APP_TITLE, MessageBoxButton.OK, MessageBoxImage.Information);
                    ToastMessage.Show("Nueva gasto creado");
            }
            else
            {
                if (DataLayer.UpdateGasto(IdTablaAsociada, TablaAsociada, FechaGasto, MontoGasto, DescripcionGasto, ComentariosGasto, SelectedGasto?.IdGasto.ToString()))
                {
                    idGastoSeleccionar = SelectedGasto.IdGasto;
                    //MessageBox.Show("Gasto actualizado.", App.APP_TITLE, MessageBoxButton.OK, MessageBoxImage.Information);
                    ToastMessage.Show("Gasto actualizado");
                }
            }

            if (idGastoSeleccionar > 0)
            {
                AgregandoNuevoGasto = false;
                ColGastos = DataLayer.GetGastos(IdTablaAsociada, TablaAsociada);
                IdGastoSeleccionado = idGastoSeleccionar;
            }
        }

        private void EliminarGasto(object parameter)
        {
            if (MessageBox.Show("¿Desea eliminar este gasto?", App.APP_TITLE, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if (DataLayer.DeleteGasto(SelectedGasto?.IdGasto.ToString()))
                {
                    //MessageBox.Show("Gasto eliminado.", App.APP_TITLE, MessageBoxButton.OK, MessageBoxImage.Information);
                    ToastMessage.Show("Gasto eliminado");
                    ColGastos = DataLayer.GetGastos(IdTablaAsociada, TablaAsociada);
                    SelectedGasto = null;
                }
            }
        }

        private void VerMasDatosOrdenesOPaquetes(object parameter)
        {
            Action<int> action = (idOrdenSeleccionada) =>
            {
                SetIdOrdenOPaqueteSeleccionado(idOrdenSeleccionada);
                TabManager.ShowTab(new VerGastos(idOrdenSeleccionada, TablaAsociada));  //Regresa al tab de VerGastos (que es el de este View Model)
            };

            if (TablaAsociada == "Orden")
                TabManager.ShowTab(new VerOrdenes(SelectedOrdenOPaquete?.IdOrden ?? 0, action));
            else
                TabManager.ShowTab(new VerPaquetes(SelectedOrdenOPaquete?.IdOrden ?? 0, action));
        }

        private void Regresar(object parameter)
        {
            TabManager.CloseTab();
        }
        #endregion
    }
}
