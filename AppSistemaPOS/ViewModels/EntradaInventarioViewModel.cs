using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using AppSistemaPOS.Models;
using AppSistemaPOS.Services;


namespace AppSistemaPOS.ViewModels
{
    public partial class EntradaInventarioViewModel : ObservableObject
    {
        private readonly ApiService _apiService;

        [ObservableProperty] private string observaciones;
        [ObservableProperty] private decimal totalCosto;
        [ObservableProperty] private string codigoBarrasAgregar;
        [ObservableProperty] private int cantidadAgregar;
        [ObservableProperty] private decimal costoUnitarioAgregar;

        public ObservableCollection<DetalleEntrada> ProductosEntrada { get; } = new();

        public EntradaInventarioViewModel(ApiService apiService)
        {
            _apiService = apiService;
        }

        [RelayCommand]
        private async Task AgregarProductoALista()
        {
            if (string.IsNullOrEmpty(CodigoBarrasAgregar) || CantidadAgregar <= 0 || CostoUnitarioAgregar <= 0)
                return;

            // Verificar si el producto ya existe en la API para obtener su nombre
            string nombre = "Producto Nuevo";
            var prod = await _apiService.ObtenerProductoPorCodigoAsync(CodigoBarrasAgregar);

            if (prod != null)
            {
                nombre = prod.Nombre;
            }
            else
            {
                // Si es nuevo, podríamos pedir el nombre al usuario
                nombre = await Application.Current.MainPage.DisplayPromptAsync("Nuevo Producto", $"Ingrese nombre para {CodigoBarrasAgregar}:") ?? "Nuevo";
            }

            ProductosEntrada.Add(new DetalleEntrada
            {
                CodigoProducto = CodigoBarrasAgregar,
                NombreProducto = nombre,
                Cantidad = CantidadAgregar,
                CostoUnitario = CostoUnitarioAgregar
            });

            TotalCosto += (CantidadAgregar * CostoUnitarioAgregar);

            // Limpiar inputs
            CodigoBarrasAgregar = string.Empty;
            CantidadAgregar = 0;
            CostoUnitarioAgregar = 0;
        }

        [RelayCommand]
        private async Task GuardarEntrada()
        {
            if (ProductosEntrada.Count == 0) return;

            // Mapear a la estructura DTO que espera la API
            var entradaDto = new
            {
                UsuarioId = App.UsuarioActual?.UsuarioId ?? 1,
                Observaciones = Observaciones,
                Productos = ProductosEntrada.Select(p => new
                {
                    CodigoBarras = p.CodigoProducto,
                    Cantidad = p.Cantidad,
                    CostoUnitario = p.CostoUnitario,
                    Nombre = p.NombreProducto,
                    PrecioVenta = p.CostoUnitario * 1.3m, // Margen por defecto para nuevos
                    CategoriaId = 1, // Categoría default para nuevos
                    Descripcion = "Ingreso App"
                }).ToList()
            };

            bool exito = await _apiService.RegistrarEntradaAsync(entradaDto);

            if (exito)
            {
                await Application.Current.MainPage.DisplayAlert("Éxito", "Entrada registrada", "OK");
                ProductosEntrada.Clear();
                TotalCosto = 0;
                Observaciones = string.Empty;
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "No se pudo registrar la entrada", "OK");
            }
        }
    }
}