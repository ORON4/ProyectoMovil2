using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ProyectoMovil2.Services;
using ProyectoMovil2.Models;


namespace ProyectoMovil2.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly ApiService _apiService;
        private string _nombreUsuario;
        private string _contraseña;
        private string _mensajeError;

        public LoginViewModel(ApiService apiService)
        {
            _apiService = apiService;
            Title = "Iniciar Sesión";

            LoginCommand = new Command(async () => await OnLoginAsync(), () => !IsBusy);
        }

        public string NombreUsuario
        {
            get => _nombreUsuario;
            set => SetProperty(ref _nombreUsuario, value);
        }

        public string Contraseña
        {
            get => _contraseña;
            set => SetProperty(ref _contraseña, value);
        }

        public string MensajeError
        {
            get => _mensajeError;
            set => SetProperty(ref _mensajeError, value);
        }

        public ICommand LoginCommand { get; }

        private async Task OnLoginAsync()
        {
            if (IsBusy)
                return;

            // Validación
            if (string.IsNullOrWhiteSpace(NombreUsuario) || string.IsNullOrWhiteSpace(Contraseña))
            {
                MensajeError = "Por favor ingresa tu usuario y contraseña";
                await Application.Current.MainPage.DisplayAlert("Error", MensajeError, "OK");
                return;
            }

            IsBusy = true;
            MensajeError = string.Empty;

            try
            {
                var result = await _apiService.LoginAsync(NombreUsuario, Contraseña);

                if (result.Success)
                {
                    // Guarda el token de forma segura
                    await SecureStorage.SetAsync("auth_token", result.Token);
                    await SecureStorage.SetAsync("username", NombreUsuario);

                    await _apiService.RestoreTokenAsync();

                    Shell.Current.FlyoutBehavior = FlyoutBehavior.Flyout;

                    // Navega a la página principal
                    await Shell.Current.GoToAsync("//MainPage");
                }
                else
                {
                    MensajeError = result.Mensaje;
                    await Application.Current.MainPage.DisplayAlert("Error", MensajeError, "OK");
                }
            }
            catch (Exception ex)
            {
                MensajeError = $"Error: {ex.Message}";
                await Application.Current.MainPage.DisplayAlert("Error", MensajeError, "OK");
            }
            finally
            {
                IsBusy = false;
                // Actualiza el estado del comando
                ((Command)LoginCommand).ChangeCanExecute();
            }

           
        }



        // Método para verificar si ya hay sesión
        public async Task<bool> CheckExistingSessionAsync()
        {
            try
            {
                var token = await SecureStorage.GetAsync("auth_token");
                return !string.IsNullOrEmpty(token);
            }
            catch
            {
                return false;
            }
        }
    }
}
