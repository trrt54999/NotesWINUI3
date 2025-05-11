using practice2_OPAM_KN24_Daniel_Batko.Entities;
using practice2_OPAM_KN24_Daniel_Batko.Pages;
using practice2_OPAM_KN24_Daniel_Batko.Services;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Storage;

namespace practice2_OPAM_KN24_Daniel_Batko.ViewModels;

public class EditNotesViewModel : INotifyPropertyChanged
{
    private readonly DataService _dataService;
    private readonly Notes _note;

    public Guid Id => _note.Id;

    private string _title;
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

    private string _category;
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

    private string _content;
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

    private string? _imagePath;
    public string? ImagePath
    {
        get => _imagePath;
        set
        {
            if (_imagePath != value)
            {
                _imagePath = value;
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

    public EditNotesViewModel(Notes note, DataService dataService)
    {
        _note = note ?? throw new ArgumentNullException(nameof(note));
        _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));

        _title = note.NotesTitle;
        _category = note.Category;
        _content = note.Content;
        _imagePath = note.ImagePath;
        _isReminder = note.HasReminder;

        if (note.ReminderDateTime.HasValue)
        {
            _reminderDate = new DateTimeOffset(note.ReminderDateTime.Value.Date);
            _reminderTime = note.ReminderDateTime.Value.TimeOfDay;
        }

        Debug.WriteLine($"Initialized EditNotesViewModel for note: {note.NotesTitle}, IsReminder: {_isReminder}");
    }

    public async Task UpdateNoteAsync(string title, string category, string content, StorageFile? imageFile)
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

            var updatedNote = new Notes
            {
                Id = _note.Id,
                NotesTitle = title,
                Category = category,
                Content = content,
                CreatedDate = _note.CreatedDate,
                ImagePath = _note.ImagePath,
                HasReminder = IsReminder,
                ReminderDateTime = reminderDateTime
            };

            if (!updatedNote.IsValid())
            {
                throw new ValidationException(updatedNote.Errors);
            }

            if (imageFile != null)
            {
                if (!string.IsNullOrEmpty(updatedNote.ImagePath))
                {
                    try
                    {
                        var noteImagesFolder = Path.Combine(_dataService.GetFullPath(@"Data\DataBase\NoteImages"), username);
                        var files = Directory.GetFiles(noteImagesFolder, $"{updatedNote.Id}.*");
                        foreach (var file in files)
                        {
                            var oldImageFile = await StorageFile.GetFileFromPathAsync(file);
                            await oldImageFile.DeleteAsync();
                            Debug.WriteLine($"Deleted old image: {file}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Failed to delete old image(s) for note {updatedNote.Id}: {ex.Message}");
                    }
                }

                updatedNote.ImagePath = await _dataService.SaveNoteImageAsync(imageFile, updatedNote.Id, username);
                Debug.WriteLine($"Saved new image: {updatedNote.ImagePath}");
            }

            var notes = await _dataService.LoadNotesAsync(username);
            var existingNote = notes.Find(n => n.Id == updatedNote.Id);
            if (existingNote != null)
            {
                notes.Remove(existingNote);
            }
            notes.Add(updatedNote);
            await _dataService.SaveNotesAsync(username, notes);
            Debug.WriteLine($"Updated note {updatedNote.NotesTitle} in JSON file for user {username}, IsReminder: {updatedNote.HasReminder}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error updating note: {ex.Message}");
            throw;
        }
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}