namespace Babel.Models;

public record Karyawan(
    Guid IdKaryawan,
    string NamaDepan,
    string NamaBelakang,
    Jabatan Jabatan,
    string Email,
    string NoHp,
    string Alamat)
{
    public Karyawan() : this (Guid.Empty, string.Empty, string.Empty, Jabatan.BannerOperator, string.Empty, string.Empty, string.Empty)  
    {}
}