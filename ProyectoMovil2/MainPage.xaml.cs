using ProyectoMovil2.ViewModels;

namespace ProyectoMovil2
{
    public partial class MainPage : ContentPage
    {
        private readonly MainViewModel _viewModel;

        public MainPage(MainViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Inicializa el ViewModel cuando la página aparece
            await _viewModel.InitializeAsync();
        }


    }

}
