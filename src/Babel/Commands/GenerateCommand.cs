using System.Diagnostics;
using Babel.Helpers;
using Babel.Models;
using Bogus;
using Cocona;

namespace Babel.Commands;

public sealed class GenerateCommand
{
    private static Random _randomSeed = new();
    private static readonly DateTime ReferenceDate = new(2025, 10, 15);

    [Command("generate", Aliases = ["gen"], Description = "Membuat mock data untuk database babel")]
    public async Task GenerateData([Argument] int numberOfData, int seed = 123)
    {
        Stopwatch stopwatch = new();
        
        _randomSeed = new Random(seed == 0 ? (int)DateTime.Now.Ticks : seed);
        Console.WriteLine("Sedang membuat data fake untuk babel....");
        Console.WriteLine();
        
        // Generate data untuk seluruh tabel
        stopwatch.Start();
        var pelanggan = GeneratePelanggan(numberOfData);
        var karyawan = GenerateKaryawan(numberOfData);
        var mesin = GenerateMesin(numberOfData);
        var produk = GenerateProduk(numberOfData);
        var produksi = GenerateProduksi(numberOfData, karyawan);
        var pesanan = GeneratePesanan(numberOfData, karyawan, pelanggan, produksi);
        var detailPesanan = GenerateDetailPesanan(numberOfData, pesanan, produk);
        var pembayaran = GeneratePembayaran(numberOfData, pesanan, detailPesanan, produk);
        var bahanBaku = GenerateBahanBaku(numberOfData);
        var pemakaianMesin = GeneratePemakaianMesin(numberOfData, mesin, produksi);
        var pemakaianBahan = GeneratePemakaianBahan(bahanBaku, produk);

        Console.WriteLine();
        Console.WriteLine("✅ Seluruh data berhasil dibuat!");
        Console.ResetColor();

        Console.WriteLine("Data akan disimpan ke dalam file CSV....");
        await SaveToCsv(pelanggan, karyawan, mesin, produksi, pemakaianMesin, pemakaianBahan, produk, pembayaran,
            detailPesanan, pesanan, bahanBaku);
        stopwatch.Stop();
        
        Console.ResetColor();
        Console.WriteLine($"Waktu untuk generate data dan menyimpan ke disk sebanyak {stopwatch.ElapsedMilliseconds} ms");
    }

    private List<Pelanggan> GeneratePelanggan(int numberOfData)
    {
        Randomizer.Seed = _randomSeed;
        
        var faker = new Faker<Pelanggan>(locale: "id_ID")
            .StrictMode(true)
            .RuleFor(p => p.IdPelanggan, f => f.Random.Guid())
            .RuleFor(p => p.Alamat, f => f.Address.StreetAddress(true))
            .RuleFor(p => p.Nama, f => f.Name.FullName())
            .RuleFor(p => p.Email, (f, p) => f.Internet.Email(p.Nama))
            .RuleFor(p => p.NoHp, f => f.Phone.PhoneNumber("+62##########"));
        
        var list = faker.Generate(numberOfData);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("✅ Data pelanggan berhasil dibuat!");
        return list;
    }

    private List<Karyawan> GenerateKaryawan(int numberOfData)
    {
        Randomizer.Seed = _randomSeed;
        
        var faker = new Faker<Karyawan>(locale: "id_ID")
            .StrictMode(true)
            .RuleFor(k => k.IdKaryawan, f => f.Random.Guid())
            .RuleFor(k => k.Alamat, f => f.Address.StreetAddress(true))
            .RuleFor(k => k.NamaDepan, f => f.Name.FirstName())
            .RuleFor(k => k.NamaBelakang, f => f.Name.LastName())
            .RuleFor(k => k.NoHp, f => f.Phone.PhoneNumber("+62##########"))
            .RuleFor(k => k.Jabatan, f => f.PickRandom<Jabatan>())
            .RuleFor(k => k.Email, (f, p) => f.Internet.Email(p.NamaDepan + p.NamaBelakang));
        
        var list = faker.Generate(numberOfData);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("✅ Data karyawan berhasil dibuat!");
        return list;
    }

