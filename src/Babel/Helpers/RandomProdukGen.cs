using Babel.Models;
using Bogus;

namespace Babel.Helpers;

public static class RandomProdukGen
{
    private static string[] _baseNamaProduk =
        File.ReadAllLines(Path.Combine(AppContext.BaseDirectory, "data", "nama-produk.txt"));
    
    /// <summary>
    /// Buat base nama produk agar bisa di random ulang di method GenerateNamaProduk
    /// </summary>
    private static readonly Lazy<List<string>> AllProduk = new(() =>
    {
        var faker = new Faker();
        Randomizer.Seed = new Random(123);
        
        string[] colors = ["Color", "Grayscale"];
        string[] attributes = ["Kilat", "Bundle", "Ekonomi", "Spesial", "Standar", "Premium"];

        var all = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var name in _baseNamaProduk)
        {
            var isLaminasi = name.Contains("laminasi", StringComparison.CurrentCultureIgnoreCase)
                              || name.Contains("laminating", StringComparison.CurrentCultureIgnoreCase);

            foreach (var attribute in attributes)
            {
                if (isLaminasi)
                {
                    all.Add($"{name} {attribute}".Trim());
                }
                else
                {
                    foreach (var color in colors)
                        all.Add($"{name} {color} {attribute}".Trim());
                }
            }
        }

        // Shuffle random
        return faker.Random.Shuffle(all).ToList();
    });
    
    private static readonly Dictionary<string, string[]> MaterialByCategory = new()
    {
        ["Cetak"] = ["Art Paper", "HVS", "Ivory", "Bontak", "Karton", ""],
        ["Banner"] = ["Flexi China", "Flexi Korea", "Vinyl", "Backlit Film", ""],
        ["DTF"] = ["PET Film", "Polyflex", "Rubber Ink", ""],
        ["Laminating"] = ["Glossy", "Doff", "PET", ""],
        ["Laminasi"] = ["Glossy", "Doff", "PET", ""]
    };

    public static string GenerateNamaProduk(Faker faker)
    {
        var baseNamaProduk = AllProduk.Value;
        var chosen = faker.PickRandom(baseNamaProduk);

        var prefix = chosen.Split(' ', 2)[0];

        if (!MaterialByCategory.TryGetValue(prefix, out var materials))
            materials = [""];

        var material = materials[Random.Shared.Next(materials.Length)];

        // Klo udh ada ukuran skip nambah kategori material
        var hasSize = chosen.Contains("A4", StringComparison.OrdinalIgnoreCase)
                      || chosen.Contains("A3", StringComparison.OrdinalIgnoreCase)
                      || chosen.Contains("A3+", StringComparison.OrdinalIgnoreCase);

        // Random code
        var code = faker.Random.Number(1, 1000);
        
        return $"{chosen} {(hasSize ? "" : material)} V{code}".Trim();
    }

    public static JenisProduk GenerateJenisProduk(string namaProduk)
    {
        var prefix =  namaProduk.Split(' ', 2)[0];

        return prefix switch
        {
            "Cetak" => JenisProduk.Cetak,
            "Banner" => JenisProduk.Banner,
            "DTF" => JenisProduk.DTF,
            "Laminating" => JenisProduk.Laminating,
            "Laminasi" => JenisProduk.Laminasi,
            _ => JenisProduk.Cetak
        };
    }

    public static Ukuran GenerateUkuran(string namaProduk)
    {
        var split = namaProduk.Split(' ', 2);
        var prefix = split[0];
        var restName = split[1];

        if ((prefix.Contains("Laminasi") || prefix.Contains("Laminating")) && restName.Contains("Kartu")) return Ukuran.ID;

        if (prefix.Contains("Laminasi") || prefix.Contains("Laminating")) return Ukuran.A3;
        if (prefix != "Cetak") return Ukuran.Kustom;
        
        if (restName.Contains("A4") || restName.Contains("Poster")) return Ukuran.A4;
        if (restName.Contains("A3") || restName.Contains("Buku")) return Ukuran.A3;
        if (restName.Contains("A3+") || restName.Contains("Art Paper")) return Ukuran.A3Plus;
        if (restName.Contains("Kartu")) return Ukuran.ID;

        return Ukuran.A3Plus;
    }
}