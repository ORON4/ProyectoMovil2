using ProyectoMovil2.Models;
using ProyectoMovil2.Services;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ProyectoMovil2.ViewModels
{
    public partial class EscanerQRpageViewModel : BaseViewModel
    {
        private readonly ApiService _apiService;

        private bool _CanScan;
        public bool CanScan
        {
            get => _CanScan;
            set => SetProperty(ref _CanScan, value);
        }


        // Campo privado para el texto del escaneo
        private string _resultadoDelScan = "Apunte la cámara al código QR...";

        // Propiedad pública implementada manualmente (no depende del source generator)
        public string ResultadoDelScan
        {
            get => _resultadoDelScan;
            set => SetProperty(ref _resultadoDelScan, value);
        }

        public EscanerQRpageViewModel(ApiService apiService)
        {
            _apiService = apiService;
            Title = "Escanear QR";
            _CanScan = true;
        }

        [RelayCommand]
        private async Task DetectarCodigo(string codigoQR)
        {
            if (IsBusy || !CanScan) return;
            IsBusy = true;
            CanScan = false;
            try
            {
                if (string.IsNullOrWhiteSpace(codigoQR))
                {
                    ResultadoDelScan = "Error: QR vacío.";
                    return;
                }

                JsonNode qrData = JsonNode.Parse(codigoQR);

                if (qrData["tipo"]?.ToString() == "alumno" && qrData["id"] != null)
                {
                    int idAlumno = qrData["id"].GetValue<int>();

                    ResultadoDelScan = $"Buscando ID: {idAlumno}...";
                    Alumno alumno = await _apiService.ObtenerAlumnoPorIdAsync(idAlumno);

                    if (alumno != null)
                    {
                        ResultadoDelScan = $"Alumno: {alumno.NombreAlumno}\nGrupo: {alumno.Grupo}";
                    }
                    else
                    {
                        ResultadoDelScan = $"Error: Alumno con ID {idAlumno} no encontrado.";
                    }
                }
                else
                {
                    ResultadoDelScan = "Error: QR no válido.";
                }
            }
            catch (JsonException ex)
            {
                ResultadoDelScan = $"Error: El QR no es un JSON válido. [{ex.Message}]";
                Debug.WriteLine(ex);
            }
            catch (Exception ex)
            {
                ResultadoDelScan = $"Error: {ex.Message}";
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
                CanScan = true;
                await Task.Delay(3000);
                ResultadoDelScan = "Apunte la cámara al código QR...";
            }
        }
    }
}