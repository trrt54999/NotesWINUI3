using practice2_OPAM_KN24_Daniel_Batko.Pages;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace practice2_OPAM_KN24_Daniel_Batko.Entities.UserEntities;

public class Login
{
    public Dictionary<string, List<string>> Errors { get; } = new Dictionary<string, List<string>>();

    private string _identifier = string.Empty;
    public string Identifier
    {
        get => _identifier;
        set
        {
            _identifier = value;
            ValidateIdentifier();
        }
    }

    private string _password = string.Empty;
    public string Password
    {
        get => _password;
        set
        {
            _password = value;
            ValidatePassword();
        }
    }

    public const int MaxIdentifierLength = 80;
    public const int MaxPasswordLength = 50;
    public const int MinPasswordLength = 6;
    public Login() { }

    public Login(string identifier, string password)
    {
        Identifier = identifier;
        Password = password;

        if (!IsValid()) throw new ValidationException(Errors);
    }

    private void ValidateIdentifier()
    {
        const string IdentifierPropertyName = nameof(Identifier);
        Errors.Remove(IdentifierPropertyName);
        Errors[IdentifierPropertyName] = new List<string>();

        if (string.IsNullOrWhiteSpace(_identifier))
            Errors[IdentifierPropertyName].Add("Login or email cannot be empty!");
        if (_identifier.Length > MaxIdentifierLength)
            Errors[IdentifierPropertyName].Add($"Login or email must not exceed {MaxIdentifierLength} characters.");

        if (_identifier.Contains("@") && !IsValidEmail(_identifier))
            Errors[IdentifierPropertyName].Add("Enter a valid email address!");

        if (Errors[IdentifierPropertyName].Count == 0) Errors.Remove(IdentifierPropertyName);
    }

    private bool IsValidEmail(string email)
    {
        string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, pattern);
    }

    private void ValidatePassword()
    {
        const string PasswordPropertyName = nameof(Password);
        Errors.Remove(PasswordPropertyName);
        Errors[PasswordPropertyName] = new List<string>();

        if (string.IsNullOrWhiteSpace(_password))
            Errors[PasswordPropertyName].Add("Password cannot be empty!");
        if (_password.Length < MinPasswordLength)
            Errors[PasswordPropertyName].Add($"Password must contain at least {MinPasswordLength} characters.");
        if (_password.Length > MaxPasswordLength)
            Errors[PasswordPropertyName].Add($"Password must not exceed {MaxPasswordLength} characters.");

        if (Errors[PasswordPropertyName].Count == 0) Errors.Remove(PasswordPropertyName);
    }

    public bool IsValid()
    {
        ValidateIdentifier();
        ValidatePassword();
        return Errors.Count == 0;
    }

    internal static object Username()
    {
        throw new NotImplementedException();
    }
}