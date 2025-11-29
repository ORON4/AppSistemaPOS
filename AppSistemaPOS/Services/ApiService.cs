using System.Net.Http.Json;
using System.Text.Json;
using AppSistemaPOS.Models;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AppSistemaPOS.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private string _token;

        private const string BaseUrl = "http://192.168.0.17:5241";

        public ApiService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(BaseUrl),
                Timeout = TimeSpan.FromSeconds(30)
            };
        }

        public async Task<Usuario?> LoginAsync(string email, string password)
        {
            try
            {
                var loginDto = new LoginDto { Email = email, Password = password };

                // Hacemos el POST al endpoint que creaste en la API
                var response = await _httpClient.PostAsJsonAsync("Usuarios/login", loginDto);

                if (response.IsSuccessStatusCode)
                {
                    // Si es 200 OK, devolvemos el usuario
                    return await response.Content.ReadFromJsonAsync<Usuario>();
                }
                else
                {
                    return null; // Credenciales incorrectas (401)
                }
            }
            catch (Exception ex)
            {
                // Aquí podrías guardar el log del error
                Console.WriteLine($"Error de conexión: {ex.Message}");
                throw new Exception("No se pudo conectar con el servidor.");
            }
        }
    }
}