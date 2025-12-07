using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using AppSistemaPOS.Models;
using AppSistemaPOS.Services;

namespace AppSistemaPOS.ViewModels
{
    public partial class InventarioViewModel : ObservableObject
    {
        private readonly InventarioService _inventarioService;
        private List<Producto> _allProductos = new();

        public ObservableCollection<Producto> Productos { get; } = new();

        public ObservableCollection<string> Categorias { get; } = new();

        [ObservableProperty]
        private string? filtroCategoria;

        public InventarioViewModel(InventarioService inventarioService)
        {
            _inventarioService = inventarioService;
            CargarDatos();
        }

        private void CargarDatos()
        {
            _allProductos = _inventarioService.ObtenerProductos();

            // Cargar Categorias
            Categorias.Clear();
            Categorias.Add("Todos");
            foreach (var cat in _allProductos.Select(p => p.CategoriaNombre).Distinct())
            {
                if (cat != null) Categorias.Add(cat);
            }

            // Si el filtro actual no es válido, resetear a Todos
            if (string.IsNullOrEmpty(FiltroCategoria) || !Categorias.Contains(FiltroCategoria))
            {
                FiltroCategoria = "Todos";
            }

            Filtrar();
        }

        partial void OnFiltroCategoriaChanged(string? value)
        {
            Filtrar();
        }

        private void Filtrar()
        {
            // Recargar productos desde el servicio por si hubo cambios (stock, etc)
            _allProductos = _inventarioService.ObtenerProductos();
            
            Productos.Clear();
            var query = _allProductos.AsEnumerable();

            if (!string.IsNullOrEmpty(FiltroCategoria) && FiltroCategoria != "Todos")
            {
                query = query.Where(p => p.CategoriaNombre == FiltroCategoria);
            }

            foreach (var p in query)
            {
                Productos.Add(p);
            }
        }

        [RelayCommand]
        private async Task EditarProducto(Producto producto)
        {
            if (producto == null) return;

            // Simple edición con Prompts (para no crear otra vista completa por ahora)
            // Se podría mejorar con una vista de Edición
            string nuevoNombre = await Application.Current.MainPage.DisplayPromptAsync("Editar Producto", "Nombre del producto:", initialValue: producto.Nombre);
            if (nuevoNombre == null) return; // Cancelado

            string nuevoPrecioStr = await Application.Current.MainPage.DisplayPromptAsync("Editar Producto", "Precio de venta:", initialValue: producto.PrecioVenta.ToString(), keyboard: Keyboard.Numeric);
            if (nuevoPrecioStr == null) return;

            if (decimal.TryParse(nuevoPrecioStr, out decimal nuevoPrecio))
            {
                // Actualizar modelo
                producto.Nombre = nuevoNombre;
                producto.PrecioVenta = nuevoPrecio;
                
                // Actualizar en servicio
                _inventarioService.ActualizarProducto(producto);

                // Refrescar lista
                Filtrar();
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Precio inválido", "OK");
            }
        }
    }
}
