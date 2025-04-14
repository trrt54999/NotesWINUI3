using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using practice2_OPAM_KN24_Daniel_Batko.Entities;
using System;

namespace practice2_OPAM_KN24_Daniel_Batko.UserControls;

public sealed partial class NotesCard : UserControl
{

    public Notes Notes
    {
        get { return (Notes)GetValue(NotesProperty); }
        set { SetValue(NotesProperty, value); }
    }

    public static readonly DependencyProperty NotesProperty =
        DependencyProperty.Register(nameof(Notes), typeof(Notes), typeof(NotesCard), new PropertyMetadata(null, OnNotesChanged));

    private static void OnNotesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is NotesCard control && e.NewValue is Notes note)
        {
            control.NotesTitle.Text = note.NotesTitle;
            control.Category.Text = note.Category;
            control.Content.Text = note.Content;
            try
            {
                control.Image.Source = note.ImagePath != null
                    ? new BitmapImage(new Uri(note.ImagePath))
                    : null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading image: {ex.Message}");
                control.Image.Source = new BitmapImage(new Uri("ms-appx:///Assets/note_cover.png"));
            }
        }
    }

    public NotesCard()
    {
        this.InitializeComponent();
    }
}
