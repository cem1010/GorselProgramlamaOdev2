namespace MauiApp1.Model
{
    // Rehberdeki kişi bilgilerini tutan sınıf
    public class Person
    {
        // Benzersiz ID
        public string Id { get; set; } = Guid.NewGuid().ToString();

        // Temel bilgiler - nullable olarak tanımlandı
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Address { get; set; }

        // Profil resmi yolu
        public string ProfileImage { get; set; } = "dotnet_bot.png";

        // UI gösterimi için
        public string FullName => $"{FirstName} {LastName}";
    }
}