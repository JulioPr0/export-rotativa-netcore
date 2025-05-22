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

            IEnumerable<StudentScore> scores = _scoreRepository.GetAllScores();

            using var package   = new ExcelPackage();
            var worksheet       = package.Workbook.Worksheets.Add("Scores");

            BuildExcelHeader(worksheet);
            FillExcelData(worksheet, scores);

            using var stream    = new MemoryStream();
            await package.SaveAsAsync(stream);

            stream.Position     = 0;

            string fileName     = $"scores_{DateTime.Now:yyyyMMdd}.xlsx";

            return File(
                stream,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName
            );
        }

        [HttpGet("pdf")]
        public IActionResult Pdf()
        {
            var scores = _scoreRepository.GetAllScores();

            return new ViewAsPdf("PdfView", scores)
            {
                FileName        = $"scores_{DateTime.Now:yyyyMMdd}.pdf",
                PageSize        = Rotativa.AspNetCore.Options.Size.A4,
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait
            };
        }

        #region Excel Helpers

        private static void BuildExcelHeader(ExcelWorksheet sheet)
        {
            sheet.Cells[1, 1].Value = "Rank";
            sheet.Cells[1, 2].Value = "Name";
            sheet.Cells[1, 3].Value = "Score";
            sheet.Row(1).Style.Font.Bold = true;
        }

        private static void FillExcelData(
            ExcelWorksheet sheet,
            IEnumerable<StudentScore> data)
        {
            var row = 2;
            foreach (var item in data)
            {
                sheet.Cells[row, 1].Value = item.Rank;
                sheet.Cells[row, 2].Value = item.Name;
                sheet.Cells[row, 3].Value = item.Score;
                row++;
            }
        }

        #endregion
    }
}
