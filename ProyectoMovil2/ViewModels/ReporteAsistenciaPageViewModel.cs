using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ProyectoMovil2.Models;
using ProyectoMovil2.Services;

namespace ProyectoMovil2.ViewModels
{
    public class ReporteAsistenciaPageViewModel : BaseViewModel
    {
        private readonly ApiService _apiService;
        private string _mensajeError;

        public ObservableCollection<AsistenciaReporte> Asistencias { get; }
        public ICommand CargarReporteCommand { get; }

        private DateTime _fechaSeleccionada;
        public DateTime FechaSeleccionada
        {
            get => _fechaSeleccionada;
            // Cuando la fecha cambia, recarga el reporte
            set => SetProperty(ref _fechaSeleccionada, value, onChanged: async () => await CargarReporteAsync());
        }

        public string MensajeError
        {
            get => _mensajeError;
            set => SetProperty(ref _mensajeError, value);
        }

        public ReporteAsistenciaPageViewModel(ApiService apiService)
        {
            _apiService = apiService;
            Title = "Reporte de Asistencia";
            Asistencias = new ObservableCollection<AsistenciaReporte>();
            _fechaSeleccionada = DateTime.Today;
            CargarReporteCommand = new Command(async () => await CargarReporteAsync());
        }

        public async Task CargarReporteAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            MensajeError = string.Empty;

            try
            {
                Asistencias.Clear();
                var reporte = await _apiService.ObtenerAsistenciaPorFechaAsync(FechaSeleccionada);

                if (reporte != null && reporte.Any())
                {
                    foreach (var item in reporte)
                    {
                        Asistencias.Add(item);
                    }
                }
                else
                {
                    MensajeError = "No hay asistencias para esta fecha.";
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al cargar reporte: {ex.Message}");
                MensajeError = "Error al cargar el reporte.";
                // Si la API devuelve 404 (Not Found), también se captura aquí
                if (ex.Message.Contains("404"))
                {
                    MensajeError = "No hay asistencias para esta fecha.";
                }
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
