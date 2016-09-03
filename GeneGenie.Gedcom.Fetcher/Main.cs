/*
 *  $Id: Main.cs 202 2008-12-10 11:42:10Z davek $
 * 
 *  Copyright (C) 2008 David A Knight <david@ritter.demon.co.uk>
 *
 *  This program is free software; you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation; either version 2 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA
 *
 */


using GeneGenie.Gedcom.Parser;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;

namespace GeneGenie.GedcomFetcher
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			string searchUri = "http://ajax.googleapis.com/ajax/services/search/web?v=1.0&rsz=large&start=0&q=filetype:ged";
            string dataFolder = ".\\Data\\spidered";

			// urls resulting in non gedcom files, ignore them when parsing,
			// leave the files there so we don't attempt to download again
			string[] blacklist =
			{
				"www.blickle.org/BLICKLE.ged",  // blank
				"www.louisianacajun.com/gaudet.ged", // html
				"www.suttonfamily.biz/ascendant.ged", // html;
				"www.wildensee.de/wildensee.ged", // blank
				"www.braess.de/Braess_a.ged", // completly broken file, duplicate xrefs
				"www.ferdinandus.com/Limited76.ged", // Kith and Kin Pro seems to completly screw up OCCU, or somehow allowed a newline to get into the name
				"www.scotchman.us/nichols.ged", // blank
				"www.douglasweb.net/index.ged", // blank
				"alohatown.com/Fergie.GED", // Mono bug, StreamReader.ReadLine broken
				"mydouglas.net/index.ged", // blank
				"www.volny.cz/rodokmen.ged", // duplicate xref S1 on SUBM and SOUR, can we handle this somehow?
			};
			
			bool doSearch = false;

			if (!Directory.Exists(dataFolder))
			{
				Directory.CreateDirectory(dataFolder);
			}
			
			if (doSearch)
			{
				int start = 0;

				while (start < 128)
				{
					WebClient client = new WebClient();
					string searchResult = client.DownloadString(searchUri.Replace("start=0", string.Format("start={0}", start)));
					
					JavaScriptSerializer s = new JavaScriptSerializer();
					GoogleSearchResponse results = s.Deserialize<GoogleSearchResponse>(searchResult);
		
					GoogleResponseData data = results.responseData;
	
					foreach (GoogleResult result in data.results)
					{
						Uri url = new Uri(result.url);
		
						string domainFolder = Path.Combine(dataFolder, url.Authority);
						if (!Directory.Exists(domainFolder))
						{
							Directory.CreateDirectory(domainFolder);
						}
		
						string path = url.AbsolutePath;
						string filename = path.Substring(path.LastIndexOf('/') + 1);
		
						string localFilename = Path.Combine(domainFolder, filename);
		
						if (!File.Exists(localFilename))
						{
							Console.WriteLine("Downloading {0} to {1}", result.visibleUrl, filename);
							try
							{
								client.DownloadFile(url, localFilename);
							}
							catch
							{
								Console.WriteLine("Failed to download {1} from {0}", result.visibleUrl, filename);
							}
						}
						else
						{
							Console.WriteLine("Already downloaded {0}", result.visibleUrl);
						}
					}

					bool pageSet = false;
					foreach (GooglePage page in data.cursor.pages)
					{
						if (page.start > start)
						{
							start = page.start;
							pageSet = true;
							break;
						}
					}
					if (!pageSet)
					{
						start = int.MaxValue;
					}
				}
			}

			// *.*, should only be .ged files.  GetFiles is case sensitive though
			// we don't want to miss out on .GED .Ged etc. so use *.*
			// handle filtering out others  in the foreach with an EndsWith
			string[] files = Directory.GetFiles(dataFolder, "*.*", SearchOption.AllDirectories);

			Console.WriteLine("Reading {0} GEDCOM files", files.Length);
			
			var reader = new GedcomRecordReader();
			foreach (string gedcomFile in files)
			{
				if (gedcomFile.EndsWith(".ged", StringComparison.CurrentCultureIgnoreCase))
				{
					bool blacklisted = false;
					foreach (string b in blacklist)
					{
						if (gedcomFile.EndsWith(b))
						{
							blacklisted = true;
							break;
						}
					}
					if (!blacklisted)
					{
						int expectedIndividuals = 0;
						int expectedFamilies = 0;
		
						Console.WriteLine("-------------------------------");
						Console.WriteLine("Scanning: " + gedcomFile);
		
						using (StreamReader sr = new StreamReader(gedcomFile))
						{
							string line = null;
							
							while ((line = sr.ReadLine()) != null)
							{
								if (Regex.Match(line, "^[0-9]+[ \t]@[^@]+@[ \t]INDI[ \t]*$").Success)
								{
									expectedIndividuals ++;
								}
								else if (Regex.Match(line, "^[0-9]+[ \t]@[^@]+@[ \t]FAM[ \t]*$").Success)
								{
									expectedFamilies ++;
								}
							}
						}
						
						Console.WriteLine("Reading: " + gedcomFile);
						if (!reader.ReadGedcom(gedcomFile))
						{
							Console.WriteLine("\tFailed to read: " + gedcomFile);
							Debug.Assert(false);
						}
						else
						{
							Console.WriteLine("\tRead: " + gedcomFile);
							Console.WriteLine("\t\tIndividuals: " + reader.Database.Individuals.Count + " Expected: " + expectedIndividuals);
							Console.WriteLine("\t\tFamilies: " + reader.Database.Families.Count + " Expected: " + expectedFamilies);

							Debug.Assert(reader.Database.Individuals.Count == expectedIndividuals);
							Debug.Assert(reader.Database.Families.Count == expectedFamilies);
						}
					}
				}
			}

			Console.WriteLine("-------------------------------");
			Console.WriteLine("Done!");
		}
	}

	public class GoogleSearchResponse
	{
		public GoogleResponseData responseData { get; set; }
		public string responseDetails { get; set; }
		public string responseStatus { get; set; }
	}
	
	public class GoogleResponseData
	{
		public GoogleResult[] results { get; set; }
		public GoogleCursor cursor { get; set; }
		public int estimatedResultCount { get; set; }
		public int currentPageIndex { get; set; }
		public string moreResultsUrl { get; set; }
	}

	public class GoogleCursor
	{
		public GooglePage[] pages { get; set; }
	}

	public class GooglePage
	{
		public int start { get; set; }
		public int label { get; set; }
	}
	
	public class GoogleResult
	{
		public string GsearchResultClass { get; set; }
		public string unescapedUrl { get; set; }
		public string url { get; set; }
		public string visibleUrl { get; set; }
		public string title { get; set; }
		public string titleNoFormatting { get; set; }
		public string content { get; set; }
		public string cacheUrl { get; set; }
	}

}