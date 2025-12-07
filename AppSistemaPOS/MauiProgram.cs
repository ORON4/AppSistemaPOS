using AppSistemaPOS.Services;
using AppSistemaPOS.ViewModels;
using AppSistemaPOS.Views;
using Microsoft.Extensions.Logging;


namespace AppSistemaPOS
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddSingleton<ApiService>();
            builder.Services.AddSingleton<VentasService>();
            builder.Services.AddSingleton<InventarioService>();
            builder.Services.AddSingleton<AppShell>();

            //vistas
            builder.Services.AddTransient<LoginView>();
            builder.Services.AddTransient<DashboardView>();
            builder.Services.AddTransient<EntradaInventarioView>();
            builder.Services.AddTransient<InventarioView>();
            builder.Services.AddTransient<VentasView>();
            builder.Services.AddTransient<ReportesView>();
            builder.Services.AddTransient<ReporteEntradasView>();

            //viewmodels
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<DashboardViewModel>();
            builder.Services.AddTransient<EntradaInventarioViewModel>();
            builder.Services.AddTransient<InventarioViewModel>();
            builder.Services.AddTransient<VentasViewModel>();
            builder.Services.AddTransient<ReportesViewModel>();
            builder.Services.AddTransient<ReporteEntradasViewModel>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
