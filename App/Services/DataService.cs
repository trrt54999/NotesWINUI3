using practice2_OPAM_KN24_Daniel_Batko.Entities;
using practice2_OPAM_KN24_Daniel_Batko.Entities.UserEntities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Storage;

namespace practice2_OPAM_KN24_Daniel_Batko.Services;

public class DataService
{
    private readonly string _projectRootPath;
    private readonly string _usersFilePath = @"Data\DataBase\Users.json";
    private readonly string _notesFolderPath = @"Data\DataBase\Notes";
    private readonly string _favoritesFolderPath = @"Data\DataBase\Favorites";
    private readonly string _noteImagesFolderPath = @"Data\DataBase\NoteImages";
    private readonly string _userImagesFolderPath = @"Data\DataBase\UsersImages";

    public DataService()
    {
        _projectRootPath = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
        InitializeFolders();
        Debug.WriteLine($"Project Root Path: {_projectRootPath}");
        Debug.WriteLine($"Users File Path: {GetFullPath(_usersFilePath)}");
        Debug.WriteLine($"Notes Folder Path: {GetFullPath(_notesFolderPath)}");
        Debug.WriteLine($"Favorites Folder Path: {GetFullPath(_favoritesFolderPath)}");
        Debug.WriteLine($"Note Images Folder Path: {GetFullPath(_noteImagesFolderPath)}");
        Debug.WriteLine($"User Images Folder Path: {GetFullPath(_userImagesFolderPath)}");
    }

    public string GetFullPath(string relativePath)
    {
        return Path.Combine(_projectRootPath, relativePath);
    }

    private void InitializeFolders()
    {
        Directory.CreateDirectory(GetFullPath(@"Data\DataBase"));
        Directory.CreateDirectory(GetFullPath(_notesFolderPath));
        Directory.CreateDirectory(GetFullPath(_favoritesFolderPath));
        Directory.CreateDirectory(GetFullPath(_noteImagesFolderPath));
        Directory.CreateDirectory(GetFullPath(_userImagesFolderPath));
    }

    private string GetUserImagesFolderPath()
    {
        return _userImagesFolderPath;
    }

    private string GetNoteImagesFolderPath(string username)
    {
        return Path.Combine(_noteImagesFolderPath, username);
    }

    public async Task SaveUsersAsync(List<User> users)
    {
        try
        {
            var fullPath = GetFullPath(_usersFilePath);
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
            var json = JsonSerializer.Serialize(users, options);
            await File.WriteAllTextAsync(fullPath, json);
            Debug.WriteLine($"Saved users to: {fullPath}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error saving users to {GetFullPath(_usersFilePath)}: {ex.Message}");
            throw;
        }
    }

    public async Task<List<User>> LoadUsersAsync()
    {
        try
        {
            var fullPath = GetFullPath(_usersFilePath);
            if (!File.Exists(fullPath))
                return new List<User>();

            var json = await File.ReadAllTextAsync(fullPath);
            Debug.WriteLine($"Loaded users from: {fullPath}");
            return JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error loading users from {GetFullPath(_usersFilePath)}: {ex.Message}");
            return new List<User>();
        }
    }

    public async Task SaveNotesAsync(string username, List<Notes> newNotes)
    {
        try
        {
            var filePath = Path.Combine(GetFullPath(_notesFolderPath), $"{username}.json");
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
            var json = JsonSerializer.Serialize(newNotes, options);
            await File.WriteAllTextAsync(filePath, json);
            Debug.WriteLine($"Saved notes to: {filePath}, Notes count: {newNotes.Count}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error saving notes to {Path.Combine(GetFullPath(_notesFolderPath), $"{username}.json")}: {ex.Message}");
            throw;
        }
    }

    public async Task<List<Notes>> LoadNotesAsync(string username)
    {
        try
        {
            var filePath = Path.Combine(GetFullPath(_notesFolderPath), $"{username}.json");
            if (!File.Exists(filePath))
                return new List<Notes>();

            var json = await File.ReadAllTextAsync(filePath);
            Debug.WriteLine($"Loaded notes from: {filePath}");
            return JsonSerializer.Deserialize<List<Notes>>(json) ?? new List<Notes>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error loading notes from {Path.Combine(GetFullPath(_notesFolderPath), $"{username}.json")}: {ex.Message}");
            return new List<Notes>();
        }
    }

    public async Task SaveFavoriteNotesAsync(string username, List<Guid> favoriteNoteIds)
    {
        try
        {
            var filePath = Path.Combine(GetFullPath(_favoritesFolderPath), $"{username}.json");
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
            var json = JsonSerializer.Serialize(favoriteNoteIds, options);
            await File.WriteAllTextAsync(filePath, json);
            Debug.WriteLine($"Saved favorite notes to: {filePath}, Count: {favoriteNoteIds.Count}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error saving favorite notes to {Path.Combine(GetFullPath(_favoritesFolderPath), $"{username}.json")}: {ex.Message}");
            throw;
        }
    }

