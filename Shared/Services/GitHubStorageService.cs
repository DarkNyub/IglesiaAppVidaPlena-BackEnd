using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace IglesiaBackend.Features.Shared.Services;

public class GitHubStorageService
{
    private readonly HttpClient _httpClient;
    private readonly string _token;
    private readonly string _repoOwner;
    private readonly string _repoName;

    // Asegúrate de poner estos valores en tu appsettings.json
    public GitHubStorageService(IConfiguration config, HttpClient httpClient)
    {
        _httpClient = httpClient;
        _token = config["GitHub:Token"] ?? throw new ArgumentNullException("GitHub Token no configurado");
        _repoOwner = config["GitHub:Owner"] ?? throw new ArgumentNullException("GitHub Owner no configurado");
        _repoName = config["GitHub:Repo"] ?? throw new ArgumentNullException("GitHub Repo no configurado");

        _httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("IglesiaApp", "1.0"));
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", _token);
    }

    public async Task<string> UploadImageAsync(string base64Image, string folderName, string fileName)
    {
        // Limpiamos el base64 si viene con la cabecera de datos de Flutter
        if (base64Image.Contains(",")) base64Image = base64Image.Split(',')[1];

        string path = $"{folderName}/{Guid.NewGuid()}_{fileName}";
        string url = $"https://api.github.com/repos/{_repoOwner}/{_repoName}/contents/{path}";

        var payload = new
        {
            message = $"Upload image {fileName}",
            content = base64Image
        };

        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        var response = await _httpClient.PutAsync(url, content);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Error subiendo a GitHub: {error}");
        }

        // Devolvemos la URL cruda (raw) para que Flutter la lea directo
        return $"https://raw.githubusercontent.com/{_repoOwner}/{_repoName}/main/{path}";
    }
}