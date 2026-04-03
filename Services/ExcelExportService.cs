using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using WpfAppT.Models;

namespace WpfAppT.Services
{
    public class ExcelExportService
    {
        public void ExportRecords(
            List<Record> allRecords,
            List<Record> activeRecords,
            string outputPath)
        {
            using var wb = new XLWorkbook();

            // ── Аркуш 1: Всі записи ──────────────────────
            var wsAll = wb.Worksheets.Add("Всі записи");
            wsAll.Cell(1, 1).Value = $"Витяг сформовано: {DateTime.Now:dd.MM.yyyy HH:mm}";
            wsAll.Cell(1, 1).Style.Font.Bold = true;
            wsAll.Cell(1, 1).Style.Font.FontColor = XLColor.Gray;

            WriteHeaders(wsAll, 3);
            WriteRows(wsAll, allRecords, 4);
            wsAll.Columns().AdjustToContents();

            // ── Аркуш 2: Активні записи ──────────────────
            var wsActive = wb.Worksheets.Add("Активні");
            wsActive.Cell(1, 1).Value = $"Витяг сформовано: {DateTime.Now:dd.MM.yyyy HH:mm}";
            wsActive.Cell(1, 1).Style.Font.Bold = true;
            wsActive.Cell(1, 1).Style.Font.FontColor = XLColor.Gray;

            WriteHeaders(wsActive, 3);
            WriteRows(wsActive, activeRecords, 4);
            wsActive.Columns().AdjustToContents();

            // ── Аркуш 3: Статистика ───────────────────────
            var wsStat = wb.Worksheets.Add("Статистика");
            wsStat.Cell(1, 1).Value = $"Витяг сформовано: {DateTime.Now:dd.MM.yyyy HH:mm}";
            wsStat.Cell(1, 1).Style.Font.Bold = true;
            wsStat.Cell(1, 1).Style.Font.FontColor = XLColor.Gray;

            wsStat.Cell(3, 1).Value = "Показник";
            wsStat.Cell(3, 2).Value = "Значення";
            wsStat.Row(3).Style.Font.Bold = true;

            wsStat.Cell(4, 1).Value = "Всього записів";
            wsStat.Cell(4, 2).Value = allRecords.Count;

            wsStat.Cell(5, 1).Value = "Активних записів";
            wsStat.Cell(5, 2).Value = activeRecords.Count;

            wsStat.Cell(6, 1).Value = "Завершених записів";
            wsStat.Cell(6, 2).Value = allRecords.Count - activeRecords.Count;

            // Піки по місяцях
            wsStat.Cell(8, 1).Value = "Записи по місяцях";
            wsStat.Cell(8, 1).Style.Font.Bold = true;
            wsStat.Cell(9, 1).Value = "Місяць";
            wsStat.Cell(9, 2).Value = "Кількість";
            wsStat.Row(9).Style.Font.Bold = true;

            var byMonth = new Dictionary<string, int>();
            foreach (var r in allRecords)
            {
                var key = r.DateAdded.ToString("MM.yyyy");
                if (!byMonth.ContainsKey(key)) byMonth[key] = 0;
                byMonth[key]++;
            }

            int row = 10;
            foreach (var kvp in byMonth)
            {
                wsStat.Cell(row, 1).Value = kvp.Key;
                wsStat.Cell(row, 2).Value = kvp.Value;
                row++;
            }

            wsStat.Columns().AdjustToContents();

            wb.SaveAs(outputPath);
        }

        private void WriteHeaders(IXLWorksheet ws, int row)
        {
            ws.Cell(row, 1).Value = "ID";
            ws.Cell(row, 2).Value = "Клієнт";
            ws.Cell(row, 3).Value = "Спеціаліст";
            ws.Cell(row, 4).Value = "Авто";
            ws.Cell(row, 5).Value = "Номер";
            ws.Cell(row, 6).Value = "Причина";
            ws.Cell(row, 7).Value = "Опис";
            ws.Cell(row, 8).Value = "Дата додавання";
            ws.Cell(row, 9).Value = "Дата завершення";
            ws.Cell(row, 10).Value = "Статус";
            ws.Row(row).Style.Font.Bold = true;
            ws.Row(row).Style.Fill.BackgroundColor = XLColor.LightGray;
        }

        private void WriteRows(IXLWorksheet ws, List<Record> records, int startRow)
        {
            int row = startRow;
            foreach (var r in records)
            {
                ws.Cell(row, 1).Value = r.Id;
                ws.Cell(row, 2).Value = $"{r.Customer?.FirstName} {r.Customer?.LastName}";
                ws.Cell(row, 3).Value = $"{r.Specialist?.FirstName} {r.Specialist?.LastName}";
                ws.Cell(row, 4).Value = $"{r.Car?.Brand?.Name} {r.Car?.Model}";
                ws.Cell(row, 5).Value = r.LicensePlate;
                ws.Cell(row, 6).Value = r.Reason;
                ws.Cell(row, 7).Value = r.MasterDescription;
                ws.Cell(row, 8).Value = r.DateAdded.ToString("dd.MM.yyyy");
                ws.Cell(row, 9).Value = r.DateCompleted?.ToString("dd.MM.yyyy") ?? "—";
                ws.Cell(row, 10).Value = r.IsCompleted ? "Завершено" : "Активний";
                row++;
            }
        }
    }
}