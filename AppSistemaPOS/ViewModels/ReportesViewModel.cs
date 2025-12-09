using AppSistemaPOS.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Text.Json;


namespace AppSistemaPOS.ViewModels
{
    public partial class ReportesViewModel : ObservableObject
    {
        private readonly ApiService _apiService;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotBusy))]
        private bool isBusy;

        public bool IsNotBusy => !IsBusy;

        [ObservableProperty]
        private int ventasEfectivoCount;

        [ObservableProperty]
        private int ventasTarjetaCount;

        [ObservableProperty]
        private decimal totalEfectivo;

        [ObservableProperty]
        private decimal totalTarjeta;

        [ObservableProperty]
        private decimal totalVendido;

        [ObservableProperty]
        private string resumenCorte;

        public ObservableCollection<ProductoReporteUi> ProductosMasVendidos { get; } = new();
        public ObservableCollection<ProductoReporteUi> TodosProductosVendidos { get; } = new();

        public ReportesViewModel(ApiService apiService)
        {
            _apiService = apiService;
            Task.Run(async () => await CargarReporte());
        }

        public async Task CargarReporte()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                var metodos = await _apiService.ObtenerMetodosPagoAsync();
                var masVendidosDto = await _apiService.ObtenerMasVendidosAsync();

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    // Limpiar datos previos
                    ProductosMasVendidos.Clear();
                    TodosProductosVendidos.Clear();
                    TotalEfectivo = 0;
                    TotalTarjeta = 0;
                    VentasEfectivoCount = 0;
                    VentasTarjetaCount = 0;
                    TotalVendido = 0;

                    // Procesar Métodos de Pago
                    if (metodos != null)
                    {
                        foreach (var m in metodos)
                        {
                            if (m.Metodo == "Efectivo")
                            {
                                TotalEfectivo = m.Total;
                                VentasEfectivoCount = m.Cantidad;
                            }
                            else if (m.Metodo == "Tarjeta")
                            {
                                TotalTarjeta = m.Total;
                                VentasTarjetaCount = m.Cantidad;
                            }
                        }
                        TotalVendido = TotalEfectivo + TotalTarjeta;
                    }

                    // Procesar Productos
                    if (masVendidosDto != null)
                    {
                        foreach (var item in masVendidosDto)
                        {
                            ProductosMasVendidos.Add(new ProductoReporteUi
                            {
                                Nombre = item.Producto,
                                Cantidad = item.CantidadTotal,
                                Total = item.Ingresos
                            });
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error cargando reportes: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task HacerCorte()
        {
            if (IsBusy) return;

            bool confirmar = await Shell.Current.DisplayAlert(
                "Confirmar Corte",
                "¿Estás seguro de realizar el corte? Esto calculará el total y BORRARÁ las ventas del día de la base de datos.",
                "Sí, hacer corte",
                "Cancelar");

            if (!confirmar) return;

            try
            {
                IsBusy = true;

                // Llamada a la API
                var respuesta = await _apiService.PostAsync("api/ventas/CorteDelDia", null);

                if (respuesta.IsSuccessStatusCode)
                {
                    // 2. CORRECCIÓN: Usamos System.Text.Json en lugar de JsonConvert
                    var jsonString = await respuesta.Content.ReadAsStringAsync();

                    var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var datos = JsonSerializer.Deserialize<RespuestaCorte>(jsonString, opciones);

                    await Shell.Current.DisplayAlert("Corte Exitoso",
                        $"Se vendió un total de: ${datos.Total}\nEn {datos.Transacciones} ventas.\nLos registros han sido borrados.",
                        "OK");

                    // 3. CORRECCIÓN: Limpiamos TUS listas (ListaVentas no existía)
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        ProductosMasVendidos.Clear();
                        TodosProductosVendidos.Clear();
                        TotalEfectivo = 0;
                        TotalTarjeta = 0;
                        TotalVendido = 0;
                        VentasEfectivoCount = 0;
                        VentasTarjetaCount = 0;
                        ResumenCorte = $"Corte realizado: ${datos.Total}";
                    });
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "No se pudo realizar el corte. Verifica la conexión.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Ocurrió un error: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

    }

    
    public class ProductoReporteUi
    {
        public string Nombre { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal Total { get; set; }
    }

    public class RespuestaCorte
    {
        public string Mensaje { get; set; }
        public decimal Total { get; set; }
        public int Transacciones { get; set; }
    }
}