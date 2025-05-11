using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace practice2_OPAM_KN24_Daniel_Batko.ViewModel;

public class RegisterViewModel : INotifyPropertyChanged
{
    private string _username = string.Empty;
    private string _email = string.Empty;
    private string _firstName = string.Empty;
    private string _lastName = string.Empty;
    private string _password = string.Empty;
    private string _confirmPassword = string.Empty;

    public string Username
    {
        get => _username;
        set
        {
            _username = value ?? string.Empty;
            OnPropertyChanged();
        }
    }

    public string Email
    {
        get => _email;
        set
        {
            _email = value ?? string.Empty;
            OnPropertyChanged();
        }
    }

    public string FirstName
    {
        get => _firstName;
        set
        {
            _firstName = value ?? string.Empty;
            OnPropertyChanged();
        }
    }

    public string LastName
    {
        get => _lastName;
        set
        {
            _lastName = value ?? string.Empty;
            OnPropertyChanged();
        }
    }

    public string Password
    {
        get => _password;
        set
        {
            _password = value ?? string.Empty;
            OnPropertyChanged();
        }
    }

    public string ConfirmPassword
    {
        get => _confirmPassword;
        set
        {
            _confirmPassword = value ?? string.Empty;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