    private List<Mesin> GenerateMesin(int numberOfData)
    {
        Randomizer.Seed = _randomSeed;

        int idCounter = 1;
        
        var faker = new Faker<Mesin>(locale: "id_ID")
            .StrictMode(true)
            .RuleFor(m => m.IdMesin, _ => idCounter++)
            .RuleFor(m => m.JenisMesin, f => f.PickRandom<JenisMesin>().ToDb())
            .RuleFor(m => m.NamaMesin, NamaMesinGen.GenerateNamaMesin)
            .RuleFor(m => m.StatusMesin, f => f.PickRandom<StatusMesin>());
        
        var list = faker.Generate(numberOfData);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("✅ Data mesin berhasil dibuat!");
        return list;
    }

    private List<Produk> GenerateProduk(int numberOfData)
    {
        Randomizer.Seed = _randomSeed;

        int idCounter = 1;
        
        var faker = new Faker<Produk>(locale: "id_ID")
            .StrictMode(true)
            .RuleFor(p => p.IdProduk, _ => idCounter++)
            .RuleFor(p => p.NamaProduk, RandomProdukGen.GenerateNamaProduk)
            .RuleFor(p => p.JenisProduk, (_, p) => RandomProdukGen.GenerateJenisProduk(p.NamaProduk))
            .RuleFor(p => p.Ukuran, (_, p) => RandomProdukGen.GenerateUkuran(p.NamaProduk).ToDb())
            .RuleFor(p => p.HargaPerUnit, f => Math.Round(f.Random.Decimal(500, 3_00_000) / 100) * 100);
        
        var list = faker.Generate(numberOfData);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("✅ Data produk berhasil dibuat!");
        return list;
    }

    private List<Pesanan> GeneratePesanan(int numberOfData, List<Karyawan> listKaryawan, List<Pelanggan> listPelanggan,List<Produksi> listProduksi)
    {
        Randomizer.Seed = _randomSeed;
        var customerService = listKaryawan.Where(k => k.Jabatan == Jabatan.CustomerService).ToList();
        var availableProduksi = new List<Produksi>(listProduksi.Take(numberOfData));
        
        var faker = new Faker<Pesanan>(locale: "id_ID")
            .StrictMode(true)
            .RuleFor(p => p.IdPesanan, f => f.Random.Guid())
            .RuleFor(p => p.IdProduksi, f =>
            {
                if (availableProduksi.Count == 0)
                    throw new InvalidOperationException(
                        "Data produksi sudah tidak cukup untuk memenuhi persyaratan relasi one to many di tabel pesanan");
                
                var selected = f.PickRandom(availableProduksi);
                availableProduksi.Remove(selected);
                return selected.IdProduksi;
            })
            .RuleFor(p => p.IdPelanggan, f => f.PickRandom(listPelanggan).IdPelanggan)
            .RuleFor(p => p.IdKaryawan, f => f.PickRandom(customerService).IdKaryawan)
            .RuleFor(p => p.TanggalPesanan, f => f.Date.Between(ReferenceDate.AddMonths(-12), ReferenceDate))
            .RuleFor(p => p.StatusPesanan, f => f.PickRandom<StatusPesanan>());
        
        var list = faker.Generate(numberOfData);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("✅ Data pesanan berhasil dibuat!");
        return list;
    }

    private List<Produksi> GenerateProduksi(int numberOfData, List<Karyawan> listKaryawan)
    {
        Randomizer.Seed = _randomSeed;
        
        var operators = listKaryawan
            .Where(k => k.Jabatan != Jabatan.CustomerService)
            .ToList();
        
        int idCounter = 1;
        
        var faker = new Faker<Produksi>(locale: "id_ID")
            .StrictMode(true)
            .RuleFor(p => p.IdProduksi, _ => idCounter++)
            .RuleFor(p => p.IdKaryawan, f => f.PickRandom(operators).IdKaryawan)
            .RuleFor(p => p.TanggalProduksi,
                f => f.Date.BetweenDateOnly(
                    DateOnly.FromDateTime(ReferenceDate.AddMonths(-12)),
                    DateOnly.FromDateTime(ReferenceDate)))
            .RuleFor(p => p.Keterangan, f => f.Lorem.Paragraph());
        
        var list = faker.Generate(numberOfData);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("✅ Data produksi berhasil dibuat!");
        return list;
    }

