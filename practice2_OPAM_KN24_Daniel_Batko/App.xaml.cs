using Microsoft.UI.Xaml;

namespace practice2_OPAM_KN24_Daniel_Batko;

public partial class App : Application
{
    public static Window MainWindow { get; set; } = new MainWindow();

    public App()
    {
        this.InitializeComponent();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        //  MainWindow = new MainWindow();
        MainWindow.Activate();
        MainWindow.ExtendsContentIntoTitleBar = true;
    }
}
