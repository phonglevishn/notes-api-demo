using System.Collections.Concurrent;

namespace NotesApi;

public sealed record Note(Guid Id, string Title, string Body, DateTimeOffset CreatedAt);

public sealed record CreateNoteRequest(string? Title, string? Body);

/// <summary>Kho ghi chú trong bộ nhớ. Mất dữ liệu khi khởi động lại: đủ cho bản demo.</summary>
public sealed class NoteStore
{
    private readonly ConcurrentDictionary<Guid, Note> _notes = new();

    public Note Add(string title, string body)
    {
        var note = new Note(Guid.NewGuid(), title, body, DateTimeOffset.UtcNow);
        _notes[note.Id] = note;
        return note;
    }

    public Note? Find(Guid id) => _notes.GetValueOrDefault(id);

    public bool Remove(Guid id) => _notes.TryRemove(id, out _);

    public IReadOnlyList<Note> List() => [.. _notes.Values.OrderBy(n => n.CreatedAt)];
}

public static class NoteEndpoints
{
    public const int MaxTitleLength = 100;

    public static void MapNotes(this IEndpointRouteBuilder app)
    {
        app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

        app.MapGet("/notes", (NoteStore store) => Results.Ok(store.List()));

        app.MapGet("/notes/{id:guid}", (Guid id, NoteStore store) =>
            store.Find(id) is { } note ? Results.Ok(note) : Results.NotFound());

        app.MapDelete("/notes/{id:guid}", (Guid id, NoteStore store) =>
            store.Remove(id) ? Results.NoContent() : Results.NotFound());

        app.MapPost("/notes", (CreateNoteRequest request, NoteStore store) =>
        {
            Dictionary<string, string[]> errors = Validate(request);
            if (errors.Count > 0)
            {
                return Results.ValidationProblem(errors);
            }

            Note note = store.Add(request.Title!.Trim(), request.Body?.Trim() ?? string.Empty);
            return Results.Created($"/notes/{note.Id}", note);
        });
    }

    private static Dictionary<string, string[]> Validate(CreateNoteRequest request)
    {
        var errors = new Dictionary<string, string[]>();
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            errors["title"] = ["Title is required."];
        }
        else if (request.Title.Trim().Length > MaxTitleLength)
        {
            errors["title"] = [$"Title must be at most {MaxTitleLength} characters."];
        }

        return errors;
    }
}
