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
                // CONEXIÓN REAL A LA API
                var usuario = await _apiService.LoginAsync(Email, Password);

                if (usuario != null)
                {
                    // Guardamos el usuario en una variable global o Preferences si es necesario
                    // Preferences.Set("auth_token", usuario.Token); // Si tuvieras token

                    await ShowAlert("¡Bienvenido!", $"Hola {usuario.Nombre}, sesión iniciada.");

                    // Navegación absoluta para reiniciar la pila de navegación
                    await Shell.Current.GoToAsync("/DashboardView");
                }
                else
                {
                    await ShowAlert("Acceso Denegado", "Correo o contraseña incorrectos.");
                }
            }
            catch (Exception ex)
            {
                string mensaje = $"No se pudo conectar con el servidor.\nDetalle: {ex.Message}";
                await ShowAlert("Error de Conexión", mensaje);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private Task ShowAlert(string title, string message)
        {
            return MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await Application.Current.MainPage.DisplayAlert(title, message, "OK");
            });
        }
    }
}