using ProyectoMovil2.ViewModels;

namespace ProyectoMovil2.Views;
public partial class GestionTareasPage : ContentPage
{
    private readonly GestionTareasViewModel _viewModel;
    public GestionTareasPage(GestionTareasViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.InitializeAsync(); // Recargar lista al volver
    }
}