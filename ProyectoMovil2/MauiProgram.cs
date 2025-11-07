using Microsoft.Extensions.Logging;
using ProyectoMovil2.Services;
using ProyectoMovil2.ViewModels;
using ProyectoMovil2.Views;

namespace ProyectoMovil2
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            // Registra los servicios (Singleton = una sola instancia para toda la app)
            builder.Services.AddSingleton<ApiService>();
            builder.Services.AddSingleton<AppShell>();


            // Registra ViewModels (Transient = nueva instancia cada vez)
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<MainViewModel>();
            builder.Services.AddTransient<LunesViewModel>();
            builder.Services.AddTransient<GrupoViewModel>();
            builder.Services.AddTransient<TareasViewModel>();
            builder.Services.AddTransient<CrearEditarTareaViewModel>();
            builder.Services.AddTransient <AsistenciaPageViewModel>();
            builder.Services.AddTransient<ReporteAsistenciaPageViewModel>();

            // Registra Views (Transient)
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<LunesPage>();
            builder.Services.AddTransient<GrupoPage>();
            builder.Services.AddTransient<TareasPage>();
            builder.Services.AddTransient<CrearEditarTareaPage>();
            builder.Services.AddTransient<AsistenciaPage>();
            builder.Services.AddTransient<ReporteAsistenciaPage>();

            return builder.Build();
        }
    }
}
