using Babel.Helpers;
using Cocona;
using Npgsql;

namespace Babel.Commands;

public sealed class ExplodeCommand
{
    [Command("explode", Description = "Menghapus seluruh row atau data dalam masing-masing tabel database")]
    public async Task ExplodeDb()
    {
        var connectionString = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "connection-string.txt"));

        await using var dataSource = NpgsqlDataSource.Create(connectionString);
        var sql = "TRUNCATE pelanggan, karyawan, produk, bahan_baku, mesin RESTART IDENTITY CASCADE";
        await DbCommand.Execute(dataSource, sql, "Database berhasil dihancurkan");
    }
}