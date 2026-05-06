using SkiaSharp;
using ZXing;
using ZXing.Common;
using ZXing.SkiaSharp;
using ZXing.SkiaSharp.Rendering;

namespace Infrastructure.Services;

public interface IBarcodeService
{
    /// <summary>
    /// Ürüne ait barkod (CODE_128) PNG olarak üretir.
    /// </summary>
    byte[] GenerateBarcode(string code);

    /// <summary>
    /// Verilen metinden QR kod üretir (örneğin ürün bilgisi, stok transfer fişi).
    /// </summary>
    byte[] GenerateQrCode(string content);

    /// <summary>
    /// API içinde kolay gösterim için Base64 formatında döndürür.
    /// </summary>
    string GenerateQrBase64(string content);
}

public class BarcodeService : IBarcodeService
{
    /// <summary>
    /// Barkod (CODE_128) üretimi — barkod okuyucularla uyumludur.
    /// </summary>
    public byte[] GenerateBarcode(string code)
    {
        var writer = new ZXing.SkiaSharp.BarcodeWriter
        {
            Format = BarcodeFormat.CODE_128,
            Options = new EncodingOptions
            {
                Height = 100,
                Width = 300,
                Margin = 2,
                PureBarcode = true
            },
            Renderer = new SKBitmapRenderer()
        };

        using var bitmap = writer.Write(code);
        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        return data.ToArray();
    }

    /// <summary>
    /// QR kod üretimi (örnek: stok transfer fişi, ürün bilgisi, hasta takip numarası)
    /// </summary>
    public byte[] GenerateQrCode(string content)
    {
        var writer = new ZXing.SkiaSharp.BarcodeWriter
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new EncodingOptions
            {
                Height = 300,
                Width = 300,
                Margin = 1
            },
            Renderer = new SKBitmapRenderer()
        };

        using var bitmap = writer.Write(content);
        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        return data.ToArray();
    }

    /// <summary>
    /// QR kodu Base64 string olarak döndürür (örneğin Swagger / frontend için).
    /// </summary>
    public string GenerateQrBase64(string content)
    {
        var bytes = GenerateQrCode(content);
        return $"data:image/png;base64,{Convert.ToBase64String(bytes)}";
    }
}
