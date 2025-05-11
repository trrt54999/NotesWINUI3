using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using practice2_OPAM_KN24_Daniel_Batko.Entities;
using practice2_OPAM_KN24_Daniel_Batko.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace practice2_OPAM_KN24_Daniel_Batko.Pages;

public sealed partial class Reminders : Page
{
    public ObservableCollection<Notes> ReminderNotes { get; } = new ObservableCollection<Notes>();
    private readonly DataService _dataService;

    public Reminders()
    {
        _dataService = new DataService();
        InitializeComponent();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        await RefreshRemindersAsync();
    }

    public async Task RefreshRemindersAsync()
    {
        ReminderNotes.Clear();
        var username = App.AuthService?.CurrentUser?.Username;
        if (!string.IsNullOrEmpty(username))
        {
            var notes = await _dataService.LoadNotesAsync(username);
            foreach (var note in notes)
            {
                if (note.HasReminder)
                {
                    ReminderNotes.Add(note);
                }
            }
            Debug.WriteLine($"Loaded {ReminderNotes.Count} reminders for user {username}");
        }
    }
}