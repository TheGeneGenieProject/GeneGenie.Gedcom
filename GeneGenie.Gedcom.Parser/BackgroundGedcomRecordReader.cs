/*
 *  $Id: BackgroundGedcomRecordReader.cs 197 2008-11-15 12:41:00Z davek $
 * 
 *  Copyright (C) 2007 David A Knight <david@ritter.demon.co.uk>
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
    using System.ComponentModel;

    /// <summary>
    /// A GedcomRecordReader that utilises a BackgroundWorker for loading
    /// a gedcom file in a separate thread
    /// </summary>
    public class BackgroundGedcomRecordReader : GedcomRecordReader
	{
				
		private BackgroundWorker _worker;
		
		
		
				
		/// <summary>
		/// Create a new BackgroundGedcomRecordReader
		/// </summary>
		public BackgroundGedcomRecordReader()
		{
			_worker = new BackgroundWorker();
			
			_worker.WorkerReportsProgress = true;
			_worker.WorkerSupportsCancellation = true;
			
			_worker.DoWork += new DoWorkEventHandler(DoReadGedcom_DoWork);
			_worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(DoReadGedcom_RunWorkerCompleted);
			_worker.ProgressChanged += new ProgressChangedEventHandler(DoReadGedcom_ProgressChanged);
					
			this.PercentageDone += new EventHandler(DoReadGedcom_PercentageDone);
		}
		
		
	
				
		/// <summary>
		/// Hookup to this event for notification of when the reader finished reading the GEDCOM file
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="e">
		/// A <see cref="EventArgs"/>
		/// </param>
		public event RunWorkerCompletedEventHandler Completed;
		
		/// <summary>
		/// Hook up to this event to provide feedback on the progress of reading the GEDCOM file
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="e">
		/// A <see cref="EventArgs"/>
		/// </param>
		public event ProgressChangedEventHandler ProgressChanged;
		
		
	
			
		private void DoReadGedcom_PercentageDone(object sender, EventArgs e)
		{
			_worker.ReportProgress((int)Progress);
		}
	
		private void DoReadGedcom_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (Completed != null)
			{
				Completed(this, e);
			}
		}
	
		private void DoReadGedcom_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			if (ProgressChanged != null)
			{
				ProgressChanged(this, e);
			}
		}

		
		
				
		/// <summary>
		/// Start reading the GEDCOM file refered to by the GedcomFile property
		/// </summary>
		public new void ReadGedcom()
		{
			_worker.RunWorkerAsync();
		}

        /// <summary>
        /// Start reading the given GEDCOM file
        /// </summary>
        /// <param name="gedcomFile">
        /// A <see cref="string"/>
        /// </param>
        public new void ReadGedcom(string gedcomFile)
		{
			_worker.RunWorkerAsync(gedcomFile);
		}
		
		private void DoReadGedcom_DoWork(object sender, DoWorkEventArgs e)
		{
			if (e.Argument != null)
			{
				base.ReadGedcom((string)e.Argument);
			}
			else
			{
				base.ReadGedcom();
			}
		}
		
		
	}
}
