using practice2_OPAM_KN24_Daniel_Batko.Entities;
using practice2_OPAM_KN24_Daniel_Batko.Pages;
using practice2_OPAM_KN24_Daniel_Batko.Services;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Storage;

namespace practice2_OPAM_KN24_Daniel_Batko.ViewModels;

public class AddNotesViewModel : INotifyPropertyChanged
{
    private readonly DataService _dataService;

    private string _title = string.Empty;
    public string Title
    {
        get => _title;
        set
        {
            if (_title != value)
            {
                _title = value;
                OnPropertyChanged();
            }
        }
    }

    private string _category = string.Empty;
    public string Category
    {
        get => _category;
        set
        {
            if (_category != value)
            {
                _category = value;
                OnPropertyChanged();
            }
        }
    }

    private string _content = string.Empty;
    public string Content
    {
        get => _content;
        set
        {
            if (_content != value)
            {
                _content = value;
                OnPropertyChanged();
            }
        }
    }

    private bool _isReminder;
    public bool IsReminder
    {
        get => _isReminder;
        set
        {
            if (_isReminder != value)
            {
                _isReminder = value;
                OnPropertyChanged();
            }
        }
    }

    private DateTimeOffset _reminderDate = DateTimeOffset.Now;
    public DateTimeOffset ReminderDate
    {
        get => _reminderDate;
        set
        {
            if (_reminderDate != value)
            {
                _reminderDate = value;
                OnPropertyChanged();
            }
        }
    }

    private TimeSpan _reminderTime = DateTime.Now.TimeOfDay;
    public TimeSpan ReminderTime
    {
        get => _reminderTime;
        set
        {
            if (_reminderTime != value)
            {
                _reminderTime = value;
                OnPropertyChanged();
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public AddNotesViewModel()
    {
        _dataService = new DataService();
    }

    public AddNotesViewModel(DataService dataService)
    {
        _dataService = dataService;
    }

    public async Task AddNoteAsync(string title, string category, string content, StorageFile? imageFile)
    {
        try
        {
            var username = App.AuthService?.CurrentUser?.Username;
            if (string.IsNullOrEmpty(username))
            {
                throw new InvalidOperationException("No user is logged in.");
            }

            DateTime? reminderDateTime = null;
            if (IsReminder)
            {
                reminderDateTime = ReminderDate.Date.Add(ReminderTime);
            }

            var note = new Notes
            {
                Id = Guid.NewGuid(),
                NotesTitle = title,
                Category = category,
                Content = content,
                HasReminder = IsReminder,
                ReminderDateTime = reminderDateTime
            };

            if (!note.IsValid())
            {
                throw new ValidationException(note.Errors);
            }

            if (imageFile != null)
            {
                note.ImagePath = await _dataService.SaveNoteImageAsync(imageFile, note.Id, username);
            }

            note.ImagePath ??= "ms-appx:///Assets/NotesDefaultLogo.png";

            var notes = await _dataService.LoadNotesAsync(username);
            notes.Add(note);
            await _dataService.SaveNotesAsync(username, notes);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error adding note: {ex.Message}");
            throw;
        }
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}