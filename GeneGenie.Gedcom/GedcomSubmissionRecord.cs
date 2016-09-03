/*
 *  $Id: GedcomSubmissionRecord.cs 191 2008-10-25 18:43:33Z davek $
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
    public class GedcomSubmissionRecord : GedcomRecord
    {
        private string _Submitter;

        private string _FamilyFile;
        private string _TempleCode;
        private int _GenerationsOfAncestors;
        private int _GenerationsOfDecendants;
        private bool _OrdinanceProcessFlag;

        public GedcomSubmissionRecord()
        {
        }

        public override GedcomRecordType RecordType
        {
            get { return GedcomRecordType.Submission; }
        }

        public override string GedcomTag
        {
            get { return "SUBN"; }
        }

        public string Submitter
        {
            get
            {
                return _Submitter;
            }

            set
            {
                if (value != _Submitter)
                {
                    _Submitter = value;
                    Changed();
                }
            }
        }

        public string FamilyFile
        {
            get
            {
                return _FamilyFile;
            }

            set
            {
                if (value != _FamilyFile)
                {
                    _FamilyFile = value;
                    Changed();
                }
            }
        }

        public string TempleCode
        {
            get
            {
                return _TempleCode;
            }

            set
            {
                if (value != _TempleCode)
                {
                    _TempleCode = value;
                    Changed();
                }
            }
        }

        public int GenerationsOfAncestors
        {
            get
            {
                return _GenerationsOfAncestors;
            }

            set
            {
                if (value != _GenerationsOfAncestors)
                {
                    _GenerationsOfAncestors = value;
                    Changed();
                }
            }
        }

        public int GenerationsOfDecendants
        {
            get
            {
                return _GenerationsOfDecendants;
            }

            set
            {
                if (value != _GenerationsOfDecendants)
                {
                    _GenerationsOfDecendants = value;
                    Changed();
                }
            }
        }

        public bool OrdinanceProcessFlag
        {
            get
            {
                return _OrdinanceProcessFlag;
            }

            set
            {
                if (value != _OrdinanceProcessFlag)
                {
                    _OrdinanceProcessFlag = value;
                    Changed();
                }
            }
        }

        // FIXME: add output method
    }
}
