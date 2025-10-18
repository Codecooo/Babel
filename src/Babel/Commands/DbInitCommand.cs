using Babel.Helpers;
using Cocona;
using Npgsql;

namespace Babel.Commands;

public sealed class DbInitCommand
{
    [Command("init", Description = "Insialisasi database babel untuk siap dimasukkan data")]
    public async Task InitDb()
    {
        var connectionString = BuildConnection();
        await using var dataSource = NpgsqlDataSource.Create(connectionString);

        Console.WriteLine("Database babel akan dibuat....");
        const string command = "CREATE DATABASE babel";
        var success = await DbCommand.Execute(dataSource, command, "Database babel telah dibuat!");
        
        if (!success) return;
        
        // Ubah database di connection string menjadi babel
        var connections = connectionString.Split(";");
        connections[connections.Length - 1] = "Database=babel;";
        
        connectionString = string.Join(";", connections);
        SaveConnectionString(connectionString);
        await CreateSchema(connectionString);
    }

    private static string BuildConnection()
    {
        Console.WriteLine("Masukkan detail koneksi PostgreSQL, tekan enter untuk nilai default");
        Console.Write("Host (default localhost): ");
        var host = ReadOrDefault("localhost");
        
        Console.Write("Port (default 5432): ");
        var port = ReadOrDefault("5432");
        Console.Write("Username (default postgres): ");
        var username = ReadOrDefault("postgres");
        Console.Write("Password: ");
        var password = PasswordInput.SecurePassword();
        Console.Write($"Database (default {username}): ");
        var database = ReadOrDefault(username);

        Console.WriteLine();
        return $"Host={host};Port={port};Username={username};Password={password};Database={database};";
    }

    private static async Task CreateSchema(string connectionString)
    {
        await using var dataSource = NpgsqlDataSource.Create(connectionString);
        
        var path = Path.Combine(AppContext.BaseDirectory, "schema", "babel.sql");

        Console.WriteLine("Struktur tabel database babel lagi dibuat...");
        await DbCommand.Execute(dataSource, await File.ReadAllTextAsync(path), "Seluruh tabel berhasil dibuat!");
    }

    private static void SaveConnectionString(string connectionString)
    {
        var path =  Path.Combine(AppContext.BaseDirectory, "connection-string.txt");
        File.Create(path).Close();
        File.WriteAllText(path, connectionString);
    }
    
    private static string ReadOrDefault(string defaultValue)
    {
        var input = Console.ReadLine();
        return string.IsNullOrWhiteSpace(input) ? defaultValue : input;
    }
}