    private List<DetailPesanan> GenerateDetailPesanan(int numberOfData, List<Pesanan> listPesanan, List<Produk> listProduk)
    {
        Randomizer.Seed = _randomSeed;
        var faker = new Faker("id_ID");

        var detailPesananList = new List<DetailPesanan>();

        // Generate masing-masing pesanan agar memastikan pesanan pasti punya detail
        foreach (var pesanan in listPesanan)
        {
            var produk = faker.PickRandom(listProduk);
            detailPesananList.Add(new DetailPesanan(
                IdPesanan: pesanan.IdPesanan,
                IdProduk: produk.IdProduk,
                JumlahProduk: faker.Random.Number(1, 1000)
            ));
        }

        // Generate detail pesanan lagi sampe dapet ke kuota yang diinginkan
        while (detailPesananList.Count < numberOfData)
        {
            var pesanan = faker.PickRandom(listPesanan);
            var produk = faker.PickRandom(listProduk);
            detailPesananList.Add(new DetailPesanan(
                IdPesanan: pesanan.IdPesanan,
                IdProduk: produk.IdProduk,
                JumlahProduk: faker.Random.Number(1, 1000)
            ));
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"✅ Data detail pesanan berhasil dibuat!");
        return detailPesananList;
    }

    private List<Pembayaran> GeneratePembayaran(int numberOfData, List<Pesanan> listPesanan, List<DetailPesanan> listDetailPesanan, 
        List<Produk> listProduk)
    {
        Randomizer.Seed = _randomSeed;

        var availablePesanan = new List<Pesanan>(listPesanan.Take(numberOfData));

        var faker = new Faker<Pembayaran>(locale: "id_ID")
            .StrictMode(true)
            .RuleFor(p => p.IdPembayaran, f => f.Random.Guid())
            .RuleFor(p => p.MetodePembayaran, f => f.PickRandom<MetodePembayaran>())
            .RuleFor(p => p.TanggalPembayaran, f => f.Date.Between(ReferenceDate.AddMonths(-12), ReferenceDate))
            .RuleFor(p => p.IdPesanan, f =>
            {
                if (availablePesanan.Count == 0)
                    throw new InvalidOperationException("Tidak ada pesanan tersisa untuk relasi one to one.");

                var selected = f.PickRandom(availablePesanan);
                availablePesanan.Remove(selected);
                return selected.IdPesanan;
            })
            .RuleFor(p => p.StatusPembayaran, (_, p) =>
            {
                var pesanan = listPesanan.FirstOrDefault(pe => pe.IdPesanan == p.IdPesanan);
                return PembayaranGen.GenerateStatusPembayaran(pesanan);
            })
            .RuleFor(p => p.TotalPembayaran, (_, p) =>
            {
                var pesanan = listPesanan.FirstOrDefault(pe => pe.IdPesanan == p.IdPesanan);
                return PembayaranGen.GetTotalPembayaran(pesanan, listDetailPesanan, listProduk);
            });

        var list = faker.Generate(Math.Min(numberOfData, listPesanan.Count));
    
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("✅ Data pembayaran berhasil dibuat!");
        return list;
    }

    private List<BahanBaku> GenerateBahanBaku(int numberOfData)
    {
        Randomizer.Seed = _randomSeed;

        int idCounter = 1;

        var faker = new Faker<BahanBaku>(locale: "id_ID")
            .StrictMode(true)
            .RuleFor(b => b.IdBahan, _ => idCounter++)
            .RuleFor(b => b.NamaBahan, RandomBahanGen.GenerateNamaBahan)
            .RuleFor(b => b.JenisBahan, (_, b) => RandomBahanGen.GenerateJenisBahan(b.NamaBahan))
            .RuleFor(b => b.StokBahan, f => f.Random.Number(1000));
        
        var list = faker.Generate(numberOfData);
        
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("✅ Data bahan baku berhasil dibuat!");
        return list;
    }

