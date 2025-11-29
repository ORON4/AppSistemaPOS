using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AppSistemaPOS.Models;

namespace AppSistemaPOS.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        // Propiedades enlazadas a la vista (Entry de email y password)
        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private bool isBusy; // mostrar un indicador de carga

        [RelayCommand]
        private async Task Login()
        {
            if (IsBusy) return; // Evitar doble clic

            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Ingrese correo y contraseña", "OK");
                return;
            }

            IsBusy = true;

            try
            {
                // AQUÍ LLAMAREMOS A LA API MÁS ADELANTE
                // Simulamos una espera de 1 segundo por ahora
                await Task.Delay(1000);

                // TODO: Validar credenciales reales con tu ApiService
                // var usuario = await _apiService.Login(Email, Password);

                // Si es exitoso, navegamos al Dashboard (lo crearemos después)
                await Shell.Current.DisplayAlert("Éxito", $"Bienvenido {Email}", "Entrar");

                // await Shell.Current.GoToAsync("//Dashboard"); 
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}