using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using AppSistemaPOS.Models;

namespace AppSistemaPOS.ViewModels
{
    public partial class VentasViewModel : ObservableObject
    {
        // Carrito de compras (simulado)
        public ObservableCollection<DetalleVenta> Carrito { get; } = new();

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Total))] // Avisar que si cambia Subtotal, cambia Total
        private decimal subtotal;

        public decimal Total => Subtotal; // Simplificado sin impuestos por ahora

        public VentasViewModel()
        {
            // Agregamos datos de prueba al iniciar
            Carrito.Add(new DetalleVenta { NombreProducto = "Coca Cola 600ml", Cantidad = 2, PrecioUnitario = 18.00m });
            Carrito.Add(new DetalleVenta { NombreProducto = "Sabritas Sal", Cantidad = 1, PrecioUnitario = 16.50m });

            RecalcularTotal();
        }

        private void RecalcularTotal()
        {
            Subtotal = Carrito.Sum(x => x.Subtotal);
        }

        [RelayCommand]
        private async Task Cobrar()
        {
            await Application.Current.MainPage.DisplayAlert("Cobrar", $"Total a pagar: ${Total:F2}", "OK");
            Carrito.Clear();
            RecalcularTotal();
        }
    }
}