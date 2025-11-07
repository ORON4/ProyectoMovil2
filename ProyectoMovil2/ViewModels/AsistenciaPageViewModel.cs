using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ProyectoMovil2.Models;
using ProyectoMovil2.Services;

namespace ProyectoMovil2.ViewModels
{
    public class AsistenciaPageViewModel : BaseViewModel
    {
        private readonly ApiService _apiService;
        private string _mensajeError;

        public ObservableCollection<AlumnosAsistencia> Asistencias { get; }
        public ICommand CargarAsistenciasCommand { get; }

        public string MensajeError
        {
            get => _mensajeError;
            set => SetProperty(ref _mensajeError, value);
        }

        public AsistenciaPageViewModel(ApiService apiService)
        {
            Title = "Asistencias";
            _apiService = apiService;
            Asistencias = new ObservableCollection<AlumnosAsistencia>();
            CargarAsistenciasCommand = new Command(async () => await CargarAsistenciasAsync());
        }

        // Este método se llamará desde la vista cuando la página aparezca
        public async Task InitializeAsync()
        {
            await CargarAsistenciasAsync();
        }

        private async Task CargarAsistenciasAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            MensajeError = string.Empty;

            try
            {
                var asistencias = await _apiService.ObtenerAsistenciasAsync();

                Asistencias.Clear();

                if (asistencias != null && asistencias.Any())
                {
                    foreach (var asistencia in asistencias)
                    {
                        Asistencias.Add(asistencia);
                    }
                }
                else
                {
                    MensajeError = "No hay asistencias registradas";
                }
            }
            catch (UnauthorizedAccessException)
            {
                MensajeError = "Sesión expirada. Inicia sesión nuevamente.";
                await Application.Current.MainPage.DisplayAlert("Error", MensajeError, "OK");
                await Shell.Current.GoToAsync("//LoginPage");
            }
            catch (Exception ex)
            {
                MensajeError = $"Error al cargar asistencias: {ex.Message}";
                await Application.Current.MainPage.DisplayAlert("Error", MensajeError, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}