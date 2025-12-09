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
                var usuario = await _apiService.LoginAsync(Email, Password);

                if (usuario != null)
                {
                    // 1. GUARDAR USUARIO EN SESIÓN GLOBAL
                    App.UsuarioActual = usuario;

                    // 2. NAVEGAR
                    // Usamos // para resetear la navegación y que no pueda volver atrás al login
                    await Shell.Current.GoToAsync("/DashboardView");
                }
                else
                {
                    await ShowAlert("Acceso Denegado", "Credenciales incorrectas.");
                }
            }
            catch (Exception ex)
            {
                await ShowAlert("Error de Conexión", $"No se pudo contactar a la API.\n{ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private Task ShowAlert(string title, string message) =>
            Application.Current.MainPage.DisplayAlert(title, message, "OK");
    }
}