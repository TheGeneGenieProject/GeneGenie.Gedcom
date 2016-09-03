/*
 *  $Id: GedcomRecordedEvent.cs 199 2008-11-15 15:20:44Z davek $
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

namespace GeneGenie.Gedcom
{
    using System;

    public class GedcomRecordedEvent
    {
        private GedcomDatabase _database;

        private GedcomRecordList<GedcomEventType> _types;
        private GedcomDate _date;
        private GedcomPlace _place;

        private GedcomChangeDate _ChangeDate;

        public GedcomRecordedEvent()
        {
        }

        public GedcomDatabase Database
        {
            get { return _database; }
            set { _database = value; }
        }

        public GedcomRecordList<GedcomEventType> Types
        {
            get
            {
                if (_types == null)
                {
                    _types = new GedcomRecordList<GedcomEventType>();
                }

                return _types;
            }
            set
            {
                if (_types != value)
                {
                    _types = value;
                    Changed();
                }
            }
        }

        public GedcomDate Date
        {
            get
            {
                return _date;
            }

            set
            {
                if (value != _date)
                {
                    _date = value;
                    Changed();
                }
            }
        }

        public GedcomPlace Place
        {
            get
            {
                return _place;
            }

            set
            {
                if (value != _place)
                {
                    _place = value;
                    Changed();
                }
            }
        }

        public GedcomChangeDate ChangeDate
        {
            get
            {
                GedcomChangeDate realChangeDate = null;
                GedcomChangeDate childChangeDate;
                if (_date != null)
                {
                    childChangeDate = _date.ChangeDate;
                    if (childChangeDate != null && realChangeDate != null && childChangeDate > realChangeDate)
                    {
                        realChangeDate = childChangeDate;
                    }
                }

                if (_place != null)
                {
                    childChangeDate = _place.ChangeDate;
                    if (childChangeDate != null && realChangeDate != null && childChangeDate > realChangeDate)
                    {
                        realChangeDate = childChangeDate;
                    }
                }

                return realChangeDate;
            }

            set
            {
                _ChangeDate = value;
            }
        }

        protected virtual void Changed()
        {
            if (_database == null)
            {
                //              System.Console.WriteLine("Changed() called on record with no database set");
                //
                //              System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace();
                //              foreach (System.Diagnostics.StackFrame f in trace.GetFrames())
                //              {
                //                  System.Console.WriteLine(f);
                //              }
            }
            else if (!_database.Loading)
            {
                if (_ChangeDate == null)
                {
                    _ChangeDate = new GedcomChangeDate(_database);
                    // FIXME: what level?
                }

                DateTime now = DateTime.Now;

                _ChangeDate.Date1 = now.ToString("dd MMM yyyy");
                _ChangeDate.Time = now.ToString("hh:mm:ss");
            }
        }
    }
}
