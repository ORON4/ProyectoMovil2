using ProyectoMovil2.ViewModels;

namespace ProyectoMovil2.Views;

public partial class AlumnoTareasPage : ContentPage
{
	public AlumnoTareasPage(AlumnoTareasViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}