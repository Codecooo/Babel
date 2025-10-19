using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace Babel.Helpers;

public static class CsvOps
{
    public static async Task WriteCsv<T>(string path, List<T> contents)
    {
        File.Create(path).Close();

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            ShouldQuote = _ => true
        };

        await using var writer = new StreamWriter(path);
        await using var csv = new CsvWriter(writer, config);

        // Konversi format datetime ke format ISO
        var options = new TypeConverterOptions { Formats = ["yyyy-MM-dd HH:mm:ss"] };
        csv.Context.TypeConverterOptionsCache.AddOptions<DateTime>(options);
        csv.Context.TypeConverterOptionsCache.AddOptions<DateTime?>(options);

        await csv.WriteRecordsAsync(contents);
    }
    
    public static async Task ReadCsv(TextWriter writer, string path)
    {
        using var reader = new StreamReader(path);

        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();
            await writer.WriteLineAsync(line);
        }
        
        Console.ForegroundColor =  ConsoleColor.Green;
        Console.WriteLine($"✅ Data {Path.GetFileName(path)} telah berhasil dimuat ke database");
        await writer.FlushAsync();
    }
}