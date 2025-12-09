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
        private readonly ApiService _apiService;

        public ObservableCollection<Producto> ProductosDisponibles { get; } = new();
        public ObservableCollection<DetalleVenta> Carrito { get; } = new();

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Total))]
        private decimal subtotal;

        public decimal Total => Subtotal;

        public VentasViewModel(ApiService apiService)
        {
            _apiService = apiService;
            // Cargar productos al iniciar
            Task.Run(async () => await CargarProductos());
        }

        public async Task CargarProductos()
        {
            var productos = await _apiService.ObtenerProductosAsync();

            MainThread.BeginInvokeOnMainThread(() =>
            {
                ProductosDisponibles.Clear();
                // Solo mostramos productos activos y con stock
                foreach (var p in productos.Where(x => x.Activo && x.StockActual > 0))
                {
                    ProductosDisponibles.Add(p);
                }
            });
        }

        [RelayCommand]
        private void AgregarProducto(Producto producto)
        {
            if (producto == null) return;

            // Verificar si ya está en el carrito
            var item = Carrito.FirstOrDefault(d => d.ProductoId == producto.ProductoId);

            // Validar stock local
            int cantidadEnCarrito = item?.Cantidad ?? 0;
            if (cantidadEnCarrito + 1 > producto.StockActual)
            {
                Application.Current.MainPage.DisplayAlert("Stock Insuficiente", $"Solo quedan {producto.StockActual} de {producto.Nombre}", "OK");
                return;
            }

            if (item != null)
            {
                item.Cantidad++;
                // Hack para refrescar la lista visualmente (reemplazar el item)
                var index = Carrito.IndexOf(item);
                Carrito[index] = new DetalleVenta
                {
                    ProductoId = item.ProductoId,
                    NombreProducto = item.NombreProducto,
                    Cantidad = item.Cantidad,
                    PrecioUnitario = item.PrecioUnitario
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

        private void RecalcularTotal() => Subtotal = Carrito.Sum(x => x.Subtotal);

        [RelayCommand]
        private async Task Cobrar()
        {
            if (Carrito.Count == 0) return;

            string metodoPago = await Application.Current.MainPage.DisplayActionSheet("Método de Pago", "Cancelar", null, "Efectivo", "Tarjeta");
            if (string.IsNullOrEmpty(metodoPago) || metodoPago == "Cancelar") return;

            var venta = new Venta
            {
                UsuarioId = App.UsuarioActual?.UsuarioId ?? 1, // Usa el usuario logueado
                FechaVenta = DateTime.Now,
                MetodoPago = metodoPago,
                Subtotal = Subtotal,
                Impuestos = 0, // Ajusta si manejas impuestos
                Total = Total,
                Detalles = Carrito.ToList(),
                Estado = "Completada"
            };

            bool exito = await _apiService.RegistrarVentaAsync(venta);

            if (exito)
            {
                await Application.Current.MainPage.DisplayAlert("Venta Exitosa", $"Total: ${Total:F2}", "OK");
                Carrito.Clear();
                RecalcularTotal();
                await CargarProductos(); // Recargar para actualizar los stocks en pantalla
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "No se pudo registrar la venta. Verifique conexión o stocks.", "OK");
            }
        }
    }
}