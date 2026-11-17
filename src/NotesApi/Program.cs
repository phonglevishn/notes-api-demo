using NotesApi;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<NoteStore>();

WebApplication app = builder.Build();
app.MapNotes();
app.Run();

// Cần cho WebApplicationFactory trong test.
public partial class Program;
