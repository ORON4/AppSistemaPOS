using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppSistemaPOS.Models
{
    public class Producto
    {
        public class Categoria
        {
            public int ProductoId { get; set; }
            public string CodigoBarras { get; set; } = string.Empty;
            public string Nombre { get; set; } = string.Empty;
            public string? Descripcion { get; set; }
            public int CategoriaId { get; set; }

            // Propiedades de navegación
            public string? CategoriaNombre { get; set; }

            public decimal PrecioCompra { get; set; }
            public decimal PrecioVenta { get; set; }
            public int StockActual { get; set; }
            public int StockMinimo { get; set; }
            public string? ImagenUrl { get; set; }
            public bool Activo { get; set; }
        }
    }
}
