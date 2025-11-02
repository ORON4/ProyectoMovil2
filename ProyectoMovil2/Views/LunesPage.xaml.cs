using ProyectoMovil2.ViewModels;
using ProyectoMovil2.Services;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace ProyectoMovil2.Views;

public partial class LunesPage : ContentPage
{
	private readonly LunesViewModel _viewModel;
	public LunesPage(LunesViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
		
    }
}