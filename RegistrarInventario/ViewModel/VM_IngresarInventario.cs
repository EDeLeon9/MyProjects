using System;
using System.Data;
using System.Windows;
using System.Collections.ObjectModel;
using System.Linq;
using ToastMessageNotification;

namespace RegistrarInventario
{
    public class VM_IngresarInventario : NotifyPropertyChangedBase
    {
        #region MEMBERS
        public Action<object> ScrollToInventario;

        private ObservableCollection<Producto> _colBuscarProducto;
        private ObservableCollection<CategoriaProducto> _colCategoriasProducto;
        private ObservableCollection<Producto> _colProductos;
        private ObservableCollection<Variacion> _colVariaciones;
        private ObservableCollection<string> _colColores;
        private ObservableCollection<Variacion> _colVariacionesPorColor;
        private ObservableCollection<Inventario> _colInventario;
        private string _idOrden;
        private string _gastosOrden;
        private string _descripcionOrden;
        private string _idPaquete;
        private string _gastosPaquete;
        private string _descripcionPaquete;
        private string _buscarProducto;
        private Producto _buscarProductoItem;
        private bool _verMasBuscarProductoIsChecked;
        private string _idCategoriaProducto;
        private Producto _productoItem;
        private string _precio;
        private string _comentariosProducto;
        private string _color;
        private Variacion _variacionItem;
        private string _notasVariacion;
        private string _costoProducto;
        private string _shippingProducto;
        private string _descuentoProducto;
        private string _reembolsoProducto;
        private string _comentariosInventario;
        private bool _noLimpiar;
        private Inventario _selectedInventario;
        private Orden _ordenSelect;
        private Paquete _paqueteSelect;
        private Visibility _columnCantidadVisibility;

        private Orden _lastSearchedOrden;
        private Paquete _lastSearchedPaquete;
        #endregion

        #region PROPERTIES
        public RelayCommand CommandAgregar { get; set; }
        public RelayCommand CommandEliminarProducto { get; set; }
        public RelayCommand CommandAgruparInventario { get; set; }
        public RelayCommand CommandAbrirOrdenes { get; set; }
        public RelayCommand CommandAbrirPaquetes { get; set; }
        public RelayCommand CommandAbrirGastosOrden { get; set; }
        public RelayCommand CommandAbrirGastosPaquete { get; set; }

        public ObservableCollection<Producto> ColBuscarProducto { get => _colBuscarProducto; set { _colBuscarProducto = value; NotifyPropertyChanged(); } }
        public ObservableCollection<CategoriaProducto> ColCategoriasProducto { get => _colCategoriasProducto; set { _colCategoriasProducto = value; NotifyPropertyChanged(); } }
        public ObservableCollection<Producto> ColProductos { get => _colProductos; set { _colProductos = value; NotifyPropertyChanged(); } }
        public ObservableCollection<Variacion> ColVariaciones { get => _colVariaciones; set { _colVariaciones = value; NotifyPropertyChanged(); } }
        public ObservableCollection<string> ColColores { get => _colColores; set { _colColores = value; NotifyPropertyChanged(); } }
        public ObservableCollection<Variacion> ColVariacionesPorColor { get => _colVariacionesPorColor; set { _colVariacionesPorColor = value; NotifyPropertyChanged(); } }
        public string GastosOrden { get => _gastosOrden; set { _gastosOrden = value; NotifyPropertyChanged(); } }
        public string DescripcionOrden { get => _descripcionOrden; set { _descripcionOrden = value; NotifyPropertyChanged(); } }
        public string GastosPaquete { get => _gastosPaquete; set { _gastosPaquete = value; NotifyPropertyChanged(); } }
        public string DescripcionPaquete { get => _descripcionPaquete; set { _descripcionPaquete = value; NotifyPropertyChanged(); } }
        public string Precio { get => _precio; set { _precio = value; NotifyPropertyChanged(); } }
        public string ComentariosProducto { get => _comentariosProducto; set { _comentariosProducto = value; NotifyPropertyChanged(); } }
        public string NotasVariacion { get => _notasVariacion; set { _notasVariacion = value; NotifyPropertyChanged(); } }
        public string CostoProducto { get => _costoProducto; set { _costoProducto = value; NotifyPropertyChanged(); } }
        public string ShippingProducto { get => _shippingProducto; set { _shippingProducto = value; NotifyPropertyChanged(); } }
        public string DescuentoProducto { get => _descuentoProducto; set { _descuentoProducto = value; NotifyPropertyChanged(); } }
        public string ReembolsoProducto { get => _reembolsoProducto; set { _reembolsoProducto = value; NotifyPropertyChanged(); } }
        public string ComentariosInventario { get => _comentariosInventario; set { _comentariosInventario = value; NotifyPropertyChanged(); } }
        public bool NoLimpiar { get => _noLimpiar; set { _noLimpiar = value; NotifyPropertyChanged(); } }
        public Orden OrdenSelect { get => _ordenSelect; set { _ordenSelect = value; NotifyPropertyChanged(); } }
        public Paquete PaqueteSelect { get => _paqueteSelect; set { _paqueteSelect = value; NotifyPropertyChanged(); } }
        public Visibility ColumnCantidadVisibility { get => _columnCantidadVisibility; set { _columnCantidadVisibility = value; NotifyPropertyChanged(); } }

