using ProyectoMovil2.Models;
using ProyectoMovil2.Services;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Windows.Input; // Necesario para ICommand

namespace ProyectoMovil2.ViewModels
{
   
    public class EscanerQRpageViewModel : BaseViewModel
    {
        private readonly ApiService _apiService;

        private bool _canScan;
        private string _resultadoDelScan;

        // Constructor
        public EscanerQRpageViewModel(ApiService apiService)
        {
            _apiService = apiService;
            Title = "Escanear QR";

            // Inicializamos propiedades
            CanScan = true;
            ResultadoDelScan = "Apunte la cámara al código QR...";

            // Inicializamos el comando manualmente
            DetectarCodigoCommand = new Command<string>(async (qr) => await DetectarCodigoAsync(qr));
        }

        // Propiedades con estilo clásico (usando SetProperty de tu BaseViewModel)
        public bool CanScan
        {
            get => _canScan;
            set => SetProperty(ref _canScan, value);
        }

        public string ResultadoDelScan
        {
            get => _resultadoDelScan;
            set => SetProperty(ref _resultadoDelScan, value);
        }

        // Comando público definido manualmente
        public ICommand DetectarCodigoCommand { get; }

        // Lógica del escaneo (sin atributos [RelayCommand])
        private async Task DetectarCodigoAsync(string codigoQR)
        {
            if (IsBusy || !CanScan) return;

            IsBusy = true;
            CanScan = false;

            try
            {
                if (string.IsNullOrWhiteSpace(codigoQR))
                    return;

                JsonNode qrData = JsonNode.Parse(codigoQR);

                if (qrData != null && qrData["tipo"]?.ToString() == "alumno" && qrData["id"] != null)
                {
                    int idAlumno = qrData["id"].GetValue<int>();
                    ResultadoDelScan = $"Procesando ID: {idAlumno}...";

                    try { Vibration.Vibrate(); } catch { }

                    Alumno alumno = await _apiService.ObtenerAlumnoPorIdAsync(idAlumno);

                    if (alumno != null)
                    {
                        await MainThread.InvokeOnMainThreadAsync(async () =>
                        {
                            await Shell.Current.GoToAsync($"AlumnoTareasPage?alumnoId={alumno.Id}&nombre={alumno.NombreAlumno}");
                        });
                    }
                    else
                    {
                        ResultadoDelScan = $"Error: No se encontró alumno con ID {idAlumno}";
                        await Task.Delay(2000);
                    }
                }
                else
                {
                    ResultadoDelScan = "Código QR no válido para alumnos.";
                    await Task.Delay(2000);
                }
            }
            catch (Exception ex)
            {
                ResultadoDelScan = $"Error: {ex.Message}";
                await Task.Delay(2000);
            }
            finally
            {
                IsBusy = false;
                CanScan = true;
                ResultadoDelScan = "Apunte la cámara al código QR...";
            }
        }
    }
}