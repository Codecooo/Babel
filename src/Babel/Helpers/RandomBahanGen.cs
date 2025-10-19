using Babel.Models;
using Bogus;

namespace Babel.Helpers;

public static class RandomBahanGen
{
    private static readonly Lazy<string[]> BaseNamaBahan = new(() => 
        File.ReadAllLines(Path.Combine(AppContext.BaseDirectory, "data", "nama-bahan.txt")));    
    
    private static readonly Lazy<string[]> BaseNamaVendor = new(() => 
        File.ReadAllLines(Path.Combine(AppContext.BaseDirectory, "data", "vendor-bahan.txt")));
    
    private static List<BahanBaku> _bahanHvs = new();
    private static List<BahanBaku> _bahanLaminasi = new();
    private static List<BahanBaku> _bahanLaminating = new();
    private static List<BahanBaku> _bahanId = new();
    private static List<BahanBaku> _bahanBanner = new();
    private static List<BahanBaku> _bahanDtf = new();
    
    private static readonly Lazy<List<string>> AllCombinations = new(() =>
    {
        var faker = new Faker();
        Randomizer.Seed = new Random(123);
        
        var combinatins = (
            from vendor in BaseNamaVendor.Value
            from bahan in BaseNamaBahan.Value
            select $"{vendor} {bahan}"
        ).ToList();

        // Shuffle 
        return faker.Random.Shuffle(combinatins).ToList();
    });

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
    
    private static int _currentIndex;
    
    public static string GenerateNamaBahan(Faker faker)
    {
        if (_currentIndex >= AllCombinations.Value.Count)
            throw new InvalidOperationException("Tidak ada kombinasi unik lagi.");

        return AllCombinations.Value[_currentIndex++];
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
