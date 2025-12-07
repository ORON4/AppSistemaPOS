using AppSistemaPOS.Models;
using System.Collections.ObjectModel;

namespace AppSistemaPOS.Services
{
    public class InventarioService
    {
        private readonly List<Producto> _productos = new();
        private readonly List<EntradaInventario> _entradas = new();

        public InventarioService()
        {
            // Datos Mock Iniciales
            _productos.Add(new Producto { ProductoId = 1, Nombre = "Coca Cola 600ml", PrecioVenta = 18.00m, StockActual = 50, CodigoBarras = "75010553", CategoriaNombre = "Bebidas" });
            _productos.Add(new Producto { ProductoId = 2, Nombre = "Gansito", PrecioVenta = 15.00m, StockActual = 12, CodigoBarras = "123456", CategoriaNombre = "Pastelitos" });
            _productos.Add(new Producto { ProductoId = 3, Nombre = "Emperador Chocolate", PrecioVenta = 22.00m, StockActual = 3, CodigoBarras = "987654", CategoriaNombre = "Galletas" });
            _productos.Add(new Producto { ProductoId = 4, Nombre = "Agua Ciel 1L", PrecioVenta = 12.00m, StockActual = 24, CodigoBarras = "111222", CategoriaNombre = "Bebidas" });
        }

        public List<Producto> ObtenerProductos()
        {
            return _productos.ToList();
        }

        public Producto? ObtenerProductoPorCodigo(string codigo)
        {
            return _productos.FirstOrDefault(p => p.CodigoBarras == codigo);
        }

        public void ActualizarProducto(Producto producto)
        {
            var existing = _productos.FirstOrDefault(p => p.ProductoId == producto.ProductoId);
            if (existing != null)
            {
                existing.Nombre = producto.Nombre;
                existing.PrecioVenta = producto.PrecioVenta;
                existing.CategoriaNombre = producto.CategoriaNombre;
                // Stock se actualiza via entradas/ventas
            }
        }

        public void RegistrarEntrada(EntradaInventario entrada)
        {
            entrada.FechaEntrada = DateTime.Now;
            _entradas.Add(entrada);

            // Actualizar stock de productos
            foreach (var item in entrada.Detalles)
            {
                var producto = _productos.FirstOrDefault(p => p.CodigoBarras == item.CodigoProducto);
                if (producto != null)
                {
                    producto.StockActual += item.Cantidad;
                }
                else
                {
                    // Si el producto no existe, lo creamos (simplificado)
                    var nuevoProducto = new Producto
                    {
                        ProductoId = _productos.Any() ? _productos.Max(p => p.ProductoId) + 1 : 1,
                        CodigoBarras = item.CodigoProducto,
                        Nombre = item.NombreProducto ?? "Nuevo Producto",
                        StockActual = item.Cantidad,
                        PrecioCompra = item.CostoUnitario,
                        PrecioVenta = item.CostoUnitario * 1.3m, // Margen automatico ejemplo
                        CategoriaNombre = "General"
                    };
                    _productos.Add(nuevoProducto);
                }
            }
        }

        public List<EntradaInventario> ObtenerEntradas()
        {
            return _entradas.OrderByDescending(e => e.FechaEntrada).ToList();
        }
    }

    public class EntradaInventario
    {
        public int EntradaId { get; set; }
        public DateTime FechaEntrada { get; set; }
        public string Observaciones { get; set; } = string.Empty;
        public decimal CostoTotal { get; set; }
        public List<DetalleEntrada> Detalles { get; set; } = new();
    }

    public class DetalleEntrada
    {
        public string CodigoProducto { get; set; } = string.Empty;
        public string NombreProducto { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal CostoUnitario { get; set; }
        public decimal Subtotal => Cantidad * CostoUnitario;
    }
}