    private List<PemakaianMesin> GeneratePemakaianMesin(
        int numberOfData,
        List<Mesin> listMesin,
        List<Produksi> listProduksi)
    {
        Randomizer.Seed = _randomSeed;
        var faker = new Faker();
        var result = new List<PemakaianMesin>(numberOfData);
        var usedPairs = new HashSet<(int produksi, int mesin)>();

        int maxProduksi = listProduksi.Count;
        int maxMesin = listMesin.Count;

        // Maksimum kombinasi sesuai data yang inginkan
        int maxPossible = maxProduksi * maxMesin;
        if (numberOfData > maxPossible)
            numberOfData = maxPossible;

        while (result.Count < numberOfData)
        {
            var produksi = faker.PickRandom(listProduksi).IdProduksi;
            var mesin = faker.PickRandom(listMesin).IdMesin;

            var pair = (produksi, mesin);
            if (!usedPairs.Add(pair))
                continue; // duplikat, skip

            result.Add(new PemakaianMesin(
                IdProduksi: produksi,
                IdMesin: mesin,
                WaktuPemakaian: faker.Date.Between(ReferenceDate.AddMonths(-12), ReferenceDate)
            ));
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("✅ Data pemakaian mesin berhasil dibuat!");
        return result;
    }

    private List<PemakaianBahan> GeneratePemakaianBahan(List<BahanBaku> listBahan, List<Produk> listProduk)
    {
        Randomizer.Seed = _randomSeed;
        var faker = new Faker(locale: "id_ID");
        
        RandomBahanGen.InitializeBahanLists(listBahan);
        var list = new List<PemakaianBahan>();

        foreach (var produk in listProduk)
        {
            var pemakaianBahan = RandomBahanGen.GeneratePemakaianBahan(produk, faker);
            list.Add(pemakaianBahan);
        }
        
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("✅ Data pemakaian bahan berhasil dibuat!");
        return list;
    }

    private async Task SaveToCsv(List<Pelanggan> listPelanggan, List<Karyawan> listKaryawan, List<Mesin> listMesin,
        List<Produksi> listProduksi,
        List<PemakaianMesin> listPemakaianMesin, List<PemakaianBahan> listPemakaianBahan, List<Produk> listProduk,
        List<Pembayaran> listPembayaran, List<DetailPesanan> listDetailPesanan, List<Pesanan> listPesanan, List<BahanBaku> listBahan)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "data");

        var tasks = new[]
        {
            Task.Run(() => CsvOps.WriteCsv(Path.Combine(path, "pelanggan.csv"), listPelanggan)),
            Task.Run(() => CsvOps.WriteCsv(Path.Combine(path, "karyawan.csv"), listKaryawan)),
            Task.Run(() => CsvOps.WriteCsv(Path.Combine(path, "mesin.csv"), listMesin)),
            Task.Run(() => CsvOps.WriteCsv(Path.Combine(path, "produksi.csv"), listProduksi)),
            Task.Run(() => CsvOps.WriteCsv(Path.Combine(path, "produk.csv"), listProduk)),
            Task.Run(() => CsvOps.WriteCsv(Path.Combine(path, "pemakaian_mesin.csv"), listPemakaianMesin)),
            Task.Run(() => CsvOps.WriteCsv(Path.Combine(path, "pemakaian_bahan.csv"), listPemakaianBahan)),
            Task.Run(() => CsvOps.WriteCsv(Path.Combine(path, "pembayaran.csv"), listPembayaran)),
            Task.Run(() => CsvOps.WriteCsv(Path.Combine(path, "detail_pesanan.csv"), listDetailPesanan)),
            Task.Run(() => CsvOps.WriteCsv(Path.Combine(path, "pesanan.csv"), listPesanan)),
            Task.Run(() => CsvOps.WriteCsv(Path.Combine(path, "bahan_baku.csv"), listBahan)),
        };
        
        await Task.WhenAll(tasks);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"✅ Semua data berhasil disimpan dalam CSV file di {path}");
    }
}