    public async Task<List<Guid>> LoadFavoriteNotesAsync(string username)
    {
        try
        {
            var filePath = Path.Combine(GetFullPath(_favoritesFolderPath), $"{username}.json");
            if (!File.Exists(filePath))
                return new List<Guid>();

            var json = await File.ReadAllTextAsync(filePath);
            Debug.WriteLine($"Loaded favorite notes from: {filePath}");
            return JsonSerializer.Deserialize<List<Guid>>(json) ?? new List<Guid>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error loading favorite notes from {Path.Combine(GetFullPath(_favoritesFolderPath), $"{username}.json")}: {ex.Message}");
            return new List<Guid>();
        }
    }

    public async Task<List<Notes>> LoadFavoriteNotesDetailsAsync(string username)
    {
        var favoriteIds = await LoadFavoriteNotesAsync(username);
        var allNotes = await LoadNotesAsync(username);
        return allNotes.Where(note => favoriteIds.Contains(note.Id)).ToList();
    }

    public async Task<string?> SaveNoteImageAsync(StorageFile? imageFile, Guid noteId, string username)
    {
        if (imageFile == null) return null;

        try
        {
            var noteImagesFolder = GetNoteImagesFolderPath(username);
            var fullFolderPath = GetFullPath(noteImagesFolder);

            Directory.CreateDirectory(fullFolderPath);

            var destinationPath = Path.Combine(fullFolderPath, $"{noteId}{Path.GetExtension(imageFile.Name).ToLower()}");
            if (File.Exists(destinationPath))
            {
                var existingFile = await StorageFile.GetFileFromPathAsync(destinationPath);
                await existingFile.DeleteAsync();
                Debug.WriteLine($"Deleted existing image at: {destinationPath}");
            }

            await imageFile.CopyAsync(await StorageFolder.GetFolderFromPathAsync(fullFolderPath), $"{noteId}{Path.GetExtension(imageFile.Name).ToLower()}");
            Debug.WriteLine($"Saved image to: {destinationPath}");
            return destinationPath;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error saving image to {GetNoteImagesFolderPath(username)}: {ex.Message}");
            return null;
        }
    }

    public async Task<string?> SaveUserAvatarAsync(StorageFile? imageFile, string username)
    {
        if (imageFile == null) return null;

        try
        {
            await DeleteUserAvatarAsync(username);

            var userImagesFolder = GetUserImagesFolderPath();
            var destinationPath = Path.Combine(GetFullPath(userImagesFolder), $"{username}{Path.GetExtension(imageFile.Name).ToLower()}");

            await imageFile.CopyAsync(await StorageFolder.GetFolderFromPathAsync(GetFullPath(userImagesFolder)), $"{username}{Path.GetExtension(imageFile.Name).ToLower()}");
            Debug.WriteLine($"Saved avatar to: {destinationPath}");
            return destinationPath;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error saving avatar to {Path.Combine(GetFullPath(_userImagesFolderPath), $"{username}{Path.GetExtension(imageFile.Name).ToLower()}")}: {ex.Message}");
            return null;
        }
    }

    public async Task DeleteUserAvatarAsync(string username)
    {
        try
        {
            var userImagesFolder = GetFullPath(GetUserImagesFolderPath());
            var files = Directory.GetFiles(userImagesFolder, $"{username}.*");
            foreach (var file in files)
            {
                var avatarFile = await StorageFile.GetFileFromPathAsync(file);
                await avatarFile.DeleteAsync();
                Debug.WriteLine($"Deleted avatar file: {file}");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error deleting avatar for {username}: {ex.Message}");
        }
    }

    public async Task DeleteUserDataAsync(string username)
    {
        try
        {
            var notesFilePath = Path.Combine(GetFullPath(_notesFolderPath), $"{username}.json");
            if (File.Exists(notesFilePath))
            {
                File.Delete(notesFilePath);
                Debug.WriteLine($"Deleted notes file: {notesFilePath}");
            }

            var favoritesFilePath = Path.Combine(GetFullPath(_favoritesFolderPath), $"{username}.json");
            if (File.Exists(favoritesFilePath))
            {
                File.Delete(favoritesFilePath);
                Debug.WriteLine($"Deleted favorites file: {favoritesFilePath}");
            }

            var noteImagesFolder = GetFullPath(GetNoteImagesFolderPath(username));
            if (Directory.Exists(noteImagesFolder))
            {
                Directory.Delete(noteImagesFolder, true);
                Debug.WriteLine($"Deleted note images folder: {noteImagesFolder}");
            }

            await DeleteUserAvatarAsync(username);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error deleting user data for {username}: {ex.Message}");
            throw;
        }
    }
}