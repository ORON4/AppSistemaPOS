using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using AppSistemaPOS.Models;

namespace AppSistemaPOS.ViewModels
{
    public partial class InventarioViewModel : ObservableObject
    {
        public ObservableCollection<Producto> Productos { get; } = new();

        public InventarioViewModel()
        {
            // Datos Mock
            Productos.Add(new Producto { Nombre = "Coca Cola 600ml", PrecioVenta = 18.00m, StockActual = 50, CodigoBarras = "75010553" });
            Productos.Add(new Producto { Nombre = "Gansito", PrecioVenta = 15.00m, StockActual = 12, CodigoBarras = "123456" });
            Productos.Add(new Producto { Nombre = "Emperador Chocolate", PrecioVenta = 22.00m, StockActual = 3, CodigoBarras = "987654" }); // Stock bajo
        }
    }
}