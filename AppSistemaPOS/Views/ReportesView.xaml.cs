using AppSistemaPOS.ViewModels;

namespace AppSistemaPOS.Views;

public partial class ReportesView : ContentPage
{
	public ReportesView(ReportesViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is ReportesViewModel vm)
        {
            vm.CargarReporte();
        }
    }
}
