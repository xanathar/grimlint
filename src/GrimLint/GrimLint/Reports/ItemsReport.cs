using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using GrimLint.Model;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;

namespace GrimLint.Reports
{
	public class ItemsReport : Report
	{
		public override string GetReportName()
		{
			return this.GetType().Name;
		}

		public override byte[] Run(Dungeon D)
		{
			ExcelWorksheet ws;
			using (ExcelPackage pck = new ExcelPackage())
			{
				ws = pck.Workbook.Worksheets.Add("All Items");
				FillItems(D, ws, i => true);

				ws = pck.Workbook.Worksheets.Add("Count");
				FillItemsCount(D, ws);

				ws = pck.Workbook.Worksheets.Add("Secrets");
				FillSecrets(D, ws);

				ws = pck.Workbook.Worksheets.Add("Uncategorized");
				FillItems(D, ws, i => i.ItemClass == 0);

				ws = pck.Workbook.Worksheets.Add("Of Interest");
				FillItems(D, ws, i => i.ItemClass != ItemClass.PlaceHolder && (i.Name != "note") && (i.Name != "scroll") && (i.ItemClass != ItemClass.Food) && (i.Name != "torch")
					&&(i.ItemClass != ItemClass.Potion));

				foreach (ItemClass ic in Enum.GetValues(typeof(ItemClass)).OfType<ItemClass>())
				{
					ws = pck.Workbook.Worksheets.Add(ic.ToString());
					FillItems(D, ws, i => i.ItemClass.HasFlag(ic));
				}

				return pck.GetAsByteArray();
			}
		}

		private void FillSecrets(Dungeon D, ExcelWorksheet ws)
		{
			ws.Cells["A1"].Value = "Id";
			ws.Cells["B1"].Value = "Level";
			ws.Cells["C1"].Value = "X,Y";
			ws.Cells["D1"].Value = "Activated-By";

			ExcelRow rng = ws.Row(1);
			{
				rng.Style.Font.Bold = true;
				rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
				rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 129, 189));
				rng.Style.Font.Color.SetColor(Color.White);
			}

			int row = 2;
			foreach (Entity E in D.EntitiesByClass[EntityClass.Secret])
			{
				ws.Cells[row, 1].Value = E.Id;
				ws.Cells[row, 2].Value = E.Level;
				ws.Cells[row, 3].Value = E.X + "," + E.Y;
				ws.Cells[row, 4].Value = string.Join(",", E.ReverseConnectors.Select(c => c.Source));
				++row;
			}

			ExcelRange range1 = ws.Cells["A1:D" + (row - 1).ToString()];
			ExcelTable table1 = ws.Tables.Add(range1, "tbl_" + Guid.NewGuid().ToString("N"));
			table1.TableStyle = OfficeOpenXml.Table.TableStyles.Light1;

			ws.Column(1).AutoFit();
			ws.Column(2).AutoFit();
			ws.Column(3).AutoFit();
			ws.Column(4).AutoFit();
		}

		class ItemCounter
		{
			public int Count = 0;
			public int Instances = 0;
			public int MinLevel = int.MaxValue;
			public int MaxLevel = int.MinValue;
		}

		private void FillItemsCount(Dungeon D, ExcelWorksheet ws)
		{
			ws.Cells["A1"].Value = "Name";
			ws.Cells["B1"].Value = "Count";
			ws.Cells["C1"].Value = "Instances";
			ws.Cells["D1"].Value = "Min-Level";
			ws.Cells["E1"].Value = "Max-Level";
			ws.Cells["F1"].Value = "Subclass";

			Dictionary<string, ItemCounter> counters = new Dictionary<string, ItemCounter>();

			int row = 2;
			foreach (Entity E in D.EntitiesByClass[EntityClass.Item])
			{
				if (!counters.ContainsKey(E.Name))
					counters.Add(E.Name, new ItemCounter());

				ItemCounter C = counters[E.Name];

				string stack = E.GetProperty("StackSize");
				int qty = 0;

				if (int.TryParse(stack, out qty))
					C.Count += qty;
				else
					C.Count++;

				C.Instances++;
				C.MaxLevel = Math.Max(C.MaxLevel, E.Level);
				C.MinLevel = Math.Min(C.MinLevel, E.Level);
			}

			foreach (KeyValuePair<string, ItemCounter> K in counters.OrderBy(k => k.Key))
			{
				Asset A = D.Assets.Get(K.Key);
				string cat = (A != null) ? A.ItemClass.ToString() : "0";
				ws.Cells[row, 1].Value = K.Key;
				ws.Cells[row, 2].Value = K.Value.Count;
				ws.Cells[row, 3].Value = K.Value.Instances;
				ws.Cells[row, 4].Value = K.Value.MinLevel;
				ws.Cells[row, 5].Value = K.Value.MaxLevel;
				ws.Cells[row, 6].Value = cat ;
				++row;
			}


			ExcelRange range1 = ws.Cells["A1:F" + (row - 1).ToString()];
			ExcelTable table1 = ws.Tables.Add(range1, "tbl_" + Guid.NewGuid().ToString("N"));
			table1.TableStyle = OfficeOpenXml.Table.TableStyles.Light1;

			ws.Column(1).AutoFit();
			ws.Column(2).AutoFit();
			ws.Column(3).AutoFit();
			ws.Column(4).AutoFit();
			ws.Column(5).AutoFit();
		}

		private void FillItems(Dungeon D, ExcelWorksheet ws, Func<Entity, bool> filter)
		{
			ws.Cells["A1"].Value = "Id";
            ws.Cells["B1"].Value = "Name";
            ws.Cells["C1"].Value = "Level";
            ws.Cells["D1"].Value = "X,Y";
            ws.Cells["E1"].Value = "StackSize";        

            ExcelRow rng = ws.Row(1);
            {
                rng.Style.Font.Bold = true;
                rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 129, 189));
                rng.Style.Font.Color.SetColor(Color.White);
            }

            int row = 2;
            foreach (Entity E in D.EntitiesByClass[EntityClass.Item].Where(filter))
			{
				ws.Cells[row, 1].Value = E.Id;
				ws.Cells[row, 2].Value = E.Name;
				ws.Cells[row, 3].Value = E.Level;
				ws.Cells[row, 4].Value = E.X + "," + E.Y;
				ws.Cells[row, 5].Value = E.GetProperty("StackSize") ?? "";
				++row;
			}

			ExcelRange range1 = ws.Cells["A1:E" + (row - 1).ToString()];
			ExcelTable table1 = ws.Tables.Add(range1, "tbl_" + Guid.NewGuid().ToString("N"));
			table1.TableStyle = OfficeOpenXml.Table.TableStyles.Light1; 

            ws.Column(1).AutoFit();
            ws.Column(2).AutoFit();
            ws.Column(3).AutoFit();
            ws.Column(4).AutoFit();
            ws.Column(5).AutoFit();
		}



	}
}
