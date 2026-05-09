namespace KitapCell.Models
{
    public enum UserRole
    {
        Uye = 0,
        Kutuphaneci = 1,
        Admin = 2
    }

    public enum LoanStatus
    {
        Aktif = 0,
        IadeEdildi = 1,
        Gecikti = 2
    }

    public enum NotificationType
    {
        Bilgi = 0,
        IadeHatirlatma = 1,
        CezaBilgisi = 2,
        Rozet = 3,
        Duyuru = 4
    }
}