        public string IdOrden 
        { 
            get => _idOrden; 
            set 
            { 
                _idOrden = value; 
                NotifyPropertyChanged();

                ColInventario = DataLayer.GetInventario(_idOrden, "IdOrden", ColumnCantidadVisibility == Visibility.Visible);

                _lastSearchedOrden = DataLayer.GetOrden(_idOrden);

                GastosOrden = _lastSearchedOrden.IdOrden > 0 ? _lastSearchedOrden.GastosOrden.ToString() : "";  //Si tiene lastSearchedOrden.IdOrden quiere decir que lo encontró

                _descripcionOrden = null;
                if (!string.IsNullOrWhiteSpace(_lastSearchedOrden.DescripcionOrden))
                    _descripcionOrden = _lastSearchedOrden.DescripcionOrden;
                if (!string.IsNullOrWhiteSpace(_lastSearchedOrden.ComentariosOrden))
                    _descripcionOrden = _descripcionOrden + " (" + _lastSearchedOrden.ComentariosOrden + ")";  //Los comentarios irán entre paréntesis siempre
                DescripcionOrden = _descripcionOrden?.Trim();  //Si es null no hace nada

                if (SelectedInventario == null)
                    OrdenSelect = _lastSearchedOrden.CloneIfValid();
            } 
        }

        public string IdPaquete 
        { 
            get => _idPaquete; 
            set 
            {
                _idPaquete = value; 
                NotifyPropertyChanged();

                if (!SCut.IsValidId(IdOrden))
                    ColInventario = DataLayer.GetInventario(_idPaquete, "IdPaquete", ColumnCantidadVisibility == Visibility.Visible);

                _lastSearchedPaquete = DataLayer.GetPaquete(_idPaquete);

                GastosPaquete = _lastSearchedPaquete.IdPaquete > 0 ? _lastSearchedPaquete.GastosPaquete.ToString() : "";  //Si tiene lastSearchedPaquete.IdPaquete quiere decir que lo encontró

                _descripcionPaquete = null;
                if (!string.IsNullOrWhiteSpace(_lastSearchedPaquete.DescripcionPaquete))
                    _descripcionPaquete = _lastSearchedPaquete.DescripcionPaquete;
                if (!string.IsNullOrWhiteSpace(_lastSearchedPaquete.ComentariosPaquete))
                    _descripcionPaquete = _descripcionPaquete + " (" + _lastSearchedPaquete.ComentariosPaquete + ")";  //Los comentarios irán entre paréntesis siempre
                DescripcionPaquete = _descripcionPaquete?.Trim();  //Si es null no hace nada

                if (SelectedInventario == null)
                    PaqueteSelect = _lastSearchedPaquete.CloneIfValid();
            } 
        }

        public string BuscarProducto
        {
            get => _buscarProducto;
            set
            {
                _buscarProducto = value;
                NotifyPropertyChanged();

                ColBuscarProducto = DataLayer.GetBuscarProducto(_buscarProducto);
                if (ColBuscarProducto.Count > 0)
                    BuscarProductoItem = ColBuscarProducto.Where(x => x.IdProducto == ColBuscarProducto[0].IdProducto).FirstOrDefault();
            }
        }

        public Producto BuscarProductoItem
        {
            get => _buscarProductoItem;
            set
            {
                _buscarProductoItem = value;
                NotifyPropertyChanged();

                VerMasBuscarProductoIsChecked = false;

                if (_buscarProductoItem != null)
                {
                    IdCategoriaProducto = _buscarProductoItem.IdCategoriaProducto.ToString();
                    ProductoItem = ColProductos.Where(x => x.IdProducto == _buscarProductoItem.IdProducto).FirstOrDefault();
                }
            }
        }

