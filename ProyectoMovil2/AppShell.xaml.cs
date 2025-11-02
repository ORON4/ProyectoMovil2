using ProyectoMovil2.Services;
using ProyectoMovil2.Views;
using System.Windows.Input;

namespace ProyectoMovil2
{
    public partial class AppShell : Shell
    {
        private readonly ApiService _apiService;
        public AppShell(ApiService apiService)
        {
            InitializeComponent();

            BindingContext = this;

            _apiService = apiService;
            Task.Run(async () => await CheckLoginStateAsync());
            Task.Run(async () => await _apiService.RestoreTokenAsync());

            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(GrupoPage), typeof(GrupoPage));
            Routing.RegisterRoute(nameof(TareasPage), typeof(TareasPage));

            Routing.RegisterRoute($"{nameof(LunesPage)}/GrupoPage", typeof(GrupoPage));
            Routing.RegisterRoute("GrupoPage", typeof(GrupoPage));
            Routing.RegisterRoute("TareasPage", typeof(TareasPage));
            Routing.RegisterRoute("CrearEditarTareaPage", typeof(CrearEditarTareaPage));
        }

        // Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
        // Routing.RegisterRoute(nameof(LunesPage), typeof(LunesPage));

        private async Task CheckLoginStateAsync()
        {
            // Intenta restaurar el token
            await _apiService.RestoreTokenAsync();

            // Comprueba si el servicio AHORA cree que estamos autenticados
            if (_apiService.IsAuthenticated())
            {
                // SÍ HAY TOKEN: Habilita el menú y navega a MainPage
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    Shell.Current.FlyoutBehavior = FlyoutBehavior.Flyout;
                    Shell.Current.GoToAsync("//MainPage");
                });
            }
            else
            {
                // NO HAY TOKEN: Asegura menú deshabilitado y navega a Login
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    Shell.Current.FlyoutBehavior = FlyoutBehavior.Disabled;
                    Shell.Current.GoToAsync("//LoginPage");
                });
            }
        }


        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            var confirm = await DisplayAlert(
                 "Cerrar Sesión",
                 "¿Estás seguro que deseas cerrar sesión?",
                 "Sí", "No");

            if (confirm)
            {
                // Llama al método centralizado de Logout
                _apiService.Logout();

                // Navega al login y oculta el menú
                Shell.Current.FlyoutBehavior = FlyoutBehavior.Disabled;

                // Usamos nameof(LoginPage) para evitar errores de escritura
                await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
            }
        }
        
    }
}
