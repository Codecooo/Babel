using System.Diagnostics;
using Babel.Helpers;
using Cocona;
using Npgsql;

namespace Babel.Commands;

public sealed class LoadCommand
{
    [Command("load", Aliases = ["-l"], Description = "Load data yang sudah di generate ke database")]
    public async Task LoadDb()
    {
        Stopwatch stopwatch = new();
        var connectionString = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "connection-string.txt"));

        Console.WriteLine("Data akan dimuat ke database...");
        Console.WriteLine();
        stopwatch.Start();
        await LoadParentTables(connectionString);
        await LoadChildTables(connectionString);
        
        stopwatch.Stop();
        Console.WriteLine();

        Console.WriteLine("✅ Seluruh data telah dimuat ke dalam database");
        Console.ResetColor();
        Console.WriteLine($"Waktu operasi untuk memasukkan data ke dalam database adalah {stopwatch.Elapsed.Milliseconds} ms");
    }
    
    private static string GetSqlCommand(string tableName) => 
        $"COPY {tableName} FROM STDIN WITH (FORMAT csv, HEADER true)";
    
    private static string GetPath(string tableName) => 
        Path.Combine(AppContext.BaseDirectory, "data", $"{tableName}.csv");
    
    private static async Task LoadParentTables(string connectionString)
    {
        await using var dataSource = NpgsqlDataSource.Create(connectionString);
        
        // Connection untuk masing-masing query tabel
        await using var connectionPelanggan = await dataSource.OpenConnectionAsync();
        await using var connectionKaryawan = await dataSource.OpenConnectionAsync();
        await using var connectionMesin = await dataSource.OpenConnectionAsync();
        await using var connectionProduk = await dataSource.OpenConnectionAsync();
        await using var connectionBahan = await dataSource.OpenConnectionAsync();
        
        // Buat writer untuk impor data CSV ke dalam tabel
        await using var writerPelanggan = await connectionPelanggan.BeginTextImportAsync(GetSqlCommand("pelanggan"));
        await using var writerKaryawan = await connectionKaryawan.BeginTextImportAsync(GetSqlCommand("karyawan"));
        await using var writerMesin = await connectionMesin.BeginTextImportAsync(GetSqlCommand("mesin"));
        await using var writerProduk = await connectionProduk.BeginTextImportAsync(GetSqlCommand("produk"));
        await using var writerBahan = await connectionBahan.BeginTextImportAsync(GetSqlCommand("bahan_baku"));
        
        // Baca data dari CSV untuk masing-masing tabel
        var taskPelanggan = CsvOps.ReadCsv(writerPelanggan, GetPath("pelanggan"));
        var taskKaryawan = CsvOps.ReadCsv(writerKaryawan, GetPath("karyawan"));
        var taskMesin = CsvOps.ReadCsv(writerMesin, GetPath("mesin"));
        var taskProduk = CsvOps.ReadCsv(writerProduk, GetPath("produk"));
        var taskBahan = CsvOps.ReadCsv(writerBahan, GetPath("bahan_baku"));
        
        await Task.WhenAll(taskPelanggan, taskKaryawan, taskMesin, taskProduk, taskBahan);
    }

    private static async Task LoadChildTables(string connectionString)
    {
        await using var dataSource = NpgsqlDataSource.Create(connectionString);
        
        // Wrap transaksi pesanan sama produksi ke { supaya dispose benar
        {
            await using var connectionProduksi = await dataSource.OpenConnectionAsync();
            await using var writerProduksi = await connectionProduksi.BeginTextImportAsync(GetSqlCommand("produksi"));
            await CsvOps.ReadCsv(writerProduksi, GetPath("produksi"));
        }

        {
            await using var connectionPesanan = await dataSource.OpenConnectionAsync();
            await using var writerPesanan = await connectionPesanan.BeginTextImportAsync(GetSqlCommand("pesanan"));
            await CsvOps.ReadCsv(writerPesanan, GetPath("pesanan"));
        } 
        
        // Load tabel yang lainnya ketika produksi dan pesanan sdh loaded
        await using var connectionPembayaran = await dataSource.OpenConnectionAsync();
        await using var connectionDetailPesanan = await dataSource.OpenConnectionAsync();
        await using var connectionPmMesin = await dataSource.OpenConnectionAsync();
        await using var connectionPmBahan = await dataSource.OpenConnectionAsync();
        
        await using var writerPembayaran = await connectionPembayaran.BeginTextImportAsync(GetSqlCommand("pembayaran"));
        await using var writerDetailPesanan = await connectionDetailPesanan.BeginTextImportAsync(GetSqlCommand("detail_pesanan"));
        await using var writerPmMesin = await connectionPmMesin.BeginTextImportAsync(GetSqlCommand("pemakaian_mesin"));
        await using var writerPmBahan = await connectionPmBahan.BeginTextImportAsync(GetSqlCommand("pemakaian_bahan"));
        
        var taskPembayaran = CsvOps.ReadCsv(writerPembayaran, GetPath("pembayaran"));
        var taskDetailPesanan = CsvOps.ReadCsv(writerDetailPesanan, GetPath("detail_pesanan"));
        var taskPmMesin = CsvOps.ReadCsv(writerPmMesin, GetPath("pemakaian_mesin"));
        var taskPmBahan = CsvOps.ReadCsv(writerPmBahan, GetPath("pemakaian_bahan"));

        await Task.WhenAll(taskPembayaran, taskDetailPesanan, taskPmMesin, taskPmBahan);
    }
}