using Bogus;

namespace Babel.Helpers;

public static class NamaMesinGen
{
    private static readonly Lazy<string[]> _baseNamaMesin = new(() => 
        File.ReadAllLines(Path.Combine(AppContext.BaseDirectory, "data", "nama-mesin.txt")));
    
    public static string GenerateNamaMesin(Faker faker)
    {
        var baseNamaMesin = _baseNamaMesin.Value;
        
        var chosenMesin = faker.PickRandom(baseNamaMesin);
        
        // Random code untuk ditambah ke suffix mesin
        var randomCode = faker.Random.Number(100, 1000);
        // Pilih atribut yang ditambah ke nama mesin agar memberi kesan berbeda
        string[] attribute = ["Wi-Fi", "Eco", "USB", "", "IPP", "Office"];
        var chosenAttribute = faker.PickRandom(attribute);

        return $"{chosenMesin} Series {randomCode} {chosenAttribute}";
    }
}