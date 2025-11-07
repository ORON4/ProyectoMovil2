using System;
using System.Threading.Tasks;
using ProyectoMovil2.Services;
using ProyectoMovil2.Views;
using Microsoft.Maui.ApplicationModel; // Required for MainThread

namespace ProyectoMovil2
{
    public partial class AppShell : Shell
    {
        private readonly ApiService _apiService;

        public AppShell(ApiService apiService)
        {
            InitializeComponent();
            _apiService = apiService;

            // Esta es la lógica de arranque correcta.
            Task.Run(async () => await CheckLoginStateAsync());

            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(GrupoPage), typeof(GrupoPage));
            Routing.RegisterRoute(nameof(TareasPage), typeof(TareasPage));
            Routing.RegisterRoute($"{nameof(LunesPage)}/GrupoPage", typeof(GrupoPage));
            Routing.RegisterRoute("GrupoPage", typeof(GrupoPage));
            Routing.RegisterRoute("TareasPage", typeof(TareasPage));
            Routing.RegisterRoute("CrearEditarTareaPage", typeof(CrearEditarTareaPage));
            Routing.RegisterRoute("AsistenciaPage", typeof(AsistenciaPage));
            Routing.RegisterRoute(nameof(ReporteAsistenciaPage), typeof(ReporteAsistenciaPage));
        }

        // Esta es la lógica que debe decidir a dónde ir al arrancar.
        private async Task CheckLoginStateAsync()
        {
            // 1. Intenta restaurar el token
            await _apiService.RestoreTokenAsync();

            // 2. Comprueba si el servicio AHORA cree que estamos autenticados
            if (_apiService.IsAuthenticated())
            {
                // 3. SÍ HAY TOKEN: Habilita el menú y navega a MainPage
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    Shell.Current.FlyoutBehavior = FlyoutBehavior.Flyout;
                    await Shell.Current.GoToAsync("//MainPage");
                });
            }
            else
            {
                // 4. NO HAY TOKEN: Asegura menú deshabilitado y navega a Login
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    Shell.Current.FlyoutBehavior = FlyoutBehavior.Disabled;
                    await Shell.Current.GoToAsync("//LoginPage");
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
                // Llama al método Logout() centralizado en ApiService.
                _apiService.Logout();

                // Navega al login y oculta el menú
                Shell.Current.FlyoutBehavior = FlyoutBehavior.Disabled;
                await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
            }
        }
    }
}
