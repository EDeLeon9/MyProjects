using System;
using System.Windows;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Collections.ObjectModel;
using DBConnections;

namespace RegistrarInventario
{
    public static class DataLayer
    {
        #region SELECT
        public static ObservableCollection<Producto> GetBuscarProducto(string texto)
        {
            List<Producto> list = new List<Producto>();
            if (!string.IsNullOrWhiteSpace(texto))
                DBConnection.GlobalConnections[0].Execute<Producto>("SELECT producto.IdProducto, tipoproducto.IdCategoriaProducto, producto.NombreProducto, producto.Precio, producto.Comentarios" +
                    " FROM producto INNER JOIN tipoproducto ON producto.IdTipoProducto = tipoproducto.IdTipoProducto" +
                    $" WHERE producto.NombreProducto LIKE '%{texto.Replace(" ", "%")}%' ORDER BY producto.NombreProducto ASC", ref list);
            return new ObservableCollection<Producto>(list);
        }

        public static ObservableCollection<Orden> GetOrdenes()
        {
            List<Orden> list = new List<Orden>();
            DBConnection.GlobalConnections[0].Execute<Orden>("SELECT IdOrden, FechaOrden, TotalProdSinShippings, GastosOrden, DescripcionOrden, IdProveedor, NombreProveedor, IdTienda, NombreTienda, CantProductosOrden, TotalOrden, ComentariosOrden, Terminado" +
                " FROM vistaorden ORDER BY FechaOrden DESC, IdOrden DESC", ref list);
            return new ObservableCollection<Orden>(list);
        }

        public static ObservableCollection<Paquete> GetPaquetes()
        {
            List<Paquete> list = new List<Paquete>();
            DBConnection.GlobalConnections[0].Execute<Paquete>("SELECT IdPaquete, FechaConsolidacion, FechaLlegadaPanama, DescripcionPaquete, GastosPaquete, IdCourier, NombreCourier, CantProductosPaquete, ComentariosPaquete, Terminado" +
                " FROM vistapaquete ORDER BY FechaConsolidacionOLlegadaPanama DESC, IdPaquete DESC", ref list);
            return new ObservableCollection<Paquete>(list);
        }

        public static ObservableCollection<CategoriaProducto> GetCategoriasProducto()
        {
            List<CategoriaProducto> list = new List<CategoriaProducto>();
            DBConnection.GlobalConnections[0].Execute<CategoriaProducto>("SELECT IdCategoriaProducto, CategoriaProducto AS Categoria FROM categoriaproducto ORDER BY CategoriaProducto ASC", ref list);
            return new ObservableCollection<CategoriaProducto>(list);
        }

        public static ObservableCollection<Producto> GetProductos(string idCategoria)
        {
            List<Producto> list = new List<Producto>();
            if (SCut.IsValidId(idCategoria))
                DBConnection.GlobalConnections[0].Execute<Producto>("SELECT producto.IdProducto, tipoproducto.IdCategoriaProducto, producto.NombreProducto, producto.Precio, producto.Comentarios" +
                    " FROM producto INNER JOIN tipoproducto ON producto.IdTipoProducto = tipoproducto.IdTipoProducto" +
                    $" WHERE tipoproducto.IdCategoriaProducto = {idCategoria} ORDER BY producto.NombreProducto ASC", ref list);
            return new ObservableCollection<Producto>(list);
        }

        public static ObservableCollection<Variacion> GetVariaciones(string idProducto)
        {
            List<Variacion> list = new List<Variacion>();
            if (SCut.IsValidId(idProducto))
                DBConnection.GlobalConnections[0].Execute<Variacion>("SELECT IdVariacion, Color, IFNULL(Talla,'NULL') AS Talla, IFNULL(TallaReal,'NULL') AS TallaReal, IFNULL(NotaTalla,'') AS NotaTalla, Comentarios" +
                    $" FROM variacion WHERE IdProducto = {idProducto} ORDER BY Talla ASC, Color ASC, IdVariacion ASC", ref list);
            return new ObservableCollection<Variacion>(list);
        }

        public static ObservableCollection<Tienda> GetTiendas()
        {
            List<Tienda> list = new List<Tienda>();
            DBConnection.GlobalConnections[0].Execute<Tienda>("SELECT IdTienda, NombreTienda FROM tienda ORDER BY NombreTienda ASC", ref list);
            return new ObservableCollection<Tienda>(list);
        }

