using System.Text;
using Npgsql;

namespace Babel.Helpers;

public static class DbCommand
{
    public static async Task<bool> Execute(NpgsqlDataSource source, string sql, string successMsg = "")
    {
        Console.OutputEncoding = Encoding.UTF8;

        try
        {
            Console.ForegroundColor = ConsoleColor.Green;

            await using var cmd = source.CreateCommand(sql);
            cmd.ExecuteNonQuery();
            if (!string.IsNullOrEmpty(successMsg))
                Console.WriteLine($"✅ {successMsg}");
            
            Console.ResetColor();
            return true;
        }
        catch (PostgresException ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"❌ Error PSQL: {ex.MessageText}");
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"💥 Error: {ex.Message}");
        }
        
        Console.ResetColor();
        return false;
    }
}