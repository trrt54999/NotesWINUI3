using practice2_OPAM_KN24_Daniel_Batko.Pages;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace practice2_OPAM_KN24_Daniel_Batko.Entities.UserEntities;

public class Register
{
    public Dictionary<string, List<string>> Errors { get; } = new Dictionary<string, List<string>>();

    private string _username = string.Empty;
    public string Username
    {
        get => _username;
        set
        {
            _username = value;
            ValidateUsername();
        }
    }

    private string _email = string.Empty;
    public string Email
    {
        get => _email;
        set
        {
            _email = value;
            ValidateEmail();
        }
    }

    private string _firstName = string.Empty;
    public string FirstName
    {
        get => _firstName;
        set
        {
            _firstName = value;
            ValidateFirstName();
        }
    }

    private string _lastName = string.Empty;
    public string LastName
    {
        get => _lastName;
        set
        {
            _lastName = value;
            ValidateLastName();
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
            ValidateConfirmPassword();
        }
    }

    private string _confirmPassword = string.Empty;
    public string ConfirmPassword
    {
        get => _confirmPassword;
        set
        {
            _confirmPassword = value;
            ValidateConfirmPassword();
        }
    }

    public const int MaxUsernameLength = 50;
    public const int MinUsernameLength = 5;
    public const int MaxPasswordLength = 50;
    public const int MinPasswordLength = 6;
    public const int MaxEmailLength = 80;
    public const int MaxFirstNameLength = 70;
    public const int MaxLastNameLength = 70;

    public Register() { }

    public Register(string username, string email, string firstName, string lastName, string password, string confirmPassword)
    {
        Username = username;
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        Password = password;
        ConfirmPassword = confirmPassword;

        if (!IsValid()) throw new ValidationException(Errors);
    }

    private void ValidateUsername()
    {
        const string UsernamePropertyName = nameof(Username);
        Errors.Remove(UsernamePropertyName);
        Errors[UsernamePropertyName] = new List<string>();

        if (string.IsNullOrWhiteSpace(_username))
            Errors[UsernamePropertyName].Add("Login cannot be empty!");
        if (_username.Length < MinUsernameLength)
            Errors[UsernamePropertyName].Add($"Login must contain at least {MinUsernameLength} characters.");
        if (_username.Length > MaxUsernameLength)
            Errors[UsernamePropertyName].Add($"Login must not exceed {MaxUsernameLength} characters.");
        if (_username.Contains("@"))
            Errors[UsernamePropertyName].Add("Login cannot contain the '@' symbol.");

        if (Errors[UsernamePropertyName].Count == 0) Errors.Remove(UsernamePropertyName);
    }

    private void ValidateEmail()
    {
        const string EmailPropertyName = nameof(Email);
        Errors.Remove(EmailPropertyName);
        Errors[EmailPropertyName] = new List<string>();

        if (string.IsNullOrWhiteSpace(_email))
            Errors[EmailPropertyName].Add("Email cannot be empty!");
        if (!IsValidEmail(_email))
            Errors[EmailPropertyName].Add("Enter a valid email address!");
        if (_email.Length > MaxEmailLength)
            Errors[EmailPropertyName].Add($"Email must not exceed {MaxEmailLength} characters.");
        if (!IsEnglishOnly(_email))
            Errors[EmailPropertyName].Add("Email must contain only English characters!");

        if (Errors[EmailPropertyName].Count == 0) Errors.Remove(EmailPropertyName);
    }

    private void ValidateFirstName()
    {
        const string FirstNamePropertyName = nameof(FirstName);
        Errors.Remove(FirstNamePropertyName);
        Errors[FirstNamePropertyName] = new List<string>();

        if (string.IsNullOrWhiteSpace(_firstName))
            Errors[FirstNamePropertyName].Add("First name cannot be empty!");
        if (_firstName.Length > MaxFirstNameLength)
            Errors[FirstNamePropertyName].Add($"First name must not exceed {MaxFirstNameLength} characters.");
        if (!Regex.IsMatch(_firstName, @"^\p{L}+$"))
            Errors[FirstNamePropertyName].Add("First name cannot have numbers or special symbols.");

        if (Errors[FirstNamePropertyName].Count == 0) Errors.Remove(FirstNamePropertyName);
    }

    private void ValidateLastName()
    {
        const string LastNamePropertyName = nameof(LastName);
        Errors.Remove(LastNamePropertyName);
        Errors[LastNamePropertyName] = new List<string>();

        if (string.IsNullOrWhiteSpace(_lastName))
            Errors[LastNamePropertyName].Add("Last name cannot be empty!");
        if (_lastName.Length > MaxLastNameLength)
            Errors[LastNamePropertyName].Add($"Last name must not exceed {MaxLastNameLength} characters.");
        if (!Regex.IsMatch(_lastName, @"^\p{L}+$"))
            Errors[LastNamePropertyName].Add("Last name cannot have numbers or special symbols.");

        if (Errors[LastNamePropertyName].Count == 0) Errors.Remove(LastNamePropertyName);
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
        if (!Regex.IsMatch(_password, @"\p{Ll}"))
            Errors[PasswordPropertyName].Add("Password must contain at least one lowercase letter.");
        if (!Regex.IsMatch(_password, @"\p{Lu}"))
            Errors[PasswordPropertyName].Add("Password must contain at least one uppercase letter.");
        if (!Regex.IsMatch(_password, @"\d"))
            Errors[PasswordPropertyName].Add("Password must contain at least one number.");
        if (!Regex.IsMatch(_password, @"[@$!%*?&]"))
            Errors[PasswordPropertyName].Add("Password must contain at least one special character (@, $, !, %, *, ?, &).");

        if (Errors[PasswordPropertyName].Count == 0) Errors.Remove(PasswordPropertyName);
    }

    private void ValidateConfirmPassword()
    {
        const string ConfirmPasswordPropertyName = nameof(ConfirmPassword);
        Errors.Remove(ConfirmPasswordPropertyName);
        Errors[ConfirmPasswordPropertyName] = new List<string>();

        if (_password != _confirmPassword)
            Errors[ConfirmPasswordPropertyName].Add("Passwords do not match!");

        if (Errors[ConfirmPasswordPropertyName].Count == 0) Errors.Remove(ConfirmPasswordPropertyName);
    }

    private bool IsValidEmail(string email)
    {
        string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, pattern);
    }

    private bool IsEnglishOnly(string input)
    {
        return Regex.IsMatch(input, @"^[\x00-\x7F]+$");
    }

    public bool IsValid()
    {
        ValidateUsername();
        ValidateEmail();
        ValidateFirstName();
        ValidateLastName();
        ValidatePassword();
        ValidateConfirmPassword();
        return Errors.Count == 0;
    }
}