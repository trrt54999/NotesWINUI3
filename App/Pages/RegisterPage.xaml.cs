using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using practice2_OPAM_KN24_Daniel_Batko.Entities.UserEntities;
using practice2_OPAM_KN24_Daniel_Batko.ViewModel;
using System;

namespace practice2_OPAM_KN24_Daniel_Batko.Pages;

public sealed partial class RegisterPage : Page
{
    public RegisterViewModel ViewModel { get; } = new RegisterViewModel();

    public RegisterPage()
    {
        InitializeComponent();
    }

    private async void RegisterButton_Click(object sender, RoutedEventArgs e)
    {
        ClearErrors();

        try
        {
            var credentials = new Register(
                ViewModel.Username,
                ViewModel.Email,
                ViewModel.FirstName,
                ViewModel.LastName,
                ViewModel.Password,
                ViewModel.ConfirmPassword
            );

            if (!credentials.IsValid())
            {
                if (credentials.Errors.ContainsKey(nameof(Register.Username)))
                    ShowError(UsernameErrorTextBlock, string.Join("\n", credentials.Errors[nameof(Register.Username)]));
                if (credentials.Errors.ContainsKey(nameof(Register.Email)))
                    ShowError(EmailErrorTextBlock, string.Join("\n", credentials.Errors[nameof(Register.Email)]));
                if (credentials.Errors.ContainsKey(nameof(Register.FirstName)))
                    ShowError(FirstNameErrorTextBlock, string.Join("\n", credentials.Errors[nameof(Register.FirstName)]));
                if (credentials.Errors.ContainsKey(nameof(Register.LastName)))
                    ShowError(LastNameErrorTextBlock, string.Join("\n", credentials.Errors[nameof(Register.LastName)]));
                if (credentials.Errors.ContainsKey(nameof(Register.Password)))
                    ShowError(InputPasswordErrorTextBlock, string.Join("\n", credentials.Errors[nameof(Register.Password)]));
                if (credentials.Errors.ContainsKey(nameof(Register.ConfirmPassword)))
                    ShowError(ConfirmPasswordErrorTextBlock, string.Join("\n", credentials.Errors[nameof(Register.ConfirmPassword)]));
                return;
            }

            User newUser = new User(
                ViewModel.Username,
                ViewModel.Password,
                ViewModel.Email,
                ViewModel.FirstName,
                ViewModel.LastName
            );

            var registerResult = await App.AuthService?.Register(newUser);
            if (registerResult.Success)
            {
                Frame.Navigate(typeof(MainPage));
            }
            else
            {
                if (registerResult.UsernameErrors.Count > 0)
                {
                    ShowError(UsernameErrorTextBlock, string.Join("\n", registerResult.UsernameErrors));
                }
                if (registerResult.EmailErrors.Count > 0)
                {
                    ShowError(EmailErrorTextBlock, string.Join("\n", registerResult.EmailErrors));
                }
            }
        }
        catch (ValidationException ex)
        {
            if (ex.Errors.ContainsKey(nameof(Register.Username)))
                ShowError(UsernameErrorTextBlock, string.Join("\n", ex.Errors[nameof(Register.Username)]));
            if (ex.Errors.ContainsKey(nameof(Register.Email)))
                ShowError(EmailErrorTextBlock, string.Join("\n", ex.Errors[nameof(Register.Email)]));
            if (ex.Errors.ContainsKey(nameof(Register.FirstName)))
                ShowError(FirstNameErrorTextBlock, string.Join("\n", ex.Errors[nameof(Register.FirstName)]));
            if (ex.Errors.ContainsKey(nameof(Register.LastName)))
                ShowError(LastNameErrorTextBlock, string.Join("\n", ex.Errors[nameof(Register.LastName)]));
            if (ex.Errors.ContainsKey(nameof(Register.Password)))
                ShowError(InputPasswordErrorTextBlock, string.Join("\n", ex.Errors[nameof(Register.Password)]));
            if (ex.Errors.ContainsKey(nameof(Register.ConfirmPassword)))
                ShowError(ConfirmPasswordErrorTextBlock, string.Join("\n", ex.Errors[nameof(Register.ConfirmPassword)]));
        }
        catch (Exception ex)
        {
            ShowError(RegisterErrorTextBlock, $"Unexpected error: {ex.Message}");
        }
    }

