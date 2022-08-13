using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using ToastMessageNotification;

namespace RegistrarInventario
{
    public class VM_VerPaquetes : NotifyPropertyChangedBase
    {
        #region MEMBERS
        public Action<object> ScrollToPaquete;
        public Action<int> ReturnIdPaquete;
        public Action UnselectAllCells;

        private ObservableCollection<Paquete> _colPaquetes;
        private ObservableCollection<Gasto> _colGastos;
        private ObservableCollection<Courier> _colCouriers;
        private string _descripcionPaquete;
        private string _fechaConsolidacion;
        private string _fechaLlegadaPanama;
        private string _idCourier;
        private string _comentariosPaquete;
        private Paquete _selectedPaquete;
        private bool _terminado;
        private bool _agregandoNuevoPaquete;
        #endregion

        #region PROPERTIES
        public RelayCommand CommandNuevoPaquete { get; set; }
        public RelayCommand CommandGuardarCambios { get; set; }
        public RelayCommand CommandElegirPaqueteSeleccionado { get; set; }
        public RelayCommand CommandVerDetalleGastos { get; set; }
        public RelayCommand CommandEliminarPaquete { get; set; }
        public RelayCommand CommandRegresar { get; set; }

        public int IdPaqueteInicial { get; set; }
        public ObservableCollection<Paquete> ColPaquetes { get => _colPaquetes; set { _colPaquetes = value; NotifyPropertyChanged(); } }
        public ObservableCollection<Gasto> ColGastos { get => _colGastos; set { _colGastos = value; NotifyPropertyChanged(); } }
        public ObservableCollection<Courier> ColCouriers { get => _colCouriers; set { _colCouriers = value; NotifyPropertyChanged(); } }
        public string DescripcionPaquete { get => _descripcionPaquete; set { _descripcionPaquete = value; NotifyPropertyChanged(); } }
        public string FechaConsolidacion { get => _fechaConsolidacion; set { _fechaConsolidacion = value; NotifyPropertyChanged(); } }
        public string FechaLlegadaPanama { get => _fechaLlegadaPanama; set { _fechaLlegadaPanama = value; NotifyPropertyChanged(); } }
        public string IdCourier { get => _idCourier; set { _idCourier = value; NotifyPropertyChanged(); } }
        public string ComentariosPaquete { get => _comentariosPaquete; set { _comentariosPaquete = value; NotifyPropertyChanged(); } }
        public bool Terminado { get => _terminado; set { _terminado = value; NotifyPropertyChanged(); } }
        public bool AgregandoNuevoPaquete { get => _agregandoNuevoPaquete; set { _agregandoNuevoPaquete = value; NotifyPropertyChanged(); } }

