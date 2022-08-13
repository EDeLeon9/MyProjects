using System;
using System.Collections.Generic;
using System.Data;

namespace RegistrarInventario
{
    //Typear prop y dar TAB dos veces si desea usar la plantilla para una Automatic Property

    public class CategoriaProducto
    {
        public int IdCategoriaProducto { get; set; }
        public string Categoria { get; set; }
    }

    public class Producto
    {
        public int IdProducto { get; set; }
        public int IdCategoriaProducto { get; set; }
        public string NombreProducto { get; set; }
        public decimal? Precio { get; set; }  //Precio es nullable (decimal?)
        public string Comentarios { get; set; }
    }

    public class Variacion
    {
        public int IdVariacion { get; set; }
        public string Color { get; set; }
        public string Talla { get; set; }
        public string TallaReal { get; set; }
        public string NotaTalla { get; set; }
        public string Comentarios { get; set; }
    }

    public class Inventario
    {
        public int IdInventario { get; set; }
        public int IdOrden { get; set; }
        public int IdPaquete { get; set; }
        public int CantidadAgrupado { get; set; }
        public string NombreProducto { get; set; }
        public string Color { get; set; }
        public string Talla { get; set; }
        public string TallaReal { get; set; }
        public string NotaTalla { get; set; }
        public decimal CostoProducto { get; set; }
        public decimal CostoFinalProducto { get; set; }
        public decimal Precio { get; set; }
        public decimal GananciaEstimada { get; set; }
        public string ComentariosInventario { get; set; }

        public string NotasTallaYComentarios
        {
            get
            {
                string notasTallaYComentarios = "";
                if (!string.IsNullOrWhiteSpace(NotaTalla))
                    notasTallaYComentarios = NotaTalla;

                if (!string.IsNullOrWhiteSpace(ComentariosInventario))
                    notasTallaYComentarios = notasTallaYComentarios + " (" + ComentariosInventario + ")";  //Los comentarios irán entre paréntesis siempre

                return notasTallaYComentarios.Trim();
            }
        }
    }

    public class Orden
    {
        public int IdOrden { get; set; }
        public DateTime FechaOrden { get; set; }
        public decimal TotalProdSinShippings { get; set; }
        public decimal GastosOrden { get; set; }
        public string DescripcionOrden { get; set; }
        public int IdProveedor { get; set; }
        public string NombreProveedor { get; set; }
        public int IdTienda { get; set; }
        public string NombreTienda { get; set; }
        public int CantProductosOrden { get; set; }
        public decimal TotalOrden { get; set; }
        public string ComentariosOrden { get; set; }
        public bool Terminado { get; set; }

        public Orden CloneIfValid()
        {
            if (this.IdOrden > 0)
            {
                return new Orden()
                {
                    IdOrden = this.IdOrden,
                    FechaOrden = this.FechaOrden,
                    TotalProdSinShippings = this.TotalProdSinShippings,
                    GastosOrden = this.GastosOrden,
                    DescripcionOrden = this.DescripcionOrden,
                    IdProveedor = this.IdProveedor,
                    NombreProveedor = this.NombreProveedor,
                    IdTienda = this.IdTienda,
                    NombreTienda = this.NombreTienda,
                    CantProductosOrden = this.CantProductosOrden,
                    TotalOrden = this.TotalOrden,
                    ComentariosOrden = this.ComentariosOrden,
                    Terminado = this.Terminado
                };
            }
            else return null;
        }
    }

    public class Paquete
    {
        public int IdPaquete { get; set; }
        public DateTime? FechaConsolidacion { get; set; }
        public DateTime? FechaLlegadaPanama { get; set; }
        public string DescripcionPaquete { get; set; }
        public decimal GastosPaquete { get; set; }
        public int IdCourier { get; set; }
        public string NombreCourier { get; set; }
        public int CantProductosPaquete { get; set; }
        public string ComentariosPaquete { get; set; }
        public bool Terminado { get; set; }

        public Paquete CloneIfValid()
        {
            if (this.IdPaquete > 0)
            {
                return new Paquete()
                {
                    IdPaquete = this.IdPaquete,
                    FechaConsolidacion = this.FechaConsolidacion,
                    FechaLlegadaPanama = this.FechaLlegadaPanama,
                    DescripcionPaquete = this.DescripcionPaquete,
                    GastosPaquete = this.GastosPaquete,
                    IdCourier = this.IdCourier,
                    NombreCourier = this.NombreCourier,
                    CantProductosPaquete = this.CantProductosPaquete,
                    ComentariosPaquete = this.ComentariosPaquete,
                    Terminado = this.Terminado
                };
            }
            else return null;
        }
    }

    public class Gasto
    {
        public int IdGasto { get; set; }
        public decimal MontoGasto { get; set; }
        public DateTime FechaGasto { get; set; }
        public string DescripcionGasto { get; set; }
        public string ComentariosGasto { get; set; }
    }

    public class Proveedor
    {
        public int IdProveedor { get; set; }
        public string NombreProveedor { get; set; }
    }

    public class Tienda
    {
        public int IdTienda { get; set; }
        public string NombreTienda { get; set; }
    }

    public class Courier
    {
        public int IdCourier { get; set; }
        public string NombreCourier { get; set; }
    }
}
