using practice2_OPAM_KN24_Daniel_Batko.Entities.UserEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace practice2_OPAM_KN24_Daniel_Batko.Services;

public class RegisterResult
{
    public bool Success { get; set; }
    public List<string> UsernameErrors { get; set; } = new List<string>();
    public List<string> EmailErrors { get; set; } = new List<string>();
}

public class AuthService
{
    private List<User> _users;
    private User? _currentUser;
    private readonly DataService _dataService;

    public User? CurrentUser => _currentUser;

    public AuthService(DataService dataService)
    {
        _dataService = dataService;
        _users = Task.Run(() => _dataService.LoadUsersAsync()).Result;
    }

    public bool Login(string identifier, string password)
    {
        foreach (var user in _users)
        {
            if ((identifier.Equals(user.Username, StringComparison.OrdinalIgnoreCase) ||
                 identifier.Equals(user.Email, StringComparison.OrdinalIgnoreCase)) &&
                BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                _currentUser = user;
                return true;
            }
        }
        return false;
    }

    public async Task<RegisterResult> Register(User newUser)
    {
        var result = new RegisterResult { Success = true };

        foreach (var user in _users)
        {
            if (user.Username.Equals(newUser.Username, StringComparison.OrdinalIgnoreCase))
            {
                result.Success = false;
                result.UsernameErrors.Add("A user with this login already exists");
            }
            if (user.Email.Equals(newUser.Email, StringComparison.OrdinalIgnoreCase))
            {
                result.Success = false;
                result.EmailErrors.Add("A user with this email already exists");
            }
        }

        if (result.Success)
        {
            newUser.RegistrationDate = DateTime.UtcNow;
            _users.Add(newUser);
            _currentUser = newUser;
            await _dataService.SaveUsersAsync(_users);
        }

        return result;
    }

    public bool IsEmailRegistered(string email)
    {
        return _users.Any(user => user.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
    }

    public void Logout()
    {
        _currentUser = null;
    }

    public async Task DeleteAccountAsync(string username)
    {
        var user = _users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        if (user != null)
        {
            _users.Remove(user);
            await _dataService.SaveUsersAsync(_users);
            await _dataService.DeleteUserDataAsync(username);
        }
    }
}