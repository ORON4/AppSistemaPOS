using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using AppSistemaPOS.Services;

namespace AppSistemaPOS.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {
        private readonly VentasService _ventasService;

        [ObservableProperty]
        private string nombreUsuario = "Alberto (Cajero)";

        [ObservableProperty]
        private string fechaHoy = DateTime.Now.ToLongDateString();

        [ObservableProperty]
        private decimal ventasHoy;

        [ObservableProperty]
        private int transaccionesHoy;

        // Lista de alertas simuladas
        public ObservableCollection<string> Alertas { get; } = new()
        {
            "[!] Coca Cola 600ml - Stock: 2 (Mín: 5)",
            "[!] Sabritas Sal - Stock: 0 (Mín: 10)"
        };

        public DashboardViewModel(VentasService ventasService)
        {
            _ventasService = ventasService;
            ActualizarDatos();
        }

        public void ActualizarDatos()
        {
            var ventasDelDia = _ventasService.ObtenerVentasDelDia(DateTime.Now);
            VentasHoy = ventasDelDia.Sum(v => v.Total);
            TransaccionesHoy = ventasDelDia.Count;
        }

        [RelayCommand]
        private async Task IrAEntradaInventario()
        {
            // Navegamos a la nueva pantalla de entrada
            await Shell.Current.GoToAsync(nameof(Views.EntradaInventarioView));
        }

        [RelayCommand]
        private async Task IrAInventario()
        {
            // Navegamos a la nueva pantalla de inventario
            await Shell.Current.GoToAsync(nameof(Views.InventarioView));
        }

        [RelayCommand]
        private async Task IrAVentas()
        {
            // Navegamos a la nueva pantalla de inventario
            await Shell.Current.GoToAsync(nameof(Views.VentasView));
        }

        [RelayCommand]
        private async Task IrAReportes()
        {
            await Shell.Current.GoToAsync(nameof(Views.ReportesView));
        }

        [RelayCommand]
        private async Task IrAReporteEntradas()
        {
            await Shell.Current.GoToAsync(nameof(Views.ReporteEntradasView));
        }
    }
}
