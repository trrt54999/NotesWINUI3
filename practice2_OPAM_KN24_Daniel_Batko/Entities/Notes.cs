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

    public string Content { get; set; } = string.Empty;
    public string? ImagePath { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Now;

    public const int MaxTitleLength = 128;

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
        Errors.Remove(nameof(NotesTitle));
        Errors[nameof(NotesTitle)] = new List<string>();
        if (string.IsNullOrWhiteSpace(_title))
        {
            Errors[nameof(NotesTitle)].Add("Enter a note title!");
        }
        if (_title.Length > MaxTitleLength)
        {
            Errors[nameof(NotesTitle)].Add($"Note title must not exceed {MaxTitleLength} characters.");
        }
    }

    private void ValidateCategory()
    {
        Errors.Remove(nameof(Category));
        Errors[nameof(Category)] = new List<string>();

        if (string.IsNullOrWhiteSpace(_category))
        {
            Errors[nameof(Category)].Add("The note category is required.");
        }
    }

    public bool IsValid()
    {
        ValidateNotesTitle();
        ValidateCategory();
        ValidateContent();
        RemoveEmptyListOfErrors();

        return Errors.Count == 0;
    }

    private void RemoveEmptyListOfErrors()
    {
        var keysToRemove = new List<string>();
        foreach (var key in Errors.Keys)
        {
            if (Errors[key].Count == 0)
            {
                keysToRemove.Add(key);
            }
        }

        foreach (var key in keysToRemove)
        {
            Errors.Remove(key);
        }
    }
    private void ValidateContent()
    {
        Errors.Remove(nameof(Content));
        Errors[nameof(Content)] = new List<string>();

        if (string.IsNullOrWhiteSpace(Content))
        {
            Errors[nameof(Content)].Add("The note content is required.");
        }
    }

}
