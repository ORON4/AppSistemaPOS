namespace AppSistemaPOS
{
    public partial class App : Application
    {
        public static Models.Usuario? UsuarioActual { get; set; }
        public App(AppShell shell)
        {
            InitializeComponent();

            MainPage = shell;
        }
    }
}
