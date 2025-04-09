using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using System;

namespace practice2_OPAM_KN24_Daniel_Batko;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        this.InitializeComponent();
    }

    private void MainNavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (args.SelectedItem is NavigationViewItem navigationViewItem)
        {
            string pageName = navigationViewItem.Tag.ToString()!;
            Type pageType = Type.GetType($"practice2_OPAM_KN24_Daniel_Batko.Pages.{pageName}")!;
            ContentFrame.Navigate(pageType, null, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });

            sender.Header = navigationViewItem.Content.ToString();
        }
    }
}
