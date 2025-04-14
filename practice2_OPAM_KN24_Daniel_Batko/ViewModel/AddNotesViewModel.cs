using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace practice2_OPAM_KN24_Daniel_Batko.ViewModel;

public class AddNotesViewModel : INotifyPropertyChanged
{


    private string _title = string.Empty;
    public string Title
    {
        get => _title;
        set
        {
            _title = value;
            OnPropertyChanged();
        }
    }

    private string _category = string.Empty;
    public string Category
    {
        get => _category;
        set
        {
            _category = value;
            OnPropertyChanged();
        }
    }

    private string _content = string.Empty;
    public string Content
    {
        get => _content;
        set
        {
            _content = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
