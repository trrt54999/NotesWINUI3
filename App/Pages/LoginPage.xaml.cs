using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using practice2_OPAM_KN24_Daniel_Batko.Entities.UserEntities;
using practice2_OPAM_KN24_Daniel_Batko.ViewModel;
using System;

namespace practice2_OPAM_KN24_Daniel_Batko.Pages;

public sealed partial class LoginPage : Page
{
    public LoginViewModel ViewModel { get; } = new LoginViewModel();

    public LoginPage()
    {
        InitializeComponent();
    }

    private void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        ClearErrors();

        try
        {
            var credentials = new Login(ViewModel.Identifier, ViewModel.Password);

            if (!credentials.IsValid())
            {
                if (credentials.Errors.ContainsKey(nameof(Login.Identifier)))
                    ShowError(IdentifierErrorTextBlock, string.Join("\n", credentials.Errors[nameof(Login.Identifier)]));
                if (credentials.Errors.ContainsKey(nameof(Login.Password)))
                    ShowError(InputPasswordErrorTextBlock, string.Join("\n", credentials.Errors[nameof(Login.Password)]));
                return;
            }

            if (App.AuthService?.Login(ViewModel.Identifier, ViewModel.Password) == true)
                Frame.Navigate(typeof(MainPage));
            else
                ShowError(LoginErrorTextBlock, "Incorrect login/email or password");
        }
        catch (ValidationException ex)
        {
            if (ex.Errors.ContainsKey(nameof(Login.Identifier)))
                ShowError(IdentifierErrorTextBlock, string.Join("\n", ex.Errors[nameof(Login.Identifier)]));
            if (ex.Errors.ContainsKey(nameof(Login.Password)))
                ShowError(InputPasswordErrorTextBlock, string.Join("\n", ex.Errors[nameof(Login.Password)]));
        }
        catch (Exception ex)
        {
            ShowError(LoginErrorTextBlock, $"Unexpected error: {ex.Message}");
        }
    }

    private void RegisterLink_Click(object sender, RoutedEventArgs e)
    {
        Frame.Navigate(typeof(RegisterPage));
    }

    private void ClearErrors()
    {
        IdentifierErrorTextBlock.Visibility = Visibility.Collapsed;
        InputPasswordErrorTextBlock.Visibility = Visibility.Collapsed;
        LoginErrorTextBlock.Visibility = Visibility.Collapsed;
    }

    private void ShowError(TextBlock errorBlock, string message)
    {
        errorBlock.Text = message;
        errorBlock.Visibility = Visibility.Visible;
    }

    private Visibility OnIdentifierChanged(string? identifier)
    {
        if (string.IsNullOrEmpty(identifier)) return Visibility.Collapsed;

        var credentials = new Login { Identifier = identifier };
        if (credentials.Errors.ContainsKey(nameof(Login.Identifier)))
        {
            ShowError(IdentifierErrorTextBlock, string.Join("\n", credentials.Errors[nameof(Login.Identifier)]));
            return Visibility.Visible;
        }

        return Visibility.Collapsed;
    }

    private Visibility OnPasswordChanged(string? password)
    {
        if (string.IsNullOrEmpty(password)) return Visibility.Collapsed;

        var credentials = new Login { Password = password };
        if (credentials.Errors.ContainsKey(nameof(Login.Password)))
        {
            ShowError(InputPasswordErrorTextBlock, string.Join("\n", credentials.Errors[nameof(Login.Password)]));
            return Visibility.Visible;
        }

        return Visibility.Collapsed;
    }
}