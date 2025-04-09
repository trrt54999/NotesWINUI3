using Microsoft.UI.Xaml;

namespace practice2_OPAM_KN24_Daniel_Batko;

public partial class App : Application
{

    public App()
    {
        this.InitializeComponent();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        window = new MainWindow();
        window.Activate();
        window.ExtendsContentIntoTitleBar = true;
    }

    private Window? window;
}
