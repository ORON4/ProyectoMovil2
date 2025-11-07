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

    private static string NormalizeEndpoint(string endpoint)
    {
        if (string.IsNullOrWhiteSpace(endpoint))
            return "/";

        // Elimina caracteres invisibles (ej. U+200B) y espacios
        var cleaned = endpoint.Replace("\u200B", string.Empty).Trim();

        // Asegura que empiece por '/'
        if (!cleaned.StartsWith("/"))
            cleaned = "/" + cleaned;

        return cleaned;
    }



    public async Task<LoginResponse> LoginAsync(string nombreUsuario, string contraseña)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($">>> LoginAsync: Intentando login para {nombreUsuario}");

            var loginRequest = new LoginRequest { NombreUsuario = nombreUsuario, Contraseña = contraseña };
            var response = await _httpClient.PostAsJsonAsync("/usuario/login", loginRequest);

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiLoginResponse>();
                if (apiResponse != null && !string.IsNullOrEmpty(apiResponse.Token))
                {
                    System.Diagnostics.Debug.WriteLine($">>> LoginAsync: Token recibido (primeros 20 chars): {apiResponse.Token.Substring(0, Math.Min(20, apiResponse.Token.Length))}...");

                    // ⭐ Configurar el token inmediatamente
                    _token = apiResponse.Token;
                    SetAuthorizationHeader();

                    System.Diagnostics.Debug.WriteLine($">>> LoginAsync: Token configurado. IsAuthenticated: {IsAuthenticated()}");

                    return new LoginResponse { Success = true, Token = apiResponse.Token, Mensaje = apiResponse.Mensaje };
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine(">>> LoginAsync: Respuesta sin token");
                    return new LoginResponse { Success = false, Mensaje = "La respuesta del servidor no contiene el token" };
                }
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                System.Diagnostics.Debug.WriteLine($">>> LoginAsync: Unauthorized - {errorResponse?.Mensaje}");
                return new LoginResponse { Success = false, Mensaje = errorResponse?.Mensaje ?? "Usuario o contraseña incorrectos" };
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($">>> LoginAsync: Error {response.StatusCode} - {errorContent}");
                return new LoginResponse { Success = false, Mensaje = $"Error: {response.StatusCode} - {errorContent}" };
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($">>> LoginAsync: Excepción - {ex}");
            return new LoginResponse { Success = false, Mensaje = $"Error inesperado: {ex.Message}" };
        }
    }

    private void SetAuthorizationHeader()
    {
        if (!string.IsNullOrEmpty(_token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            System.Diagnostics.Debug.WriteLine($">>> SetAuthorizationHeader: Header configurado con token");
        }
        else
        {
            System.Diagnostics.Debug.WriteLine($">>> SetAuthorizationHeader: Token vacío, no se configuró header");
        }
    }

    public async Task<T> GetAsync<T>(string endpoint)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($">>> GetAsync: Llamando a {endpoint}");
            System.Diagnostics.Debug.WriteLine($">>> GetAsync: Token presente: {!string.IsNullOrEmpty(_token)}");
            System.Diagnostics.Debug.WriteLine($">>> GetAsync: Auth header: {_httpClient.DefaultRequestHeaders.Authorization?.ToString() ?? "NULL"}");

            var response = await _httpClient.GetAsync(endpoint);

            System.Diagnostics.Debug.WriteLine($">>> GetAsync: Status code: {response.StatusCode}");

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                System.Diagnostics.Debug.WriteLine(">>> GetAsync: 401 Unauthorized recibido");
                throw new UnauthorizedAccessException("Token inválido o expirado. Inicia sesión nuevamente.");
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($">>> GetAsync: Error - {ex}");
            throw new Exception($"Error en la petición GET: {ex.Message}", ex);
        }
    }

    public async Task<TResponse> PutAsync<TRequest, TResponse>(string endpoint, TRequest data)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($">>> PutAsync: Llamando a {endpoint}");
            System.Diagnostics.Debug.WriteLine($">>> PutAsync: Auth header: {_httpClient.DefaultRequestHeaders.Authorization?.ToString() ?? "NULL"}");

            var response = await _httpClient.PutAsJsonAsync(endpoint, data);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                System.Diagnostics.Debug.WriteLine(">>> PutAsync: 401 Unauthorized recibido");
                throw new UnauthorizedAccessException("Token inválido o expirado.");
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TResponse>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($">>> PutAsync: Error - {ex}");
            throw new Exception($"Error en PUT: {ex.Message}", ex);
        }
    }

    public async Task<TResponse> DeleteAsync<TResponse>(string endpoint)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($">>> DeleteAsync: Llamando a {endpoint}");
            System.Diagnostics.Debug.WriteLine($">>> DeleteAsync: Auth header: {_httpClient.DefaultRequestHeaders.Authorization?.ToString() ?? "NULL"}");

            var response = await _httpClient.DeleteAsync(endpoint);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                System.Diagnostics.Debug.WriteLine(">>> DeleteAsync: 401 Unauthorized recibido");
                throw new UnauthorizedAccessException("Token inválido o expirado.");
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TResponse>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($">>> DeleteAsync: Error - {ex}");
            throw new Exception($"Error en DELETE: {ex.Message}", ex);
        }
    }

    public async Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($">>> PostAsync: Llamando a {endpoint}");
            System.Diagnostics.Debug.WriteLine($">>> PostAsync: Auth header: {_httpClient.DefaultRequestHeaders.Authorization?.ToString() ?? "NULL"}");

            var response = await _httpClient.PostAsJsonAsync(endpoint, data);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                System.Diagnostics.Debug.WriteLine(">>> PostAsync: 401 Unauthorized recibido");
                throw new UnauthorizedAccessException("Token inválido o expirado. Inicia sesión nuevamente.");
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TResponse>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($">>> PostAsync: Error - {ex}");
            throw new Exception($"Error en la petición POST: {ex.Message}", ex);
        }
    }

    public void Logout()
    {
        System.Diagnostics.Debug.WriteLine(">>> Logout: Limpiando token y headers");

        _token = null;
        _httpClient.DefaultRequestHeaders.Authorization = null;

        // Limpia el token guardado en el dispositivo
        if (SecureStorage.Default.Remove("auth_token"))
        {
            System.Diagnostics.Debug.WriteLine(">>> Logout: Token borrado de SecureStorage");
        }
        SecureStorage.Default.Remove("username");

        System.Diagnostics.Debug.WriteLine($">>> Logout: IsAuthenticated: {IsAuthenticated()}");
    }

    public bool IsAuthenticated()
    {
        var isAuth = !string.IsNullOrEmpty(_token);
        System.Diagnostics.Debug.WriteLine($">>> IsAuthenticated: {isAuth} (Token presente: {!string.IsNullOrEmpty(_token)})");
        return isAuth;
    }

    public async Task RestoreTokenAsync()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine(">>> RestoreTokenAsync: Iniciando...");

            _token = await SecureStorage.GetAsync("auth_token");

            System.Diagnostics.Debug.WriteLine($">>> RestoreTokenAsync: Token encontrado: {(!string.IsNullOrEmpty(_token) ? "SÍ" : "NO")}");

            if (!string.IsNullOrEmpty(_token))
            {
                System.Diagnostics.Debug.WriteLine($">>> RestoreTokenAsync: Token (primeros 20 chars): {_token.Substring(0, Math.Min(20, _token.Length))}...");
                SetAuthorizationHeader();
                System.Diagnostics.Debug.WriteLine($">>> RestoreTokenAsync: Authorization header configurado. IsAuthenticated: {IsAuthenticated()}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine(">>> RestoreTokenAsync: No se encontró token en SecureStorage");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($">>> RestoreTokenAsync: Error - {ex.Message}");
            _token = null;
        }
    }

    public async Task<List<AlumnosAsistencia>> ObtenerAsistenciasAsync()
    {
        try
        {
            return await GetAsync<List<AlumnosAsistencia>>("AlumnosAsistencia");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error en ObtenerAsistenciasAsync: {ex.Message}");
            throw;
        }
    }
}