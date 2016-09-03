/*
 *  $Id: GedcomFamilyLink.cs 194 2008-11-10 20:39:37Z davek $
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
    /// <summary>
    /// How an individal is linked to a family
    /// </summary>
    public class GedcomFamilyLink : GedcomRecord
    {
        private string _Family;
        private string _Indi;

        private PedegreeLinkageType _Pedigree;
        private ChildLinkageStatus _Status;

        private PedegreeLinkageType _FatherPedigree;
        private PedegreeLinkageType _MotherPedigree;

        private bool _preferedSpouse;

        public GedcomFamilyLink()
        {
            _Pedigree = PedegreeLinkageType.Unknown;
        }

        public override GedcomRecordType RecordType
        {
            get { return GedcomRecordType.FamilyLink; }
        }

        public string Family
        {
            get
            {
                return _Family;
            }

            set
            {
                if (value != _Family)
                {
                    _Family = value;
                    Changed();
                }
            }
        }

        public string Indi
        {
            get
            {
                return _Indi;
            }

            set
            {
                if (value != _Indi)
                {
                    _Indi = value;
                    Changed();
                }
            }
        }

        public PedegreeLinkageType Pedigree
        {
            get
            {
                return _Pedigree;
            }

            set
            {
                if (value != _Pedigree)
                {
                    _Pedigree = value;
                    FatherPedigree = value;
                    MotherPedigree = value;
                    Changed();
                }
            }
        }

        public PedegreeLinkageType FatherPedigree
        {
            get
            {
                return _FatherPedigree;
            }

            set
            {
                if (value != _FatherPedigree)
                {
                    _FatherPedigree = value;
                    Changed();
                }
            }
        }

        public PedegreeLinkageType MotherPedigree
        {
            get
            {
                return _MotherPedigree;
            }

            set
            {
                if (value != _MotherPedigree)
                {
                    _MotherPedigree= value;
                    Changed();
                }
            }
        }

        public ChildLinkageStatus Status
        {
            get
            {
                return _Status;
            }

            set
            {
                if (value != _Status)
                {
                    _Status = value;
                    Changed();
                }
            }
        }

        public bool PreferedSpouse
        {
            get { return _preferedSpouse; }
            set { _preferedSpouse = value; }
        }
    }
}
