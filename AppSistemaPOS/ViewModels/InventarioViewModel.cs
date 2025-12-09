using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using AppSistemaPOS.Models;
using AppSistemaPOS.Services;

namespace AppSistemaPOS.ViewModels
{
    public partial class InventarioViewModel : ObservableObject
    {
        private readonly ApiService _apiService;
        private List<Producto> _todosLosProductos = new();

        public ObservableCollection<Producto> Productos { get; } = new();
        public ObservableCollection<string> Categorias { get; } = new();

        [ObservableProperty]
        private string? filtroCategoria;

        public InventarioViewModel(ApiService apiService)
        {
            _apiService = apiService;
            // Cargar datos al iniciar
            Task.Run(async () => await CargarDatos());
        }

        public async Task CargarDatos()
        {
            var productos = await _apiService.ObtenerProductosAsync();
            var categoriasApi = await _apiService.ObtenerCategoriasAsync();

            _todosLosProductos = productos;

            MainThread.BeginInvokeOnMainThread(() =>
            {
                Categorias.Clear();
                Categorias.Add("Todos");
                foreach (var cat in categoriasApi)
                {
                    Categorias.Add(cat.Nombre);
                }

                FiltroCategoria = "Todos";
                Filtrar();
            });
        }

        partial void OnFiltroCategoriaChanged(string? value) => Filtrar();

        private void Filtrar()
        {
            Productos.Clear();
            var query = _todosLosProductos.AsEnumerable();

            if (!string.IsNullOrEmpty(FiltroCategoria) && FiltroCategoria != "Todos")
            {
               
                query = query.Where(p => p.CategoriaNombre== FiltroCategoria); 
            }

            foreach (var p in query) Productos.Add(p);
        }

        [RelayCommand]
        private async Task EditarProducto(Producto producto)
        {
            if (producto == null) return;

            string nuevoNombre = await Application.Current.MainPage.DisplayPromptAsync("Editar", "Nuevo nombre:", initialValue: producto.Nombre);
            if (string.IsNullOrEmpty(nuevoNombre)) return;

            string nuevoPrecio = await Application.Current.MainPage.DisplayPromptAsync("Editar", "Nuevo precio venta:", initialValue: producto.PrecioVenta.ToString(), keyboard: Keyboard.Numeric);
            
            if (decimal.TryParse(nuevoPrecio, out decimal precio))
            {
                producto.Nombre = nuevoNombre;
                producto.PrecioVenta = precio;

                bool exito = await _apiService.ActualizarProductoAsync(producto);
                if (exito)
                {
                    await Application.Current.MainPage.DisplayAlert("Éxito", "Producto actualizado", "OK");
                    await CargarDatos(); // Recargar para asegurar
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "No se pudo actualizar en el servidor", "OK");
                }
            }
        }
    }
}