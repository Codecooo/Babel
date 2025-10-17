namespace Babel.Models;

public enum StatusPesanan
{
    MenungguPembayaran,
    Pending,
    Selesai,
    Batal
}

public enum StatusPembayaran
{
    MenungguPembayaran,
    Berhasil,
    Batal
}

public enum StatusMesin
{
    Online,
    Offline,
    Maintenance
}

public enum MetodePembayaran
{
    QRIS,
    Cash,
    Transfer
}

public enum Jabatan
{
    PrintOperator,
    BannerOperator,
    DTFOperator,
    CustomerService
}

public enum JenisMesin
{
    PrinterA3Plus,
    PrinterA4,
    MesinDTF,
    MesinBanner,
    MesinLaminating
}

public enum JenisProduk
{
    Cetak,
    Laminating,
    Laminasi,
    Banner,
    DTF
}

public enum JenisBahan
{
    HVS,
    PlastikLaminating,
    PlastikLaminasi,
    Banner,
    IDCard,
    DTFPet
}

public enum Ukuran
{
    A3Plus,
    A4,
    A3,
    ID,
    Kustom
}