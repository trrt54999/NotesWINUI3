using practice2_OPAM_KN24_Daniel_Batko.Pages;
using System;
using System.Collections.Generic;

namespace practice2_OPAM_KN24_Daniel_Batko.Entities;

public class Notes
{
    public Dictionary<string, List<string>> Errors { get; } = new Dictionary<string, List<string>>();

    public Guid Id { get; set; } = Guid.NewGuid();

    private string _title = string.Empty;

    public string NotesTitle
    {
        get => _title;
        set
        {
            _title = value;
            ValidateNotesTitle();
        }
    }

    private string _category = string.Empty;

    public string Category
    {
        get => _category;
        set
        {
            _category = value;
            ValidateCategory();
        }
    }

    private string _content = string.Empty;
    public string Content
    {
        get => _content;
        set
        {
            _content = value;
            ValidateContent();
        }
    }

    public string? ImagePath { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Now;

    public const int MaxTitleLength = 128;
    public Notes() { }

    public Notes(string title, string category, string content = "", string? imagePath = null)
    {
        NotesTitle = title;
        Category = category;
        Content = content;
        ImagePath = imagePath;

        if (!IsValid()) throw new ValidationException(Errors);
    }

    private void ValidateNotesTitle()
    {
        const string TitlePropertyName = nameof(NotesTitle);

        Errors.Remove(TitlePropertyName);
        Errors[TitlePropertyName] = new List<string>();
        if (string.IsNullOrWhiteSpace(_title))
            Errors[TitlePropertyName].Add("Enter a note title!");
        if (_title.Length > MaxTitleLength)
            Errors[TitlePropertyName].Add($"Note title must not exceed {MaxTitleLength} characters.");

        if (Errors[TitlePropertyName].Count == 0) Errors.Remove(TitlePropertyName);
    }

    private void ValidateCategory()
    {
        const string CategoryPropertyName = nameof(Category);

        Errors.Remove(CategoryPropertyName);
        Errors[CategoryPropertyName] = new List<string>();

        if (string.IsNullOrWhiteSpace(_category)) Errors[CategoryPropertyName].Add("The note category is required.");

        if (Errors[CategoryPropertyName].Count == 0) Errors.Remove(CategoryPropertyName);
    }

    public bool IsValid()
    {
        ValidateNotesTitle();
        ValidateCategory();
        ValidateContent();

        return Errors.Count == 0;
    }

    private void ValidateContent()
    {
        const string ContentPropertyName = nameof(Content);

        Errors.Remove(ContentPropertyName);
        Errors[ContentPropertyName] = new List<string>();

        if (string.IsNullOrWhiteSpace(Content)) Errors[ContentPropertyName].Add("The note content is required."); // TODO CONTENT ON THIS

        if (Errors[ContentPropertyName].Count == 0) Errors.Remove(ContentPropertyName);
    }
}
