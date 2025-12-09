using AppSistemaPOS.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Net.Http.Json;
using System.Text.Json.Serialization;


namespace AppSistemaPOS.ViewModels
{
    public partial class ReportesViewModel : ObservableObject
    {
        private readonly ApiService _apiService;

        [ObservableProperty]
        private bool isBusy;

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
            try
            {
                // 1. Obtener Resumen por Método de Pago desde la API
                var metodos = await _apiService.ObtenerMetodosPagoAsync();

                // 2. Obtener Top Productos desde la API
                var masVendidosDto = await _apiService.ObtenerMasVendidosAsync();

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    // Limpiar valores anteriores
                    ProductosMasVendidos.Clear();
                    TotalEfectivo = 0;
                    TotalTarjeta = 0;
                    VentasEfectivoCount = 0;
                    VentasTarjetaCount = 0;

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
        }

        [RelayCommand]
        public async Task HacerCorte()
        {
            bool confirmar = await Shell.Current.DisplayAlert(
                "Confirmar Corte",
                "¿Estás seguro de realizar el corte? Esto calculará el total y BORRARÁ las ventas del día de la base de datos.",
                "Sí, hacer corte",
                "Cancelar");

            if (!confirmar) return;

            try
            {
                IsBusy = true;

                // Suponiendo que tienes un servicio 'ApiService'
                var respuesta = await _apiService.PostAsync("api/ventas/CorteDelDia", null);

                if (respuesta.IsSuccess)
                {
                    // Deserializar la respuesta para mostrar los datos
                    var datos = JsonConvert.DeserializeObject<RespuestaCorte>(respuesta.Content);

                    await Shell.Current.DisplayAlert("Corte Exitoso",
                        $"Se vendió un total de: ${datos.Total}\nEn {datos.Transacciones} ventas.\nLos registros han sido borrados.",
                        "OK");

                    // Limpiar la lista local de ventas porque ya no existen en la BD
                    ListaVentas.Clear();
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "No se pudo realizar el corte.", "OK");
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

        // Clase para mostrar en la UI de la App
        public class ProductoReporteUi
        {
            public string Nombre { get; set; } = string.Empty;
            public int Cantidad { get; set; }
            public decimal Total { get; set; }
        }
    }
}