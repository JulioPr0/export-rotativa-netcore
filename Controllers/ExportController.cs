using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;          
using Rotativa.AspNetCore;        
using RotativaDemo.Data;          
using RotativaDemo.Models;        

namespace RotativaDemo.Controllers
{
    [Route("export")]
    public class ExportController : Controller
    {
        private readonly IScoreRepository _scoreRepository;

        public ExportController(IScoreRepository scoreRepository)
        {
            _scoreRepository = scoreRepository;
        }

        [HttpGet("excel")]
        public async Task<IActionResult> Excel()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var scores = _scoreRepository.GetAllScores();

            using var package = new ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("Sheet1");
            BuildExcelHeader(ws);
            FillExcelData(ws, scores);

            var stream = new MemoryStream();
            await package.SaveAsAsync(stream);
            stream.Position = 0;

            return File(stream,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        $"data_{DateTime.Now:yyyyMMdd}.xlsx");
        }


        [HttpGet("pdf")]
        public IActionResult Pdf()
        {
            var scores = _scoreRepository.GetAllScores();

            return new ViewAsPdf("PdfView", scores)
            {
                FileName        = $"laporan_{DateTime.Now:yyyyMMdd}.pdf",
                PageSize        = Rotativa.AspNetCore.Options.Size.A4,
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait
            };
        }

        #region Excel Helpers

        private static void BuildExcelHeader(ExcelWorksheet sheet)
        {
            sheet.Cells[1, 1].Value = "No";     
            sheet.Cells[1, 2].Value = "Nama";   
            sheet.Cells[1, 3].Value = "Nilai";   
            sheet.Row(1).Style.Font.Bold = true;
        }

        private static void FillExcelData(
            ExcelWorksheet sheet,
            IEnumerable<StudentScore> data)
        {
            var row = 2;
            foreach (var item in data)
            {
                sheet.Cells[row, 1].Value = item.No;
                sheet.Cells[row, 2].Value = item.Nama;
                sheet.Cells[row, 3].Value = item.Nilai;
                row++;
            }
        }

        #endregion
    }
}
