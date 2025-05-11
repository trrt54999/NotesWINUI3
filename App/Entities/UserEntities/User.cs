using BCrypt.Net;
using System;
using System.Text.Json.Serialization;

namespace practice2_OPAM_KN24_Daniel_Batko.Entities.UserEntities;

public class User
{
    [JsonPropertyName("username")]
    public string Username { get; set; }

    [JsonPropertyName("passwordHash")]
    public string PasswordHash { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; }

    [JsonPropertyName("firstName")]
    public string FirstName { get; set; }

    [JsonPropertyName("lastName")]
    public string LastName { get; set; }

    [JsonPropertyName("avatarPath")]
    public string AvatarPath { get; set; }

    [JsonPropertyName("registrationDate")]
    public DateTime RegistrationDate { get; set; }

    public User() { }

    public User(string username, string password, string email, string firstName, string lastName)
    {
        Username = username;
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        RegistrationDate = DateTime.UtcNow;
        AvatarPath = string.Empty;
    }
}