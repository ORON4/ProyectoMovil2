using ProyectoMovil2.Models;
using ProyectoMovil2.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;

namespace ProyectoMovil2.ViewModels
{
    public class AsistenciaPageViewModel : BaseViewModel
    {
        private readonly ApiService _apiService;

        // La lista de alumnos que se muestra
        public ObservableCollection<Alumno> Alumnos { get; }

        // La propiedad enlazada al DatePicker
        private DateTime _fechaSeleccionada;
        public DateTime FechaSeleccionada
        {
            get => _fechaSeleccionada;
            // Cuando la fecha cambia, recargamos la lista de alumnos
            set => SetProperty(ref _fechaSeleccionada, value, onChanged: async () => await CargarAlumnosAsync());
        }

        public ICommand CargarAlumnosCommand { get; }

        // --- INICIO DE LA CORRECCIÓN ---
        // 1. Declarar dos comandos separados
        public ICommand MarcarPresenteCommand { get; }
        public ICommand MarcarAusenteCommand { get; }
        // --- FIN DE LA CORRECCIÓN ---

        public AsistenciaPageViewModel(ApiService apiService)
        {
            _apiService = apiService;
            Title = "Tomar Asistencia";

            Alumnos = new ObservableCollection<Alumno>();
            _fechaSeleccionada = DateTime.Today;

            CargarAlumnosCommand = new Command(async () => await CargarAlumnosAsync());

            // --- INICIO DE LA CORRECCIÓN ---
            // 2. Inicializar ambos comandos. 
            //    El CommandParameter (Alumno) se pasa con <Alumno>
            MarcarPresenteCommand = new Command<Alumno>(async (alumno) =>
            {
                if (alumno == null) return;
                await MarcarAsistenciaAsync(alumno, true); // Marcar como Presente (true)
            });

            MarcarAusenteCommand = new Command<Alumno>(async (alumno) =>
            {
                if (alumno == null) return;
                await MarcarAsistenciaAsync(alumno, false); // Marcar como Ausente (false)
            });
            // --- FIN DE LA CORRECCIÓN ---
        }

        public async Task CargarAlumnosAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                Alumnos.Clear();

                // OJO: Asumimos '1' como IdGrupo.
                var listaAlumnos = await _apiService.ObtenerAlumnosPorGrupoAsync(301);

                if (listaAlumnos != null)
                {
                    foreach (var alumno in listaAlumnos)
                    {
                        Alumnos.Add(alumno);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al cargar alumnos: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error", "No se pudo cargar la lista de alumnos.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        // 3. Este método (el que guarda) no necesita cambios.
        //    Recibe el alumno y el booleano (true/false) de los nuevos comandos.
        private async Task MarcarAsistenciaAsync(Alumno alumno, bool estaPresente)
        {
            if (alumno == null) return;

            try
            {
                var nuevaAsistencia = new AlumnosAsistencia
                {
                    IdAlumno = alumno.Id,
                    Fecha = FechaSeleccionada,
                    Asistencia = estaPresente
                };

                await _apiService.GuardarAsistenciaAsync(nuevaAsistencia);

                Debug.WriteLine($"Asistencia guardada para {alumno.NombreAlumno}: {(estaPresente ? "Presente" : "Ausente")}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al guardar asistencia: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("", "Asistencia Guardada con exito.", "OK");
            }
        }
    }
}