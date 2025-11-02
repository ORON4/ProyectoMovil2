using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using ProyectoMovil2.Models;
using System.Net.Http;



namespace ProyectoMovil2.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private string _token;

        // Configura aquí tu URL base según la plataforma
        private const string BaseUrl = "http://localhost:5117"; 
                                                               

        public ApiService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(BaseUrl),
                Timeout = TimeSpan.FromSeconds(30)
            };
        }

        // Método para login - adaptado a tu endpoint
        public async Task<LoginResponse> LoginAsync(string nombreUsuario, string contraseña)
        {
            try
            {
                var loginRequest = new LoginRequest
                {
                    NombreUsuario = nombreUsuario,
                    Contraseña = contraseña
                };

                // Envía la petición POST al endpoint /usuario/login
                var response = await _httpClient.PostAsJsonAsync("/usuario/login", loginRequest);

                if (response.IsSuccessStatusCode)
                {
                    // Tu API devuelve { mensaje: "...", token: "..." }
                    var apiResponse = await response.Content.ReadFromJsonAsync<ApiLoginResponse>();

                    if (apiResponse != null && !string.IsNullOrEmpty(apiResponse.Token))
                    {
                        _token = apiResponse.Token;
                        SetAuthorizationHeader();

                        return new LoginResponse
                        {
                            Success = true,
                            Token = apiResponse.Token,
                            Mensaje = apiResponse.Mensaje
                        };
                    }
                    else
                    {
                        return new LoginResponse
                        {
                            Success = false,
                            Mensaje = "La respuesta del servidor no contiene el token"
                        };
                    }
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    // Maneja el error 401 Unauthorized
                    var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                    return new LoginResponse
                    {
                        Success = false,
                        Mensaje = errorResponse?.Mensaje ?? "Usuario o contraseña incorrectos"
                    };
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return new LoginResponse
                    {
                        Success = false,
                        Mensaje = $"Error: {response.StatusCode} - {errorContent}"
                    };
                }
            }
            catch (HttpRequestException ex)
            {
                return new LoginResponse
                {
                    Success = false,
                    Mensaje = $"Error de conexión: {ex.Message}. Verifica que la API esté corriendo y la URL sea correcta."
                };
            }
            catch (JsonException ex)
            {
                return new LoginResponse
                {
                    Success = false,
                    Mensaje = $"Error al procesar la respuesta: {ex.Message}"
                };
            }
            catch (Exception ex)
            {
                return new LoginResponse
                {
                    Success = false,
                    Mensaje = $"Error inesperado: {ex.Message}"
                };
            }
        }

        // Configura el token en el header para peticiones futuras
        private void SetAuthorizationHeader()
        {
            if (!string.IsNullOrEmpty(_token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _token);
            }
        }



        // Método GET
        public async Task<T> GetAsync<T>(string endpoint)
        {
            try
            {
                var response = await _httpClient.GetAsync(endpoint);

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedAccessException("Token inválido o expirado. Inicia sesión nuevamente.");
                }

                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<T>();
            }
            catch (UnauthorizedAccessException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error en la petición GET: {ex.Message}");
            }
        }
        //metodo put
        public async Task<TResponse> PutAsync<TRequest, TResponse>(string endpoint, TRequest data)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync(endpoint, data);

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedAccessException("Token inválido o expirado.");
                }

                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<TResponse>();
            }
            catch (UnauthorizedAccessException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error en PUT: {ex.Message}");
            }
        }

        //metodo delete
        public async Task<TResponse> DeleteAsync<TResponse>(string endpoint)
        {
            try
            {
                var response = await _httpClient.DeleteAsync(endpoint);

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedAccessException("Token inválido o expirado.");
                }

                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<TResponse>();
            }
            catch (UnauthorizedAccessException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error en DELETE: {ex.Message}");
            }
        }

        // Método POST autenticado
        public async Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(endpoint, data);

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedAccessException("Token inválido o expirado. Inicia sesión nuevamente.");
                }

                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<TResponse>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error en la petición POST: {ex.Message}");
            }
        }

        // Limpia el token (para logout)
        public void Logout()
        {
            _token = null;
            _httpClient.DefaultRequestHeaders.Authorization = null;

            if (SecureStorage.Default.Remove("auth_token"))
            {
                System.Diagnostics.Debug.WriteLine(">>> Token borrado de SecureStorage");
            }
            SecureStorage.Default.Remove("username");
        }

            // Verifica si hay un token activo
            public bool IsAuthenticated()
            {
                return !string.IsNullOrEmpty(_token);
            }

        public async Task RestoreTokenAsync()
        {
            try
            {
                _token = await SecureStorage.GetAsync("auth_token");

                System.Diagnostics.Debug.WriteLine($">>> RestoreTokenAsync llamado");
                System.Diagnostics.Debug.WriteLine($">>> Token recuperado: {(!string.IsNullOrEmpty(_token) ? "SÍ" : "NO")}");

                if (!string.IsNullOrEmpty(_token))
                {
                    System.Diagnostics.Debug.WriteLine($">>> Token (primeros 30 chars): {_token.Substring(0, Math.Min(30, _token.Length))}...");
                    SetAuthorizationHeader();
                    System.Diagnostics.Debug.WriteLine($">>> Authorization header configurado: {_httpClient.DefaultRequestHeaders.Authorization}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine(">>> No hay token guardado");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($">>> Error en RestoreTokenAsync: {ex.Message}");
                _token = null;
            }
        }
    }
}
