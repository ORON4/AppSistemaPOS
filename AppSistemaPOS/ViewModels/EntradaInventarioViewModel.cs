using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using AppSistemaPOS.Models; // Asegúrate de tener un modelo DetalleEntrada o usar uno temporal

namespace AppSistemaPOS.ViewModels
{
    public partial class EntradaInventarioViewModel : ObservableObject
    {
        [ObservableProperty]
        private string observaciones;

        // Lista de productos que van a entrar
        public ObservableCollection<DetalleEntradaItem> ProductosEntrada { get; } = new();

        [ObservableProperty]
        private decimal totalCosto;

        // Campos para agregar un producto individual a la lista
        [ObservableProperty]
        private string codigoBarrasAgregar;
        [ObservableProperty]
        private int cantidadAgregar;
        [ObservableProperty]
        private decimal costoUnitarioAgregar;

        [RelayCommand]
        private void AgregarProductoALista()
        {
            if (string.IsNullOrEmpty(CodigoBarrasAgregar) || CantidadAgregar <= 0 || CostoUnitarioAgregar <= 0)
                return;

            ProductosEntrada.Add(new DetalleEntradaItem
            {
                Codigo = CodigoBarrasAgregar,
                Cantidad = CantidadAgregar,
                Costo = CostoUnitarioAgregar,
                Subtotal = CantidadAgregar * CostoUnitarioAgregar
            });

            TotalCosto += (CantidadAgregar * CostoUnitarioAgregar);

            // Limpiar campos
            CodigoBarrasAgregar = string.Empty;
            CantidadAgregar = 0;
            CostoUnitarioAgregar = 0;
        }

        [RelayCommand]
        private async Task GuardarEntrada()
        {
            await Application.Current.MainPage.DisplayAlert("Guardar", $"Se registrará una entrada por ${TotalCosto:F2}", "OK");
            await Shell.Current.GoToAsync(".."); // Volver atrás
        }
    }

    // Clase auxiliar para la vista (si no tienes el modelo completo aún)
    public class DetalleEntradaItem
    {
        public string Codigo { get; set; }
        public int Cantidad { get; set; }
        public decimal Costo { get; set; }
        public decimal Subtotal { get; set; }
    }
}