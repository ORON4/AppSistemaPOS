using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using AppSistemaPOS.Models;
using AppSistemaPOS.Services;
using System.Text;

namespace AppSistemaPOS.ViewModels
{
    public partial class VentasViewModel : ObservableObject
    {
        private readonly VentasService _ventasService;
        private readonly InventarioService _inventarioService;

        // Lista de productos disponibles para seleccionar
        public ObservableCollection<Producto> ProductosDisponibles { get; } = new();

        // Carrito de compras
        public ObservableCollection<DetalleVenta> Carrito { get; } = new();

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Total))]
        private decimal subtotal;

        public decimal Total => Subtotal;

        public VentasViewModel(VentasService ventasService, InventarioService inventarioService)
        {
            _ventasService = ventasService;
            _inventarioService = inventarioService;

            CargarProductos();
        }

        public void CargarProductos()
        {
            ProductosDisponibles.Clear();
            var productos = _inventarioService.ObtenerProductos();
            foreach (var p in productos)
            {
                ProductosDisponibles.Add(p);
            }
        }

        [RelayCommand]
        private void AgregarProducto(Producto producto)
        {
            if (producto == null) return;

            var itemExistente = Carrito.FirstOrDefault(d => d.ProductoId == producto.ProductoId);
            if (itemExistente != null)
            {
                itemExistente.Cantidad++;
                // Notificar cambio en propiedad calculada Subtotal si es necesario, 
                // pero como es propiedad simple en modelo, tal vez necesitemos forzar actualización UI
                // O mejor, reemplazar el item o usar ObservableObject en DetalleVenta.
                // Por simplicidad, recalculemos total del VM.
                
                // Hack para refrescar UI de la lista si el item no es Observable:
                var index = Carrito.IndexOf(itemExistente);
                Carrito[index] = new DetalleVenta 
                { 
                    ProductoId = itemExistente.ProductoId,
                    NombreProducto = itemExistente.NombreProducto,
                    Cantidad = itemExistente.Cantidad,
                    PrecioUnitario = itemExistente.PrecioUnitario
                };
            }
            else
            {
                Carrito.Add(new DetalleVenta 
                { 
                    ProductoId = producto.ProductoId, 
                    NombreProducto = producto.Nombre, 
                    Cantidad = 1, 
                    PrecioUnitario = producto.PrecioVenta 
                });
            }

            RecalcularTotal();
        }

        private void RecalcularTotal()
        {
            Subtotal = Carrito.Sum(x => x.Subtotal);
        }

        [RelayCommand]
        private async Task Cobrar()
        {
            if (Carrito.Count == 0)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "El carrito está vacío.", "OK");
                return;
            }

            // Preguntar método de pago
            string metodoPago = await Application.Current.MainPage.DisplayActionSheet("Seleccione Método de Pago", "Cancelar", null, "Efectivo", "Tarjeta");
            if (string.IsNullOrEmpty(metodoPago) || metodoPago == "Cancelar") return;

            // Crear venta
            var venta = new Venta
            {
                FechaVenta = DateTime.Now,
                MetodoPago = metodoPago,
                Subtotal = Subtotal,
                Total = Total,
                Detalles = Carrito.ToList(),
                UsuarioId = 1, // Usuario simulado
                Estado = "Pagada"
            };

            // Guardar venta
            _ventasService.RegistrarVenta(venta);

            // Generar Ticket
            var sb = new StringBuilder();
            sb.AppendLine("=== SistemaPOS ===");
            sb.AppendLine($"Sucursal: SistemaPOS");
            sb.AppendLine($"Cajero: Alberto"); // Nombre simulado
            sb.AppendLine($"Fecha: {venta.FechaVenta}");
            sb.AppendLine("--------------------------------");
            foreach (var item in venta.Detalles)
            {
                sb.AppendLine($"{item.NombreProducto}");
                sb.AppendLine($"{item.Cantidad} x ${item.PrecioUnitario:F2} = ${item.Subtotal:F2}");
            }
            sb.AppendLine("--------------------------------");
            sb.AppendLine($"TOTAL: ${venta.Total:F2}");
            sb.AppendLine("--------------------------------");
            sb.AppendLine("¡Gracias por su compra!");
            sb.AppendLine("¡Vuelva pronto!");

            await Application.Current.MainPage.DisplayAlert("Ticket de Compra", sb.ToString(), "OK");

            Carrito.Clear();
            RecalcularTotal();
        }
    }
}
