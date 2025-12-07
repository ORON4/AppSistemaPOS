using AppSistemaPOS.Models;
using System.Collections.ObjectModel;

namespace AppSistemaPOS.Services
{
    public class VentasService
    {
        private readonly List<Venta> _ventas = new();

        public void RegistrarVenta(Venta venta)
        {
            _ventas.Add(venta);
        }

        public List<Venta> ObtenerVentas()
        {
            return _ventas.ToList();
        }

        public List<Venta> ObtenerVentasDelDia(DateTime fecha)
        {
            return _ventas.Where(v => v.FechaVenta.Date == fecha.Date).ToList();
        }
    }
}
