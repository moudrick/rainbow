using System;
using System.Collections;
using System.IO;
using System.Xml;

namespace VsProjectChecker
{
	/// <summary>
	/// Descrizione di riepilogo per Class1.
	/// </summary>
	class Class1
	{
		/// <summary>
		/// Il punto di ingresso principale dell'applicazione.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			if (args.Length == 0)
			{
				Console.WriteLine("Provide a project name");
				return;
			}

			//Open project
			XmlDocument xml = new XmlDocument();
			xml.Load(args[0]);

			bool verbose = false;
			if (args.Length > 1 && args[1] == "verbose")
				verbose = true;
            
			ArrayList a = new ArrayList();

			//Select file nodes
			XmlNodeList l = xml.DocumentElement.SelectNodes("//Files/Include/File");
			if (l.Count == 0)
				Console.WriteLine("No files here");
			else
				Console.WriteLine("Checking files");

			//Cycles files
			foreach(XmlNode n  in l)
			{
				//Checks if file exists
				if (n.Attributes["RelPath"] != null)
				{
					//If not exists print a warning
					if (!File.Exists(n.Attributes["RelPath"].Value))
					{
						a.Add("File KO:'" + n.Attributes["RelPath"].Value + "' does not exists");
						if (verbose)
							Console.WriteLine("File KO:'" + n.Attributes["RelPath"].Value + "' does not exists");
					}
					else
					{
						if (verbose)
							Console.WriteLine("File OK:'" + n.Attributes["RelPath"].Value + "' exists");
					}
				}
			}

			//Select file nodes
			XmlNodeList d = xml.DocumentElement.SelectNodes("//Files/Include/Folder");
			if (d.Count == 0)
				Console.WriteLine("No folders here");
			else
				Console.WriteLine("Checking folders");

			//Cycles files
			foreach(XmlNode n in d)
			{
				//Checks if file exists
				if (n.Attributes["RelPath"] != null)
				{
					//If not exists print a warning
					if (!Directory.Exists(n.Attributes["RelPath"].Value))
					{
						a.Add("Folder KO:'" + n.Attributes["RelPath"].Value + "' does not exists");
						if (verbose)
							Console.WriteLine("Folder KO:'" + n.Attributes["RelPath"].Value + "' does not exists");
					}
					else
					{
						if (verbose)
							Console.WriteLine("Folder OK:'" + n.Attributes["RelPath"].Value + "' exists");
					}
				}
			}

			//Cycles folders
			if (a.Count > 0)
			{
				Console.WriteLine("");
				Console.WriteLine("Missing files:");
				foreach(string f in a)
					Console.WriteLine(f);
			}

			//Exit
			Console.WriteLine("Check complete");
			return;
		}
	}
}