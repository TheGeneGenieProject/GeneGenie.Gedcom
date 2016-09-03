/*
 *  $Id: GedcomRecordWriter.cs 200 2008-11-30 14:34:07Z davek $
 * 
 *  Copyright (C) 2007-2008 David A Knight
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

namespace GeneGenie.Gedcom.Parser
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Text;

    /// <summary>
    /// Used to save a GedcomDatabase to a GEDCOM file.
    /// </summary>
    public class GedcomRecordWriter
	{
				
		private string _gedcomFile;
		
		private GedcomDatabase _gedcomDatabase;
		
		private bool _test;

		private string _applicationName;
		private string _applicationVersion;
		private string _applicationSystemID;
		private string _corporation;
		private GedcomAddress _corporationAddress;

		
		
				
		/// <summary>
		/// Create a GEDCOM writer for saving a database to a GEDCOM file.
		/// </summary>
		public GedcomRecordWriter()
		{
		}
		
		
		
				
		/// <value>
		/// The name of the GEDCOM file being written
		/// </value>
		public string GedcomFile
		{
			get { return _gedcomFile; }
			set { _gedcomFile = value; }
		}

		/// <value>
		/// The database being written to the GEDCOM file
		/// </value>
		public GedcomDatabase Database
		{
			get { return _gedcomDatabase; }
			set { _gedcomDatabase = value; }	
		}
		
		/// <value>
		/// A flag to indiciate if this is a test output,
		/// used in test code to ensure the output can match
		/// against the expected output by making the date the same
		/// </value>
		public bool Test
		{
			get { return _test; }
			set { _test = value; }
		}

		public string ApplicationName
		{
			get { return _applicationName; }
			set { _applicationName = value; }
		}

		public string ApplicationVersion
		{
			get { return _applicationVersion; }
			set { _applicationVersion = value; }
		}

		public string ApplicationSystemID
		{
			get { return _applicationSystemID; }
			set { _applicationSystemID = value; }
		}

		public string Corporation
		{
			get { return _corporation; }
			set { _corporation = value; }
		}

		public GedcomAddress CorporationAddress
		{
			get { return _corporationAddress; }
			set { _corporationAddress = value; }
		}

		public bool AllowInformationSeparatorOneSave { get; set; }
		public bool AllowLineTabsSave { get; set; }
		public bool AllowTabsSave { get; set; }
		
		
		
				
		/// <summary>
		/// Outputs the currently set GedcomDatabase to the currently set file
		/// </summary>
		public void WriteGedcom()
		{
			WriteGedcom(_gedcomDatabase,_gedcomFile);	
		}
		
		/// <summary>
		/// Outputs a GedcomDatabase to the given file
		/// </summary>
		/// <param name="database">The GedcomDatabase to write</param>
		/// <param name="file">The filename to write to</param>
		public void WriteGedcom(GedcomDatabase database, string file)
		{
			Encoding enc = new UTF8Encoding();
			using (GedcomStreamWriter w = new GedcomStreamWriter(file, false, enc))
			{
				w.AllowInformationSeparatorOneSave = AllowInformationSeparatorOneSave;
				w.AllowLineTabsSave = AllowLineTabsSave;
				w.AllowTabsSave = AllowTabsSave;
				
				// write header

				GedcomHeader header = Database.Header;
				header.Test = Test;
				header.Filename = file;

				header.ApplicationName = ApplicationName;
				header.ApplicationSystemID = ApplicationSystemID;
				header.ApplicationVersion = ApplicationVersion;
				header.Corporation = Corporation;
				header.CorporationAddress = CorporationAddress;
				
				header.Output(w);

				// write records
						
				foreach (DictionaryEntry entry in database)
				{
					GedcomRecord record = entry.Value as GedcomRecord;
					
					record.Output(w);
					w.Write(Environment.NewLine);
				}
				
				w.Write(Environment.NewLine);
				w.WriteLine("0 TRLR");
				w.Write(Environment.NewLine);
			}
		}
		
		

		private class GedcomStreamWriter : StreamWriter
		{
			
			private int _tabSize = 4;
			private string _tab = "    ";
			
			
			
						
			public GedcomStreamWriter(Stream s) : base (s) { }
			public GedcomStreamWriter(string s) : base (s) { }
			public GedcomStreamWriter(Stream s, Encoding e) : base (s, e) { }
			public GedcomStreamWriter(string s, bool append) : base (s, append) { }
			public GedcomStreamWriter(Stream s, Encoding e, Int32 bufSize) : base (s, e, bufSize) { }
			public GedcomStreamWriter(string s, bool append, Encoding e) : base (s, append, e) { }
			public GedcomStreamWriter(string s, bool append, Encoding e, Int32 bufSize) : base (s, append, e, bufSize) { }

			

			
			public bool AllowInformationSeparatorOneSave { get; set; }
			public bool AllowLineTabsSave { get; set; }
			public bool AllowTabsSave { get; set; }

			public int TabSize 
			{ 
				get { return _tabSize;} 
				set 
				{ 
					_tabSize = value;
					_tab = "".PadRight(_tabSize);
				}
			}
			
			
			
			
			public override void Write (char value)
			{
				if (!AllowInformationSeparatorOneSave && value == 0x1f)
				{
					base.Write(" ");
				}
				else if ((!AllowLineTabsSave && value == 0x0b) || (!AllowTabsSave && value == 0x09))
				{
					base.Write(_tab);
				}
				else
				{
					base.Write(value);
				}
			}

			public override void Write (char[] buffer, int index, int count)
			{
				base.Write (buffer, index, count);
			}

			public override void Write (object value)
			{
				Write(value.ToString());
			}

			public override void Write (string value)
			{
				string tmp = value;

				if (!AllowInformationSeparatorOneSave)
				{
					tmp = tmp.Replace("\x001f", " ");
				}
				if (!AllowLineTabsSave)
				{
					tmp = tmp.Replace("\x000b", _tab);
				}
				if (!AllowTabsSave)
				{
					tmp = tmp.Replace("\x0009", _tab);
				}
				
				base.Write(value);
			}

			public override void Write (string format, object arg0)
			{
				Write(string.Format(format, arg0));
			}

			public override void Write (string format, object arg0, object arg1)
			{
				Write(string.Format(format, arg0, arg1));
			}

			public override void Write (string format, object arg0, object arg1, object arg2)
			{
				Write(string.Format(format, arg0, arg1, arg2));
			}

			public override void WriteLine (char value)
			{
				if (!AllowInformationSeparatorOneSave && value == 0x1f)
				{
					base.WriteLine(" ");
				}
				else if ((!AllowLineTabsSave && value == 0x0b) || (!AllowTabsSave && value == 0x09))
				{
					base.WriteLine(_tab);
				}
				else
				{
					base.WriteLine(value);
				}
			}

			public override void WriteLine (char[] buffer, int index, int count)
			{
				base.WriteLine (buffer, index, count);
			}

			public override void WriteLine (object value)
			{
				WriteLine(value.ToString());
			}

			public override void WriteLine (string value)
			{
				string tmp = value;
				
				if (!AllowInformationSeparatorOneSave)
				{
					tmp = tmp.Replace("\x001f", " ");
				}
				if (!AllowLineTabsSave)
				{
					tmp = tmp.Replace("\x000b", _tab);
				}
				if (!AllowTabsSave)
				{
					tmp = tmp.Replace("\x0009", _tab);
				}

				base.WriteLine(tmp);
			}

			public override void WriteLine (string format, object arg0)
			{
				WriteLine(string.Format(format, arg0));
			}

			public override void WriteLine (string format, object arg0, object arg1)
			{
				WriteLine(string.Format(format, arg0, arg1));
			}

			public override void WriteLine (string format, object arg0, object arg1, object arg2)
			{
				WriteLine(string.Format(format, arg0, arg1, arg2));
			}
			
			
		}
	}

}
