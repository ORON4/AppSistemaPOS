using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using AppSistemaPOS.Services;

namespace AppSistemaPOS.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {
        private readonly ApiService _apiService;

        [ObservableProperty]
        private string nombreUsuario;

        [ObservableProperty]
        private string fechaHoy = DateTime.Now.ToLongDateString();

        [ObservableProperty]
        private decimal ventasHoy;

        [ObservableProperty]
        private int transaccionesHoy;

        public ObservableCollection<string> Alertas { get; } = new();

        public DashboardViewModel(ApiService apiService)
        {
            _apiService = apiService;
            NombreUsuario = App.UsuarioActual?.Nombre ?? "Usuario";
        }

        public async void ActualizarDatos()
        {
            if (App.UsuarioActual != null)
                NombreUsuario = App.UsuarioActual.Nombre;

            // 1. Cargar Resumen Ventas
            var stats = await _apiService.ObtenerResumenHoyAsync();
            if (stats != null)
            {
                VentasHoy = stats.TotalIngresos;
                TransaccionesHoy = stats.CantidadVentas;
            }

            // 2. Cargar Alertas de Stock Bajo
            var bajoStock = await _apiService.ObtenerStockBajoAsync();
            Alertas.Clear();

            if (bajoStock.Count > 0)
            {
                foreach (var p in bajoStock)
                {
                    Alertas.Add($"!! {p.Nombre} - Quedan: {p.StockActual} (Mínimo: {p.StockMinimo})");
                }
            }
            else
            {
                Alertas.Add("No hay alertas de stock.");
            }
        }

        [RelayCommand]
        private async Task IrAEntradaInventario() => await Shell.Current.GoToAsync(nameof(Views.EntradaInventarioView));

        [RelayCommand]
        private async Task IrAInventario() => await Shell.Current.GoToAsync(nameof(Views.InventarioView));

        [RelayCommand]
        private async Task IrAVentas() => await Shell.Current.GoToAsync(nameof(Views.VentasView));

        [RelayCommand]
        private async Task IrAReportes() => await Shell.Current.GoToAsync(nameof(Views.ReportesView));

        [RelayCommand]
        private async Task IrAReporteEntradas() => await Shell.Current.GoToAsync(nameof(Views.ReporteEntradasView));
    }
}