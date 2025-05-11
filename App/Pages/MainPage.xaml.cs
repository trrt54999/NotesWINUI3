using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.System;

namespace practice2_OPAM_KN24_Daniel_Batko.Pages;

public sealed partial class MainPage : Page
{
    private readonly Dictionary<string, Type> _pages = new()
    {
        { "AllNotes", typeof(AllNotes) },
        { "Reminders", typeof(Reminders) },
        { "Categories", typeof(Categories) },
        { "Favorites", typeof(Favorites) },
        { "AddNote", typeof(AddNote) },
        { "Settings", typeof(Settings) },
        { "profile", typeof(ProfilePage) },
        { "NoteDetails", typeof(NoteDetailsPage) },
        { "EditNote", typeof(EditNotePage) }
    };

    public MainPage()
    {
        InitializeComponent();

        if (App.AuthService.CurrentUser == null)
            Frame.Navigate(typeof(LoginPage));
        else
            ContentFrame.Navigate(typeof(AllNotes));
        NavView.SelectedItem = NavView.MenuItems[0];
    }

    private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (args.SelectedItemContainer == null) return;

        string tag = args.SelectedItemContainer.Tag.ToString()!;
        NavView.Header = args.SelectedItemContainer.Content.ToString();

        Debug.WriteLine($"Navigating to page with tag: {tag}");

        if (_pages.TryGetValue(tag, out Type pageType))
        {
            ContentFrame.BackStack.Clear();
            ContentFrame.Navigate(pageType);
            Debug.WriteLine($"Navigated to {pageType.Name}");
        }
        else
        {
            Debug.WriteLine($"Page with tag {tag} not found in _pages dictionary");
        }
    }

    private void LogoutItem_Tapped(object sender, TappedRoutedEventArgs e)
    {
        App.AuthService.Logout();
        Frame.Navigate(typeof(LoginPage));
    }

    private async void GitHubLogo_Tapped(object sender, TappedRoutedEventArgs e)
    {
        var githubUrl = new Uri("https://github.com/trrt54999");
        await Launcher.LaunchUriAsync(githubUrl);
    }
}