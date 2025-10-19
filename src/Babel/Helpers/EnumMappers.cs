using Babel.Models;

namespace Babel.Helpers;

public static class EnumMappers
{
    public static string ToDb(this JenisMesin jenisMesin)
    {
        return jenisMesin switch
        {
            JenisMesin.MesinBanner =>  "MesinBanner",
            JenisMesin.PrinterA3Plus => "PrinterA3+",
            JenisMesin.PrinterA4 => "PrinterA4",
            JenisMesin.MesinDTF =>  "MesinDTF",
            JenisMesin.MesinLaminating => "MesinLaminating",
            _ => "PrinterA4"
        };
    }

    public static string ToDb(this Ukuran ukuran)
    {
        return ukuran switch
        {
            Ukuran.A3 => "A3",
            Ukuran.A3Plus => "A3+",
            Ukuran.A4 => "A4",
            Ukuran.ID => "ID",
            _ => "Kustom"
        };
    }
}