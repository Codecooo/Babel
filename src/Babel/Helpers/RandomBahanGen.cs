using Babel.Models;
using Bogus;

namespace Babel.Helpers;

public static class RandomBahanGen
{
    private static readonly Lazy<string[]> _baseNamaBahan = new(() => 
        File.ReadAllLines(Path.Combine(AppContext.BaseDirectory, "data", "nama-bahan.txt")));    
    
    private static readonly Lazy<string[]> _baseNamaVendor = new(() => 
        File.ReadAllLines(Path.Combine(AppContext.BaseDirectory, "data", "vendor-bahan.txt")));
    
    private static readonly HashSet<string> _allNamaBahan = new(StringComparer.OrdinalIgnoreCase);
    
    private static List<BahanBaku> _bahanHvs = new();
    private static List<BahanBaku> _bahanLaminasi = new();
    private static List<BahanBaku> _bahanLaminating = new();
    private static List<BahanBaku> _bahanId = new();
    private static List<BahanBaku> _bahanBanner = new();
    private static List<BahanBaku> _bahanDtf = new();

    public static void InitializeBahanLists(IEnumerable<BahanBaku> bahanList)
    {
        var bahanBakus = bahanList.ToList();
        _bahanHvs = bahanBakus.Where(b => b.JenisBahan == JenisBahan.HVS).ToList();
        _bahanLaminasi = bahanBakus.Where(b => b.JenisBahan == JenisBahan.PlastikLaminasi).ToList();
        _bahanLaminating = bahanBakus.Where(b => b.JenisBahan == JenisBahan.PlastikLaminating).ToList();
        _bahanId = bahanBakus.Where(b => b.JenisBahan == JenisBahan.IDCard).ToList();
        _bahanBanner = bahanBakus.Where(b => b.JenisBahan == JenisBahan.Banner).ToList();
        _bahanDtf = bahanBakus.Where(b => b.JenisBahan == JenisBahan.DTFPet).ToList();
    }

    public static string GenerateNamaBahan(Faker faker)
    {
        while (true)
        {
            var bahan = faker.PickRandom(_baseNamaBahan.Value);
            var vendor = faker.PickRandom(_baseNamaVendor.Value);
            var name = $"{vendor} {bahan}";
            if (_allNamaBahan.Add(name))
                return name;
        }
    }

    public static JenisBahan GenerateJenisBahan(string namaBahan)
    {
        if (namaBahan.Contains("Kertas", StringComparison.OrdinalIgnoreCase)) return JenisBahan.HVS;
        if (namaBahan.Contains("ID Card", StringComparison.OrdinalIgnoreCase)) return JenisBahan.IDCard;
        if (namaBahan.Contains("Laminating", StringComparison.OrdinalIgnoreCase)) return JenisBahan.PlastikLaminating;
        if (namaBahan.Contains("Laminasi", StringComparison.OrdinalIgnoreCase)) return JenisBahan.PlastikLaminasi;
        if (namaBahan.Contains("Banner", StringComparison.OrdinalIgnoreCase)) return JenisBahan.Banner;

        return JenisBahan.DTFPet;
    }

    public static PemakaianBahan GeneratePemakaianBahan(Produk produk, Faker faker)
    {
        List<BahanBaku> sourceList;

        if (produk.NamaProduk.Contains("Kartu", StringComparison.OrdinalIgnoreCase) ||
            produk.NamaProduk.Contains("ID", StringComparison.OrdinalIgnoreCase))
            sourceList = _bahanId;
        else if (produk.NamaProduk.Contains("Banner", StringComparison.OrdinalIgnoreCase))
            sourceList = _bahanBanner;
        else if (produk.NamaProduk.Contains("DTF", StringComparison.OrdinalIgnoreCase))
            sourceList = _bahanDtf;
        else if (produk.NamaProduk.Contains("Laminasi", StringComparison.OrdinalIgnoreCase))
            sourceList = _bahanLaminasi;
        else if (produk.NamaProduk.Contains("Laminating", StringComparison.OrdinalIgnoreCase))
            sourceList = _bahanLaminating;
        else
            sourceList = _bahanHvs;

        if (sourceList.Count == 0)
            sourceList = _bahanHvs;

        var bahan = faker.PickRandom(sourceList);

        return new PemakaianBahan(bahan.IdBahan, produk.IdProduk);
    }
}
