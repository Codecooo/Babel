using System.Globalization;
using CsvHelper;

namespace Babel.Helpers;

public static class CsvOps
{
    public static async Task WriteCsv<T>(string path, List<T> contents)
    {
        File.Create(path).Close();
        await using var writer = new StreamWriter(path);
        await using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        
        await csv.WriteRecordsAsync(contents);
    }
}