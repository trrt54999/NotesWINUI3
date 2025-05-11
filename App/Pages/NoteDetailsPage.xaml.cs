using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using practice2_OPAM_KN24_Daniel_Batko.Entities;
using System.Diagnostics;

namespace practice2_OPAM_KN24_Daniel_Batko.Pages;

public sealed partial class NoteDetailsPage : Page
{
    public Notes Note { get; private set; }

    public string ReminderDateTimeText
    {
        get
        {
            if (Note?.HasReminder == true && Note.ReminderDateTime.HasValue)
                return Note.ReminderDateTime.Value.ToString("dd.MM.yyyy HH:mm");
            return string.Empty;
        }
    }

    public NoteDetailsPage()
    {
        this.InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        if (e.Parameter is Notes note)
        {
            Note = note;
            Debug.WriteLine($"Navigated to NoteDetailsPage with note: {Note.NotesTitle}");

            ReminderInfoPanel.Visibility = note.HasReminder ? Visibility.Visible : Visibility.Collapsed;
        }
    }

    private void Back_Click(object sender, RoutedEventArgs e)
    {
        if (Frame.CanGoBack)
        {
            Frame.GoBack();
        }
        else
        {
            Frame.Navigate(typeof(AllNotes));
        }
    }
}