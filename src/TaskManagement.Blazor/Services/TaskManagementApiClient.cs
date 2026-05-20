using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using TaskManagement.Blazor.Models;

namespace TaskManagement.Blazor.Services;

public sealed class TaskManagementApiClient(HttpClient httpClient)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync("auth/login", request, cancellationToken);
        return await ReadRequiredAsync<AuthResponse>(response, cancellationToken);
    }

    public async Task<PagedResult<ProjectSummaryDto>> GetProjectsAsync(string token, CancellationToken cancellationToken = default)
    {
        using var request = CreateAuthorizedRequest(HttpMethod.Get, "projects", token);
        using var response = await httpClient.SendAsync(request, cancellationToken);
        return await ReadRequiredAsync<PagedResult<ProjectSummaryDto>>(response, cancellationToken);
    }

    public async Task<ProjectDto> GetProjectAsync(string token, Guid projectId, CancellationToken cancellationToken = default)
    {
        using var request = CreateAuthorizedRequest(HttpMethod.Get, $"projects/{projectId}", token);
        using var response = await httpClient.SendAsync(request, cancellationToken);
        return await ReadRequiredAsync<ProjectDto>(response, cancellationToken);
    }

    public async Task<ProjectSummaryDto> CreateProjectAsync(
        string token,
        CreateProjectRequest requestModel,
        CancellationToken cancellationToken = default)
    {
        using var request = CreateAuthorizedRequest(HttpMethod.Post, "projects", token);
        request.Content = JsonContent.Create(requestModel);
        using var response = await httpClient.SendAsync(request, cancellationToken);
        return await ReadRequiredAsync<ProjectSummaryDto>(response, cancellationToken);
    }

    public async Task CreateTaskAsync(
        string token,
        Guid projectId,
        CreateTaskRequest requestModel,
        CancellationToken cancellationToken = default)
    {
        using var request = CreateAuthorizedRequest(HttpMethod.Post, $"projects/{projectId}/tasks", token);
        request.Content = JsonContent.Create(requestModel);
        using var response = await httpClient.SendAsync(request, cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
    }

    public async Task ChangeTaskStatusAsync(
        string token,
        Guid projectId,
        Guid taskId,
        string newStatus,
        CancellationToken cancellationToken = default)
    {
        using var request = CreateAuthorizedRequest(
            HttpMethod.Patch,
            $"projects/{projectId}/tasks/{taskId}/status",
            token);
        request.Content = JsonContent.Create(new ChangeTaskStatusRequest(newStatus));

        using var response = await httpClient.SendAsync(request, cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
    }

    private static HttpRequestMessage CreateAuthorizedRequest(HttpMethod method, string path, string token)
    {
        var request = new HttpRequestMessage(method, path);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return request;
    }

    private static async Task<T> ReadRequiredAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        await EnsureSuccessAsync(response, cancellationToken);
        var model = await response.Content.ReadFromJsonAsync<T>(JsonOptions, cancellationToken);
        return model ?? throw new InvalidOperationException("The API returned an empty response.");
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var message = await response.Content.ReadAsStringAsync(cancellationToken);
        throw new InvalidOperationException(string.IsNullOrWhiteSpace(message)
            ? $"The API call failed with status code {(int)response.StatusCode}."
            : message);
    }
}