        public static ObservableCollection<Proveedor> GetProveedores(string idTienda)
        {
            List<Proveedor> list = new List<Proveedor>();
            if (SCut.IsValidId(idTienda))
                DBConnection.GlobalConnections[0].Execute<Proveedor>($"SELECT IdProveedor, NombreProveedor FROM proveedor WHERE IdTienda = {idTienda} ORDER BY NombreProveedor ASC", ref list);
            return new ObservableCollection<Proveedor>(list);
        }

        public static ObservableCollection<Courier> GetCouriers()
        {
            List<Courier> list = new List<Courier>();
            DBConnection.GlobalConnections[0].Execute<Courier>("SELECT IdCourier, NombreCourier FROM courier ORDER BY NombreCourier ASC", ref list);
            return new ObservableCollection<Courier>(list);
        }

        public static ObservableCollection<Gasto> GetGastos(string idTablaAsociada, string tablaAsociada)
        {
            List<Gasto> list = new List<Gasto>();
            DBConnection.GlobalConnections[0].Execute<Gasto>($"SELECT IdGasto, MontoGasto, FechaGasto, DescripcionGasto, Comentarios AS ComentariosGasto" +
                $" FROM gasto WHERE Id{tablaAsociada} = {idTablaAsociada} ORDER BY FechaGasto ASC, IdGasto ASC", ref list);
            return new ObservableCollection<Gasto>(list);
        }

        public static ObservableCollection<Inventario> GetInventario(string idOrdenOPaquete, string campoWhere, bool agrupar)
        {
            List<Inventario> list = new List<Inventario>();
            if (SCut.IsValidId(idOrdenOPaquete))
                DBConnection.GlobalConnections[0].Execute<Inventario>($"SELECT {(agrupar? "MIN(IdInventario)" : "IdInventario")} AS IdInventario, IdOrden, IdPaquete, COUNT(IdInventario) AS CantidadAgrupado," +
                    $" NombreProducto, Color, Talla, TallaReal, NotaTalla, CostoProducto, CostoFinalProducto, Precio, GananciaEstimada, ComentariosInventario FROM vistatodo WHERE {campoWhere} = {idOrdenOPaquete}" +
                    $" GROUP BY {(agrupar? "" : "IdInventario,")} IdOrden, IdPaquete, NombreProducto, Color, Talla, TallaReal, NotaTalla, CostoProducto, CostoFinalProducto, GananciaEstimada, ComentariosInventario" +
                    " ORDER BY IdOrden ASC, IdPaquete ASC, IdInventario ASC", ref list);
            return new ObservableCollection<Inventario>(list);
        }

        public static Orden GetOrden(string idOrden)
        {
            Orden orden = new Orden();
            if (SCut.IsValidId(idOrden))
                DBConnection.GlobalConnections[0].Execute<Orden>("SELECT IdOrden, FechaOrden, GastosOrden, DescripcionOrden, IdProveedor, NombreProveedor, IdTienda, NombreTienda, CantProductosOrden, TotalOrden, ComentariosOrden, Terminado" +
                    $" FROM vistaorden WHERE IdOrden = {idOrden}", ref orden);
            return orden;
        }

        public static Paquete GetPaquete(string idPaquete)
        {
            Paquete paquete = new Paquete();
            if (SCut.IsValidId(idPaquete))
                DBConnection.GlobalConnections[0].Execute<Paquete>("SELECT IdPaquete, DescripcionPaquete, GastosPaquete, CantProductosPaquete, ComentariosPaquete" +
                    $" FROM vistapaquete WHERE IdPaquete = {idPaquete}", ref paquete);
            return paquete;
        }
        #endregion


