using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AppSistemaPOS.Models;
using AppSistemaPOS.Services;

namespace AppSistemaPOS.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly ApiService _apiService;

        public LoginViewModel(ApiService apiService)
        {
            _apiService = apiService;
        }

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private bool isBusy;

        [RelayCommand]
        private async Task Login()
        {
            if (IsBusy) return;

            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                await ShowAlert("Error", "Ingrese correo y contraseña");
                return;
            }

            IsBusy = true;

            try
            {
                await Task.Delay(1000);
                // La llamada a la API es asíncrona y puede correr en otro hilo
                // var usuario = await _apiService.LoginAsync(Email, Password);

                var usuario = new Usuario
                {
                    Nombre = "Alberto",
                };

                if (usuario != null)
                {
                    await ShowAlert("¡Bienvenido!", $"Hola {usuario.Nombre}, acceso concedido.");
                    await Shell.Current.GoToAsync("/DashboardView");
                }
                else
                {
                    await ShowAlert("Acceso Denegado", "Correo o contraseña incorrectos.");
                }
            }
            catch (Exception ex)
            {
                string mensaje = $"No se pudo conectar: {ex.Message}";
                if (ex.InnerException != null) mensaje += $"\n{ex.InnerException.Message}";

                await ShowAlert("Error Técnico", mensaje);
            }
            finally
            {
                IsBusy = false;
            }
        }

        // --- MÉTODO AUXILIAR SEGURO PARA LA UI ---
        private Task ShowAlert(string title, string message)
        {
            return MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await Application.Current.MainPage.DisplayAlert(title, message, "OK");
            });
        }
    }
}