namespace Babel.Models;

public record Mesin(int IdMesin, string NamaMesin, StatusMesin StatusMesin, string JenisMesin)
{
    public Mesin() : this(0, string.Empty, StatusMesin.Online, string.Empty) {}
}