        #region INSERT
        public static int InsertInventario(string idOrden, string idPaquete, string idVariacion, string costo, string shippingProducto, string descuentoProducto, string reembolsoProducto, string comentarios)
        {
            if (!SCut.IsValidId(idOrden))
            {
                MessageBox.Show("El número de orden no es válido.", App.APP_TITLE, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return 0;
            }
            if (!SCut.IsValidId(idPaquete))
            {
                MessageBox.Show("El número de paquete no es válido.", App.APP_TITLE, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return 0;
            }
            if (!SCut.IsValidId(idVariacion))
            {
                MessageBox.Show("Falta seleccionar datos del producto, color y/o talla.", App.APP_TITLE, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return 0;
            }
            if (SCut.ToDecimal(costo) == 0)
            {
                if (MessageBox.Show("¿Agregar producto sin costo?", App.APP_TITLE, MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.No)
                    return 0;
            }

            int insertedIdInventario = 0;

            if (DBConnection.GlobalConnections[0].Execute("INSERT INTO inventario(IdOrden, IdPaquete, IdVariacion, Costo, ShippingProducto, DescuentoProducto, ReembolsoProducto, Comentarios)" +
                $" VALUES({idOrden}, {idPaquete}, {idVariacion}, {costo}, {SCut.NullIfUnassigned(shippingProducto, false)}, {SCut.NullIfUnassigned(descuentoProducto, false)}, " +
                $" {SCut.NullIfUnassigned(reembolsoProducto, false)}, {SCut.NullIfUnassigned(comentarios, true)})"))
            {
                DBConnection.GlobalConnections[0].Execute<int>("SELECT CAST(LAST_INSERT_ID() AS INT) AS IdInventario", ref insertedIdInventario);
            }
            
            return insertedIdInventario;
        }

        public static int InsertOrden(string descripcionOrden, string fechaOrden, string idProveedor, string comentariosOrden, bool terminado)
        {
            if (string.IsNullOrWhiteSpace(descripcionOrden))
            {
                MessageBox.Show("La descripción de la orden no puede estar vacía.", App.APP_TITLE, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return 0;
            }
            string fechaOrdenDB = SCut.DateTimeStringToDB(fechaOrden);
            if (string.IsNullOrWhiteSpace(fechaOrdenDB))
            {
                MessageBox.Show("La fecha de la orden no es válida.", App.APP_TITLE, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return 0;
            }
            if (!SCut.IsValidId(idProveedor))
            {
                MessageBox.Show("Elija un proveedor válido.", App.APP_TITLE, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return 0;
            }

            int insertedIdOrden = 0;

            if (DBConnection.GlobalConnections[0].Execute("INSERT INTO orden(DescripcionOrden, FechaOrden, IdProveedor, Comentarios, Terminado)" +
                $" VALUES('{descripcionOrden}', '{fechaOrdenDB}', {idProveedor}, {SCut.NullIfUnassigned(comentariosOrden, true)}, {terminado.ToString().ToUpper()})"))
            {
                DBConnection.GlobalConnections[0].Execute<int>("SELECT CAST(LAST_INSERT_ID() AS INT) AS IdOrden", ref insertedIdOrden);
            }
            
            return insertedIdOrden;
        }
        
        public static int InsertPaquete(string descripcionPaquete, string fechaConsolidacion, string fechaLlegadaPanama, string idCourier, string comentariosPaquete, bool terminado)
        {
            if (string.IsNullOrWhiteSpace(descripcionPaquete))
            {
                MessageBox.Show("La descripción del paquete no puede estar vacía.", App.APP_TITLE, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return 0;
            }
            string fechaConsolidacionDB = SCut.DateTimeStringToDB(fechaConsolidacion);
            if (!string.IsNullOrWhiteSpace(fechaConsolidacion) && string.IsNullOrWhiteSpace(fechaConsolidacionDB))
            {
                MessageBox.Show("La fecha de consolidación no es válida.", App.APP_TITLE, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return 0;
            }
            string fechaLlegadaPanamaDB = SCut.DateTimeStringToDB(fechaLlegadaPanama);
            if (!string.IsNullOrWhiteSpace(fechaLlegadaPanama) && string.IsNullOrWhiteSpace(fechaLlegadaPanamaDB))
            {
                MessageBox.Show("La fecha de llegada a Panamá no es válida.", App.APP_TITLE, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return 0;
            }
            if (!SCut.IsValidId(idCourier))
            {
                MessageBox.Show("Elija un courier válido.", App.APP_TITLE, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return 0;
            }

            int insertedIdPaquete = 0;

            if (DBConnection.GlobalConnections[0].Execute("INSERT INTO paquete(DescripcionPaquete, FechaConsolidacion, FechaLlegadaPanama, IdCourier, Comentarios, Terminado)" +
                $" VALUES('{descripcionPaquete}', {SCut.NullIfUnassigned(fechaConsolidacionDB, true)}, {SCut.NullIfUnassigned(fechaLlegadaPanamaDB, true)}, {idCourier}, {SCut.NullIfUnassigned(comentariosPaquete, true)}, {terminado.ToString().ToUpper()})"))
            {
                DBConnection.GlobalConnections[0].Execute<int>("SELECT CAST(LAST_INSERT_ID() AS INT) AS IdPaquete", ref insertedIdPaquete);
            }
            
            return insertedIdPaquete;
        }

        public static int InsertGasto(string idTablaAsociada, string tablaAsociada, string fechaGasto, string montoGasto, string descripcionGasto, string comentariosGasto)
        {
            if (!SCut.IsValidId(idTablaAsociada))
            {
                MessageBox.Show(tablaAsociada == "Orden" ? "La orden del gasto no es válida" : "El paquete del gasto no es válido", App.APP_TITLE, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return 0;
            }
            string fechaGastoDB = SCut.DateTimeStringToDB(fechaGasto);
            if (string.IsNullOrWhiteSpace(fechaGastoDB))
            {
                MessageBox.Show("La fecha del gasto no es válida.", App.APP_TITLE, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return 0;
            }
            if (SCut.ToDecimal(montoGasto) == 0)
            {
                MessageBox.Show("Ingrese un monto de gasto válido.", App.APP_TITLE, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return 0;
            }
            if (string.IsNullOrWhiteSpace(descripcionGasto))
            {
                MessageBox.Show("La descripción del gasto no es válida.", App.APP_TITLE, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return 0;
            }

            int insertedIdGasto = 0;

            if (DBConnection.GlobalConnections[0].Execute($"INSERT INTO gasto(Id{tablaAsociada}, FechaGasto, MontoGasto, DescripcionGasto, Comentarios)" +
                $" VALUES({idTablaAsociada}, '{fechaGastoDB}', {montoGasto}, '{descripcionGasto}', {SCut.NullIfUnassigned(comentariosGasto, true)})"))
            {
                DBConnection.GlobalConnections[0].Execute<int>("SELECT CAST(LAST_INSERT_ID() AS INT) AS IdGasto", ref insertedIdGasto);
            }

            return insertedIdGasto;
        }
        #endregion


        #region UPDATE
        public static bool UpdateOrden(string descripcionOrden, string fechaOrden, string idProveedor, string comentariosOrden, bool terminado, string idOrden)
        {
            if (!SCut.IsValidId(idOrden))
            {
                MessageBox.Show("Seleccione una orden válida para modificar.", App.APP_TITLE, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }
            if (string.IsNullOrWhiteSpace(descripcionOrden))
            {
                MessageBox.Show("La descripción de la orden no puede estar vacía.", App.APP_TITLE, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }
            string fechaOrdenDB = SCut.DateTimeStringToDB(fechaOrden);
            if (string.IsNullOrWhiteSpace(fechaOrdenDB))
            {
                MessageBox.Show("La fecha de la orden no es válida.", App.APP_TITLE, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }
            if (!SCut.IsValidId(idProveedor))
            {
                MessageBox.Show("Elija un proveedor válido.", App.APP_TITLE, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }
            return DBConnection.GlobalConnections[0].Execute($"UPDATE orden SET DescripcionOrden = '{descripcionOrden}', FechaOrden = '{fechaOrdenDB}', IdProveedor = {idProveedor}," +
                $" Comentarios = {SCut.NullIfUnassigned(comentariosOrden, true)}, Terminado = {terminado.ToString().ToUpper()} WHERE IdOrden = {idOrden}");
        }

        public static bool UpdatePaquete(string descripcionPaquete, string fechaConsolidacion, string fechaLlegadaPanama, string idCourier, string comentariosPaquete, bool terminado, string idPaquete)
        {
            if (!SCut.IsValidId(idPaquete))
            {
                MessageBox.Show("Seleccione un paquete válido para modificar.", App.APP_TITLE, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }
            if (string.IsNullOrWhiteSpace(descripcionPaquete))
            {
                MessageBox.Show("La descripción del paquete no puede estar vacía.", App.APP_TITLE, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }
            string fechaConsolidacionDB = SCut.DateTimeStringToDB(fechaConsolidacion);
            if (!string.IsNullOrWhiteSpace(fechaConsolidacion) && string.IsNullOrWhiteSpace(fechaConsolidacionDB))
            {
                MessageBox.Show("La fecha de consolidación no es válida.", App.APP_TITLE, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }
            string fechaLlegadaPanamaDB = SCut.DateTimeStringToDB(fechaLlegadaPanama);
            if (!string.IsNullOrWhiteSpace(fechaLlegadaPanama) && string.IsNullOrWhiteSpace(fechaLlegadaPanamaDB))
            {
                MessageBox.Show("La fecha de llegada a Panamá no es válida.", App.APP_TITLE, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }
            if (!SCut.IsValidId(idCourier))
            {
                MessageBox.Show("Elija un courier válido.", App.APP_TITLE, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }
            return DBConnection.GlobalConnections[0].Execute($"UPDATE paquete SET DescripcionPaquete = '{descripcionPaquete}', FechaConsolidacion = {SCut.NullIfUnassigned(fechaConsolidacionDB, true)}, FechaLlegadaPanama = {SCut.NullIfUnassigned(fechaLlegadaPanamaDB, true)}," +
                $" IdCourier = {idCourier}, Comentarios = {SCut.NullIfUnassigned(comentariosPaquete, true)}, Terminado = {terminado.ToString().ToUpper()} WHERE IdPaquete = {idPaquete}");
        }

        public static bool UpdateGasto(string idTablaAsociada, string tablaAsociada, string fechaGasto, string montoGasto, string descripcionGasto, string comentariosGasto, string idGasto)
        {
            if (!SCut.IsValidId(idGasto))
            {
                MessageBox.Show("Seleccione un gasto válido para modificar.", App.APP_TITLE, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }
            if (!SCut.IsValidId(idTablaAsociada))
            {
                MessageBox.Show(tablaAsociada == "Orden" ? "La orden del gasto no es válida" : "El paquete del gasto no es válido", App.APP_TITLE, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }
            string fechaGastoDB = SCut.DateTimeStringToDB(fechaGasto);
            if (string.IsNullOrWhiteSpace(fechaGastoDB))
            {
                MessageBox.Show("La fecha del gasto no es válida.", App.APP_TITLE, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }
            if (SCut.ToDecimal(montoGasto) == 0)
            {
                MessageBox.Show("Ingrese un monto de gasto válido.", App.APP_TITLE, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }
            if (string.IsNullOrWhiteSpace(descripcionGasto))
            {
                MessageBox.Show("La descripción del gasto no es válida.", App.APP_TITLE, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }

            return DBConnection.GlobalConnections[0].Execute($"UPDATE gasto SET Id{tablaAsociada} = {idTablaAsociada}, Id{(tablaAsociada == "Orden" ? "Paquete" : "Orden")} = NULL, FechaGasto = '{fechaGastoDB}', MontoGasto = {montoGasto}," +
                $" DescripcionGasto = '{descripcionGasto}', Comentarios = {SCut.NullIfUnassigned(comentariosGasto, true)} WHERE IdGasto = {idGasto}");
        }
        #endregion


        #region DELETE
        public static bool DeleteOrdenYGastos(string idOrden)
        {
            if (DBConnection.GlobalConnections[0].Execute($"DELETE FROM gasto WHERE IdOrden = {idOrden} AND IdPaquete IS NULL"))
                return DBConnection.GlobalConnections[0].Execute($"DELETE FROM orden WHERE IdOrden = {idOrden}");
            else
                return false;
        }

        public static bool DeletePaqueteYGastos(string idPaquete)
        {
            if (DBConnection.GlobalConnections[0].Execute($"DELETE FROM gasto WHERE IdPaquete = {idPaquete} AND IdOrden IS NULL"))
                return DBConnection.GlobalConnections[0].Execute($"DELETE FROM paquete WHERE IdPaquete = {idPaquete}");
            else
                return false;
        }

        public static bool DeleteInventario(string idInventario)
        {
            return DBConnection.GlobalConnections[0].Execute($"DELETE FROM inventario WHERE IdInventario = {idInventario}");
        }

        public static bool DeleteGasto(string idGasto)
        {
            return DBConnection.GlobalConnections[0].Execute($"DELETE FROM gasto WHERE IdGasto = {idGasto}");
        }
        #endregion
    }
}
