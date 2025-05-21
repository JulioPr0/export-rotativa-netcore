using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;         
using Rotativa.AspNetCore;

namespace RotativaDemo.Controllers
{
    public class ExportController : Controller
    {
        [HttpGet("/export/excel")]
        public async Task<IActionResult> Excel()
        {
            using var package = new ExcelPackage();
            var sheet = package.Workbook.Worksheets.Add("Sheet1");

            sheet.Cells[1, 1].Value = "No";
            sheet.Cells[1, 2].Value = "Nama";
            sheet.Cells[1, 3].Value = "Nilai";

            var data = new[]
            {
                new { No = 1, Nama = "Julio", Nilai = 95 },
                new { No = 2, Nama = "Hoiluj", Nilai = 90 },
                new { No = 3, Nama = "永城", Nilai = 90 }
            };
            for (int i = 0; i < data.Length; i++)
            {
                sheet.Cells[i + 2, 1].Value = data[i].No;
                sheet.Cells[i + 2, 2].Value = data[i].Nama;
                sheet.Cells[i + 2, 3].Value = data[i].Nilai;
            }

            var stream = new MemoryStream(); 
            await package.SaveAsAsync(stream);
            stream.Position = 0;

            string fileName = $"data_{DateTime.Now:yyyyMMdd}.xlsx";
            return File(
                stream,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName
            );
        }

        [HttpGet("/export/pdf")]
        public IActionResult Pdf()
        {
            return new ViewAsPdf("PdfView")
            {
                FileName = $"Laporan_{DateTime.Now:yyyyMMdd}.pdf",
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait
            };
        }
    }
}
