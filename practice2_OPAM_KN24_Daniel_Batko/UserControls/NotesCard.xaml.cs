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
        if (d is NotesCard control && e.NewValue is Notes notes)
        {
            control.NotesTitle.Text = notes.NotesTitle;
            control.Category.Text = notes.Category;
            control.Content.Text = notes.Content;
            control.Image.Source = notes.ImagePath != null
                 ? new BitmapImage(new Uri(notes.ImagePath))
                : null;
        }
    }

    public NotesCard()
    {
        this.InitializeComponent();
    }
}
