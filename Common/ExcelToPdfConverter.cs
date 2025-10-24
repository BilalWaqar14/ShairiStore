using Aspose.Cells;
using Aspose.Cells.Rendering;
namespace ShairiStore.Common;

public static class ExcelToPdfConverter
{
    public static byte[] ConvertXlsBytesToPdf(byte[] xlsBytes)
    {
        using var inputStream = new MemoryStream(xlsBytes);
        using var outputStream = new MemoryStream();

        // Load workbook from byte stream
        var workbook = new Workbook(inputStream);

        // Optional: fine-tune save options
        var pdfOptions = new PdfSaveOptions
        {
            Compliance = PdfCompliance.PdfA1b,
            OnePagePerSheet = false,
            AllColumnsInOnePagePerSheet = false
        };

        // Save workbook as PDF into output stream
        workbook.Save(outputStream, pdfOptions);

        return outputStream.ToArray(); // return PDF as byte array
    }
}
