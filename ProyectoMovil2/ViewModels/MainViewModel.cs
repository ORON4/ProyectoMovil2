using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ProyectoMovil2.Services;

namespace ProyectoMovil2.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly ApiService _apiService;
        private string _nombreUsuario;

        public MainViewModel(ApiService apiService)
        {
            _apiService = apiService;
            Title = "Inicio";

            LogoutCommand = new Command(async () => await OnLogoutAsync());
            LoadDataCommand = new Command(async () => await LoadDataAsync());
        }

        public string NombreUsuario
        {
            get => _nombreUsuario;
            set => SetProperty(ref _nombreUsuario, value);
        }

        public ICommand LogoutCommand { get; }
        public ICommand LoadDataCommand { get; }

        // Se ejecuta cuando la página aparece
        public async Task InitializeAsync()
        {
            try
            {
                // Obtiene el nombre de usuario guardado
                NombreUsuario = await SecureStorage.GetAsync("username") ?? "Usuario";

                // Carga datos iniciales
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error",
                    $"Error al inicializar: {ex.Message}", "OK");
            }
        }

        private async Task LoadDataAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                // Aquí puedes hacer peticiones a tu API usando _apiService
                // Ejemplo: var datos = await _apiService.GetAsync<List<TuModelo>>("/api/datos");

                await Task.Delay(1000); // Simulación
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error",
                    $"Error al cargar datos: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task OnLogoutAsync()
        {
            var confirm = await Application.Current.MainPage.DisplayAlert(
                "Cerrar Sesión",
                "¿Estás seguro que deseas cerrar sesión?",
                "Sí", "No");

            if (!confirm)
                return;

            try
            {
                // Limpia el token del servicio
                _apiService.Logout();

                // Limpia el almacenamiento seguro
                SecureStorage.Remove("auth_token");
                SecureStorage.Remove("username");

                // Navega de vuelta al login
                await Shell.Current.GoToAsync("//LoginPage");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error",
                    $"Error al cerrar sesión: {ex.Message}", "OK");
            }
        }
    }
}
