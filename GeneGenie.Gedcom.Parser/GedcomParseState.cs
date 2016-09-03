/*
 *  $Id: GedcomParseState.cs 201 2008-12-01 20:00:26Z davek $
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
    using System.Collections.Generic;
    using Utility;

    /// <summary>
    /// GedcomParseState is used to maintain the current parser status
    /// for GedcomRecordReader
    /// </summary>
    public class GedcomParseState
	{
				
		private Stack<GedcomRecord> _Records;
	
		private Stack<Pair<string,int>> _PreviousTags;
		
		private GedcomDatabase _Database;
		
		private Pair<string,int>[] _PairPool;
		
		
		
				
		/// <summary>
		/// Create a new parse state object
		/// </summary>
		public GedcomParseState()
		{
			_Records = new Stack<GedcomRecord>();
			_Database = new GedcomDatabase();
			// max level of 99, pre alloc size of stack
			_PreviousTags = new Stack<Pair<string,int>>(100);
			
			_PairPool = new Pair<string,int>[100];
		}
		
		
		
				
		/// <value>
		/// The previous tag name
		/// </value>
		public string PreviousTag
		{
			get 
			{ 
				string ret = string.Empty;
				
				if (_PreviousTags.Count > 0)
				{
					ret = _PreviousTags.Peek().First;
				}
				
				return ret;
			}
		}
		
		/// <value>
		/// Level of the previous tag
		/// </value>
		public int PreviousLevel
		{
			get
			{
				int ret = -1;
					
				if (_PreviousTags.Count > 0)
				{
					ret = _PreviousTags.Peek().Second;
				}
					
				return ret;
			}
		}
		
		/// <value>
		/// The stack of previous tag names / levels
		/// </value>
		public Stack<Pair<string,int>> PreviousTags
		{
			get { return _PreviousTags; }
		}
		
		/// <value>
		/// Parse stack of current records, back to the last level 0 record
		/// </value>
		public Stack<GedcomRecord> Records
		{
			get { return _Records; }	
		}

		/// <value>
		/// The current database the GedcomRecordReader is working with
		/// </value>
		public GedcomDatabase Database
		{
			get { return _Database; }	
		}
		
		
		
				
		/// <summary>
		/// Obtain the name of the parent GEDCOM tag
		/// </summary>
		/// <param name="level">
		/// A <see cref="System.Int32"/>.  The level of the current tag
		/// </param>
		/// <returns>
		/// A <see cref="System.String"/>.  The name of the parent GEDCOM tag.
		/// </returns>
		public string ParentTag(int level)
		{
			string ret = string.Empty;
			
			if (_PreviousTags.Count > 0)
			{
				foreach (Pair<string,int> previous in _PreviousTags)
				{
					if (previous.Second < level)
					{
						ret = previous.First;
						break;
					}
				}
			}
			
			return ret;
		}
		
		public void AddPreviousTag(string name, int level)
		{
			if (level > 99)
			{
				throw new Exception("Only 99 levels supported, as per GEDCOM spec");
			}
			else
			{
				Pair<string,int> pair = _PairPool[level];
				if (pair == null)
				{
					pair = new Pair<string,int>();
					_PairPool[level] = pair;
				}
				pair.First = name;
				pair.Second = level;
			
				_PreviousTags.Push(pair);
			}
		}
		
		
	}
}
