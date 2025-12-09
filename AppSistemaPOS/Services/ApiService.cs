using AppSistemaPOS.Models;
using AppSistemaPOS.ViewModels;
using System.Net.Http.Json;
using System.Text.Json;

namespace AppSistemaPOS.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        
        private const string BaseUrl = "http://192.168.1.67:5241/api/";

        public ApiService()
        {
            var handler = new HttpClientHandler();
            // Esto permite que funcione con IPs locales sin certificado SSL válido (útil en desarrollo)
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;

            _httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(BaseUrl),
                Timeout = TimeSpan.FromSeconds(20)
            };

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        // ================= AUTH =================
        public async Task<Usuario?> LoginAsync(string email, string password)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("Usuarios/login", new { Email = email, Password = password });
                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadFromJsonAsync<Usuario>(_jsonOptions);
            }
            catch (Exception ex) { Console.WriteLine($"[Login] Error: {ex.Message}"); throw; }
            return null;
        }

        // ================= PRODUCTOS =================
        public async Task<List<Producto>> ObtenerProductosAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("Productos");
                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadFromJsonAsync<List<Producto>>(_jsonOptions) ?? new();
            }
            catch { }
            return new List<Producto>();
        }

        public async Task<Producto?> ObtenerProductoPorCodigoAsync(string codigo)
        {
            try
            {
                var response = await _httpClient.GetAsync($"Productos/codigo/{codigo}");
                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadFromJsonAsync<Producto>(_jsonOptions);
            }
            catch { }
            return null;
        }

        public async Task<bool> ActualizarProductoAsync(Producto producto)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"Productos/{producto.ProductoId}", producto);
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        // ================= CATEGORIAS =================
        public async Task<List<Categoria>> ObtenerCategoriasAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("Categorias");
                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadFromJsonAsync<List<Categoria>>(_jsonOptions) ?? new();
            }
            catch { }
            return new List<Categoria>();
        }

        // ================= VENTAS =================
        public async Task<bool> RegistrarVentaAsync(Venta venta)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("Ventas", venta);
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        // ================= INVENTARIO =================
        public async Task<bool> RegistrarEntradaAsync(object entradaDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("Inventario/entrada", entradaDto);
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        public async Task<List<EntradaInventario>> ObtenerHistorialEntradasAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("Inventario");
                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadFromJsonAsync<List<EntradaInventario>>(_jsonOptions) ?? new();
            }
            catch { }
            return new List<EntradaInventario>();
        }

        // ================= REPORTES =================
        public async Task<DashboardStats?> ObtenerResumenHoyAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("Reportes/ventas-hoy");
                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadFromJsonAsync<DashboardStats>(_jsonOptions);
            }
            catch { }
            return new DashboardStats();
        }

        public async Task<List<Producto>> ObtenerStockBajoAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("Reportes/stock-bajo");
                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadFromJsonAsync<List<Producto>>(_jsonOptions) ?? new();
            }
            catch { }
            return new List<Producto>();
        }

        public async Task<List<ProductoReporte>> ObtenerMasVendidosAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("Reportes/mas-vendidos");
                if (response.IsSuccessStatusCode)
                    // Tu API devuelve una lista anónima { producto, cantidadTotal, ingresos }
                    // La mapearemos en el ViewModel o creamos una clase DTO auxiliar aquí si fuera estricto,
                    // pero usaremos ProductoReporte que ya tienes en Models.
                    return await response.Content.ReadFromJsonAsync<List<ProductoReporte>>(_jsonOptions) ?? new();
            }
            catch { }
            return new List<ProductoReporte>();
        }

        public async Task<List<MetodoPagoReporte>> ObtenerMetodosPagoAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("Reportes/metodos-pago");
                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadFromJsonAsync<List<MetodoPagoReporte>>(_jsonOptions) ?? new();
            }
            catch { }
            return new List<MetodoPagoReporte>();
        }
    }

    // DTOs Auxiliares para respuestas específicas de Reportes
    public class DashboardStats
    {
        public decimal TotalIngresos { get; set; }
        public int CantidadVentas { get; set; }
    }

    public class MetodoPagoReporte
    {
        public string Metodo { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public int Cantidad { get; set; }
    }
}