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
                UsuarioId = App.UsuarioActual?.UsuarioId ?? 1,
                FechaVenta = DateTime.Now,
                MetodoPago = metodoPago,
                Subtotal = Subtotal,
                Impuestos = 0,
                Total = Total,
                Detalles = Carrito.ToList(),
                Estado = "Completada"
                // Nota: NumeroVenta se genera en el backend, pero para el ticket visual
                // usaremos un placeholder o esperaremos la respuesta si tu API devolviera el objeto completo.
                // Por ahora, el ticket mostrará el folio vacío o pendiente hasta que recargues,
                // pero la lista de productos sí saldrá bien.
            };

            // --- CAMBIO AQUÍ: Usamos string para recibir el mensaje de error si falla ---
            string resultado = await _apiService.RegistrarVentaAsync(venta);

            if (resultado == "OK")
            {
                // Generamos el texto del ticket
                string ticketTexto = GenerarTicket(venta);

                // Mostramos el ticket en la alerta
                await Application.Current.MainPage.DisplayAlert("Ticket de Venta", ticketTexto, "Imprimir / OK");

                Carrito.Clear();
                RecalcularTotal();
                await CargarProductos();
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", resultado, "OK");
            }
        }

        private string GenerarTicket(Venta venta)
        {
            var sb = new StringBuilder();

            // Encabezado y Mensaje de Bienvenida
            sb.AppendLine("--------------------------------");
            sb.AppendLine("       SISTEMA POS       ");
            sb.AppendLine("   ¡Bienvenido a tu tienda!   ");
            sb.AppendLine("--------------------------------");
            sb.AppendLine($"Fecha: {venta.FechaVenta:dd/MM/yyyy HH:mm}");
            sb.AppendLine($"Folio: {venta.NumeroVenta}");
            sb.AppendLine("--------------------------------");

            // Encabezados de columnas
            // Formato: Cantidad | Producto | Total
            sb.AppendLine("Cant  Producto          Total");
            sb.AppendLine("--------------------------------");

            // Listado de productos
            foreach (var item in venta.Detalles)
            {
                // Truncar nombre si es muy largo para que no rompa el formato
                string nombre = item.NombreProducto.Length > 12
                    ? item.NombreProducto.Substring(0, 12) + "."
                    : item.NombreProducto.PadRight(13);

                string linea = $"{item.Cantidad,-4} {nombre} ${item.Subtotal,8:F2}";
                sb.AppendLine(linea);
            }

            // Totales y Pie de página
            sb.AppendLine("--------------------------------");
            sb.AppendLine($"TOTAL:           ${venta.Total,10:F2}");
            sb.AppendLine("--------------------------------");
            sb.AppendLine("     ¡Gracias por su compra!    ");
            sb.AppendLine("          Vuelva pronto         ");
            sb.AppendLine("--------------------------------");

            return sb.ToString();
        }
    }
}