        public bool VerMasBuscarProductoIsChecked 
        { 
            get => _verMasBuscarProductoIsChecked; 
            set 
            {
                //Evita que se coloque IsChecked a True y por ende abra el popup
                if ((ColBuscarProducto?.Count ?? 0) > 0)
                    _verMasBuscarProductoIsChecked = value;
                else
                    _verMasBuscarProductoIsChecked = false;
                NotifyPropertyChanged(); 
            } 
        }

        public string IdCategoriaProducto
        { 
            get => _idCategoriaProducto; 
            set
            {
                _idCategoriaProducto = value;
                NotifyPropertyChanged();

                ColProductos = DataLayer.GetProductos(_idCategoriaProducto);
                ProductoItem = ColProductos.Count == 1 ? ColProductos[0] : null;
            }
        }

        public Producto ProductoItem
        {
            get => _productoItem;
            set
            {
                _productoItem = value;
                NotifyPropertyChanged();

                if (_productoItem != null)
                {
                    ColVariaciones = DataLayer.GetVariaciones(_productoItem.IdProducto.ToString());
                    ColColores = new ObservableCollection<string>(ColVariaciones.Select(registro => registro.Color).Distinct());
                    Color = ColColores.Count == 1 ? ColColores[0] : null;
                    Precio = _productoItem.Precio.ToString();
                    ComentariosProducto = _productoItem.Comentarios;
                }
                else
                {
                    ColVariaciones = null;
                    ColColores = null;
                    Color = null;
                    Precio = null;
                    ComentariosProducto = null;
                }
            }
        }

        public string Color
        {
            get => _color;
            set
            {
                _color = value;
                NotifyPropertyChanged();

                if (_color != null)
                {
                    ColVariacionesPorColor = new ObservableCollection<Variacion>(from x in ColVariaciones.AsEnumerable() where x.Color == _color select x);
                    VariacionItem = ColVariacionesPorColor.Count == 1 ? ColVariacionesPorColor[0] : null;
                }
                else
                {
                    ColVariacionesPorColor = null;
                    VariacionItem = null;
                }
            }
        }

        public Variacion VariacionItem
        {
            get => _variacionItem;
            set
            {
                _variacionItem = value;
                NotifyPropertyChanged();

                NotasVariacion = null;
                if (_variacionItem != null)
                {
                    if (!string.IsNullOrWhiteSpace(_variacionItem.NotaTalla))
                        NotasVariacion = _variacionItem.NotaTalla;

                    if (!string.IsNullOrWhiteSpace(_variacionItem.Comentarios))
                        NotasVariacion = NotasVariacion+" (" + _variacionItem.Comentarios + ")";  //Los comentarios irán entre paréntesis siempre

                    NotasVariacion = NotasVariacion?.Trim();  //Si es null no hace nada
                }
            }
        }

        public ObservableCollection<Inventario> ColInventario 
        { 
            get => _colInventario;
            set
            {
                _colInventario = value; 
                NotifyPropertyChanged();
            }
        }

        public Inventario SelectedInventario 
        { 
            get => _selectedInventario; 
            set 
            { 
                _selectedInventario = value; 
                NotifyPropertyChanged();

                if (_selectedInventario != null)
                {
                    if (_selectedInventario.IdOrden != _lastSearchedOrden?.IdOrden)
                        _lastSearchedOrden = DataLayer.GetOrden(_selectedInventario.IdOrden.ToString());

                    if (_selectedInventario.IdPaquete != _lastSearchedPaquete?.IdPaquete)
                        _lastSearchedPaquete = DataLayer.GetPaquete(_selectedInventario.IdPaquete.ToString());
                }
                else
                {
                    if (SCut.ToInt(IdOrden) != _lastSearchedOrden?.IdOrden)
                        _lastSearchedOrden = DataLayer.GetOrden(IdOrden);

                    if (SCut.ToInt(IdPaquete) != _lastSearchedPaquete?.IdPaquete)
                        _lastSearchedPaquete = DataLayer.GetPaquete(IdPaquete);
                }

                OrdenSelect = _lastSearchedOrden.CloneIfValid();
                PaqueteSelect = _lastSearchedPaquete.CloneIfValid();
            } 
        }
        #endregion


