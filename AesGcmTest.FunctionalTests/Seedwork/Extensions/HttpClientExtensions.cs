using FluentAssertions;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.Json;

namespace AesGcmTest.FunctionalTests;

public static class HttpClientExtensions
{
    public static async Task GetAndExpectNotFoundAsync(this HttpClient client, string url)
    {
        var response = await client.GetAsync(url);
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    public static async Task<T> GetAsync<T>(this HttpClient client, string url)
    {
        using var getResponse = await client.GetAsync(url);
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        getResponse.IsSuccessStatusCode.Should().BeTrue();
        var responseModel = await getResponse.DeserializeAsync<T>();
        responseModel.Should().NotBeNull();
        return responseModel;
    }

    public static async Task PostAndExpectBadRequestAsync(this HttpClient client, string url, object request)
    {
        var json = JsonSerializer.Serialize(request);
        var httpContent = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);
        var response = await client.PostAsync(url, httpContent);
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    public static async Task PostAndExpectCreatedAsync(this HttpClient client, string url, object request)
    {
        var json = JsonSerializer.Serialize(request);
        var httpContent = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);
        var response = await client.PostAsync(url, httpContent);
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    public static async Task PutAndExpectNoContentAsync(this HttpClient client, string url)
    {
        var response = await client.PutAsync(url, null);
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    public static async Task PutAndExpectNoContentAsync(this HttpClient client, string url, object request)
    {
        var json = JsonSerializer.Serialize(request);
        var httpContent = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);
        var response = await client.PutAsync(url, httpContent);
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    public static async Task PutAndExpectNotFoundAsync(this HttpClient client, string url)
    {
       
        var response = await client.PutAsync(url, null);
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    public static async Task PutAndExpectBadRequestAsync(this HttpClient client, string url)
    {
        var response = await client.PutAsync(url, null);
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
