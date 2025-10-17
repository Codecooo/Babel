using Babel.Commands;
using Cocona;

// Inisialisasi aplikasi
var app = CoconaLiteApp.Create();
app.AddCommands<DbInitCommand>();

app.Run();