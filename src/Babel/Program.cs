using Babel.Commands;
using Cocona;

// Inisialisasi aplikasi
var app = CoconaLiteApp.Create();

app.AddCommands<DbInitCommand>();
app.AddCommands<GenerateCommand>();
app.AddCommands<LoadCommand>();
app.AddCommands<ExplodeCommand>();

app.Run();