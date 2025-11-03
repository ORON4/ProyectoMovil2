

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
using Microsoft.Maui.Storage; 

namespace ProyectoMovil2.Services;

    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private string _token;

        private const string BaseUrl = "http://localhost:5117";

        public ApiService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(BaseUrl),
                Timeout = TimeSpan.FromSeconds(30)
            };
        }

        public async Task<LoginResponse> LoginAsync(string nombreUsuario, string contraseña)
        {
            try
            {
                var loginRequest = new LoginRequest { NombreUsuario = nombreUsuario, Contraseña = contraseña };
                var response = await _httpClient.PostAsJsonAsync("/usuario/login", loginRequest);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadFromJsonAsync<ApiLoginResponse>();
                    if (apiResponse != null && !string.IsNullOrEmpty(apiResponse.Token))
                    {
                        _token = apiResponse.Token;
                        SetAuthorizationHeader();
                        return new LoginResponse { Success = true, Token = apiResponse.Token, Mensaje = apiResponse.Mensaje };
                    }
                    else
                    {
                        return new LoginResponse { Success = false, Mensaje = "La respuesta del servidor no contiene el token" };
                    }
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                    return new LoginResponse { Success = false, Mensaje = errorResponse?.Mensaje ?? "Usuario o contraseña incorrectos" };
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return new LoginResponse { Success = false, Mensaje = $"Error: {response.StatusCode} - {errorContent}" };
                }
            }
            catch (Exception ex)
            {
                return new LoginResponse { Success = false, Mensaje = $"Error inesperado: {ex.Message}" };
            }
        }

        private void SetAuthorizationHeader()
        {
            if (!string.IsNullOrEmpty(_token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            }
        }

        public async Task<T> GetAsync<T>(string endpoint)
        {
            try
            {
                var response = await _httpClient.GetAsync(endpoint);
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    throw new UnauthorizedAccessException("Token inválido o expirado. Inicia sesión nuevamente.");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<T>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error en la petición GET: {ex.Message}", ex);
            }
        }

        public async Task<TResponse> PutAsync<TRequest, TResponse>(string endpoint, TRequest data)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync(endpoint, data);
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    throw new UnauthorizedAccessException("Token inválido o expirado.");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<TResponse>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error en PUT: {ex.Message}", ex);
            }
        }

        public async Task<TResponse> DeleteAsync<TResponse>(string endpoint)
        {
            try
            {
                var response = await _httpClient.DeleteAsync(endpoint);
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    throw new UnauthorizedAccessException("Token inválido o expirado.");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<TResponse>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error en DELETE: {ex.Message}", ex);
            }
        }

        public async Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(endpoint, data);
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    throw new UnauthorizedAccessException("Token inválido o expirado. Inicia sesión nuevamente.");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<TResponse>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error en la petición POST: {ex.Message}", ex);
            }
        }

        // --- INICIO DE LA CORRECCIÓN ---
        // Este es el Logout centralizado y correcto.
        public void Logout()
        {
            _token = null;
            _httpClient.DefaultRequestHeaders.Authorization = null;

            // Limpia el almacenamiento persistente
            if (SecureStorage.Default.Remove("auth_token"))
            {
                System.Diagnostics.Debug.WriteLine(">>> Token borrado de SecureStorage");
            }
            SecureStorage.Default.Remove("username");
        }
        // --- FIN DE LA CORRECCIÓN ---

        public bool IsAuthenticated()
        {
            return !string.IsNullOrEmpty(_token);
        }

        public async Task RestoreTokenAsync()
        {
            try
            {
                _token = await SecureStorage.GetAsync("auth_token");
                System.Diagnostics.Debug.WriteLine($">>> RestoreTokenAsync llamado. Token encontrado: {(!string.IsNullOrEmpty(_token) ? "SÍ" : "NO")}");

                if (!string.IsNullOrEmpty(_token))
                {
                    SetAuthorizationHeader();
                    System.Diagnostics.Debug.WriteLine($">>> Authorization header configurado.");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($">>> Error en RestoreTokenAsync: {ex.Message}");
                _token = null;
            }
        }
    
    }