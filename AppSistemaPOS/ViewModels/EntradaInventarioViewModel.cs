using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using AppSistemaPOS.Models;
using AppSistemaPOS.Services;

namespace AppSistemaPOS.ViewModels
{
    public partial class EntradaInventarioViewModel : ObservableObject
    {
        private readonly InventarioService _inventarioService;

        [ObservableProperty]
        private string observaciones;

        // Lista de productos que van a entrar
        public ObservableCollection<DetalleEntrada> ProductosEntrada { get; } = new();

        [ObservableProperty]
        private decimal totalCosto;

        // Campos para agregar un producto individual a la lista
        [ObservableProperty]
        private string codigoBarrasAgregar;
        [ObservableProperty]
        private int cantidadAgregar;
        [ObservableProperty]
        private decimal costoUnitarioAgregar;

        public EntradaInventarioViewModel(InventarioService inventarioService)
        {
            _inventarioService = inventarioService;
        }

        [RelayCommand]
        private void AgregarProductoALista()
        {
            if (string.IsNullOrEmpty(CodigoBarrasAgregar) || CantidadAgregar <= 0 || CostoUnitarioAgregar <= 0)
                return;

            ProductosEntrada.Add(new DetalleEntrada
            {
                CodigoProducto = CodigoBarrasAgregar,
                Cantidad = CantidadAgregar,
                CostoUnitario = CostoUnitarioAgregar,
                // NombreProducto opcional o buscarlo si ya existe
                NombreProducto = _inventarioService.ObtenerProductoPorCodigo(CodigoBarrasAgregar)?.Nombre ?? "Nuevo"
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
            var nuevaEntrada = new EntradaInventario
            {
                Observaciones = Observaciones,
                CostoTotal = TotalCosto,
                Detalles = ProductosEntrada.ToList()
            };

            _inventarioService.RegistrarEntrada(nuevaEntrada);

            await Application.Current.MainPage.DisplayAlert("Guardar", $"Se registró una entrada por ${TotalCosto:F2}", "OK");
            
            // Limpiar
            ProductosEntrada.Clear();
            TotalCosto = 0;
            Observaciones = string.Empty;

            await Shell.Current.GoToAsync(".."); // Volver atrás
        }
    }
}
