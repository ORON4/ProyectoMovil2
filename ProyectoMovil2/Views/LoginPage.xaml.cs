using ProyectoMovil2.ViewModels;

namespace ProyectoMovil2.Views;

public partial class LoginPage : ContentPage
{
    private readonly LoginViewModel _viewModel;
    public LoginPage(LoginViewModel viewModel)

         
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Verifica si ya hay una sesión activa
        var hasSession = await _viewModel.CheckExistingSessionAsync();
        if (hasSession)
        {
            await Shell.Current.GoToAsync("//MainPage");
        }
    }
}