        public VM_IngresarInventario()
        {
            ColumnCantidadVisibility = Visibility.Collapsed;
            ColBuscarProducto = new ObservableCollection<Producto>();  //Se instancia porque su propiedad Count se usa en un Binding con Converter
            CommandAgregar = new RelayCommand(Agregar);
            CommandEliminarProducto = new RelayCommand(EliminarProducto);
            CommandAgruparInventario = new RelayCommand(AgruparInventario);
            CommandAbrirOrdenes = new RelayCommand(AbrirOrdenes);
            CommandAbrirPaquetes = new RelayCommand(AbrirPaquetes);
            CommandAbrirGastosOrden = new RelayCommand(AbrirGastosOrden);
            CommandAbrirGastosPaquete = new RelayCommand(AbrirGastosPaquete);

            SCut.DispatcherAux(() => ColCategoriasProducto = DataLayer.GetCategoriasProducto());
            DependencyObject auxObject = new DependencyObject();
        }

        #region METHODS
        private void Agregar(object parameter)
        {
            //Recordar que se usan strings para que se muestre correctamente los datos en la ventana (los textos de los controles son strings)
            int insertedIdInventario = DataLayer.InsertInventario(IdOrden, IdPaquete, VariacionItem != null ? VariacionItem.IdVariacion.ToString() : "", CostoProducto, ShippingProducto, DescuentoProducto, ReembolsoProducto, ComentariosInventario);
            if (insertedIdInventario > 0)
            {
                if (!NoLimpiar)
                {
                    CostoProducto = "";
                    ShippingProducto = "";
                    DescuentoProducto = "";
                    ReembolsoProducto = "";
                    ComentariosInventario = "";
                }

                //Se limpian para que se busquen de nuevo y se actualicen los datos al asignar SelectedInventario
                _lastSearchedOrden = null;
                _lastSearchedPaquete = null;

                ColInventario = DataLayer.GetInventario(IdOrden, "IdOrden", ColumnCantidadVisibility == Visibility.Visible);
                SelectedInventario = (ColumnCantidadVisibility == Visibility.Collapsed) ? ColInventario.Where(x => x.IdInventario == insertedIdInventario).FirstOrDefault() : null;
                ScrollToInventario.Invoke(SelectedInventario);
            }
        }

        private void EliminarProducto(object parameter)
        {
            if (MessageBox.Show("¿Desea eliminar el producto?", App.APP_TITLE, MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
            {
                if (DataLayer.DeleteInventario(SelectedInventario?.IdInventario.ToString()))
                {
                    ToastMessage.Show("Producto eliminado del inventario");
                    ColInventario = DataLayer.GetInventario(SCut.IsValidId(IdOrden) ? IdOrden : IdPaquete, SCut.IsValidId(IdOrden) ? "IdOrden" : "IdPaquete", ColumnCantidadVisibility == Visibility.Visible);
                    SelectedInventario = null;
                }
            }
        }

        private void AgruparInventario(object parameter)
        {
            ColumnCantidadVisibility = ColumnCantidadVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            ColInventario = DataLayer.GetInventario(SCut.IsValidId(IdOrden) ? IdOrden : IdPaquete, SCut.IsValidId(IdOrden) ? "IdOrden" : "IdPaquete", ColumnCantidadVisibility == Visibility.Visible);
        }

        private void AbrirOrdenes(object parameter)
        {
            TabManager.ShowTab(new VerOrdenes(SCut.ToInt(IdOrden), (idOrdenSeleccionada) => 
            {
                IdOrden = idOrdenSeleccionada.ToString();
                TabManager.ShowTab(new IngresarInventario());  //Regresa al tab de IngresarInventario (que es el de este View Model)
            }
            ));
        }

        private void AbrirPaquetes(object parameter)
        {
            TabManager.ShowTab(new VerPaquetes(SCut.ToInt(IdPaquete), (idPaqueteSeleccionado) =>
            {
                IdPaquete = idPaqueteSeleccionado.ToString();
                TabManager.ShowTab(new IngresarInventario());  //Regresa al tab de IngresarInventario (que es el de este View Model)
            }
            ));
        }

        private void AbrirGastosOrden(object parameter)
        {
            if (OrdenSelect != null)
                TabManager.ShowTab(new VerGastos(OrdenSelect.IdOrden, "Orden"));
        }

        private void AbrirGastosPaquete(object parameter)
        {
            if (PaqueteSelect != null)
                TabManager.ShowTab(new VerGastos(PaqueteSelect.IdPaquete, "Paquete"));
        }
        #endregion
    }
}
