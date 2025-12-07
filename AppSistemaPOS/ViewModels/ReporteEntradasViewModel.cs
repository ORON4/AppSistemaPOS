using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using AppSistemaPOS.Services;

namespace AppSistemaPOS.ViewModels
{
    public partial class ReporteEntradasViewModel : ObservableObject
    {
        private readonly InventarioService _inventarioService;

        public ObservableCollection<EntradaInventario> Entradas { get; } = new();

        [ObservableProperty]
        private decimal totalInversion;

        public ReporteEntradasViewModel(InventarioService inventarioService)
        {
            _inventarioService = inventarioService;
            CargarDatos();
        }

        public void CargarDatos()
        {
            Entradas.Clear();
            var lista = _inventarioService.ObtenerEntradas();
            
            foreach (var e in lista)
            {
                Entradas.Add(e);
            }

            TotalInversion = Entradas.Sum(e => e.CostoTotal);
        }
    }
}
