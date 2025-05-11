using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace practice2_OPAM_KN24_Daniel_Batko.ViewModel;

public class LoginViewModel : INotifyPropertyChanged
{
    private string _identifier = string.Empty;
    private string _password = string.Empty;

    public string Identifier
    {
        get => _identifier;
        set
        {
            if (_identifier != value)
            {
                _identifier = value;
                OnPropertyChanged();
            }
        }
    }

    public string Password
    {
        get => _password;
        set
        {
            _password = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}