        public Paquete SelectedPaquete
        {
            get => _selectedPaquete;
            set
            {
                if (AgregandoNuevoPaquete && value != null)
                {
                    if (MessageBox.Show("Actualmente está agregando un nuevo paquete ¿Desea descartar los datos ingresados?", App.APP_TITLE, MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                    {
                        _selectedPaquete = null;
                        //NotifyPropertyChanged();
                        //Se usa DataGrid.UnselectAllCells() en lugar de NotifyPropertyChanged() para que la propiedad no afecte el proceso de selección que se está llevando a cabo y así 
                        //evitar una excepción en tiempo de ejecución, y también que NotifyPropertyChanged no quita visualmente la selección, y DataGrid.UnselectAllCells() si lo hace.
                        UnselectAllCells.Invoke();
                        return;
                    }
                    else AgregandoNuevoPaquete = false;
                }

                _selectedPaquete = value;
                NotifyPropertyChanged();

                if (_selectedPaquete != null)
                {
                    DescripcionPaquete = _selectedPaquete.DescripcionPaquete;
                    FechaConsolidacion = (new DateTimeConverter().Convert(_selectedPaquete.FechaConsolidacion, null, null, null) as string);
                    FechaLlegadaPanama = (new DateTimeConverter().Convert(_selectedPaquete.FechaLlegadaPanama, null, null, null) as string);
                    IdCourier = _selectedPaquete.IdCourier.ToString();
                    ComentariosPaquete = _selectedPaquete.ComentariosPaquete;
                    Terminado = _selectedPaquete.Terminado;
                    ColGastos = DataLayer.GetGastos(_selectedPaquete.IdPaquete.ToString(), "Paquete");
                }
                else
                {
                    DescripcionPaquete = null;
                    FechaConsolidacion = null;
                    FechaLlegadaPanama = null;
                    IdCourier = null;
                    ComentariosPaquete = null;
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

        public VM_VerPaquetes()
        {
            CommandNuevoPaquete = new RelayCommand(NuevoPaquete);
            CommandGuardarCambios = new RelayCommand(GuardarCambios);
            CommandElegirPaqueteSeleccionado = new RelayCommand(ElegirPaqueteSeleccionado);
            CommandVerDetalleGastos = new RelayCommand(VerDetalleGastos);
            CommandEliminarPaquete = new RelayCommand(EliminarPaquete);
            CommandRegresar = new RelayCommand(Regresar);
            SCut.DispatcherAux(InitGuiElements);
        }

        #region METHODS
        private void InitGuiElements()
        {
            ColPaquetes = DataLayer.GetPaquetes();
            ColCouriers = DataLayer.GetCouriers();

            if (IdPaqueteInicial > 0)
                SetIdPaqueteSeleccionado(IdPaqueteInicial);
        }

        public void SetIdPaqueteSeleccionado(int value)
        {
            SelectedPaquete = ColPaquetes.Where(x => x.IdPaquete == value).FirstOrDefault();
            ScrollToPaquete.Invoke(SelectedPaquete);
        }

        private void NuevoPaquete(object parameter)
        {
            SelectedPaquete = null;
        }

        private void GuardarCambios(object parameter)
        {
            int idPaqueteSeleccionar = 0;

            if (AgregandoNuevoPaquete)
            {
                idPaqueteSeleccionar = DataLayer.InsertPaquete(DescripcionPaquete, FechaConsolidacion, FechaLlegadaPanama, IdCourier, ComentariosPaquete, Terminado);
                if (idPaqueteSeleccionar > 0)
                    //MessageBox.Show("Nuevo paquete creado.", App.APP_TITLE, MessageBoxButton.OK, MessageBoxImage.Information);
                    ToastMessage.Show("Nuevo paquete creado");
            }
            else
            {
                if (DataLayer.UpdatePaquete(DescripcionPaquete, FechaConsolidacion, FechaLlegadaPanama, IdCourier, ComentariosPaquete, Terminado, SelectedPaquete?.IdPaquete.ToString()))
                {
                    idPaqueteSeleccionar = SelectedPaquete.IdPaquete;
                    //MessageBox.Show("Paquete actualizado.", App.APP_TITLE, MessageBoxButton.OK, MessageBoxImage.Information);
                    ToastMessage.Show("Paquete actualizado");
                }
            }

            if (idPaqueteSeleccionar > 0)
            {
                AgregandoNuevoPaquete = false;
                ColPaquetes = DataLayer.GetPaquetes();
                SetIdPaqueteSeleccionado(idPaqueteSeleccionar);
            }
        }

        private void ElegirPaqueteSeleccionado(object parameter)
        {
            if (SelectedPaquete != null)
                ReturnIdPaquete?.Invoke(SelectedPaquete.IdPaquete);
            else 
                MessageBox.Show("Por favor seleccione un paquete.", App.APP_TITLE, MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        private void VerDetalleGastos(object parameter)
        {
            if (SelectedPaquete != null)
                TabManager.ShowTab(new VerGastos(SelectedPaquete.IdPaquete, "Paquete"));
        }

        private void EliminarPaquete(object parameter)
        {
            if (MessageBox.Show("¿Desea eliminar este paquete sin productos? Si tiene gastos asociados también se eliminarán.", App.APP_TITLE, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if (DataLayer.DeletePaqueteYGastos(SelectedPaquete?.IdPaquete.ToString()))
                {
                    ToastMessage.Show("Paquete eliminado");
                    ColPaquetes = DataLayer.GetPaquetes();
                    SelectedPaquete = null;
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
