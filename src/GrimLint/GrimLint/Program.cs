using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using GrimLint.Model;
using GrimLint.Readers;
using GrimLint.Readers.LineReader;
using GrimLint.Readers.LuaReader;
using GrimLint.Reports;
using GrimLint.Rules.Base;

namespace GrimLint
{
	class Program
	{
		static bool interactive = true;
		static bool deepAnalysis = true;
		static bool reportsEnabled = false;
		static bool definitionParsing = true;
		static string pathToDungeon = null;


		[STAThread]
		static void Main(string[] args)
		{
			MoonSharp.Interpreter.UserData.RegisterAssembly();
			RunMain(args);
			Console.ResetColor();
		}

		static void RunMain(string[] args)
		{
			Lint.MsgBanner("GrimLint v1.1 - Static Analysis for GrimRock mods");
			Lint.MsgBanner("Program by Marco Mastropaolo");

			Config.LoadXml("Data\\config.xml");

			foreach (string arg in args)
			{
				if (arg == "-c")
					interactive = false;
				else if (arg == "-d")
					deepAnalysis = true;
				else if (arg == "-a")
					definitionParsing = false;
				else if (arg == "-r")
					reportsEnabled = true;
				else if (arg == "-h" || arg == "-H" || arg == "--help" || arg == "-?" || arg == "/?")
				{
					ShowHelp();
					return;
				}
				else
					pathToDungeon = arg;
			}

			if (pathToDungeon == null)
			{
				if (interactive)
				{
					OpenFileDialog ofd = new OpenFileDialog();
					ofd.DefaultExt = "*.dungeon_editor";
					ofd.Filter = "Dungeon Editor files|*.dungeon_editor";
					if (ofd.ShowDialog() == DialogResult.OK)
						pathToDungeon = ofd.FileName;
					else
						return;
				}
				else
				{
					Lint.MsgErr("You must specify a dungeon filename");
					return;
				}
			}



			if (!interactive)
			{
				Exec();
				return;
			}

			while (true)
			{
				PrintMenu();

				while (true)
				{
					ConsoleKeyInfo cki = Console.ReadKey(true);

					if (cki.KeyChar.ToString().ToUpper() == "Q" || cki.Key == ConsoleKey.Escape)
					{
						return;
					}
					else if (cki.KeyChar.ToString().ToUpper() == "M")
					{
						deepAnalysis = !deepAnalysis;
						break;
					}
					else if (cki.KeyChar.ToString().ToUpper() == "A")
					{
						definitionParsing = !definitionParsing;
						break;
					}
					else if (cki.KeyChar.ToString().ToUpper() == "R")
					{
						reportsEnabled = !reportsEnabled;
						break;
					}
					else if (cki.Key == ConsoleKey.Spacebar || cki.Key == ConsoleKey.Enter)
					{
						Exec();
						break;
					}
				}
			}
		}

		private static void ShowHelp()
		{
			Console.WriteLine();
			Console.WriteLine();
			Console.WriteLine("Usage: grimlint [dungeonfile] [-c][-d][-a][-r]");
			Console.WriteLine();
			Console.WriteLine("\t-c : Disable interactive mode");
			Console.WriteLine("\t-d : Use deep analysis mode");
			Console.WriteLine("\t-a : Disable parsing of mod_assets lua files");
			Console.WriteLine("\t-r : Enable creation of Excel reports");
			Console.WriteLine();
			return;
		}

		private static void PrintMenu()
		{
			Lint.MsgBanner("====== INTERACTIVE MENU =====");
			Lint.MsgBanner("    A - {0} mod_assets loading (now {1})", (definitionParsing ? "disable" : "enable"), (definitionParsing ? "enabled" : "disabled"));
			Lint.MsgBanner("    R - {0} reports (now {1})", (reportsEnabled ? "disable" : "enable"), (reportsEnabled ? "enabled" : "disabled"));
			Lint.MsgBanner("    M - change analysis mode to {0} (now {1})", GetModeString(!deepAnalysis), GetModeString(deepAnalysis));
			Lint.MsgBanner("Enter - run analysis");
			Lint.MsgBanner("Q/Esc - quit");
		}

		private static object GetModeString(bool deepAnalysis)
		{
			return deepAnalysis ? "'deep'" : "'fast'";
		}

		private static void Exec()
		{
			Lint.MsgBanner("===============================================================================");
			Lint.MsgBanner("ANALYSIS DONE AT: {0} - {1} - MODE = {2} - REPORTS = {3}", DateTime.Now.ToLongDateString(), DateTime.Now.ToLongTimeString(), GetModeString(deepAnalysis), reportsEnabled);
			Lint.MsgBanner("===============================================================================");

			IDungeonReader reader = (deepAnalysis) ? new LuaDungeonReader() as IDungeonReader : new LineDungeonReader() as IDungeonReader;
			string dir = Path.GetDirectoryName(pathToDungeon);

			Assets assets = new Assets();
			assets.LoadXml("Data\\assets.xml");

			if (definitionParsing)
			{
				Lint.MsgProfileStart("init.lua", "Loading definitions from init.lua...");
				DefinitionsLoader defs = new DefinitionsLoader(assets, dir);
				defs.LoadLuaAssets();
				assets.CreateAssetsFromDefs();
				Lint.MsgProfileEnd("init.lua", "Definitions loaded in");
			}
			assets.LoadXml("Data\\mod_assets.xml");

			Dungeon D = new Dungeon(dir);
			D.Assets = assets;
			Lint.MsgProfileStart("dungeon.lua", "Loading dungeon from dungeon.lua...");
			reader.Load(D);
			Lint.MsgProfileEnd("dungeon.lua", "dungeon.lua loaded in");


			foreach (Rule R in GetAllRules())
			{
				string rulename = R.GetRuleName();
				if (Config.IsRuleEnabled(rulename))
				{
					Lint.MsgProfileStart(rulename, "Running {0}...", rulename);
					R.Run(D);
					Lint.MsgProfileEnd(rulename, rulename + " completed in");
				}
			}

			if (reportsEnabled)
			{
				foreach (Report R in GetAllReports())
				{
					string reportname = R.GetReportName();
					Lint.MsgProfileStart(reportname, "Running {0}...", reportname);
					R.Process(D);
					Lint.MsgProfileEnd(reportname, reportname + " completed in");
				}
			}
		}

		public static IEnumerable<Rule> GetAllRules()
		{
			foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
			{
				foreach (Type t in asm.GetTypes())
				{
					if (!t.IsAbstract && t.IsClass && (t.BaseType == typeof(Rule) || t.BaseType == typeof(SimpleRule)))
						yield return (Rule)Activator.CreateInstance(t);
				}
			}
		}

		public static IEnumerable<Report> GetAllReports()
		{
			foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
			{
				foreach (Type t in asm.GetTypes())
				{
					if (!t.IsAbstract && t.IsClass && (t.BaseType == typeof(Report)))
						yield return (Report)Activator.CreateInstance(t);
				}
			}
		}
	}
}
