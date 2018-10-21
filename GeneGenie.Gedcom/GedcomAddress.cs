// <copyright file="GedcomAddress.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>
// <author> Copyright (C) 2007 David A Knight david@ritter.demon.co.uk </author>
// <author> Copyright (C) 2016 Ryan O'Neill r@genegenie.com </author>

namespace GeneGenie.Gedcom
{
    using System;
    using System.IO;
    using System.Xml;

    /// <summary>
    /// Stores details of an address
    /// </summary>
    public class GedcomAddress : IComparable<GedcomAddress>, IComparable, IEquatable<GedcomAddress>
    {
        private string addressLine;
        private string addressLine1;
        private string addressLine2;
        private string addressLine3;
        private string city;
        private string country;
        private GedcomDatabase database;
        private string email1;
        private string email2;
        private string email3;
        private string fax1;
        private string fax2;
        private string fax3;
        private string phone1;
        private string phone2;
        private string phone3;
        private string postCode;
        private string state;
        private string www1;
        private string www2;
        private string www3;

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomAddress"/> class.
        /// </summary>
        public GedcomAddress()
        {
        }

        /// <summary>
        /// Gets or sets a complete address as a single line.
        /// </summary>
        public string AddressLine
        {
            get
            {
                return addressLine;
            }

            set
            {
                if (value != addressLine)
                {
                    addressLine = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets the first line in an address.
        /// </summary>
        public string AddressLine1
        {
            get
            {
                return addressLine1;
            }

            set
            {
                if (value != addressLine1)
                {
                    addressLine1 = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets the second line in an address.
        /// </summary>
        public string AddressLine2
        {
            get
            {
                return addressLine2;
            }

            set
            {
                if (value != addressLine2)
                {
                    addressLine2 = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets the third line in an address.
        /// </summary>
        public string AddressLine3
        {
            get
            {
                return addressLine3;
            }

            set
            {
                if (value != addressLine3)
                {
                    addressLine3 = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets the date the address was changed.
        /// </summary>
        public GedcomChangeDate ChangeDate { get; set; }

        /// <summary>
        /// Gets or sets the city for the address.
        /// </summary>
        public string City
        {
            get
            {
                return city;
            }

            set
            {
                if (value != city)
                {
                    city = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets the country the address is in.
        /// </summary>
        public string Country
        {
            get
            {
                return country;
            }

            set
            {
                if (value != country)
                {
                    country = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets the database the address is in.
        /// </summary>
        public GedcomDatabase Database
        {
            get { return database; }
            set { database = value; }
        }

        /// <summary>
        /// Gets or sets the main email address.
        /// </summary>
        public string Email1
        {
            get
            {
                return email1;
            }

            set
            {
                if (value != email1)
                {
                    email1 = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets the secondary email address.
        /// </summary>
        public string Email2
        {
            get
            {
                return email2;
            }

            set
            {
                if (value != email2)
                {
                    email2 = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets the tertiary email address.
        /// </summary>
        public string Email3
        {
            get
            {
                return email3;
            }

            set
            {
                if (value != email3)
                {
                    email3 = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets the main fax number.
        /// </summary>
        public string Fax1
        {
            get
            {
                return fax1;
            }

            set
            {
                if (value != fax1)
                {
                    fax1 = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets the secondary fax number.
        /// </summary>
        public string Fax2
        {
            get
            {
                return fax2;
            }

            set
            {
                if (value != fax2)
                {
                    fax2 = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets the tertiary fax number.
        /// </summary>
        public string Fax3
        {
            get
            {
                return fax3;
            }

            set
            {
                if (value != fax3)
                {
                    fax3 = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets the main phone number.
        /// </summary>
        public string Phone1
        {
            get
            {
                return phone1;
            }

            set
            {
                if (value != phone1)
                {
                    phone1 = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets secondary phone number.
        /// </summary>
        public string Phone2
        {
            get
            {
                return phone2;
            }

            set
            {
                if (value != phone2)
                {
                    phone2 = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets the tertiary phone number.
        /// </summary>
        public string Phone3
        {
            get
            {
                return phone3;
            }

            set
            {
                if (value != phone3)
                {
                    phone3 = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets the PostCode / zip code for the address.
        /// </summary>
        public string PostCode
        {
            get
            {
                return postCode;
            }

            set
            {
                if (value != postCode)
                {
                    postCode = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets the state or county for the address.
        /// </summary>
        public string State
        {
            get
            {
                return state;
            }

            set
            {
                if (value != state)
                {
                    state = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets the main website URI.
        /// </summary>
        public string Www1
        {
            get
            {
                return www1;
            }

            set
            {
                if (value != www1)
                {
                    www1 = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets the secondary website URI.
        /// </summary>
        public string Www2
        {
            get
            {
                return www2;
            }

            set
            {
                if (value != www2)
                {
                    www2 = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets the tertiary website URI.
        /// </summary>
        public string Www3
        {
            get
            {
                return www3;
            }

            set
            {
                if (value != www3)
                {
                    www3 = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Compares the current and passed-in address to see if they are the same.
        /// </summary>
        /// <param name="otherAddress">The address to compare the current instance against.</param>
        /// <returns>
        /// A 32-bit signed integer that indicates whether this instance precedes, follows, or appears in the same position in the sort order as the value parameter.
        /// </returns>
        public int CompareTo(GedcomAddress otherAddress)
        {
            if (otherAddress == null)
            {
                return 1;
            }

            var compare = string.Compare(AddressLine, otherAddress.AddressLine);
            if (compare != 0)
            {
                return compare;
            }

            compare = string.Compare(AddressLine1, otherAddress.AddressLine1);
            if (compare != 0)
            {
                return compare;
            }

            compare = string.Compare(AddressLine2, otherAddress.AddressLine2);
            if (compare != 0)
            {
                return compare;
            }

            compare = string.Compare(AddressLine3, otherAddress.AddressLine3);
            if (compare != 0)
            {
                return compare;
            }

            compare = string.Compare(City, otherAddress.City);
            if (compare != 0)
            {
                return compare;
            }

            compare = string.Compare(Country, otherAddress.Country);
            if (compare != 0)
            {
                return compare;
            }

            compare = string.Compare(Email1, otherAddress.Email1);
            if (compare != 0)
            {
                return compare;
            }

            compare = string.Compare(Email2, otherAddress.Email2);
            if (compare != 0)
            {
                return compare;
            }

            compare = string.Compare(Email3, otherAddress.Email3);
            if (compare != 0)
            {
                return compare;
            }

            compare = string.Compare(Fax1, otherAddress.Fax1);
            if (compare != 0)
            {
                return compare;
            }

            compare = string.Compare(Fax2, otherAddress.Fax2);
            if (compare != 0)
            {
                return compare;
            }

            compare = string.Compare(Fax3, otherAddress.Fax3);
            if (compare != 0)
            {
                return compare;
            }

            compare = string.Compare(Phone1, otherAddress.Phone1);
            if (compare != 0)
            {
                return compare;
            }

            compare = string.Compare(Phone2, otherAddress.Phone2);
            if (compare != 0)
            {
                return compare;
            }

            compare = string.Compare(Phone3, otherAddress.Phone3);
            if (compare != 0)
            {
                return compare;
            }

            compare = string.Compare(PostCode, otherAddress.PostCode);
            if (compare != 0)
            {
                return compare;
            }

            compare = string.Compare(State, otherAddress.State);
            if (compare != 0)
            {
                return compare;
            }

            compare = string.Compare(Www1, otherAddress.Www1);
            if (compare != 0)
            {
                return compare;
            }

            compare = string.Compare(Www2, otherAddress.Www2);
            if (compare != 0)
            {
                return compare;
            }

            compare = string.Compare(Www3, otherAddress.Www3);
            if (compare != 0)
            {
                return compare;
            }

            return compare;
        }

        /// <summary>
        /// Compares the current and passed-in address to see if they are the same.
        /// </summary>
        /// <param name="obj">The object to compare the current instance against.</param>
        /// <returns>
        /// A 32-bit signed integer that indicates whether this instance precedes, follows, or appears in the same position in the sort order as the value parameter.
        /// </returns>
        public int CompareTo(object obj)
        {
            return CompareTo(obj as GedcomAddress);
        }

        /// <summary>
        /// Compares the current and passed-in address to see if they are the same.
        /// </summary>
        /// <param name="otherAddress">The address to compare the current instance against.</param>
        /// <returns>
        /// True if they match, False otherwise.
        /// </returns>
        public bool Equals(GedcomAddress otherAddress)
        {
            return CompareTo(otherAddress) == 0;
        }

        /// <summary>
        /// Compares the current and passed-in address to see if they are the same.
        /// </summary>
        /// <param name="obj">The address to compare the current instance against.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return CompareTo(obj) == 0;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            // Overflow is fine, just wrap.
            unchecked
            {
                int hash = 17;

                hash *= 23 + (addressLine == null ? 0 : addressLine.GetHashCode());
                hash *= 23 + (addressLine1 == null ? 0 : addressLine1.GetHashCode());
                hash *= 23 + (addressLine2 == null ? 0 : addressLine2.GetHashCode());
                hash *= 23 + (addressLine3 == null ? 0 : addressLine3.GetHashCode());
                hash *= 23 + (city == null ? 0 : city.GetHashCode());
                hash *= 23 + (country == null ? 0 : country.GetHashCode());
                hash *= 23 + (database == null ? 0 : database.GetHashCode());
                hash *= 23 + (email1 == null ? 0 : email1.GetHashCode());
                hash *= 23 + (email2 == null ? 0 : email2.GetHashCode());
                hash *= 23 + (email3 == null ? 0 : email3.GetHashCode());
                hash *= 23 + (fax1 == null ? 0 : fax1.GetHashCode());
                hash *= 23 + (fax2 == null ? 0 : fax2.GetHashCode());
                hash *= 23 + (fax3 == null ? 0 : fax3.GetHashCode());
                hash *= 23 + (phone1 == null ? 0 : phone1.GetHashCode());
                hash *= 23 + (phone2 == null ? 0 : phone2.GetHashCode());
                hash *= 23 + (phone3 == null ? 0 : phone3.GetHashCode());
                hash *= 23 + (postCode == null ? 0 : postCode.GetHashCode());
                hash *= 23 + (state == null ? 0 : state.GetHashCode());
                hash *= 23 + (www1 == null ? 0 : www1.GetHashCode());
                hash *= 23 + (www2 == null ? 0 : www2.GetHashCode());
                hash *= 23 + (www3 == null ? 0 : www3.GetHashCode());

                return hash;
            }
        }

        /// <summary>
        /// Add the GEDCOM 6 XML elements for the data in this object as child
        /// nodes of the given root.
        /// </summary>
        /// <param name="root">
        /// A <see cref="XmlNode"/>
        /// </param>
        public void GenerateXML(XmlNode root)
        {
            XmlDocument doc = root.OwnerDocument;

            XmlNode node = doc.CreateElement("MailAddress");

            root.AppendChild(node);

            if (!string.IsNullOrEmpty(phone1))
            {
                node = doc.CreateElement("Phone");
                node.AppendChild(doc.CreateTextNode(phone1));
                root.AppendChild(node);
            }

            if (!string.IsNullOrEmpty(phone2))
            {
                node = doc.CreateElement("Phone");
                node.AppendChild(doc.CreateTextNode(phone2));
                root.AppendChild(node);
            }

            if (!string.IsNullOrEmpty(phone3))
            {
                node = doc.CreateElement("Phone");
                node.AppendChild(doc.CreateTextNode(phone3));
                root.AppendChild(node);
            }

            if (!string.IsNullOrEmpty(email1))
            {
                node = doc.CreateElement("Email");
                node.AppendChild(doc.CreateTextNode(email1));
                root.AppendChild(node);
            }

            if (!string.IsNullOrEmpty(email2))
            {
                node = doc.CreateElement("Email");
                node.AppendChild(doc.CreateTextNode(email2));
                root.AppendChild(node);
            }

            if (!string.IsNullOrEmpty(email3))
            {
                node = doc.CreateElement("Email");
                node.AppendChild(doc.CreateTextNode(email3));
                root.AppendChild(node);
            }

            if (!string.IsNullOrEmpty(www1))
            {
                node = doc.CreateElement("URI");
                node.AppendChild(doc.CreateTextNode(www1));
                root.AppendChild(node);
            }

            if (!string.IsNullOrEmpty(www2))
            {
                node = doc.CreateElement("URI");
                node.AppendChild(doc.CreateTextNode(www2));
                root.AppendChild(node);
            }

            if (!string.IsNullOrEmpty(www3))
            {
                node = doc.CreateElement("URI");
                node.AppendChild(doc.CreateTextNode(www3));
                root.AppendChild(node);
            }
        }

        /// <summary>
        /// Get the GEDCOM 5.5 lines for the data in this object.
        /// Lines start at the given level
        /// </summary>
        /// <param name="sw">
        /// A <see cref="TextWriter"/>
        /// </param>
        /// <param name="level">
        /// A <see cref="int"/>
        /// </param>
        public void Output(TextWriter sw, int level)
        {
            sw.Write(Environment.NewLine);
            sw.Write(level.ToString());
            sw.Write(" ADDR");

            if (!string.IsNullOrEmpty(AddressLine))
            {
                sw.Write(" ");

                Util.SplitLineText(sw, AddressLine, level, 60, 3, true);
            }

            string levelStr = null;
            string levelPlusOne = null;

            if (!string.IsNullOrEmpty(AddressLine1))
            {
                if (levelPlusOne == null)
                {
                    levelPlusOne = (level + 1).ToString();
                }

                string line = AddressLine1.Replace("@", "@@");

                sw.Write(Environment.NewLine);
                sw.Write(levelPlusOne);
                sw.Write(" ADR1 ");
                if (line.Length <= 60)
                {
                    sw.Write(AddressLine1);
                }
                else
                {
                    sw.Write(line.Substring(0, 60));
                    System.Diagnostics.Debug.WriteLine("Truncating AddressLine1");
                }
            }

            if (!string.IsNullOrEmpty(AddressLine2))
            {
                if (levelPlusOne == null)
                {
                    levelPlusOne = (level + 1).ToString();
                }

                string line = AddressLine2.Replace("@", "@@");

                sw.Write(Environment.NewLine);
                sw.Write(levelPlusOne);
                sw.Write(" ADR2 ");
                if (line.Length <= 60)
                {
                    sw.Write(line);
                }
                else
                {
                    sw.Write(line.Substring(0, 60));
                    System.Diagnostics.Debug.WriteLine("Truncating AddressLine2");
                }
            }

            if (!string.IsNullOrEmpty(AddressLine3))
            {
                if (levelPlusOne == null)
                {
                    levelPlusOne = (level + 1).ToString();
                }

                string line = AddressLine3.Replace("@", "@@");

                sw.Write(Environment.NewLine);
                sw.Write(levelPlusOne);
                sw.Write(" ADR3 ");
                if (line.Length <= 60)
                {
                    sw.Write(line);
                }
                else
                {
                    sw.Write(line.Substring(0, 60));
                    System.Diagnostics.Debug.WriteLine("Truncating AddressLine3");
                }
            }

            if (!string.IsNullOrEmpty(City))
            {
                if (levelPlusOne == null)
                {
                    levelPlusOne = (level + 1).ToString();
                }

                string line = City.Replace("@", "@@");

                sw.Write(Environment.NewLine);
                sw.Write(levelPlusOne);
                sw.Write(" CITY ");
                if (line.Length <= 60)
                {
                    sw.Write(line);
                }
                else
                {
                    sw.Write(line.Substring(0, 60));
                    System.Diagnostics.Debug.WriteLine("Truncating City");
                }
            }

            if (!string.IsNullOrEmpty(State))
            {
                if (levelPlusOne == null)
                {
                    levelPlusOne = (level + 1).ToString();
                }

                string line = State.Replace("@", "@@");

                sw.Write(Environment.NewLine);
                sw.Write(levelPlusOne);
                sw.Write(" STAE ");
                if (line.Length <= 60)
                {
                    sw.Write(line);
                }
                else
                {
                    sw.Write(line.Substring(0, 60));
                    System.Diagnostics.Debug.WriteLine("Truncating State");
                }
            }

            if (!string.IsNullOrEmpty(PostCode))
            {
                if (levelPlusOne == null)
                {
                    levelPlusOne = (level + 1).ToString();
                }

                string line = PostCode.Replace("@", "@@");

                sw.Write(Environment.NewLine);
                sw.Write(levelPlusOne);
                sw.Write(" POST ");
                if (line.Length <= 10)
                {
                    sw.Write(line);
                }
                else
                {
                    sw.Write(line.Substring(0, 10));
                    System.Diagnostics.Debug.WriteLine("Truncating PostCode");
                }
            }

            if (!string.IsNullOrEmpty(Country))
            {
                if (levelPlusOne == null)
                {
                    levelPlusOne = (level + 1).ToString();
                }

                string line = Country.Replace("@", "@@");

                sw.Write(Environment.NewLine);
                sw.Write(levelPlusOne);
                sw.Write(" CTRY ");
                if (line.Length <= 60)
                {
                    sw.Write(line);
                }
                else
                {
                    sw.Write(line.Substring(0, 60));
                    System.Diagnostics.Debug.WriteLine("Truncating Country");
                }
            }

            if (!string.IsNullOrEmpty(Phone1))
            {
                if (levelStr == null)
                {
                    levelStr = level.ToString();
                }

                string line = Phone1.Replace("@", "@@");

                sw.Write(Environment.NewLine);
                sw.Write(levelStr);
                sw.Write(" PHON ");
                if (line.Length <= 25)
                {
                    sw.Write(line);
                }
                else
                {
                    sw.Write(line.Substring(0, 25));
                    System.Diagnostics.Debug.WriteLine("Truncating Phone1");
                }
            }

            if (!string.IsNullOrEmpty(Phone2))
            {
                if (levelStr == null)
                {
                    levelStr = level.ToString();
                }

                string line = Phone2.Replace("@", "@@");

                sw.Write(Environment.NewLine);
                sw.Write(levelStr);
                sw.Write(" PHON ");
                if (line.Length <= 25)
                {
                    sw.Write(line);
                }
                else
                {
                    sw.Write(line.Substring(0, 25));
                    System.Diagnostics.Debug.WriteLine("Truncating Phone2");
                }
            }

            if (!string.IsNullOrEmpty(Phone3))
            {
                if (levelStr == null)
                {
                    levelStr = level.ToString();
                }

                string line = Phone3.Replace("@", "@@");

                sw.Write(Environment.NewLine);
                sw.Write(levelStr);
                sw.Write(" PHON ");
                if (line.Length <= 25)
                {
                    sw.Write(line);
                }
                else
                {
                    sw.Write(line.Substring(0, 25));
                    System.Diagnostics.Debug.WriteLine("Truncating Phone3");
                }
            }

            if (!string.IsNullOrEmpty(Fax1))
            {
                if (levelStr == null)
                {
                    levelStr = level.ToString();
                }

                string line = Fax1.Replace("@", "@@");

                sw.Write(Environment.NewLine);
                sw.Write(levelStr);
                sw.Write(" FAX ");
                if (line.Length <= 60)
                {
                    sw.Write(line);
                }
                else
                {
                    sw.Write(line.Substring(0, 60));
                    System.Diagnostics.Debug.WriteLine("Truncating Fax1");
                }
            }

            if (!string.IsNullOrEmpty(Fax2))
            {
                if (levelStr == null)
                {
                    levelStr = level.ToString();
                }

                string line = Fax2.Replace("@", "@@");

                sw.Write(Environment.NewLine);
                sw.Write(levelStr);
                sw.Write(" FAX ");
                if (line.Length <= 60)
                {
                    sw.Write(line);
                }
                else
                {
                    sw.Write(line.Substring(0, 60));
                    System.Diagnostics.Debug.WriteLine("Truncating Fax2");
                }
            }

            if (!string.IsNullOrEmpty(Fax3))
            {
                if (levelStr == null)
                {
                    levelStr = level.ToString();
                }

                string line = Fax3.Replace("@", "@@");

                sw.Write(Environment.NewLine);
                sw.Write(levelStr);
                sw.Write(" FAX ");
                if (line.Length <= 60)
                {
                    sw.Write(line);
                }
                else
                {
                    sw.Write(line.Substring(0, 60));
                    System.Diagnostics.Debug.WriteLine("Truncating Fax3");
                }
            }

            if (!string.IsNullOrEmpty(Email1))
            {
                if (levelStr == null)
                {
                    levelStr = level.ToString();
                }

                string line = Email1.Replace("@", "@@");

                sw.Write(Environment.NewLine);
                sw.Write(levelStr);
                sw.Write(" EMAIL ");
                if (line.Length <= 120)
                {
                    sw.Write(line);
                }
                else
                {
                    sw.Write(line.Substring(0, 120));
                    System.Diagnostics.Debug.WriteLine("Truncating Email1");
                }
            }

            if (!string.IsNullOrEmpty(Email2))
            {
                if (levelStr == null)
                {
                    levelStr = level.ToString();
                }

                string line = Email2.Replace("@", "@@");

                sw.Write(Environment.NewLine);
                sw.Write(levelStr);
                sw.Write(" EMAIL ");
                if (line.Length <= 120)
                {
                    sw.Write(line);
                }
                else
                {
                    sw.Write(line.Substring(0, 120));
                    System.Diagnostics.Debug.WriteLine("Truncating Email2");
                }
            }

            if (!string.IsNullOrEmpty(Email3))
            {
                if (levelStr == null)
                {
                    levelStr = level.ToString();
                }

                string line = Email3.Replace("@", "@@");

                sw.Write(Environment.NewLine);
                sw.Write(levelStr);
                sw.Write(" EMAIL ");
                if (line.Length <= 120)
                {
                    sw.Write(line);
                }
                else
                {
                    sw.Write(line.Substring(0, 120));
                    System.Diagnostics.Debug.WriteLine("Truncating Email3");
                }
            }

            if (!string.IsNullOrEmpty(Www1))
            {
                if (levelStr == null)
                {
                    levelStr = level.ToString();
                }

                string line = Www1.Replace("@", "@@");

                sw.Write(Environment.NewLine);
                sw.Write(levelStr);
                sw.Write(" WWW ");
                if (line.Length <= 120)
                {
                    sw.Write(line);
                }
                else
                {
                    sw.Write(line.Substring(0, 120));
                    System.Diagnostics.Debug.WriteLine("Truncating Www1");
                }
            }

            if (!string.IsNullOrEmpty(Www2))
            {
                if (levelStr == null)
                {
                    levelStr = level.ToString();
                }

                string line = Www2.Replace("@", "@@");

                sw.Write(Environment.NewLine);
                sw.Write(levelStr);
                sw.Write(" WWW ");
                if (line.Length <= 120)
                {
                    sw.Write(line);
                }
                else
                {
                    sw.Write(line.Substring(0, 120));
                    System.Diagnostics.Debug.WriteLine("Truncating Www2");
                }
            }

            if (!string.IsNullOrEmpty(Www3))
            {
                if (levelStr == null)
                {
                    levelStr = level.ToString();
                }

                string line = Www3.Replace("@", "@@");

                sw.Write(Environment.NewLine);
                sw.Write(levelStr);
                sw.Write(" WWW ");
                if (line.Length <= 120)
                {
                    sw.Write(line);
                }
                else
                {
                    sw.Write(line.Substring(0, 120));
                    System.Diagnostics.Debug.WriteLine("Truncating Www3");
                }
            }
        }

        private void Changed()
        {
            if (database == null)
            {
                // System.Console.WriteLine("Changed() called on record with no database set");
                //
                // System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace();
                // foreach (System.Diagnostics.StackFrame f in trace.GetFrames())
                // {
                //  System.Console.WriteLine(f);
                // }
            }
            else if (!database.Loading)
            {
                if (ChangeDate == null)
                {
                    ChangeDate = new GedcomChangeDate(database);

                    // TODO: what level?
                }

                DateTime now = DateTime.Now;

                ChangeDate.Date1 = now.ToString("dd MMM yyyy");
                ChangeDate.Time = now.ToString("hh:mm:ss");
            }
        }
    }
}