using IglesiaBackend.Features.Shared.Services; // Para el GitHubStorageService

namespace IglesiaBackend.Features.Settings;

public class SettingsService
{
    private readonly SettingsRepository _repository;
    private readonly GitHubStorageService _gitHubStorage;

    public SettingsService(SettingsRepository repository, GitHubStorageService gitHubStorage)
    {
        _repository = repository;
        _gitHubStorage = gitHubStorage;
    }

    public async Task<List<SettingDto>> GetAllSettingsAsync()
    {
        var settings = await _repository.GetAllAsync();
        return settings.Select(s => new SettingDto { Key = s.Key, Value = s.Value }).ToList();
    }

    // Tu método de subida de imagen de GitHub, ahora en la capa Service
    public async Task<string> UploadAndSaveImageAsync(string base64Image, string typeKey)
    {
        if (string.IsNullOrEmpty(base64Image))
            throw new ArgumentException("La imagen en Base64 es requerida.");

        // Subimos a GitHub
        string fileName = $"{typeKey}_{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}.jpg";
        string url = await _gitHubStorage.UploadImageAsync(base64Image, "system", fileName);

        // Guardamos en la base de datos en la tabla Settings
        await _repository.UpsertAsync(typeKey, url);

        return url;
    }
}