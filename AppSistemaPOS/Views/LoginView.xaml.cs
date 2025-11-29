using AppSistemaPOS.ViewModels;

namespace AppSistemaPOS.Views;

public partial class LoginView : ContentPage
{
	public LoginView(LoginViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
    }
}