    private void LoginLink_Click(object sender, RoutedEventArgs e)
    {
        Frame.Navigate(typeof(LoginPage));
    }

    private void ClearErrors()
    {
        UsernameErrorTextBlock.Visibility = Visibility.Collapsed;
        EmailErrorTextBlock.Visibility = Visibility.Collapsed;
        FirstNameErrorTextBlock.Visibility = Visibility.Collapsed;
        LastNameErrorTextBlock.Visibility = Visibility.Collapsed;
        InputPasswordErrorTextBlock.Visibility = Visibility.Collapsed;
        ConfirmPasswordErrorTextBlock.Visibility = Visibility.Collapsed;
        RegisterErrorTextBlock.Visibility = Visibility.Collapsed;
    }

    private void ShowError(TextBlock errorBlock, string message)
    {
        errorBlock.Text = message;
        errorBlock.Visibility = Visibility.Visible;
    }

    private Visibility OnUsernameChanged(string? username)
    {
        if (string.IsNullOrEmpty(username)) return Visibility.Collapsed;

        var credentials = new Register { Username = username };
        if (credentials.Errors.ContainsKey(nameof(Register.Username)))
        {
            ShowError(UsernameErrorTextBlock, string.Join("\n", credentials.Errors[nameof(Register.Username)]));
            return Visibility.Visible;
        }

        return Visibility.Collapsed;
    }

    private Visibility OnEmailChanged(string? email)
    {
        if (string.IsNullOrEmpty(email)) return Visibility.Collapsed;

        var credentials = new Register { Email = email };
        if (credentials.Errors.ContainsKey(nameof(Register.Email)))
        {
            ShowError(EmailErrorTextBlock, string.Join("\n", credentials.Errors[nameof(Register.Email)]));
            return Visibility.Visible;
        }

        return Visibility.Collapsed;
    }

    private Visibility OnFirstNameChanged(string? firstName)
    {
        if (string.IsNullOrEmpty(firstName)) return Visibility.Collapsed;

        var credentials = new Register { FirstName = firstName };
        if (credentials.Errors.ContainsKey(nameof(Register.FirstName)))
        {
            ShowError(FirstNameErrorTextBlock, string.Join("\n", credentials.Errors[nameof(Register.FirstName)]));
            return Visibility.Visible;
        }

        return Visibility.Collapsed;
    }

    private Visibility OnLastNameChanged(string? lastName)
    {
        if (string.IsNullOrEmpty(lastName)) return Visibility.Collapsed;

        var credentials = new Register { LastName = lastName };
        if (credentials.Errors.ContainsKey(nameof(Register.LastName)))
        {
            ShowError(LastNameErrorTextBlock, string.Join("\n", credentials.Errors[nameof(Register.LastName)]));
            return Visibility.Visible;
        }

        return Visibility.Collapsed;
    }

    private Visibility OnPasswordChanged(string? password)
    {
        if (string.IsNullOrEmpty(password)) return Visibility.Collapsed;

        var credentials = new Register { Password = password };
        if (credentials.Errors.ContainsKey(nameof(Register.Password)))
        {
            ShowError(InputPasswordErrorTextBlock, string.Join("\n", credentials.Errors[nameof(Register.Password)]));
            return Visibility.Visible;
        }

        return Visibility.Collapsed;
    }

    private Visibility OnConfirmPasswordChanged(string? confirmPassword, string? password)
    {
        if (string.IsNullOrEmpty(confirmPassword)) return Visibility.Collapsed;

        var credentials = new Register { ConfirmPassword = confirmPassword, Password = password ?? string.Empty };
        if (credentials.Errors.ContainsKey(nameof(Register.ConfirmPassword)))
        {
            ShowError(ConfirmPasswordErrorTextBlock, string.Join("\n", credentials.Errors[nameof(Register.ConfirmPassword)]));
            return Visibility.Visible;
        }

        return Visibility.Collapsed;
    }
}