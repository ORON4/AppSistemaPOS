using AppSistemaPOS.ViewModels;

namespace AppSistemaPOS.Views;

public partial class InventarioView : ContentPage
{
	public InventarioView(InventarioViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
	}
}
