using Domain.Entities;
using System.Text.RegularExpressions;

namespace Application.Common.Extensions;

public static class MaskingExtensions
{
    /// <summary>
    /// Kullanıcının PII (kişisel) verilerini maskeler veya olduğu gibi döner.
    /// </summary>
    /// <param name="user">User entity</param>
    /// <param name="canViewPII">true ise veriler maskelenmeden döner</param>
    /// <returns>(phone, address, piiRedacted)</returns>
    public static (string? Phone, string? Address, bool PiiRedacted) ApplyPrivacy(User user, bool canViewPII)
    {
        if (canViewPII)
        {
            return (user.PhoneEncrypted, user.AddressEncrypted, false);
        }

        return (
            MaskPhone(user.PhoneEncrypted),
            MaskAddress(user.AddressEncrypted),
            true
        );
    }

    // Telefon maskesi: sadece son 2–3 hane açık kalır.
    private static string? MaskPhone(string? phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return null;

        // Rakam dışı karakterleri koruyarak *** ekler
        var digits = Regex.Replace(phone, @"\D", "");
        if (digits.Length <= 3)
            return "***";

        var visible = digits[^3..];
        var masked = new string('*', digits.Length - 3) + visible;

        // format koruma: +90 (***) ***-**
        return Regex.Replace(phone, @"\d", m => masked.Length > 0 ? masked[..1] : "*")
                    .Replace("*", "*")
                    .Trim();
    }

    // Adres maskesi: ilk kelime hariç yıldız
    private static string? MaskAddress(string? address)
    {
        if (string.IsNullOrWhiteSpace(address))
            return null;

        var parts = address.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0)
            return "***";

        var first = parts[0];
        var maskedRest = string.Join(' ', parts.Skip(1).Select(_ => "****"));
        return $"{first} {maskedRest}".Trim();
    }

    // Kimlik numarası maskesi (isteğe bağlı, şu an kullanılmıyor)
    public static string? MaskNationalId(string? id)
    {
        if (string.IsNullOrWhiteSpace(id) || id.Length < 4)
            return "***";

        var visible = id[^2..];
        return new string('*', id.Length - 2) + visible;
    }

    // Genel amaçlı e-posta maskesi (isteğe bağlı)
    public static string? MaskEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return null;

        var parts = email.Split('@');
        if (parts.Length != 2) return "***";

        var local = parts[0];
        var domain = parts[1];
        var maskedLocal = local.Length <= 2 ? new string('*', local.Length) : local[..2] + new string('*', local.Length - 2);
        return $"{maskedLocal}@{domain}";
    }
}
