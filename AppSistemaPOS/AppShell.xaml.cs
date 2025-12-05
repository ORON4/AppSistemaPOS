using AppSistemaPOS.Services;
using AppSistemaPOS.Views;

namespace AppSistemaPOS
{
    public partial class AppShell : Shell
    {
        private readonly ApiService _apiService;
        public AppShell(ApiService apiService)
        {
            InitializeComponent();
            _apiService = apiService;

            Routing.RegisterRoute(nameof(DashboardView), typeof(DashboardView));
            Routing.RegisterRoute(nameof(EntradaInventarioView), typeof(EntradaInventarioView));
            Routing.RegisterRoute(nameof(InventarioView), typeof(InventarioView));
            Routing.RegisterRoute(nameof(LoginView), typeof(LoginView));
            Routing.RegisterRoute(nameof(VentasView), typeof(VentasView));
        }
    }
}
