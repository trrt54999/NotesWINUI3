using practice2_OPAM_KN24_Daniel_Batko.Pages;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace practice2_OPAM_KN24_Daniel_Batko.Entities;

public class Notes
{
    [JsonIgnore]
    public Dictionary<string, List<string>> Errors { get; } = new Dictionary<string, List<string>>();

    [JsonPropertyName("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    private string _title = string.Empty;

    [JsonPropertyName("title")]
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

    [JsonPropertyName("category")]
    public string Category
    {
        get => _category;
        set
        {
            _category = value;
            ValidateCategory();
        }
    }

    public const int MaxContentLength = 5000;
    private string _content = string.Empty;
    [JsonPropertyName("content")]
    public string Content
    {
        get => _content;
        set
        {
            _content = value;
            ValidateContent();
        }
    }

    [JsonPropertyName("imagePath")]
    public string? ImagePath { get; set; }

    [JsonPropertyName("createdDate")]
    public DateTime CreatedDate { get; set; } = DateTime.Now;

    [JsonPropertyName("hasReminder")]
    public bool HasReminder { get; set; }

    [JsonPropertyName("reminderDateTime")]
    public DateTime? ReminderDateTime { get; set; }

    public const int MaxTitleLength = 128;

    public Notes() { }

    public Notes(string title, string category, string content = "", string? imagePath = null, bool validate = true, bool hasReminder = false, DateTime? reminderDateTime = null)
    {
        _title = title;
        _category = category;
        _content = content;
        ImagePath = imagePath;
        HasReminder = hasReminder;
        ReminderDateTime = reminderDateTime;

        if (validate)
        {
            if (!IsValid()) throw new ValidationException(Errors);
        }
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

    private void ValidateContent()
    {
        const string ContentPropertyName = nameof(Content);

        Errors.Remove(ContentPropertyName);
        Errors[ContentPropertyName] = new List<string>();

        if (string.IsNullOrWhiteSpace(_content)) Errors[ContentPropertyName].Add("The note content is required.");
        if (_content.Length > MaxContentLength)
            Errors[ContentPropertyName].Add($"Note content must not exceed {MaxContentLength} characters.");

        if (Errors[ContentPropertyName].Count == 0) Errors.Remove(ContentPropertyName);
    }

    public bool IsValid()
    {
        ValidateNotesTitle();
        ValidateCategory();
        ValidateContent();

        return Errors.Count == 0;
    }
}