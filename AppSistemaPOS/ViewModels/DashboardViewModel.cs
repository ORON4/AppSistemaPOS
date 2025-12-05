using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace AppSistemaPOS.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {
        [ObservableProperty]
        private string nombreUsuario = "Juan (Cajero)";

        [ObservableProperty]
        private string fechaHoy = DateTime.Now.ToLongDateString();

        [ObservableProperty]
        private decimal ventasHoy = 1250.50m;

        [ObservableProperty]
        private int transaccionesHoy = 15;

        // Lista de alertas simuladas
        public ObservableCollection<string> Alertas { get; } = new()
        {
            "[!] Coca Cola 600ml - Stock: 2 (Mín: 5)",
            "[!] Sabritas Sal - Stock: 0 (Mín: 10)"
        };

        [RelayCommand]
        private async Task IrAEntradaInventario()
        {
            // Navegamos a la nueva pantalla de entrada
            await Shell.Current.GoToAsync(nameof(Views.EntradaInventarioView));
        }
    }
}