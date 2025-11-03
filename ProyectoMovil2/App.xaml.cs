using ProyectoMovil2.Services;
using ProyectoMovil2.Views;


namespace ProyectoMovil2

{
    public partial class App : Application
    {
        public App(AppShell shell)
        {
            InitializeComponent();
           MainPage = shell;
        }

        protected override async void OnStart()
        {
            base.OnStart();
            System.Diagnostics.Debug.WriteLine(">>> App.OnStart ejecutado");

        }
    }
}
