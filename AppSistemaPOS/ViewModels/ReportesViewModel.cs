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

                // 1. Obtener datos de la API
                var metodos = await _apiService.ObtenerMetodosPagoAsync();
                var masVendidosDto = await _apiService.ObtenerMasVendidosAsync();

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    LimpiarDatos();

                    // 2. Procesar Métodos de Pago 
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

                    // 3. Procesar Productos 
                    if (masVendidosDto != null)
                    {
                        foreach (var item in masVendidosDto)
                        {
                            // Calculamos el precio unitario promedio (Total / Cantidad)
                            decimal precioUnitarioCalc = item.CantidadTotal > 0
                                ? item.Ingresos / item.CantidadTotal
                                : 0;

                            var productoUi = new ProductoReporteUi
                            {
                                Nombre = item.Producto,
                                CantidadVendida = item.CantidadTotal, 
                                TotalVendido = item.Ingresos,         
                                PrecioUnitario = precioUnitarioCalc   
                            };

                           
                            TodosProductosVendidos.Add(productoUi);
                        }

                    
                        foreach (var item in TodosProductosVendidos.Take(5))
                        {
                            ProductosMasVendidos.Add(item);
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

        private void LimpiarDatos()
        {
            ProductosMasVendidos.Clear();
            TodosProductosVendidos.Clear();
            TotalEfectivo = 0;
            TotalTarjeta = 0;
            VentasEfectivoCount = 0;
            VentasTarjetaCount = 0;
            TotalVendido = 0;
        }

        [RelayCommand]
        public async Task HacerCorte()
        {
            if (IsBusy) return;

            bool confirmar = await Shell.Current.DisplayAlert(
                "Confirmar Corte",
                "¿Estás seguro de realizar el corte? Se calculará el total y se BORRARÁN las ventas de hoy.",
                "Sí, cerrar día",
                "Cancelar");

            if (!confirmar) return;

            try
            {
                IsBusy = true;
                
                var respuesta = await _apiService.PostAsync("Ventas/CorteDelDia", null);

                if (respuesta.IsSuccessStatusCode)
                {
                    var jsonString = await respuesta.Content.ReadAsStringAsync();
                    var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var datos = JsonSerializer.Deserialize<RespuestaCorte>(jsonString, opciones);

                    await Shell.Current.DisplayAlert("Corte Exitoso",
                        $"Total cerrado: ${datos.Total}\nTransacciones: {datos.Transacciones}",
                        "OK");

                    MainThread.BeginInvokeOnMainThread(LimpiarDatos);
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
    }


    public class ProductoReporteUi
    {
        public string Nombre { get; set; } = string.Empty;

        public int CantidadVendida { get; set; }

        public decimal PrecioUnitario { get; set; }

        public decimal TotalVendido { get; set; }
    }

    public class RespuestaCorte
    {
        public string Mensaje { get; set; }
        public decimal Total { get; set; }
        public int Transacciones { get; set; }
    }
}