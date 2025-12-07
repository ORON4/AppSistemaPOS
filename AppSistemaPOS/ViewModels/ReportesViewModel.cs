using CommunityToolkit.Mvvm.ComponentModel;
using AppSistemaPOS.Services;
using System.Collections.ObjectModel;
using AppSistemaPOS.Models;

namespace AppSistemaPOS.ViewModels
{
    public partial class ReportesViewModel : ObservableObject
    {
        private readonly VentasService _ventasService;

        [ObservableProperty]
        private int ventasEfectivoCount;

        [ObservableProperty]
        private int ventasTarjetaCount;

        [ObservableProperty]
        private decimal totalEfectivo;

        [ObservableProperty]
        private decimal totalTarjeta;

        [ObservableProperty]
        private decimal totalVendido;

        public ObservableCollection<ProductoReporte> ProductosMasVendidos { get; } = new();
        public ObservableCollection<ProductoReporte> TodosProductosVendidos { get; } = new();

        public ReportesViewModel(VentasService ventasService)
        {
            _ventasService = ventasService;
            CargarReporte();
        }

        public void CargarReporte()
        {
            var ventas = _ventasService.ObtenerVentas();

            // Resumen por método de pago
            VentasEfectivoCount = ventas.Count(v => v.MetodoPago == "Efectivo");
            VentasTarjetaCount = ventas.Count(v => v.MetodoPago == "Tarjeta");

            TotalEfectivo = ventas.Where(v => v.MetodoPago == "Efectivo").Sum(v => v.Total);
            TotalTarjeta = ventas.Where(v => v.MetodoPago == "Tarjeta").Sum(v => v.Total);
            TotalVendido = ventas.Sum(v => v.Total);

            // Productos
            var todosDetalles = ventas.SelectMany(v => v.Detalles);

            var reporteProductos = todosDetalles
                .GroupBy(d => d.NombreProducto)
                .Select(g => new ProductoReporte
                {
                    Nombre = g.Key ?? "Desconocido",
                    CantidadVendida = g.Sum(d => d.Cantidad),
                    PrecioUnitario = g.First().PrecioUnitario, // Asumiendo precio constante por ahora
                    TotalVendido = g.Sum(d => d.Subtotal)
                })
                .ToList();

            // Todos los productos
            TodosProductosVendidos.Clear();
            foreach (var item in reporteProductos)
            {
                TodosProductosVendidos.Add(item);
            }

            // Más vendidos (Top 5)
            ProductosMasVendidos.Clear();
            foreach (var item in reporteProductos.OrderByDescending(x => x.CantidadVendida).Take(5))
            {
                ProductosMasVendidos.Add(item);
            }
        }
    }

    public class ProductoReporte
    {
        public string Nombre { get; set; } = string.Empty;
        public int CantidadVendida { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal TotalVendido { get; set; }
    }
}
