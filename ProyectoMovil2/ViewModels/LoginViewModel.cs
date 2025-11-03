using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using ProyectoMovil2.Models;
using ProyectoMovil2.Services;

namespace ProyectoMovil2.ViewModels;

    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly ApiService _apiService;
        private string _nombreUsuario;
        private string _contraseña;
        private string _mensajeError;
        private bool _isBusy;

        public event PropertyChangedEventHandler PropertyChanged;

        public string NombreUsuario
        {
            get => _nombreUsuario;
            set { _nombreUsuario = value; OnPropertyChanged(); }
        }

        public string Contraseña
        {
            get => _contraseña;
            set { _contraseña = value; OnPropertyChanged(); }
        }

        public string MensajeError
        {
            get => _mensajeError;
            set { _mensajeError = value; OnPropertyChanged(); }
        }

        public bool IsBusy
        {
            get => _isBusy;
            set { _isBusy = value; OnPropertyChanged(); }
        }

        public ICommand LoginCommand { get; }

        public LoginViewModel(ApiService apiService)
        {
            _apiService = apiService;
            LoginCommand = new Command(async () => await OnLoginAsync());
        }

        private async Task OnLoginAsync()
        {
            if (IsBusy)
                return;

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
                    await SecureStorage.SetAsync("auth_token", result.Token);
                    await SecureStorage.SetAsync("username", NombreUsuario);

                    // ¡¡ESTA LÍNEA ES LA QUE FALTABA!!
                    // Carga el token que acabamos de recibir en el ApiService.
                    await _apiService.RestoreTokenAsync();
                    

                    Shell.Current.FlyoutBehavior = FlyoutBehavior.Flyout;
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
            }
        }


        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
