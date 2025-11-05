using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ProyectoMovil2.Services;
using ProyectoMovil2.Views;
using Microsoft.Maui;

namespace ProyectoMovil2
{
    public partial class App : Application
    {
        public App(AppShell shell, ApiService apiService)
        {
            InitializeComponent();

            // Usa la instancia de AppShell proporcionada por DI
            MainPage = shell;

            // Restaura token en segundo plano usando el ApiService inyectado
            Task.Run(async () =>
            {
                try
                {
                    if (apiService != null)
                    {
                        await apiService.RestoreTokenAsync();
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($">>> App: RestoreTokenAsync falló: {ex}");
                }
            });
        }

        protected override async void OnStart()
        {
            base.OnStart();
            System.Diagnostics.Debug.WriteLine(">>> App.OnStart ejecutado");
        }
    }
}
