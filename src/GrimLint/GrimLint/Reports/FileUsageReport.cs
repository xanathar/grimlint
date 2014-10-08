using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using GrimLint.Model;
using GrimLint.Reports.FileUsage;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;

namespace GrimLint.Reports
{
	public class FileUsageReport : Report
	{
		public override byte[] Run(Dungeon D)
		{
			FileUsageCalculator fuck = new FileUsageCalculator();
			fuck.Process(D);
			return CreateExcelReport(fuck);
		}

		private byte[] CreateExcelReport(FileUsageCalculator fuck)
		{
			ExcelWorksheet ws;
			using (ExcelPackage pck = new ExcelPackage())
			{
				ws = pck.Workbook.Worksheets.Add("Models");
				DumpModels(ws, fuck.Models);

				ws = pck.Workbook.Worksheets.Add("Materials");
				DumpModels(ws, fuck.Materials);
				
				ws = pck.Workbook.Worksheets.Add("Textures");
				DumpModels(ws, fuck.Textures);

				return pck.GetAsByteArray();
			}
		}

		private void DumpModels(ExcelWorksheet ws, IEnumerable<StatObject> stat_objs)
		{
			StatObject[] objs = stat_objs.ToArray();

			if (objs.Length == 0)
			{
				ws.Cells["A1"].Value = "None found";
				ws.Column(1).AutoFit();
				return;
			}

			int col = 1;
			foreach (string str in objs[0].GetPropertyNames())
			{
				ws.Cells[1, col].Value = str;
				col++;
			}

			int maxcolumn = col - 1;

			for (int i = 0; i < objs.Length; i++)
			{
				col = 1;
				foreach (object oo in objs[i].GetPropertyValues())
				{
					ws.Cells[2 + i, col].Value = oo;
					col++;
				}
			}

			ExcelRow rng = ws.Row(1);
			{
				rng.Style.Font.Bold = true;
				rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
				rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 129, 189));
				rng.Style.Font.Color.SetColor(Color.White);
			}

			ExcelRange range1 = ws.Cells[1, 1, objs.Length + 1, maxcolumn];
			ExcelTable table1 = ws.Tables.Add(range1, "tbl_" + Guid.NewGuid().ToString("N"));
			table1.TableStyle = OfficeOpenXml.Table.TableStyles.Light1;

			for (int i = 1; i <= maxcolumn; i++)
				ws.Column(i).AutoFit();
		}





		public override string GetReportName()
		{
			return this.GetType().Name;
		}
	}
}
