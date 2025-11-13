using System.Collections.ObjectModel;
using System.Windows.Input;
using ProyectoMovil2.Models;
using ProyectoMovil2.Services;
using ProyectoMovil2.Views; // Asegúrate de tener este namespace

namespace ProyectoMovil2.ViewModels
{
    public class TareasViewModel : BaseViewModel
    {
        private readonly ApiService _apiService;
        private bool _isRefreshing;
        private string _mensajeError;
        private Alumno _selectedAlumno;

        public TareasViewModel(ApiService apiService)
        {
            _apiService = apiService;
            Title = "Seleccionar Alumno"; // Cambiamos el título

            Alumnos = new ObservableCollection<Alumno>();

            // Comandos
            CargarAlumnosCommand = new Command(async () => await CargarAlumnosAsync());
            RefreshCommand = new Command(async () => await RefreshAsync());

            // Este comando sigue llevando a la pantalla de crear tarea GLOBAL
            CrearTareaCommand = new Command(async () => await CrearTareaAsync());

            // Este comando navega al detalle del alumno seleccionado
            IrAAlumnoCommand = new Command<Alumno>(async (a) => await IrAAlumnoAsync(a));
        }

        public ObservableCollection<Alumno> Alumnos { get; }

        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }

        public string MensajeError
        {
            get => _mensajeError;
            set => SetProperty(ref _mensajeError, value);
        }

        public ICommand CargarAlumnosCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand CrearTareaCommand { get; }
        public ICommand IrAAlumnoCommand { get; }

        public async Task InitializeAsync()
        {
            await CargarAlumnosAsync();
        }

        private async Task CargarAlumnosAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            MensajeError = string.Empty;

            try
            {
                // Aquí asumimos que cargamos el Grupo 1 por defecto, o podrías hacerlo dinámico
                // Ajusta la ruta si tu API usa otra
                var lista = await _apiService.GetAsync<List<Alumno>>("alumnos/grupo/301");

                Alumnos.Clear();
                if (lista != null)
                {
                    foreach (var alumno in lista)
                    {
                        Alumnos.Add(alumno);
                    }
                }
                else
                {
                    MensajeError = "No se encontraron alumnos.";
                }
            }
            catch (Exception ex)
            {
                MensajeError = $"Error al cargar alumnos: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task RefreshAsync()
        {
            IsRefreshing = true;
            await CargarAlumnosAsync();
            IsRefreshing = false;
        }

        private async Task CrearTareaAsync()
        {
            // Navega a la pantalla de creación (Global)
            await Shell.Current.GoToAsync("CrearEditarTareaPage");
        }

        private async Task IrAAlumnoAsync(Alumno alumno)
        {
            if (alumno == null) return;

            // Navegamos a la NUEVA página de tareas del alumno
            // Pasamos el ID del alumno como parámetro
            await Shell.Current.GoToAsync($"AlumnoTareasPage?alumnoId={alumno.Id}&nombre={alumno.NombreAlumno}");
        }
    }
}