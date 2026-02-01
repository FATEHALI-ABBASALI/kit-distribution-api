using QRCoder;
using System.IO;

namespace KitDistributionAPI.Services
{
    public class QrCodeService
    {
        public string GenerateQrCode(string beneficiaryId, string fullName, string city)
        {
            string qrData = $"ID:{beneficiaryId}, Name:{fullName}, City:{city}";

            using QRCodeGenerator qrGenerator = new QRCodeGenerator();
            using QRCodeData qrCodeData =
                qrGenerator.CreateQrCode(qrData, QRCodeGenerator.ECCLevel.Q);

            // ✅ SAFE: No System.Drawing
            PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
            byte[] qrBytes = qrCode.GetGraphic(20);

            string fileName = $"QR_{beneficiaryId}.png";
            string folderPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "QRCodes"
            );

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string fullPath = Path.Combine(folderPath, fileName);

            File.WriteAllBytes(fullPath, qrBytes);

            return fileName; // stored as Card_ID
        }
    }
}
