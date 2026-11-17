using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace NotesApi.Tests;

public sealed class NotesApiTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Health_ReturnsOk()
    {
        HttpResponseMessage response = await _client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CreateNote_ThenGetIt_ReturnsTheSameNote()
    {
        HttpResponseMessage created = await _client.PostAsJsonAsync("/notes", new CreateNoteRequest("  Mua sữa  ", "2 hộp"));

        Assert.Equal(HttpStatusCode.Created, created.StatusCode);
        Note? note = await created.Content.ReadFromJsonAsync<Note>();
        Assert.NotNull(note);
        Assert.Equal("Mua sữa", note.Title);
        Assert.Equal($"/notes/{note.Id}", created.Headers.Location?.ToString());

        Note? fetched = await _client.GetFromJsonAsync<Note>($"/notes/{note.Id}");
        Assert.Equal(note, fetched);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task CreateNote_WithoutTitle_IsRejected(string? title)
    {
        HttpResponseMessage response = await _client.PostAsJsonAsync("/notes", new CreateNoteRequest(title, "x"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateNote_WithTooLongTitle_IsRejected()
    {
        string title = new('a', NoteEndpoints.MaxTitleLength + 1);

        HttpResponseMessage response = await _client.PostAsJsonAsync("/notes", new CreateNoteRequest(title, null));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetNote_Unknown_ReturnsNotFound()
    {
        HttpResponseMessage response = await _client.GetAsync($"/notes/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
