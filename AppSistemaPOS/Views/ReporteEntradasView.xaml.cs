using AppSistemaPOS.ViewModels;

namespace AppSistemaPOS.Views;

public partial class ReporteEntradasView : ContentPage
{
	public ReporteEntradasView(ReporteEntradasViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is ReporteEntradasViewModel vm)
        {
            vm.CargarDatos();
        }
    }
}
