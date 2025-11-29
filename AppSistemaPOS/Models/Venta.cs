using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppSistemaPOS.Models
{
    public class Venta
    {
        public int VentaId { get; set; }
        public string NumeroVenta { get; set; } = string.Empty;
        public int UsuarioId { get; set; }
        public DateTime FechaVenta { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Impuestos { get; set; }
        public decimal Total { get; set; }
        public string MetodoPago { get; set; } = "Efectivo";
        public string Estado { get; set; } = "Completada";

        // Lista de detalles para enviar a la API
        public List<DetalleVenta> Detalles { get; set; } = new List<DetalleVenta>();
    }
}
