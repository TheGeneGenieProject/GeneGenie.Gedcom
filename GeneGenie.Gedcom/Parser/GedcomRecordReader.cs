// <copyright file="GedcomRecordReader.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>
// <author> Copyright (C) 2016 Ryan O'Neill r@genegenie.com </author>
// <author> Copyright (C) 2007-2008 David A Knight david@ritter.demon.co.uk </author>

namespace GeneGenie.Gedcom.Parser
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using GeneGenie.Gedcom.Enums;
    using GeneGenie.Gedcom.Helpers;

    /// <summary>
    /// GedcomRecordReader will read in a given gedcom file
    /// producing a GedcomDatabase and related objects.
    /// This is generally what you want to use to read in a GEDCOM
    /// file for manipulation.
    /// </summary>
    public class GedcomRecordReader
    {
        private GedcomParseState parseState;

        private XRefIndexedKeyCollection xrefCollection;

        private int percent;

        private List<string> missingReferences;

        private List<GedcomSourceCitation> sourceCitations;
        private List<GedcomRepositoryCitation> repoCitations;

        private List<string> removedNotes;

        private int lineNumber = 0; // For reporting errors and warnings.
        private int level;
        private string tag;
        private string xrefId;
        private string lineValue;
        private GedcomLineValueType lineValueType;

        private StreamReader stream;

        // Newline varies by input file, we scan for it on open so we can preserve for writing.
        private string newlineDelimiter;

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomRecordReader"/> class.
        /// Create a GedcomRecordReader for reading a GEDCOM file into a GedcomDatabase.
        /// </summary>
        public GedcomRecordReader()
        {
            Parser = new GedcomParser();

            // we don't care if delims are multiple spaces
            Parser.IgnoreInvalidDelim = true;

            // we don't care if lines are missing delimeters
            Parser.IgnoreMissingTerms = true;

            // apply hack for lines that are just part of the line value
            // for the previous CONC/CONT in invalid GEDCOM files
            Parser.ApplyConcContOnNewLineHack = true;

            // allow tabs in line values, seen from RootsMagic and GenealogyJ
            Parser.AllowTabs = true;

            // allow line tabs in line values, seen from Legacy
            Parser.AllowLineTabs = true;

            // allow information separator one chars, seen from that bastion
            // of spec compliance RootsMagic
            Parser.AllowInformationSeparatorOne = true;

            // allow - or _ in tag names (GenealogyJ?)
            Parser.AllowHyphenOrUnderscoreInTag = true;

            Parser.ParserError += Parser_ParseError;
            Parser.TagFound += Parser_TagFound;
        }

        /// <summary>
        /// Fired as each line is parsed from the given file in GedcomRead
        /// </summary>
        public event EventHandler PercentageDone;

        /// <summary>
        /// Gets or sets the parser to be used when reading the GEDCOM file.
        /// </summary>
        public GedcomParser Parser { get; set; }

        /// <summary>
        /// Gets or sets the GEDCOM file being read.
        /// </summary>
        public string GedcomFile { get; set; }

        /// <summary>
        /// Gets the database the records will be added to.
        /// </summary>
        public GedcomDatabase Database
        {
            get { return parseState.Database; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether xrefs are replaced.
        /// When reading GEDCOM files into a database the
        /// xref ids may already exist, settings this to true
        /// will cause new ids to be generated created for the
        /// records being read.
        /// </summary>
        public bool ReplaceXRefs { get; set; }

        /// <summary>
        /// Gets percentage progress of GedcomRead.
        /// </summary>
        public int Progress
        {
            get { return percent; }
        }

        /// <summary>
        /// A static helper for reading a gedcom file and returning the reader in one go.
        /// </summary>
        /// <param name="gedcomFilePath">The gedcom file path.</param>
        /// <param name="replaceXRefs">The value indicating whether [replace x refs].</param>
        /// <returns>The reader used to load the file.</returns>
        public static GedcomRecordReader CreateReader(string gedcomFilePath, bool replaceXRefs = true)
        {
            var reader = new GedcomRecordReader();
            reader.ReplaceXRefs = replaceXRefs;
            reader.ReadGedcom(gedcomFilePath);
            return reader;
        }

        /// <summary>
        /// Starts reading the gedcom file currently set via the GedcomFile property.
        /// </summary>
        /// <returns>bool indicating if the file was successfully read.</returns>
        public bool ReadGedcom()
        {
            return ReadGedcom(GedcomFile);
        }

        /// <summary>
        /// Starts reading the specified gedcom file.
        /// </summary>
        /// <param name="gedcomFile">Filename to read.</param>
        /// <returns>bool indicating if the file was successfully read.</returns>
        public bool ReadGedcom(string gedcomFile)
        {
            bool success = false;

            GedcomFile = gedcomFile;

            percent = 0;

            FileInfo info = new FileInfo(gedcomFile);
            long fileSize = info.Length;
            long read = 0;

            missingReferences = new List<string>();
            sourceCitations = new List<GedcomSourceCitation>();
            repoCitations = new List<GedcomRepositoryCitation>();

            // Register additional code pages from nuget package so we can deal with exotic character sets.
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            try
            {
                stream = null;
                Encoding enc = Encoding.Default;

                using (FileStream fileStream = File.OpenRead(gedcomFile))
                {
                    ResetParse();

                    byte[] bom = new byte[4];

                    fileStream.Read(bom, 0, 4);

                    // look for BOMs, if found we will ignore the CHAR tag
                    // don't use .net look for bom as we also want to detect
                    // unicode where there isn't a BOM, as far as the parser
                    // is concerned the data is utf16le if we detect this way
                    // as the conversion is already done
                    if (bom[0] == 0xEF && bom[1] == 0xBB && bom[2] == 0xBF)
                    {
                        Parser.Charset = GedcomCharset.UTF16LE;
                        enc = Encoding.UTF8;
                    }
                    else if (bom[0] == 0xFE && bom[1] == 0xFF)
                    {
                        Parser.Charset = GedcomCharset.UTF16LE;
                        enc = Encoding.BigEndianUnicode;
                    }
                    else if (bom[0] == 0xFF && bom[1] == 0xFE && bom[2] == 0x00 && bom[3] == 0x00)
                    {
                        Parser.Charset = GedcomCharset.UTF16LE;
                        enc = Encoding.UTF32;
                    }
                    else if (bom[0] == 0xFF && bom[1] == 0xFE)
                    {
                        Parser.Charset = GedcomCharset.UTF16LE;
                        enc = Encoding.Unicode;
                    }
                    else if (bom[0] == 0x00 && bom[1] == 0x00 && bom[2] == 0xFE && bom[3] == 0xFF)
                    {
                        Parser.Charset = GedcomCharset.UTF16LE;
                        enc = Encoding.UTF32;
                    }
                    else if (bom[0] == 0x00 && bom[2] == 0x00)
                    {
                        Parser.Charset = GedcomCharset.UTF16LE;
                        enc = Encoding.BigEndianUnicode;
                    }
                    else if (bom[1] == 0x00 && bom[3] == 0x00)
                    {
                        Parser.Charset = GedcomCharset.UTF16LE;
                        enc = Encoding.Unicode;
                    }
                }

                newlineDelimiter = DetectNewline(gedcomFile, enc);

                stream = new StreamReader(gedcomFile, enc);

                while (!stream.EndOfStream)
                {
                    lineNumber++;
                    string line = stream.ReadLine();

                    if (line != null)
                    {
                        read += line.Length + newlineDelimiter.Length;
                        Parser.GedcomParse(line);

                        // to allow for inaccuracy above
                        int percentDone = (int)Math.Min(100, (read * 100.0F) / fileSize);
                        if (percentDone != percent)
                        {
                            percent = percentDone;
                            if (PercentageDone != null)
                            {
                                PercentageDone(this, EventArgs.Empty);
                            }
                        }
                    }
                }

                Flush();
            }
            finally
            {
                if (stream != null)
                {
                    stream.Dispose();
                }
            }

            success = Parser.ErrorState == GedcomErrorState.NoError;

            if (success)
            {
                percent = 100;

                // cleanup header record, don't want submitter record or content description in the main
                // database submitters / notes
                GedcomHeader header = Database.Header;

                if (header != null)
                {
                    if (header.Notes.Count > 0)
                    {
                        string xref = header.Notes[0];

                        // belongs in content description, not top level record notes
                        header.Notes.Remove(xref);
                        header.ContentDescription = (GedcomNoteRecord)Database[xref];

                        // fix up level, note is inline in the header + remove from database
                        // list of notes
                        header.ContentDescription.Level = 1;
                        header.ContentDescription.XRefID = string.Empty;
                        Database.Remove(xref, header.ContentDescription);
                    }

                    // brothers keeper doesn't output a source name, so set the name to
                    // the same as the ID if it is empty
                    if (string.IsNullOrEmpty(header.ApplicationName) && !string.IsNullOrEmpty(header.ApplicationSystemId))
                    {
                        header.ApplicationName = header.ApplicationSystemId;
                    }
                }

                // add any missing child in and spouse in linkage
                foreach (GedcomFamilyRecord family in Database.Families)
                {
                    string husbandID = family.Husband;
                    if (!string.IsNullOrEmpty(husbandID))
                    {
                        GedcomIndividualRecord husband = Database[husbandID] as GedcomIndividualRecord;
                        if (husband != null)
                        {
                            GedcomFamilyLink famLink = null;

                            if (!husband.SpouseInFamily(family.XRefID, out famLink))
                            {
                                famLink = new GedcomFamilyLink();
                                famLink.Database = Database;
                                famLink.Family = family.XRefID;
                                famLink.Individual = husbandID;
                                famLink.Level = 1;
                                famLink.PreferedSpouse = husband.SpouseIn.Count == 0;
                                husband.SpouseIn.Add(famLink);
                            }
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("Husband in family points to non individual record");
                        }
                    }

                    string wifeID = family.Wife;
                    if (!string.IsNullOrEmpty(wifeID))
                    {
                        GedcomIndividualRecord wife = Database[wifeID] as GedcomIndividualRecord;
                        if (wife != null)
                        {
                            GedcomFamilyLink famLink = null;

                            if (!wife.SpouseInFamily(family.XRefID, out famLink))
                            {
                                famLink = new GedcomFamilyLink();
                                famLink.Database = Database;
                                famLink.Family = family.XRefID;
                                famLink.Individual = wifeID;
                                famLink.Level = 1;
                                wife.SpouseIn.Add(famLink);
                            }
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("Wife in family points to non individual record");
                        }
                    }

                    foreach (string childID in family.Children)
                    {
                        GedcomIndividualRecord child = Database[childID] as GedcomIndividualRecord;

                        if (child != null)
                        {
                            GedcomFamilyLink famLink = null;

                            // add a family link record if one doesn't already exist
                            if (!child.ChildInFamily(family.XRefID, out famLink))
                            {
                                famLink = new GedcomFamilyLink();
                                famLink.Database = Database;
                                famLink.Family = family.XRefID;
                                famLink.Individual = childID;
                                famLink.Level = 1;
                                famLink.Status = ChildLinkageStatus.Unknown;

                                // pedigree now set below
                                child.ChildIn.Add(famLink);
                            }

                            // set pedigree here to allow for ADOP/FOST in the FAM tag
                            // FAM record overrides link status if they differ
                            famLink.Pedigree = family.GetLinkageType(childID);
                            famLink.FatherPedigree = family.GetHusbandLinkageType(childID);
                            famLink.MotherPedigree = family.GetWifeLinkageType(childID);

                            // check BIRT event for a FAMC record, check ADOP for FAMC / ADOP records
                            foreach (GedcomIndividualEvent indiEv in child.Events)
                            {
                                if (indiEv.Famc == family.XRefID)
                                {
                                    switch (indiEv.EventType)
                                    {
                                        case GedcomEventType.Birth:
                                            // BIRT records do not state father/mother birth,
                                            // all we can say is both are natural
                                            famLink.Pedigree = PedigreeLinkageType.Birth;
                                            break;
                                        case GedcomEventType.ADOP:
                                            switch (indiEv.AdoptedBy)
                                            {
                                                case GedcomAdoptionType.Husband:
                                                    famLink.FatherPedigree = PedigreeLinkageType.Adopted;
                                                    break;
                                                case GedcomAdoptionType.Wife:
                                                    famLink.MotherPedigree = PedigreeLinkageType.Adopted;
                                                    break;
                                                case GedcomAdoptionType.HusbandAndWife:
                                                default:
                                                    // default is both as well, has to be adopted by someone if
                                                    // there is an event on the family.
                                                    famLink.Pedigree = PedigreeLinkageType.Adopted;
                                                    break;
                                            }

                                            break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("Child in family points to non individual record");
                        }
                    }

                    family.ClearLinkageTypes();
                }

                // look for any broken references / update ref counts
                foreach (string xref in missingReferences)
                {
                    GedcomRecord record = Database[xref];
                    if (record != null)
                    {
                        switch (record.RecordType)
                        {
                            case GedcomRecordType.Individual:
                                // TODO: don't increase ref count on individuals,
                                // a bit of a hack, only place where it may be
                                // needed is on assocciations
                                break;
                            case GedcomRecordType.Family:
                                // TODO: don't increase ref count on families
                                break;
                            default:
                                record.RefCount++;
                                break;
                        }
                    }
                    else if (!removedNotes.Contains(xref))
                    {
                        System.Diagnostics.Debug.WriteLine("Missing reference: " + xref);
                    }
                }

                missingReferences = null;

                // link sources with citations which reference them
                foreach (GedcomSourceCitation citation in sourceCitations)
                {
                    GedcomSourceRecord source = Database[citation.Source] as GedcomSourceRecord;
                    if (source != null)
                    {
                        source.Citations.Add(citation);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Missing source reference: " + citation.Source);
                    }
                }

                sourceCitations = null;

                // link repos with citations which reference them
                foreach (GedcomRepositoryCitation citation in repoCitations)
                {
                    GedcomRepositoryRecord repo = Database[citation.Repository] as GedcomRepositoryRecord;
                    if (repo != null)
                    {
                        repo.Citations.Add(citation);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Missing repo reference: " + citation.Repository);
                    }
                }

                repoCitations = null;

                // find any sources without a title and give them one, happens with Database1.ged,
                // could be bad parsing, not sure, try and make up for it anyway
                int missingSourceTitleCount = 1;
                foreach (GedcomSourceRecord source in Database.Sources)
                {
                    if (string.IsNullOrEmpty(source.Title))
                    {
                        source.Title = string.Format("Source {0}", missingSourceTitleCount++);
                    }
                }

                Database.Name = gedcomFile;
            }

            if (PercentageDone != null)
            {
                PercentageDone(this, EventArgs.Empty);
            }

            Database.Loading = false;

            return success;
        }

        private static string DetectNewline(string gedcomFile, Encoding enc)
        {
            using (var sr = new StreamReader(gedcomFile, enc))
            {
                return DetectNewline(sr);
            }
        }

        internal static string DetectNewline(StreamReader sr)
        {
            int i = 0;
            while (!sr.EndOfStream && i < 512)
            {
                var nextChar = sr.Read();

                if (nextChar == '\r')
                {
                    nextChar = sr.Read();

                    if (nextChar == '\n')
                    {
                        // This is a Windows CRLF formatted line.
                        return "\r\n";
                    }

                    // Odd format, just a CR on it's own.
                    return "\r";
                }
                else if (nextChar == '\n')
                {
                    // Looks like Linux / Unix.
                    sr.Read(); // Throw away the LF character.
                    return "\n";
                }

                i++;
            }

            return Environment.NewLine;
        }

        private void Parser_ParseError(object sender, EventArgs e)
        {
            string error = GedcomParser.GedcomErrorString(Parser.ErrorState);
            Debug.WriteLine(error);
            System.Console.WriteLine(error);
        }

        private void Parser_TagFound(object sender, EventArgs e)
        {
            level = Parser.Level;
            xrefId = Parser.XrefID;
            tag = TagMap(Parser.Tag);
            lineValue = Parser.LineValue;
            lineValueType = Parser.LineValueType;

            GedcomRecord current = null;

            // pop previous levels from the stack
            current = PopStack(level);

            if (current == null)
            {
                switch (tag)
                {
                    case "FAM":

                        // must have an xref id to have a family record
                        // otherwise it can't be referenced anywhere
                        if (!string.IsNullOrEmpty(xrefId))
                        {
                            current = new GedcomFamilyRecord();
                        }

                        break;
                    case "INDI":

                        // must have an xref id to have an individual record
                        // otherwise it can't be referenced anywhere
                        if (!string.IsNullOrEmpty(xrefId))
                        {
                            current = new GedcomIndividualRecord();
                        }

                        break;
                    case "OBJE":

                        // must have an xref id to have a multimedia record
                        // otherwise it can't be referenced anywhere
                        if (!string.IsNullOrEmpty(xrefId))
                        {
                            current = new GedcomMultimediaRecord();
                        }

                        break;
                    case "NOTE":

                        // must have an xref id to have a note record
                        // otherwise it can't be referenced anywhere
                        if (!string.IsNullOrEmpty(xrefId))
                        {
                            GedcomNoteRecord note = new GedcomNoteRecord();
                            current = note;

                            // set initial note text if needed
                            if (lineValueType == GedcomLineValueType.DataType)
                            {
                                note.ParsedText.Append(lineValue);
                            }
                            else if (lineValue != string.Empty)
                            {
                                // pointer to a note, this should not occur
                                // as we should be at level 0 here
                                Debug.WriteLine("Spurious Note pointer: " + xrefId + "\t at level: " + level);
                            }
                        }

                        break;
                    case "REPO":

                        // must have an xref id to have a repository record
                        // otherwise it can't be referenced anywhere
                        if (!string.IsNullOrEmpty(xrefId))
                        {
                            current = new GedcomRepositoryRecord();
                        }

                        break;
                    case "SOUR":

                        // must have an xref id to have a source record
                        // otherwise it can't be referenced anywhere
                        if (!string.IsNullOrEmpty(xrefId))
                        {
                            current = new GedcomSourceRecord();
                        }

                        break;
                    case "SUBM":

                        // must have an xref id to have a submitter record
                        // otherwise it can't be referenced anywhere
                        if (!string.IsNullOrEmpty(xrefId))
                        {
                            current = new GedcomSubmitterRecord();
                        }

                        break;
                    case "HEAD":

                        // header record
                        current = new GedcomHeader();

                        break;

                    case "SUBN":

                        // Submission record
                        if (!string.IsNullOrEmpty(xrefId))
                        {
                            current = new GedcomSubmissionRecord();
                        }

                        break;

                    case "TRLR":

                        break;
                    default:

                        // Unknown tag
                        Debug.WriteLine("Unknown: " + tag + " at level: " + level);
                        break;
                }

                // if we created a new record push it onto the stack
                if (current != null)
                {
                    if (!string.IsNullOrEmpty(xrefId))
                    {
                        current.XRefID = xrefId;
                    }

                    current.Database = parseState.Database;
                    current.Level = level;
                    parseState.Records.Push(current);
                }
            }
            else
            {
                switch (current.RecordType)
                {
                    case GedcomRecordType.Header:
                        ReadHeaderRecord();
                        break;
                    case GedcomRecordType.Family:
                        ReadFamilyRecord();
                        break;
                    case GedcomRecordType.Individual:
                        ReadIndividualRecord();
                        break;
                    case GedcomRecordType.Multimedia:
                        ReadMultimediaRecord();
                        break;
                    case GedcomRecordType.Note:
                        ReadNoteRecord();
                        break;
                    case GedcomRecordType.Repository:
                        ReadRepositoryRecord();
                        break;
                    case GedcomRecordType.Source:
                        ReadSourceRecord();
                        break;
                    case GedcomRecordType.Submitter:
                        ReadSubmitterRecord();
                        break;
                    case GedcomRecordType.Submission:
                        ReadSubmissionRecord();
                        break;

                    // Non top level records
                    case GedcomRecordType.Event:
                        ReadEventRecord();
                        break;
                    case GedcomRecordType.FamilyEvent:
                        ReadEventRecord();
                        break;
                    case GedcomRecordType.IndividualEvent:
                        ReadEventRecord();
                        break;

                    case GedcomRecordType.Place:
                        ReadPlaceRecord();
                        break;
                    case GedcomRecordType.SourceCitation:
                        ReadSourceCitationRecord();
                        break;
                    case GedcomRecordType.SpouseSealing:
                        ReadSpouseSealingRecord();
                        break;
                    case GedcomRecordType.FamilyLink:
                        ReadFamilyLinkRecord();
                        break;
                    case GedcomRecordType.Association:
                        ReadAssociationRecord();
                        break;
                    case GedcomRecordType.Name:
                        ReadNameRecord();
                        break;
                    case GedcomRecordType.Date:
                        ReadDateRecord();
                        break;
                    case GedcomRecordType.RepositoryCitation:
                        ReadRepositoryCitation();
                        break;
                    case GedcomRecordType.CustomRecord:
                        ReadEventRecord();
                        break;
                }
            }

            parseState.AddPreviousTag(tag, level);
        }

        private GedcomRecord PopStack(int level)
        {
            GedcomRecord current = null;

            if (parseState.Records.Count != 0)
            {
                current = parseState.Records.Peek();
            }

            while ((parseState.PreviousTags.Count > 0) &&
                   (parseState.PreviousTags.Peek().Level >= level))
            {
                parseState.PreviousTags.Pop();
            }

            while (current != null && level <= current.ParsingLevel)
            {
                switch (current.RecordType)
                {
                    // hack for ADDR appearing on INDI, done by Family Tree Maker
                    // convert it to a RESI
                    case GedcomRecordType.Individual:
                        GedcomIndividualRecord indi = (GedcomIndividualRecord)current;
                        GedcomAddress address = indi.Address;

                        if (address != null)
                        {
                            GedcomIndividualEvent resi = new GedcomIndividualEvent();
                            resi.EventType = GedcomEventType.RESIFact;
                            resi.Database = Database;
                            resi.Level = indi.Level + 1;
                            resi.IndiRecord = indi;
                            resi.Address = address;

                            indi.Events.Add(resi);

                            indi.Address = null;
                        }

                        break;

                    // hacks to avoid allocating lots of strings, we use a string builder
                    // as we have hit the end of the record set the value from the string builder
                    case GedcomRecordType.Note:
                        GedcomNoteRecord note = (GedcomNoteRecord)current;
                        note.Text = note.ParsedText.ToString();
                        note.ParsedText = null;

                        if (string.IsNullOrWhiteSpace(note.Text))
                        {
                            removedNotes.Add(note.XRefID);
                            current = null;
                        }

                        break;
                    case GedcomRecordType.SourceCitation:
                        GedcomSourceCitation citation = (GedcomSourceCitation)current;
                        if (citation.ParsedText != null)
                        {
                            citation.Text = citation.ParsedText.ToString();
                            citation.ParsedText = null;
                        }

                        break;
                    case GedcomRecordType.Source:
                        GedcomSourceRecord source = (GedcomSourceRecord)current;
                        if (source.TitleText != null)
                        {
                            source.Title = source.TitleText.ToString();
                            source.TitleText = null;
                        }
                        else if (source.OriginatorText != null)
                        {
                            source.Originator = source.OriginatorText.ToString();
                            source.OriginatorText = null;
                        }
                        else if (source.PublicationText != null)
                        {
                            source.PublicationFacts = source.PublicationText.ToString();
                            source.PublicationText = null;
                        }
                        else if (source.TextText != null)
                        {
                            source.Text = source.TextText.ToString();
                            source.TextText = null;
                        }

                        break;
                }

                if (current != null)
                {
                    // ensure database is set on all records
                    // or at least those that have been pushed on the stack
                    if (current.Database != Database)
                    {
                        current.Database = Database;
                    }

                    // pop as we are at a higher level now
                    if (current.Level == 0 && current.RecordType != GedcomRecordType.Header)
                    {
                        parseState.Database.Add(current.XRefID, current);
                    }

                    current = null;
                }

                parseState.Records.Pop();

                if (parseState.Records.Count > 0)
                {
                    current = parseState.Records.Peek();
                }
            }

            return current;
        }

        private void ResetParse()
        {
            // set specialist IndexedKeyCollection that supports replacing xrefs
            xrefCollection = new XRefIndexedKeyCollection();

            // always replace xrefs
            xrefCollection.ReplaceXRefs = ReplaceXRefs;
            Parser.XrefCollection = xrefCollection;

            Parser.ResetParseState();
            parseState = new GedcomParseState();
            xrefCollection.Database = parseState.Database;
            missingReferences = new List<string>();
            sourceCitations = new List<GedcomSourceCitation>();
            repoCitations = new List<GedcomRepositoryCitation>();

            removedNotes = new List<string>();
            lineNumber = 0;

            Database.Loading = true;
        }

        private void Flush()
        {
            // process / clean up anything left in the parser
            PopStack(0);
        }

        private bool AddressParse(GedcomAddress address, string tag, string lineValue, GedcomLineValueType lineValueType)
        {
            bool done = false;

            // TODO: checking for ADDR is wrong, doesn't work properly, ok to just
            // check address is not null?  Real solution is to use a stack for PreviousTag
            // like it should have been doing in the first place
            // PreviousTag is now using a stack so will return the parent tag, which should be ADDR
            if (address != null && parseState.PreviousTag == "ADDR")
            {
                switch (tag)
                {
                    case "CONT":
                        address.AddressLine += newlineDelimiter;
                        address.AddressLine += lineValue;
                        done = true;
                        break;
                    case "ADR1":
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            address.AddressLine1 = lineValue;
                        }

                        done = true;
                        break;
                    case "ADR2":
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            address.AddressLine2 = lineValue;
                        }

                        done = true;
                        break;
                    case "ADR3":
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            address.AddressLine3 = lineValue;
                        }

                        done = true;
                        break;
                    case "CITY":
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            address.City = lineValue;
                        }

                        done = true;
                        break;
                    case "STAE":
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            address.State = lineValue;
                        }

                        done = true;
                        break;
                    case "POST":
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            address.PostCode = lineValue;
                        }

                        done = true;
                        break;
                    case "CTRY":
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            address.Country = lineValue;
                        }

                        done = true;
                        break;
                }
            }

            return done;
        }

        private void DateParse(GedcomDate date, string lineValue)
        {
            date.ParseDateString(lineValue);

            // no parsed date, perhaps it was an age?
            if (date.DateTime1 == null)
            {
                // date handling is severly broken in genealogy applications,
                // with many not taking any notice of the mandated formats when
                // outputting gedcom, and some such as Family Tree Maker
                // inserting what belongs in AGE as the date, e.g. INFANT

                // this is the date record
                GedcomRecord record = parseState.Records.Pop();

                // this is the one we are interested in
                record = parseState.Records.Peek();

                // put the date record back
                parseState.Records.Push(date);

                GedcomIndividualEvent ev = record as GedcomIndividualEvent;
                if (ev != null)
                {
                    GedcomAge age = GedcomAge.Parse(lineValue, Database);
                    if (age != null)
                    {
                        // we have a valid age, could calc a date at some point
                        // based off birth of individual, don't do that here though

                        // don't clear lineValue, we need something to keep
                        // the event active!
                        ev.Age = age;
                    }
                }
            }
        }

        private void ReadHeaderRecord()
        {
            GedcomHeader headerRecord;

            headerRecord = parseState.Records.Peek() as GedcomHeader;

            if (tag.StartsWith("_"))
            {
                switch (tag)
                {
                    default:
                        GedcomCustomRecord custom = new GedcomCustomRecord();
                        custom.Level = level;
                        custom.XRefID = xrefId;
                        custom.Tag = tag;

                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            custom.Classification = lineValue;
                        }

                        // TODO: may want to use customs at some point
                        parseState.Records.Push(custom);
                        break;
                }
            }

            if (level == headerRecord.ParsingLevel + 1)
            {
                switch (tag)
                {
                    case "CHAR":
                        // special case to get the character set we should be using
                        // only do if charset is unknown or we will get in a nice loop
                        if (Parser.Charset == GedcomCharset.Unknown)
                        {
                            Encoding enc = null;
                            GedcomCharset charset = GedcomCharset.Unsupported;
                            switch (lineValue)
                            {
                                case "ANSEL":
                                    charset = GedcomCharset.Ansel;
                                    enc = new AnselEncoder();
                                    break;
                                case "ANSI":
                                    charset = GedcomCharset.Ansi;
                                    enc = Encoding.GetEncoding(1252);
                                    break;
                                case "IBMPC": // Not a valid character set as the code page is ambiguous, but we try to import it anyway.
                                    enc = Encoding.GetEncoding(437);
                                    break;
                                case "UTF8":
                                    // this is correct, we will already have converted from utf8
                                    charset = GedcomCharset.UTF16LE;
                                    break;
                                case "ASCII":
                                    // yes, ASCII is the same as UTF8 but extended ascii spoils that
                                    // which is probably in use
                                    charset = GedcomCharset.Ascii;
                                    enc = Encoding.ASCII;
                                    break;
                                default:
                                    break;
                            }

                            if (enc != null)
                            {
                                stream.Close();
                                stream.Dispose();
                                stream = new StreamReader(GedcomFile, enc);

                                ResetParse();
                            }

                            Parser.Charset = charset;
                        }

                        break;
                    case "SOUR":
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            headerRecord.ApplicationSystemId = lineValue;
                        }

                        break;
                    case "DEST":
                        break;
                    case "SUBM":
                        string submXref = AddSubmitterRecord(headerRecord);
                        headerRecord.SubmitterXRefID = submXref;
                        break;
                    case "SUBN":
                        if (lineValueType == GedcomLineValueType.PointerType)
                        {
                        }
                        else
                        {
                        }

                        break;
                    case "COPR":
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            headerRecord.Copyright = lineValue;
                        }

                        break;
                    case "FILE":
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            headerRecord.Filename = lineValue;
                        }

                        break;
                    case "LANG":
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            headerRecord.Language = lineValue;
                        }

                        break;
                    case "PLAC":
                        break;
                    case "DATE":
                        GedcomDate date = new GedcomDate(Database);
                        date.Level = level;
                        parseState.Records.Push(date);
                        headerRecord.TransmissionDate = date;
                        level++;
                        ReadDateRecord();
                        level--;
                        parseState.Records.Pop();
                        break;
                    case "NOTE":
                        AddNoteRecord(headerRecord);
                        break;
                }
            }
            else if (level == headerRecord.ParsingLevel + 2)
            {
                switch (tag)
                {
                    case "NAME":
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            headerRecord.ApplicationName = lineValue;
                        }

                        break;
                    case "VERS":
                        switch (parseState.ParentTag(level))
                        {
                            case "SOUR":
                                if (lineValueType == GedcomLineValueType.DataType)
                                {
                                    headerRecord.ApplicationVersion = lineValue;
                                }

                                break;
                            case "CHAR":
                                break;
                            case "GEDC":
                                break;
                        }

                        break;
                    case "CORP":
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            headerRecord.Corporation = lineValue;
                        }

                        break;

                    case "DATA":
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            headerRecord.SourceName = lineValue;
                        }

                        break;
                }
            }
            else if (level == headerRecord.ParsingLevel + 3)
            {
                switch (tag)
                {
                    case "TIME":
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            if (headerRecord.TransmissionDate != null)
                            {
                                headerRecord.TransmissionDate.Time = lineValue;
                            }
                        }

                        break;
                    case "DATE":
                        GedcomDate date = new GedcomDate(Database);
                        date.Level = level;
                        parseState.Records.Push(date);
                        headerRecord.SourceDate = date;
                        level++;
                        ReadDateRecord();
                        level--;
                        parseState.Records.Pop();
                        break;
                    case "COPR":
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            headerRecord.SourceCopyright = lineValue;
                        }

                        break;
                    case "ADDR":
                        if (headerRecord.CorporationAddress == null)
                        {
                            headerRecord.CorporationAddress = new GedcomAddress();
                            headerRecord.CorporationAddress.Database = Database;
                        }

                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            headerRecord.CorporationAddress.AddressLine = lineValue;
                        }

                        break;
                    case "PHON":
                        if (headerRecord.CorporationAddress == null)
                        {
                            headerRecord.CorporationAddress = new GedcomAddress();
                            headerRecord.CorporationAddress.Database = Database;
                        }

                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            if (string.IsNullOrEmpty(headerRecord.CorporationAddress.Phone1))
                            {
                                headerRecord.CorporationAddress.Phone1 = lineValue;
                            }
                            else if (string.IsNullOrEmpty(headerRecord.CorporationAddress.Phone2))
                            {
                                headerRecord.CorporationAddress.Phone2 = lineValue;
                            }
                            else if (string.IsNullOrEmpty(headerRecord.CorporationAddress.Phone3))
                            {
                                headerRecord.CorporationAddress.Phone3 = lineValue;
                            }
                            else
                            {
                                // should never occur only 3 phone numbers are allowed
                            }
                        }

                        break;
                    case "EMAIL":
                        if (headerRecord.CorporationAddress == null)
                        {
                            headerRecord.CorporationAddress = new GedcomAddress();
                            headerRecord.CorporationAddress.Database = Database;
                        }

                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            if (string.IsNullOrEmpty(headerRecord.CorporationAddress.Email1))
                            {
                                headerRecord.CorporationAddress.Email1 = lineValue;
                            }
                            else if (string.IsNullOrEmpty(headerRecord.CorporationAddress.Email2))
                            {
                                headerRecord.CorporationAddress.Email2 = lineValue;
                            }
                            else if (string.IsNullOrEmpty(headerRecord.CorporationAddress.Email3))
                            {
                                headerRecord.CorporationAddress.Email3 = lineValue;
                            }
                            else
                            {
                                // should never occur only 3 emails are allowed
                            }
                        }

                        break;
                    case "FAX":
                        if (headerRecord.CorporationAddress == null)
                        {
                            headerRecord.CorporationAddress = new GedcomAddress();
                            headerRecord.CorporationAddress.Database = Database;
                        }

                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            if (string.IsNullOrEmpty(headerRecord.CorporationAddress.Fax1))
                            {
                                headerRecord.CorporationAddress.Fax1 = lineValue;
                            }
                            else if (string.IsNullOrEmpty(headerRecord.CorporationAddress.Fax2))
                            {
                                headerRecord.CorporationAddress.Fax2 = lineValue;
                            }
                            else if (string.IsNullOrEmpty(headerRecord.CorporationAddress.Fax3))
                            {
                                headerRecord.CorporationAddress.Fax3 = lineValue;
                            }
                            else
                            {
                                // should never occur only 3 fax numbers are allowed
                            }
                        }

                        break;
                    case "WWW":
                        if (headerRecord.CorporationAddress == null)
                        {
                            headerRecord.CorporationAddress = new GedcomAddress();
                            headerRecord.CorporationAddress.Database = Database;
                        }

                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            if (string.IsNullOrEmpty(headerRecord.CorporationAddress.Www1))
                            {
                                headerRecord.CorporationAddress.Www1 = lineValue;
                            }
                            else if (string.IsNullOrEmpty(headerRecord.CorporationAddress.Www2))
                            {
                                headerRecord.CorporationAddress.Www2 = lineValue;
                            }
                            else if (string.IsNullOrEmpty(headerRecord.CorporationAddress.Www3))
                            {
                                headerRecord.CorporationAddress.Www3 = lineValue;
                            }
                            else
                            {
                                // should never occur only 3 urls are allowed
                            }
                        }

                        break;
                }
            }
            else if (level == headerRecord.ParsingLevel + 4)
            {
                AddressParse(headerRecord.CorporationAddress, tag, lineValue, lineValueType);
            }
        }

        private void ReadFamilyRecord()
        {
            GedcomFamilyRecord familyRecord;

            // allowed sub records
            GedcomFamilyEvent familyEvent;

            familyRecord = parseState.Records.Peek() as GedcomFamilyRecord;

            if (tag.StartsWith("_"))
            {
                switch (tag)
                {
                    case "_MSTAT":
                        try
                        {
                            familyRecord.StartStatus = EnumHelper.Parse<MarriageStartStatus>(lineValue, true);
                        }
                        catch
                        {
                            System.Diagnostics.Debug.WriteLine("Unknown marriage start state: " + lineValue);
                        }

                        break;
                    case "_FREL":
                    case "_MREL":
                        if ((!string.IsNullOrEmpty(parseState.PreviousTag)) &&
                            parseState.PreviousTag == "CHIL" &&
                            level == parseState.PreviousLevel + 1)
                        {
                            string childID = familyRecord.Children[familyRecord.Children.Count - 1];
                            PedigreeLinkageType currentType = familyRecord.GetLinkageType(childID);

                            GedcomAdoptionType linkTo = GedcomAdoptionType.Husband;
                            if (tag == "_MREL")
                            {
                                linkTo = GedcomAdoptionType.Wife;
                            }

                            switch (lineValue)
                            {
                                case "Natural":
                                    familyRecord.SetLinkageType(childID, PedigreeLinkageType.Birth, linkTo);
                                    break;
                                case "Adopted":
                                    familyRecord.SetLinkageType(childID, PedigreeLinkageType.Adopted, linkTo);
                                    break;
                                default:
                                    System.Diagnostics.Debug.WriteLine("Unsupported value for " + tag + ": " + lineValue);
                                    break;
                            }

                            break;
                        }

                        break;
                    default:
                        GedcomCustomRecord custom = new GedcomCustomRecord();
                        custom.Level = level;
                        custom.XRefID = xrefId;
                        custom.Tag = tag;

                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            custom.Classification = lineValue;
                        }

                        // TODO: may want to use customs at some point
                        // familyRecord.Events.Add(custom);
                        parseState.Records.Push(custom);
                        break;
                }
            }
            else if (level == familyRecord.ParsingLevel + 1)
            {
                switch (tag)
                {
                    case "RESN":

                        // restriction notice
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            try
                            {
                                familyRecord.RestrictionNotice = EnumHelper.Parse<GedcomRestrictionNotice>(lineValue, true);
                            }
                            catch
                            {
                                Debug.WriteLine("Invalid restriction type: " + lineValue);

                                // default to confidential to protect privacy
                                familyRecord.RestrictionNotice = GedcomRestrictionNotice.Confidential;
                            }
                        }

                        break;
                    case "ANUL":

                        // event
                        familyEvent = familyRecord.AddNewEvent(GedcomEventType.ANUL);
                        parseState.Records.Push(familyEvent);

                        break;
                    case "CENS":

                        // event
                        familyEvent = familyRecord.AddNewEvent(GedcomEventType.CENS_FAM);
                        parseState.Records.Push(familyEvent);

                        break;
                    case "DIV":

                        // event
                        familyEvent = familyRecord.AddNewEvent(GedcomEventType.DIV);
                        parseState.Records.Push(familyEvent);

                        break;
                    case "DIVF":

                        // event
                        familyEvent = familyRecord.AddNewEvent(GedcomEventType.DIVF);
                        parseState.Records.Push(familyEvent);

                        break;
                    case "ENGA":

                        // event
                        familyEvent = familyRecord.AddNewEvent(GedcomEventType.ENGA);
                        parseState.Records.Push(familyEvent);

                        break;
                    case "MARB":

                        // event
                        familyEvent = familyRecord.AddNewEvent(GedcomEventType.MARB);
                        parseState.Records.Push(familyEvent);

                        break;
                    case "MARC":

                        // event
                        familyEvent = familyRecord.AddNewEvent(GedcomEventType.MARC);
                        parseState.Records.Push(familyEvent);

                        break;
                    case "MARR":

                        // event
                        familyEvent = familyRecord.AddNewEvent(GedcomEventType.MARR);
                        parseState.Records.Push(familyEvent);

                        break;
                    case "MARL":

                        // event
                        familyEvent = familyRecord.AddNewEvent(GedcomEventType.MARL);
                        parseState.Records.Push(familyEvent);

                        break;
                    case "MARS":

                        // event
                        familyEvent = familyRecord.AddNewEvent(GedcomEventType.MARS);
                        parseState.Records.Push(familyEvent);

                        break;
                    case "RESI":

                        // event
                        familyEvent = familyRecord.AddNewEvent(GedcomEventType.RESI);
                        parseState.Records.Push(familyEvent);

                        break;
                    case "EVEN":

                        // event
                        familyEvent = familyRecord.AddNewEvent(GedcomEventType.GenericEvent);

                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            familyEvent.EventName = lineValue;
                        }

                        parseState.Records.Push(familyEvent);

                        break;

                    case "HUSB":
                        if (lineValueType == GedcomLineValueType.PointerType)
                        {
                            familyRecord.Husband = lineValue;
                            missingReferences.Add(lineValue);
                        }

                        break;
                    case "WIFE":
                        if (lineValueType == GedcomLineValueType.PointerType)
                        {
                            familyRecord.Wife = lineValue;
                            missingReferences.Add(lineValue);
                        }

                        break;
                    case "CHIL":
                        if (lineValueType == GedcomLineValueType.PointerType)
                        {
                            familyRecord.Children.Add(lineValue);
                            missingReferences.Add(lineValue);
                        }

                        break;
                    case "NCHI":
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            try
                            {
                                familyRecord.NumberOfChildren = Convert.ToInt32(lineValue);
                            }
                            catch
                            {
                                Debug.WriteLine("Invalid number for Number of children tag");
                            }
                        }

                        break;
                    case "SUBM":
                        if (lineValueType == GedcomLineValueType.PointerType)
                        {
                            familyRecord.SubmitterRecords.Add(lineValue);
                            missingReferences.Add(lineValue);
                        }
                        else
                        {
                            GedcomSubmitterRecord submitter = new GedcomSubmitterRecord();
                            submitter.Level = 0; // new top level submitter, always 0;
                            submitter.ParsingLevel = level;
                            submitter.XRefID = Database.GenerateXref("SUBM");

                            parseState.Records.Push(submitter);

                            familyRecord.SubmitterRecords.Add(submitter.XRefID);
                        }

                        break;
                    case "SLGS": // LDS spouse sealing.
                        familyRecord.SpouseSealing = new GedcomSpouseSealingRecord();
                        familyRecord.SpouseSealing.Description = lineValue;
                        familyRecord.SpouseSealing.Level = level;
                        parseState.Records.Push(familyRecord.SpouseSealing);

                        break;
                    case "REFN":
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            familyRecord.UserReferenceNumber = lineValue;
                        }

                        break;
                    case "RIN":
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            familyRecord.AutomatedRecordId = lineValue;
                        }

                        break;
                    case "CHAN":
                        GedcomChangeDate date = new GedcomChangeDate(Database);
                        date.Level = level;
                        parseState.Records.Push(date);
                        break;
                    case "NOTE":
                        AddNoteRecord(familyRecord);
                        break;
                    case "SOUR":
                        AddSourceCitation(familyRecord);
                        break;
                    case "OBJE":
                        AddMultimediaRecord(familyRecord);
                        break;
                }
            }
            else if ((!string.IsNullOrEmpty(parseState.PreviousTag)) &&
                        parseState.PreviousTag == "REFN" &&
                        level == parseState.PreviousLevel + 1)
            {
                if (tag == "TYPE")
                {
                    if (lineValueType == GedcomLineValueType.DataType)
                    {
                        familyRecord.UserReferenceType = lineValue;
                    }
                }
            }

            // not valid GEDCOM, but Family Tree Maker adds ADOP/FOST tags
            // to CHIL in a FAM, this is apparently valid in GEDCOM < 5.5
            else if ((!string.IsNullOrEmpty(parseState.PreviousTag)) &&
                     parseState.PreviousTag == "CHIL" &&
                     level == parseState.PreviousLevel + 1)
            {
                string childID = familyRecord.Children[familyRecord.Children.Count - 1];
                switch (tag)
                {
                    case "ADOP":
                        switch (lineValue)
                        {
                            case "HUSB":
                                familyRecord.SetLinkageType(childID, PedigreeLinkageType.Adopted, GedcomAdoptionType.Husband);
                                break;
                            case "WIFE":
                                familyRecord.SetLinkageType(childID, PedigreeLinkageType.Adopted, GedcomAdoptionType.Wife);
                                break;
                            case "BOTH":
                            default:
                                familyRecord.SetLinkageType(childID, PedigreeLinkageType.Adopted);
                                break;
                        }

                        break;
                    case "FOST":
                        switch (lineValue)
                        {
                            case "HUSB":
                                familyRecord.SetLinkageType(childID, PedigreeLinkageType.Foster, GedcomAdoptionType.Husband);
                                break;
                            case "WIFE":
                                familyRecord.SetLinkageType(childID, PedigreeLinkageType.Foster, GedcomAdoptionType.Wife);
                                break;
                            case "BOTH":
                            default:
                                familyRecord.SetLinkageType(childID, PedigreeLinkageType.Foster);
                                break;
                        }

                        break;
                }
            }
            else
            {
                // shouldn't be here
                Debug.WriteLine("Unknown state / tag parsing family node: " + tag + "\t at level: " + level);
            }
        }

        private void ReadIndividualRecord()
        {
            GedcomIndividualRecord individualRecord;

            individualRecord = parseState.Records.Peek() as GedcomIndividualRecord;

            GedcomIndividualEvent individualEvent;

            // some custom tags we convert to generic facts/events
            // this means we have to set the line value to the type
            // they represent, so store the real line value and use
            // for the event classification.
            string customToGenericClassification = string.Empty;

            if (tag.StartsWith("_"))
            {
                switch (tag)
                {
                    // we convert _MILT to EVEN Military Service
                    case "_MILT":
                        tag = "EVEN";
                        lineValue = "Military Service";
                        lineValueType = GedcomLineValueType.DataType;
                        break;

                    // we convert _MDCL to FACT Medical
                    case "_MDCL":
                        tag = "FACT";
                        customToGenericClassification = lineValue;
                        lineValue = "Medical";
                        lineValueType = GedcomLineValueType.DataType;
                        break;

                    // we convert _HEIG to FACT Height
                    case "_HEIG":
                        tag = "FACT";
                        customToGenericClassification = lineValue;
                        lineValue = "Height";
                        lineValueType = GedcomLineValueType.DataType;
                        break;

                    // we convert _WEIG to FACT Weight
                    case "_WEIG":
                        tag = "FACT";
                        customToGenericClassification = lineValue;
                        lineValue = "Weight";
                        lineValueType = GedcomLineValueType.DataType;
                        break;
                    default:
                        GedcomCustomRecord custom = new GedcomCustomRecord();
                        custom.Level = level;
                        custom.XRefID = xrefId;
                        custom.Tag = tag;

                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            custom.Classification = lineValue;
                        }

                        individualRecord.Custom.Add(custom);
                        parseState.Records.Push(custom);
                        break;
                }
            }

            if (level == individualRecord.ParsingLevel + 1)
            {
                switch (tag)
                {
                    case "FAMC":
                        if (lineValueType == GedcomLineValueType.PointerType)
                        {
                            GedcomFamilyLink childIn = new GedcomFamilyLink();
                            childIn.Level = level;
                            childIn.Family = lineValue;
                            childIn.Individual = individualRecord.XRefID;

                            missingReferences.Add(lineValue);

                            individualRecord.ChildIn.Add(childIn);
                            parseState.Records.Push(childIn);
                        }

                        break;
                    case "FAMS":
                        if (lineValueType == GedcomLineValueType.PointerType)
                        {
                            GedcomFamilyLink spouseIn = new GedcomFamilyLink();
                            spouseIn.Level = level;
                            spouseIn.Family = lineValue;
                            spouseIn.Individual = individualRecord.XRefID;
                            spouseIn.PreferedSpouse = individualRecord.SpouseIn.Count == 0;

                            missingReferences.Add(lineValue);

                            individualRecord.SpouseIn.Add(spouseIn);
                            parseState.Records.Push(spouseIn);
                        }

                        break;
                    case "ASSO":
                        if (lineValueType == GedcomLineValueType.PointerType)
                        {
                            GedcomAssociation association = new GedcomAssociation();
                            association.Level = level;
                            association.Individual = lineValue;

                            missingReferences.Add(lineValue);

                            individualRecord.Associations.Add(association);
                            parseState.Records.Push(association);
                        }

                        break;
                    case "RESN":
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            try
                            {
                                individualRecord.RestrictionNotice = EnumHelper.Parse<GedcomRestrictionNotice>(lineValue, true);
                            }
                            catch
                            {
                                Debug.WriteLine("Invalid restriction type: " + lineValue);

                                // default to confidential to protect privacy
                                individualRecord.RestrictionNotice = GedcomRestrictionNotice.Confidential;
                            }
                        }

                        break;
                    case "NAME":
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            GedcomName name = new GedcomName();
                            name.Database = parseState.Database;
                            name.Level = level;
                            name.Name = lineValue;
                            name.PreferredName = individualRecord.Names.Count == 0;

                            individualRecord.Names.Add(name);
                            parseState.Records.Push(name);
                        }

                        break;

                    // Invalid, but seen from Family Origins, Family Tree Maker, Personal Ancestral File, and Legacy
                    case "AKA":
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            GedcomName name = new GedcomName();
                            name.Database = parseState.Database;
                            name.Level = level;
                            name.Name = lineValue;
                            name.Type = "aka";
                            name.PreferredName = individualRecord.Names.Count == 0;
                            individualRecord.Names.Add(name);
                        }

                        break;
                    case "SEX":
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            switch (lineValue)
                            {
                                case "M":
                                    individualRecord.Sex = GedcomSex.Male;
                                    break;
                                case "F":
                                    individualRecord.Sex = GedcomSex.Female;
                                    break;

                                // non standard
                                case "B":
                                    individualRecord.Sex = GedcomSex.Both;
                                    break;

                                // non standard
                                case "N":
                                    individualRecord.Sex = GedcomSex.Neuter;
                                    break;

                                // non standard
                                case "U":
                                    individualRecord.Sex = GedcomSex.Undetermined;
                                    break;
                            }
                        }

                        break;
                    case "SUBM":
                        if (lineValueType == GedcomLineValueType.PointerType)
                        {
                            individualRecord.SubmitterRecords.Add(lineValue);
                            missingReferences.Add(lineValue);
                        }
                        else
                        {
                            GedcomSubmitterRecord submitter = new GedcomSubmitterRecord();
                            submitter.Level = 0; // new top level submitter, always 0
                            submitter.ParsingLevel = level;
                            submitter.XRefID = Database.GenerateXref("SUBM");

                            parseState.Records.Push(submitter);

                            individualRecord.SubmitterRecords.Add(submitter.XRefID);
                        }

                        break;
                    case "ALIA":
                        if (lineValueType == GedcomLineValueType.PointerType)
                        {
                            individualRecord.Alia.Add(lineValue);
                            missingReferences.Add(lineValue);
                        }
                        else if (lineValueType == GedcomLineValueType.DataType)
                        {
                            // Family Tree Maker doing this?
                            // ALIA is unsupported in gedcom 5.5 as a way of
                            // adding multiple names, the spec
                            // does say it should be a pointer to an individual
                            // though, not another name.
                            // spec allows multiple NAME though, so add one
                            // with this name
                            GedcomName name = new GedcomName();
                            name.Database = parseState.Database;
                            name.Level = level;
                            name.Name = lineValue;
                            name.Type = "aka";
                            name.PreferredName = individualRecord.Names.Count == 0;
                            individualRecord.Names.Add(name);
                        }

                        break;
                    case "ANCI":
                        if (lineValueType == GedcomLineValueType.PointerType)
                        {
                            individualRecord.Anci.Add(lineValue);
                            missingReferences.Add(lineValue);
                        }

                        break;
                    case "DESI":
                        if (lineValueType == GedcomLineValueType.PointerType)
                        {
                            individualRecord.Desi.Add(lineValue);
                            missingReferences.Add(lineValue);
                        }

                        break;
                    case "RFN":
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            individualRecord.PermanentRecordFileNumber = lineValue;
                        }

                        break;
                    case "AFN":
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            individualRecord.AncestralFileNumber = lineValue;
                        }

                        break;
                    case "REFN":
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            individualRecord.UserReferenceNumber = lineValue;
                        }

                        break;
                    case "RIN":
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            individualRecord.AutomatedRecordId = lineValue;
                        }

                        break;
                    case "CHAN":
                        GedcomChangeDate date = new GedcomChangeDate(Database);
                        date.Level = level;
                        parseState.Records.Push(date);
                        break;
                    case "NOTE":
                        AddNoteRecord(individualRecord);
                        break;
                    case "SOUR":
                        AddSourceCitation(individualRecord);
                        break;
                    case "OBJE":
                        AddMultimediaRecord(individualRecord);
                        break;
                    case "BIRT":

                        // event
                        individualEvent = new GedcomIndividualEvent();
                        individualEvent.EventType = GedcomEventType.Birth;
                        individualEvent.Level = level;
                        individualEvent.IndiRecord = individualRecord;

                        individualRecord.Events.Add(individualEvent);

                        parseState.Records.Push(individualEvent);

                        break;
                    case "CHR":

                        // event
                        individualEvent = new GedcomIndividualEvent();
                        individualEvent.EventType = GedcomEventType.CHR;
                        individualEvent.Level = level;
                        individualEvent.IndiRecord = individualRecord;

                        individualRecord.Events.Add(individualEvent);

                        parseState.Records.Push(individualEvent);

                        break;
                    case "DEAT":

                        // event
                        individualEvent = new GedcomIndividualEvent();
                        individualEvent.EventType = GedcomEventType.DEAT;
                        individualEvent.Level = level;
                        individualEvent.IndiRecord = individualRecord;

                        individualRecord.Events.Add(individualEvent);

                        parseState.Records.Push(individualEvent);

                        break;
                    case "BURI":

                        // event
                        individualEvent = new GedcomIndividualEvent();
                        individualEvent.EventType = GedcomEventType.BURI;
                        individualEvent.Level = level;
                        individualEvent.IndiRecord = individualRecord;

                        individualRecord.Events.Add(individualEvent);

                        parseState.Records.Push(individualEvent);

                        break;
                    case "CREM":

                        // event
                        individualEvent = new GedcomIndividualEvent();
                        individualEvent.EventType = GedcomEventType.CREM;
                        individualEvent.Level = level;
                        individualEvent.IndiRecord = individualRecord;

                        individualRecord.Events.Add(individualEvent);

                        parseState.Records.Push(individualEvent);

                        break;
                    case "ADOP":

                        // event
                        individualEvent = new GedcomIndividualEvent();
                        individualEvent.EventType = GedcomEventType.ADOP;
                        individualEvent.Level = level;
                        individualEvent.IndiRecord = individualRecord;

                        individualRecord.Events.Add(individualEvent);

                        parseState.Records.Push(individualEvent);

                        break;
                    case "BAPM":

                        // event
                        individualEvent = new GedcomIndividualEvent();
                        individualEvent.EventType = GedcomEventType.BAPM;
                        individualEvent.Level = level;
                        individualEvent.IndiRecord = individualRecord;

                        individualRecord.Events.Add(individualEvent);

                        parseState.Records.Push(individualEvent);

                        break;
                    case "BARM":

                        // event
                        individualEvent = new GedcomIndividualEvent();
                        individualEvent.EventType = GedcomEventType.BARM;
                        individualEvent.Level = level;
                        individualEvent.IndiRecord = individualRecord;

                        individualRecord.Events.Add(individualEvent);

                        parseState.Records.Push(individualEvent);

                        break;
                    case "BASM":

                        // event
                        individualEvent = new GedcomIndividualEvent();
                        individualEvent.EventType = GedcomEventType.BASM;
                        individualEvent.Level = level;
                        individualEvent.IndiRecord = individualRecord;

                        individualRecord.Events.Add(individualEvent);

                        parseState.Records.Push(individualEvent);

                        break;
                    case "BLES":

                        // event
                        individualEvent = new GedcomIndividualEvent();
                        individualEvent.EventType = GedcomEventType.BLES;
                        individualEvent.Level = level;
                        individualEvent.IndiRecord = individualRecord;

                        individualRecord.Events.Add(individualEvent);

                        parseState.Records.Push(individualEvent);

                        break;
                    case "CHRA":

                        // event
                        individualEvent = new GedcomIndividualEvent();
                        individualEvent.EventType = GedcomEventType.CHRA;
                        individualEvent.Level = level;
                        individualEvent.IndiRecord = individualRecord;

                        individualRecord.Events.Add(individualEvent);

                        parseState.Records.Push(individualEvent);

                        break;
                    case "CONF":

                        // event
                        individualEvent = new GedcomIndividualEvent();
                        individualEvent.EventType = GedcomEventType.CONF;
                        individualEvent.Level = level;
                        individualEvent.IndiRecord = individualRecord;

                        individualRecord.Events.Add(individualEvent);

                        parseState.Records.Push(individualEvent);

                        break;
                    case "FCOM":

                        // event
                        individualEvent = new GedcomIndividualEvent();
                        individualEvent.EventType = GedcomEventType.FCOM;
                        individualEvent.Level = level;
                        individualEvent.IndiRecord = individualRecord;

                        individualRecord.Events.Add(individualEvent);

                        parseState.Records.Push(individualEvent);

                        break;
                    case "ORDN":

                        // event
                        individualEvent = new GedcomIndividualEvent();
                        individualEvent.EventType = GedcomEventType.ORDN;
                        individualEvent.Level = level;
                        individualEvent.IndiRecord = individualRecord;

                        individualRecord.Events.Add(individualEvent);

                        parseState.Records.Push(individualEvent);

                        break;
                    case "NATU":

                        // event
                        individualEvent = new GedcomIndividualEvent();
                        individualEvent.EventType = GedcomEventType.NATU;
                        individualEvent.Level = level;
                        individualEvent.IndiRecord = individualRecord;

                        individualRecord.Events.Add(individualEvent);

                        parseState.Records.Push(individualEvent);

                        break;
                    case "EMIG":

                        // event
                        individualEvent = new GedcomIndividualEvent();
                        individualEvent.EventType = GedcomEventType.EMIG;
                        individualEvent.Level = level;
                        individualEvent.IndiRecord = individualRecord;

                        individualRecord.Events.Add(individualEvent);

                        parseState.Records.Push(individualEvent);

                        break;
                    case "IMMI":

                        // event
                        individualEvent = new GedcomIndividualEvent();
                        individualEvent.EventType = GedcomEventType.IMMI;
                        individualEvent.Level = level;
                        individualEvent.IndiRecord = individualRecord;

                        individualRecord.Events.Add(individualEvent);

                        parseState.Records.Push(individualEvent);

                        break;
                    case "CENS":

                        // event
                        individualEvent = new GedcomIndividualEvent();
                        individualEvent.EventType = GedcomEventType.CENS;
                        individualEvent.Level = level;
                        individualEvent.IndiRecord = individualRecord;

                        individualRecord.Events.Add(individualEvent);

                        parseState.Records.Push(individualEvent);

                        break;
                    case "PROB":

                        // event
                        individualEvent = new GedcomIndividualEvent();
                        individualEvent.EventType = GedcomEventType.PROB;
                        individualEvent.Level = level;
                        individualEvent.IndiRecord = individualRecord;

                        individualRecord.Events.Add(individualEvent);

                        parseState.Records.Push(individualEvent);

                        break;
                    case "WILL":

                        // event
                        individualEvent = new GedcomIndividualEvent();
                        individualEvent.EventType = GedcomEventType.WILL;
                        individualEvent.Level = level;
                        individualEvent.IndiRecord = individualRecord;

                        individualRecord.Events.Add(individualEvent);

                        parseState.Records.Push(individualEvent);

                        break;
                    case "GRAD":

                        // event
                        individualEvent = new GedcomIndividualEvent();
                        individualEvent.EventType = GedcomEventType.GRAD;
                        individualEvent.Level = level;
                        individualEvent.IndiRecord = individualRecord;

                        individualRecord.Events.Add(individualEvent);

                        parseState.Records.Push(individualEvent);

                        break;
                    case "RETI":

                        // event
                        individualEvent = new GedcomIndividualEvent();
                        individualEvent.EventType = GedcomEventType.RETI;
                        individualEvent.Level = level;
                        individualEvent.IndiRecord = individualRecord;

                        individualRecord.Events.Add(individualEvent);

                        parseState.Records.Push(individualEvent);

                        break;
                    case "EVEN":

                        // event
                        individualEvent = new GedcomIndividualEvent();
                        individualEvent.EventType = GedcomEventType.GenericEvent;
                        individualEvent.Level = level;
                        individualEvent.IndiRecord = individualRecord;

                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            individualEvent.EventName = lineValue;
                        }

                        individualRecord.Events.Add(individualEvent);

                        parseState.Records.Push(individualEvent);

                        break;
                    case "CAST":

                        // fact
                        individualEvent = new GedcomIndividualEvent();
                        individualEvent.EventType = GedcomEventType.CASTFact;
                        individualEvent.Level = level;
                        individualEvent.IndiRecord = individualRecord;

                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            individualEvent.EventName = lineValue;
                        }

                        individualRecord.Attributes.Add(individualEvent);

                        parseState.Records.Push(individualEvent);

                        break;
                    case "DSCR":

                        // fact
                        individualEvent = new GedcomIndividualEvent();
                        individualEvent.EventType = GedcomEventType.DSCRFact;
                        individualEvent.Level = level;
                        individualEvent.IndiRecord = individualRecord;

                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            individualEvent.EventName = lineValue;
                        }

                        individualRecord.Attributes.Add(individualEvent);

                        parseState.Records.Push(individualEvent);

                        break;
                    case "EDUC":

                        // fact
                        individualEvent = new GedcomIndividualEvent();
                        individualEvent.EventType = GedcomEventType.EDUCFact;
                        individualEvent.Level = level;
                        individualEvent.IndiRecord = individualRecord;

                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            individualEvent.EventName = lineValue;
                        }

                        individualRecord.Attributes.Add(individualEvent);

                        parseState.Records.Push(individualEvent);

                        break;
                    case "IDNO":

                        // fact
                        individualEvent = new GedcomIndividualEvent();
                        individualEvent.EventType = GedcomEventType.IDNOFact;
                        individualEvent.Level = level;
                        individualEvent.IndiRecord = individualRecord;

                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            individualEvent.EventName = lineValue;
                        }

                        individualRecord.Attributes.Add(individualEvent);

                        parseState.Records.Push(individualEvent);

                        break;
                    case "NATI":

                        // fact
                        individualEvent = new GedcomIndividualEvent();
                        individualEvent.EventType = GedcomEventType.NATIFact;
                        individualEvent.Level = level;
                        individualEvent.IndiRecord = individualRecord;

                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            individualEvent.EventName = lineValue;
                        }

                        individualRecord.Attributes.Add(individualEvent);

                        parseState.Records.Push(individualEvent);

                        break;
                    case "NCHI":

                        // fact
                        individualEvent = new GedcomIndividualEvent();
                        individualEvent.EventType = GedcomEventType.NCHIFact;
                        individualEvent.Level = level;
                        individualEvent.IndiRecord = individualRecord;

                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            individualEvent.EventName = lineValue;
                        }

                        individualRecord.Attributes.Add(individualEvent);

                        parseState.Records.Push(individualEvent);

                        break;
                    case "NMR":

                        // fact
                        individualEvent = new GedcomIndividualEvent();
                        individualEvent.EventType = GedcomEventType.NMRFact;
                        individualEvent.Level = level;
                        individualEvent.IndiRecord = individualRecord;

                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            individualEvent.EventName = lineValue;
                        }

                        individualRecord.Attributes.Add(individualEvent);

                        parseState.Records.Push(individualEvent);

                        break;
                    case "OCCU":

                        // fact
                        individualEvent = new GedcomIndividualEvent();
                        individualEvent.EventType = GedcomEventType.OCCUFact;
                        individualEvent.Level = level;
                        individualEvent.IndiRecord = individualRecord;

                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            individualEvent.EventName = lineValue;
                        }

                        individualRecord.Attributes.Add(individualEvent);

                        parseState.Records.Push(individualEvent);

                        break;
                    case "PROP":

                        // fact
                        individualEvent = new GedcomIndividualEvent();
                        individualEvent.EventType = GedcomEventType.PROPFact;
                        individualEvent.Level = level;
                        individualEvent.IndiRecord = individualRecord;

                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            individualEvent.EventName = lineValue;
                        }

                        individualRecord.Attributes.Add(individualEvent);

                        parseState.Records.Push(individualEvent);

                        break;
                    case "RELI":

                        // fact
                        individualEvent = new GedcomIndividualEvent();
                        individualEvent.EventType = GedcomEventType.RELIFact;
                        individualEvent.Level = level;
                        individualEvent.IndiRecord = individualRecord;

                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            individualEvent.EventName = lineValue;
                        }

                        individualRecord.Attributes.Add(individualEvent);

                        parseState.Records.Push(individualEvent);

                        break;
                    case "RESI":

                        // fact
                        individualEvent = new GedcomIndividualEvent();
                        individualEvent.EventType = GedcomEventType.RESIFact;
                        individualEvent.Level = level;
                        individualEvent.IndiRecord = individualRecord;

                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            individualEvent.EventName = lineValue;
                        }

                        individualRecord.Attributes.Add(individualEvent);

                        parseState.Records.Push(individualEvent);

                        break;
                    case "SSN":

                        // fact
                        individualEvent = new GedcomIndividualEvent();
                        individualEvent.EventType = GedcomEventType.SSNFact;
                        individualEvent.Level = level;
                        individualEvent.IndiRecord = individualRecord;

                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            individualEvent.EventName = lineValue;
                        }

                        individualRecord.Attributes.Add(individualEvent);

                        parseState.Records.Push(individualEvent);

                        break;
                    case "TITL":

                        // fact
                        individualEvent = new GedcomIndividualEvent();
                        individualEvent.EventType = GedcomEventType.TITLFact;
                        individualEvent.Level = level;
                        individualEvent.IndiRecord = individualRecord;

                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            individualEvent.EventName = lineValue;
                        }

                        individualRecord.Attributes.Add(individualEvent);

                        parseState.Records.Push(individualEvent);

                        break;
                    case "FACT":

                        // fact
                        individualEvent = new GedcomIndividualEvent();
                        individualEvent.EventType = GedcomEventType.GenericFact;
                        individualEvent.Level = level;
                        individualEvent.IndiRecord = individualRecord;

                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            individualEvent.EventName = lineValue;
                        }

                        if (!string.IsNullOrEmpty(customToGenericClassification))
                        {
                            individualEvent.Classification = customToGenericClassification;
                        }

                        individualRecord.Attributes.Add(individualEvent);

                        parseState.Records.Push(individualEvent);

                        break;

                    // Not according to the spec, but Family Tree Maker sticks
                    // an address under an individual so we will support reading it
                    case "ADDR":
                        if (individualRecord.Address == null)
                        {
                            individualRecord.Address = new GedcomAddress();
                            individualRecord.Address.Database = Database;
                        }

                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            individualRecord.Address.AddressLine = lineValue;
                        }

                        break;
                    case "PHON":
                        if (individualRecord.Address == null)
                        {
                            individualRecord.Address = new GedcomAddress();
                            individualRecord.Address.Database = Database;
                        }

                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            if (string.IsNullOrEmpty(individualRecord.Address.Phone1))
                            {
                                individualRecord.Address.Phone1 = lineValue;
                            }
                            else if (string.IsNullOrEmpty(individualRecord.Address.Phone2))
                            {
                                individualRecord.Address.Phone2 = lineValue;
                            }
                            else if (string.IsNullOrEmpty(individualRecord.Address.Phone3))
                            {
                                individualRecord.Address.Phone3 = lineValue;
                            }
                            else
                            {
                                // should never occur only 3 phone numbers are allowed
                            }
                        }

                        break;
                    case "EMAIL":
                        if (individualRecord.Address == null)
                        {
                            individualRecord.Address = new GedcomAddress();
                            individualRecord.Address.Database = Database;
                        }

                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            if (string.IsNullOrEmpty(individualRecord.Address.Email1))
                            {
                                individualRecord.Address.Email1 = lineValue;
                            }
                            else if (string.IsNullOrEmpty(individualRecord.Address.Email2))
                            {
                                individualRecord.Address.Email2 = lineValue;
                            }
                            else if (string.IsNullOrEmpty(individualRecord.Address.Email3))
                            {
                                individualRecord.Address.Email3 = lineValue;
                            }
                            else
                            {
                                // should never occur only 3 emails are allowed
                            }
                        }

                        break;
                    case "FAX":
                        if (individualRecord.Address == null)
                        {
                            individualRecord.Address = new GedcomAddress();
                            individualRecord.Address.Database = Database;
                        }

                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            if (string.IsNullOrEmpty(individualRecord.Address.Fax1))
                            {
                                individualRecord.Address.Fax1 = lineValue;
                            }
                            else if (string.IsNullOrEmpty(individualRecord.Address.Fax2))
                            {
                                individualRecord.Address.Fax2 = lineValue;
                            }
                            else if (string.IsNullOrEmpty(individualRecord.Address.Fax3))
                            {
                                individualRecord.Address.Fax3 = lineValue;
                            }
                            else
                            {
                                // should never occur only 3 fax numbers are allowed
                            }
                        }

                        break;
                    case "WWW":
                        if (individualRecord.Address == null)
                        {
                            individualRecord.Address = new GedcomAddress();
                            individualRecord.Address.Database = Database;
                        }

                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            if (string.IsNullOrEmpty(individualRecord.Address.Www1))
                            {
                                individualRecord.Address.Www1 = lineValue;
                            }
                            else if (string.IsNullOrEmpty(individualRecord.Address.Www2))
                            {
                                individualRecord.Address.Www2 = lineValue;
                            }
                            else if (string.IsNullOrEmpty(individualRecord.Address.Www3))
                            {
                                individualRecord.Address.Www3 = lineValue;
                            }
                            else
                            {
                                // should never occur only 3 urls are allowed
                            }
                        }

                        break;
                }
            }
            else if ((!string.IsNullOrEmpty(parseState.PreviousTag)) &&
                        level == parseState.PreviousLevel + 1)
            {
                string pTag = parseState.PreviousTag;

                if (pTag == "REFN" && tag == "TYPE")
                {
                    if (lineValueType == GedcomLineValueType.DataType)
                    {
                        individualRecord.UserReferenceType = lineValue;
                    }
                }
                else
                {
                    AddressParse(individualRecord.Address, tag, lineValue, lineValueType);
                }
            }
            else if ((!string.IsNullOrEmpty(parseState.PreviousTag)) &&
                        level == parseState.PreviousLevel)
            {
                AddressParse(individualRecord.Address, tag, lineValue, lineValueType);
            }
            else
            {
                // shouldn't be here
                Debug.WriteLine("Unknown state / tag parsing individual (" + individualRecord.XRefID + ") node: " + tag + "\t at level: " + level);
                System.Console.WriteLine("Unknown state / tag parsing individual (" + individualRecord.XRefID + ") node: " + tag + "\t at level: " + level);
                System.Console.WriteLine("Previous tag: " + parseState.PreviousTag + "\tPrevious Level: " + parseState.PreviousLevel);
            }
        }

        private void ReadMultimediaRecord()
        {
            GedcomMultimediaRecord multimediaRecord;

            multimediaRecord = parseState.Records.Peek() as GedcomMultimediaRecord;

            if (level == multimediaRecord.ParsingLevel + 1)
            {
                switch (tag)
                {
                    case "FORM":
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            GedcomMultimediaFile file;
                            if (multimediaRecord.Files.Count > 0)
                            {
                                file = multimediaRecord.Files[multimediaRecord.Files.Count - 1];
                            }
                            else
                            {
                                file = new GedcomMultimediaFile();
                                file.Database = Database;
                                multimediaRecord.Files.Add(file);
                            }

                            file.Format = lineValue;
                        }

                        break;
                    case "TITL":
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            multimediaRecord.Title = lineValue;
                        }

                        break;

                    case "FILE":
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            GedcomMultimediaFile file = null;
                            if (multimediaRecord.Files.Count > 0)
                            {
                                file = multimediaRecord.Files[multimediaRecord.Files.Count - 1];
                                if (!string.IsNullOrEmpty(file.Filename))
                                {
                                    file = null;
                                }
                            }

                            if (file == null)
                            {
                                file = new GedcomMultimediaFile();
                                file.Database = Database;
                                multimediaRecord.Files.Add(file);
                            }

                            file.Filename = lineValue;
                        }

                        break;
                    case "REFN":
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            multimediaRecord.UserReferenceNumber = lineValue;
                        }

                        break;
                    case "RIN":
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            multimediaRecord.AutomatedRecordId = lineValue;
                        }

                        break;
                    case "CHAN":
                        GedcomChangeDate date = new GedcomChangeDate(Database);
                        date.Level = level;
                        parseState.Records.Push(date);
                        break;
                    case "NOTE":
                        AddNoteRecord(multimediaRecord);
                        break;
                    case "SOUR":
                        AddSourceCitation(multimediaRecord);
                        break;
                }
            }
            else if (parseState.PreviousTag != string.Empty)
            {
                if (level == multimediaRecord.ParsingLevel + 2)
                {
                    if (parseState.PreviousTag == "REFN" && tag == "TYPE")
                    {
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            multimediaRecord.UserReferenceType = lineValue;
                        }
                    }
                    else if (parseState.PreviousTag == "FILE")
                    {
                        switch (tag)
                        {
                            case "FORM":
                                if (lineValueType == GedcomLineValueType.DataType)
                                {
                                    if (multimediaRecord.Files.Count > 0)
                                    {
                                        multimediaRecord.Files[multimediaRecord.Files.Count - 1].Format = lineValue;
                                    }
                                    else
                                    {
                                        GedcomMultimediaFile file = new GedcomMultimediaFile();
                                        file.Database = Database;
                                        file.Format = lineValue;
                                        multimediaRecord.Files.Add(file);
                                    }
                                }

                                break;
                        }
                    }
                    else if (parseState.PreviousTag == "FORM")
                    {
                        if (tag == "MEDI" &&
                            lineValueType == GedcomLineValueType.DataType)
                        {
                            // TODO: GedcomMultiMediaFile should use the enum?
                            multimediaRecord.Files[multimediaRecord.Files.Count - 1].SourceMediaType = lineValue;
                        }
                    }
                }
                else if (level == multimediaRecord.ParsingLevel + 3)
                {
                    if (parseState.PreviousTag == "FILE" && tag == "TYPE")
                    {
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            // TODO: GedcomMultiMediaFile should use the enum?
                            multimediaRecord.Files[multimediaRecord.Files.Count - 1].SourceMediaType = lineValue;
                        }
                    }
                }
            }
            else
            {
                // shouldn't be here
                Debug.WriteLine("Unknown state / tag parsing multimedia node: " + tag + "\t at level: " + level);
            }
        }

        private void ReadNoteRecord()
        {
            GedcomNoteRecord noteRecord;

            noteRecord = parseState.Records.Peek() as GedcomNoteRecord;

            if (level == noteRecord.ParsingLevel + 1)
            {
                switch (tag)
                {
                    case "CONT":
                        noteRecord.ParsedText.Append(newlineDelimiter);
                        noteRecord.ParsedText.Append(lineValue);
                        break;
                    case "CONC":
                        noteRecord.ParsedText.Append(lineValue);
                        break;
                    case "REFN":
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            noteRecord.UserReferenceNumber = lineValue;
                        }

                        break;
                    case "RIN":
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            noteRecord.AutomatedRecordId = lineValue;
                        }

                        break;
                    case "CHAN":
                        GedcomChangeDate date = new GedcomChangeDate(Database);
                        date.Level = level;
                        parseState.Records.Push(date);
                        break;
                    case "SOUR":
                        AddSourceCitation(noteRecord);
                        break;
                }
            }
            else if ((!string.IsNullOrEmpty(parseState.PreviousTag)) &&
                        parseState.PreviousTag == "REFN" &&
                        level == parseState.PreviousLevel + 1)
            {
                if (tag == "TYPE")
                {
                    if (lineValueType == GedcomLineValueType.DataType)
                    {
                        noteRecord.UserReferenceType = lineValue;
                    }
                }
            }
            else
            {
                // shouldn't be here
                Debug.WriteLine("Unknown state / tag parsing note node: " + tag + "\t at level: " + level);
            }
        }

        private void ReadRepositoryRecord()
        {
            GedcomRepositoryRecord repositoryRecord;

            repositoryRecord = parseState.Records.Peek() as GedcomRepositoryRecord;

            if (tag.StartsWith("_"))
            {
                switch (tag)
                {
                    default:
                        GedcomCustomRecord custom = new GedcomCustomRecord();
                        custom.Level = level;
                        custom.XRefID = xrefId;
                        custom.Tag = tag;

                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            custom.Classification = lineValue;
                        }

                        // TODO: may want to use customs at some point
                        parseState.Records.Push(custom);
                        break;
                }
            }

            if (level == repositoryRecord.ParsingLevel + 1)
            {
                switch (tag)
                {
                    case "NAME":
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            repositoryRecord.Name = lineValue;
                        }

                        break;
                    case "ADDR":
                        if (repositoryRecord.Address == null)
                        {
                            repositoryRecord.Address = new GedcomAddress();
                            repositoryRecord.Address.Database = Database;
                        }

                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            repositoryRecord.Address.AddressLine = lineValue;
                        }

                        break;
                    case "PHON":
                        if (repositoryRecord.Address == null)
                        {
                            repositoryRecord.Address = new GedcomAddress();
                            repositoryRecord.Address.Database = Database;
                        }

                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            if (string.IsNullOrEmpty(repositoryRecord.Address.Phone1))
                            {
                                repositoryRecord.Address.Phone1 = lineValue;
                            }
                            else if (string.IsNullOrEmpty(repositoryRecord.Address.Phone2))
                            {
                                repositoryRecord.Address.Phone2 = lineValue;
                            }
                            else if (string.IsNullOrEmpty(repositoryRecord.Address.Phone3))
                            {
                                repositoryRecord.Address.Phone3 = lineValue;
                            }
                            else
                            {
                                // should never occur only 3 phone numbers are allowed
                            }
                        }

                        break;
                    case "EMAIL":
                        if (repositoryRecord.Address == null)
                        {
                            repositoryRecord.Address = new GedcomAddress();
                            repositoryRecord.Address.Database = Database;
                        }

                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            if (string.IsNullOrEmpty(repositoryRecord.Address.Email1))
                            {
                                repositoryRecord.Address.Email1 = lineValue;
                            }
                            else if (string.IsNullOrEmpty(repositoryRecord.Address.Email2))
                            {
                                repositoryRecord.Address.Email2 = lineValue;
                            }
                            else if (string.IsNullOrEmpty(repositoryRecord.Address.Email3))
                            {
                                repositoryRecord.Address.Email3 = lineValue;
                            }
                            else
                            {
                                // should never occur only 3 emails are allowed
                            }
                        }

                        break;
                    case "FAX":
                        if (repositoryRecord.Address == null)
                        {
                            repositoryRecord.Address = new GedcomAddress();
                            repositoryRecord.Address.Database = Database;
                        }

                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            if (string.IsNullOrEmpty(repositoryRecord.Address.Fax1))
                            {
                                repositoryRecord.Address.Fax1 = lineValue;
                            }
                            else if (string.IsNullOrEmpty(repositoryRecord.Address.Fax2))
                            {
                                repositoryRecord.Address.Fax2 = lineValue;
                            }
                            else if (string.IsNullOrEmpty(repositoryRecord.Address.Fax3))
                            {
                                repositoryRecord.Address.Fax3 = lineValue;
                            }
                            else
                            {
                                // should never occur only 3 fax numbers are allowed
                            }
                        }

                        break;
                    case "WWW":
                        if (repositoryRecord.Address == null)
                        {
                            repositoryRecord.Address = new GedcomAddress();
                            repositoryRecord.Address.Database = Database;
                        }

                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            if (string.IsNullOrEmpty(repositoryRecord.Address.Www1))
                            {
                                repositoryRecord.Address.Www1 = lineValue;
                            }
                            else if (string.IsNullOrEmpty(repositoryRecord.Address.Www2))
                            {
                                repositoryRecord.Address.Www2 = lineValue;
                            }
                            else if (string.IsNullOrEmpty(repositoryRecord.Address.Www3))
                            {
                                repositoryRecord.Address.Www3 = lineValue;
                            }
                            else
                            {
                                // should never occur only 3 urls are allowed
                            }
                        }

                        break;
                    case "REFN":
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            repositoryRecord.UserReferenceNumber = lineValue;
                        }

                        break;
                    case "RIN":
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            repositoryRecord.AutomatedRecordId = lineValue;
                        }

                        break;
                    case "CHAN":
                        GedcomChangeDate date = new GedcomChangeDate(Database);
                        date.Level = level;
                        parseState.Records.Push(date);
                        break;
                    case "NOTE":
                        AddNoteRecord(repositoryRecord);
                        break;
                }
            }

            // _ParseState.PreviousLevel + 2)
            else if ((!string.IsNullOrEmpty(parseState.PreviousTag)) && level == repositoryRecord.Level + 2)
            {
                if (parseState.PreviousTag == "REFN" && tag == "TYPE")
                {
                    if (lineValueType == GedcomLineValueType.DataType)
                    {
                        repositoryRecord.UserReferenceType = lineValue;
                    }
                }
                else
                {
                    AddressParse(repositoryRecord.Address, tag, lineValue, lineValueType);
                }
            }
            else if ((!string.IsNullOrEmpty(parseState.PreviousTag)) &&
                        level == parseState.PreviousLevel)
            {
                AddressParse(repositoryRecord.Address, tag, lineValue, lineValueType);
            }
            else
            {
                // shouldn't be here
                Debug.WriteLine("Unknown state / tag parsing note node: " + tag + "\t at level: " + level);
            }
        }

        private void ReadSourceRecord()
        {
            GedcomSourceRecord sourceRecord;

            sourceRecord = parseState.Records.Peek() as GedcomSourceRecord;

            if (level == sourceRecord.ParsingLevel + 1)
            {
                // hack, at this level won't have CONT/CONC so end any building we
                // are doing
                if (sourceRecord.TitleText != null)
                {
                    sourceRecord.Title = sourceRecord.TitleText.ToString();
                    sourceRecord.TitleText = null;
                }
                else if (sourceRecord.OriginatorText != null)
                {
                    sourceRecord.Originator = sourceRecord.OriginatorText.ToString();
                    sourceRecord.OriginatorText = null;
                }
                else if (sourceRecord.PublicationText != null)
                {
                    sourceRecord.PublicationFacts = sourceRecord.PublicationText.ToString();
                    sourceRecord.PublicationText = null;
                }
                else if (sourceRecord.TextText != null)
                {
                    sourceRecord.Text = sourceRecord.TextText.ToString();
                    sourceRecord.TextText = null;
                }

                switch (tag)
                {
                    case "DATA":
                        // info held in child nodes
                        break;
                    case "AUTH":
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            sourceRecord.OriginatorText = new StringBuilder(lineValue);
                        }

                        break;
                    case "TITL":
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            sourceRecord.TitleText = new StringBuilder(lineValue);
                        }

                        break;
                    case "ABBR":
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            sourceRecord.FiledBy = lineValue;
                        }

                        break;
                    case "PUBL":
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            sourceRecord.PublicationText = new StringBuilder(lineValue);
                        }

                        break;
                    case "TEXT":
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            int capacity = lineValue.Length;
                            if (!string.IsNullOrEmpty(sourceRecord.Text))
                            {
                                capacity += sourceRecord.Text.Length;
                                capacity += newlineDelimiter.Length;
                            }

                            sourceRecord.TextText = new StringBuilder(capacity);

                            if (string.IsNullOrEmpty(sourceRecord.Text))
                            {
                                sourceRecord.TextText.Append(lineValue);
                            }
                            else
                            {
                                sourceRecord.TextText.Append(sourceRecord.Text);
                                sourceRecord.TextText.Append(newlineDelimiter);
                                sourceRecord.TextText.Append(lineValue);
                            }
                        }

                        break;
                    case "REPO":
                        GedcomRepositoryCitation citation = new GedcomRepositoryCitation();
                        citation.Level = level;
                        if (lineValueType == GedcomLineValueType.PointerType)
                        {
                            citation.Repository = lineValue;
                            missingReferences.Add(lineValue);
                        }

                        sourceRecord.RepositoryCitations.Add(citation);

                        parseState.Records.Push(citation);
                        break;
                    case "REFN":
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            sourceRecord.UserReferenceNumber = lineValue;
                        }

                        break;
                    case "RIN":
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            sourceRecord.AutomatedRecordId = lineValue;
                        }

                        break;
                    case "CHAN":
                        GedcomChangeDate date = new GedcomChangeDate(Database);
                        date.Level = level;
                        parseState.Records.Push(date);
                        break;
                    case "NOTE":
                        AddNoteRecord(sourceRecord);
                        break;
                    case "OBJE":
                        AddMultimediaRecord(sourceRecord);
                        break;
                }
            }
            else if ((!string.IsNullOrEmpty(parseState.PreviousTag)) && level == sourceRecord.Level + 2)
            {
                if (parseState.PreviousTag == "REFN" && tag == "TYPE")
                {
                    if (lineValueType == GedcomLineValueType.DataType)
                    {
                        sourceRecord.UserReferenceType = lineValue;
                    }
                }
                else if (sourceRecord.OriginatorText != null)
                {
                    switch (tag)
                    {
                        case "CONT":
                            sourceRecord.OriginatorText.Append(newlineDelimiter);
                            sourceRecord.OriginatorText.Append(lineValue);
                            break;
                        case "CONC":
                            sourceRecord.OriginatorText.Append(lineValue);
                            break;
                    }
                }
                else if (sourceRecord.TitleText != null)
                {
                    switch (tag)
                    {
                        case "CONT":
                            sourceRecord.TitleText.Append(newlineDelimiter);
                            sourceRecord.TitleText.Append(lineValue);
                            break;
                        case "CONC":
                            sourceRecord.TitleText.Append(lineValue);
                            break;
                    }
                }
                else if (sourceRecord.PublicationText != null)
                {
                    switch (tag)
                    {
                        case "CONT":
                            sourceRecord.PublicationText.Append(newlineDelimiter);
                            sourceRecord.PublicationText.Append(lineValue);
                            break;
                        case "CONC":
                            sourceRecord.PublicationText.Append(lineValue);
                            break;
                    }
                }

                // (_ParseState.PreviousTag == "TEXT")
                else if (sourceRecord.TextText != null)
                {
                    switch (tag)
                    {
                        case "CONT":
                            sourceRecord.TextText.Append(newlineDelimiter);
                            sourceRecord.TextText.Append(lineValue);
                            break;
                        case "CONC":
                            sourceRecord.TextText.Append(lineValue);
                            break;
                    }
                }

                // if (_ParseState.PreviousTag == "DATA")
                else
                {
                    switch (tag)
                    {
                        case "AGNC":
                            if (lineValueType == GedcomLineValueType.DataType)
                            {
                                sourceRecord.Agency = lineValue;
                            }

                            break;
                        case "EVEN":
                            if (lineValueType == GedcomLineValueType.DataType)
                            {
                                GedcomRecordedEvent recordedEvent = new GedcomRecordedEvent();

                                sourceRecord.EventsRecorded.Add(recordedEvent);

                                string[] events = lineValue.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                foreach (string e in events)
                                {
                                    string ev = e.Trim();
                                    GedcomEventType eventType;

                                    if (ev == "EVEN")
                                    {
                                        eventType = GedcomEventType.GenericEvent;
                                        recordedEvent.Types.Add(eventType);
                                    }
                                    else if (ev == "FACT")
                                    {
                                        eventType = GedcomEventType.GenericFact;
                                        recordedEvent.Types.Add(eventType);
                                    }
                                    else
                                    {
                                        try
                                        {
                                            eventType = EnumHelper.Parse<GedcomEventType>(ev, true);
                                            recordedEvent.Types.Add(eventType);
                                        }
                                        catch
                                        {
                                            try
                                            {
                                                eventType = EnumHelper.Parse<GedcomEventType>(ev + "Fact", true);
                                                recordedEvent.Types.Add(eventType);
                                            }
                                            catch
                                            {
                                                // TODO: shouldn't lose data like this
                                            }
                                        }
                                    }
                                }
                            }

                            break;
                        case "NOTE":
                            string xref = AddNoteRecord(sourceRecord);

                            // belongs in data records, not top level record notes
                            sourceRecord.Notes.Remove(xref);
                            sourceRecord.DataNotes.Add(xref);
                            break;
                    }
                }
            }

            // _ParseState.PreviousLevel + 3)
            else if ((!string.IsNullOrEmpty(parseState.PreviousTag)) && level == sourceRecord.Level + 3)
            {
                GedcomRecordedEvent recordedEvent = sourceRecord.EventsRecorded[sourceRecord.EventsRecorded.Count - 1];
                switch (tag)
                {
                    case "DATE":
                        GedcomDate date = new GedcomDate(Database);
                        date.Level = level;
                        parseState.Records.Push(date);
                        recordedEvent.Date = date;
                        level++;
                        ReadDateRecord();
                        level--;
                        parseState.Records.Pop();
                        break;
                    case "PLAC":
                        GedcomPlace place = new GedcomPlace();
                        place.Level = level;

                        recordedEvent.Place = place;

                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            place.Name = Database.PlaceNameCollection[lineValue];
                        }
                        else
                        {
                            // invalid, provide a name anyway
                            place.Name = "Unknown";
                            Debug.WriteLine("invalid place node, no name at level: " + level);
                        }

                        parseState.Records.Push(place);
                        break;
                }
            }
            else
            {
                // shouldn't be here
                Debug.WriteLine("Unknown state / tag parsing note node: " + tag + "\t at level: " + level);
            }
        }

        private void ReadSubmitterRecord()
        {
            GedcomSubmitterRecord submitterRecord;

            submitterRecord = parseState.Records.Peek() as GedcomSubmitterRecord;

            if (tag.StartsWith("_"))
            {
                switch (tag)
                {
                    default:
                        GedcomCustomRecord custom = new GedcomCustomRecord();
                        custom.Level = level;
                        custom.XRefID = xrefId;
                        custom.Tag = tag;

                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            custom.Classification = lineValue;
                        }

                        // TODO: may want to use customs at some point
                        parseState.Records.Push(custom);
                        break;
                }
            }

            if (level == submitterRecord.ParsingLevel + 1)
            {
                switch (tag)
                {
                    case "NAME":
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            submitterRecord.Name = lineValue;
                        }

                        break;
                    case "ADDR":
                        if (submitterRecord.Address == null)
                        {
                            submitterRecord.Address = new GedcomAddress();
                            submitterRecord.Address.Database = Database;
                        }

                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            submitterRecord.Address.AddressLine = lineValue;
                        }

                        break;
                    case "PHON":
                        if (submitterRecord.Address == null)
                        {
                            submitterRecord.Address = new GedcomAddress();
                            submitterRecord.Address.Database = Database;
                        }

                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            if (string.IsNullOrEmpty(submitterRecord.Address.Phone1))
                            {
                                submitterRecord.Address.Phone1 = lineValue;
                            }
                            else if (string.IsNullOrEmpty(submitterRecord.Address.Phone2))
                            {
                                submitterRecord.Address.Phone2 = lineValue;
                            }
                            else if (string.IsNullOrEmpty(submitterRecord.Address.Phone3))
                            {
                                submitterRecord.Address.Phone3 = lineValue;
                            }
                            else
                            {
                                // should never occur only 3 phone numbers are allowed
                            }
                        }

                        break;
                    case "EMAIL":
                        if (submitterRecord.Address == null)
                        {
                            submitterRecord.Address = new GedcomAddress();
                            submitterRecord.Address.Database = Database;
                        }

                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            if (string.IsNullOrEmpty(submitterRecord.Address.Email1))
                            {
                                submitterRecord.Address.Email1 = lineValue;
                            }
                            else if (string.IsNullOrEmpty(submitterRecord.Address.Email2))
                            {
                                submitterRecord.Address.Email2 = lineValue;
                            }
                            else if (string.IsNullOrEmpty(submitterRecord.Address.Email3))
                            {
                                submitterRecord.Address.Email3 = lineValue;
                            }
                            else
                            {
                                // should never occur only 3 emails are allowed
                            }
                        }

                        break;
                    case "FAX":
                        if (submitterRecord.Address == null)
                        {
                            submitterRecord.Address = new GedcomAddress();
                            submitterRecord.Address.Database = Database;
                        }

                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            if (string.IsNullOrEmpty(submitterRecord.Address.Fax1))
                            {
                                submitterRecord.Address.Fax1 = lineValue;
                            }
                            else if (string.IsNullOrEmpty(submitterRecord.Address.Fax2))
                            {
                                submitterRecord.Address.Fax2 = lineValue;
                            }
                            else if (string.IsNullOrEmpty(submitterRecord.Address.Fax3))
                            {
                                submitterRecord.Address.Fax3 = lineValue;
                            }
                            else
                            {
                                // should never occur only 3 fax numbers are allowed
                            }
                        }

                        break;
                    case "WWW":
                        if (submitterRecord.Address == null)
                        {
                            submitterRecord.Address = new GedcomAddress();
                            submitterRecord.Address.Database = Database;
                        }

                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            if (string.IsNullOrEmpty(submitterRecord.Address.Www1))
                            {
                                submitterRecord.Address.Www1 = lineValue;
                            }
                            else if (string.IsNullOrEmpty(submitterRecord.Address.Www2))
                            {
                                submitterRecord.Address.Www2 = lineValue;
                            }
                            else if (string.IsNullOrEmpty(submitterRecord.Address.Www3))
                            {
                                submitterRecord.Address.Www3 = lineValue;
                            }
                            else
                            {
                                // should never occur only 3 urls are allowed
                            }
                        }

                        break;
                    case "OBJE":
                        AddMultimediaRecord(submitterRecord);
                        break;
                    case "LANG":
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            // only 3 lang are allowed
                            if (submitterRecord.LanguagePreferences.Count < 3)
                            {
                                submitterRecord.LanguagePreferences.Add(lineValue);
                            }
                        }

                        break;
                    case "RFN":
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            submitterRecord.RegisteredRFN = lineValue;
                        }

                        break;
                    case "RIN":
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            submitterRecord.AutomatedRecordId = lineValue;
                        }

                        break;
                    case "CHAN":
                        GedcomChangeDate date = new GedcomChangeDate(Database);
                        date.Level = level;
                        parseState.Records.Push(date);
                        break;
                    case "NOTE":
                        AddNoteRecord(submitterRecord);
                        break;
                }
            }
            else if ((!string.IsNullOrEmpty(parseState.PreviousTag)) &&
                        level == submitterRecord.Level + 2)
            {
                AddressParse(submitterRecord.Address, tag, lineValue, lineValueType);
            }
            else
            {
                // shouldn't be here
                Debug.WriteLine("Unknown state / tag parsing submitter node: " + tag + "\t at level: " + level);
            }
        }

        private void ReadSubmissionRecord()
        {
            GedcomSubmissionRecord submissionRecord;

            submissionRecord = parseState.Records.Peek() as GedcomSubmissionRecord;

            if (level == submissionRecord.ParsingLevel + 1)
            {
                switch (tag)
                {
                    case "SUBM":
                        if (lineValueType == GedcomLineValueType.PointerType)
                        {
                            submissionRecord.Submitter = lineValue;
                            missingReferences.Add(lineValue);
                        }
                        else
                        {
                            GedcomSubmitterRecord submitter = new GedcomSubmitterRecord();
                            submitter.Level = 0; // new top level submitter, always 0;
                            submitter.ParsingLevel = level;
                            submitter.XRefID = Database.GenerateXref("SUBM");

                            parseState.Records.Push(submitter);

                            submissionRecord.Submitter = submitter.XRefID;
                        }

                        break;
                    case "FAMF":
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            submissionRecord.FamilyFile = lineValue;
                        }

                        break;
                    case "TEMP":
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            submissionRecord.TempleCode = lineValue;
                        }

                        break;
                    case "ANCE":
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            int num = 0;
                            if (int.TryParse(lineValue, out num))
                            {
                                submissionRecord.GenerationsOfAncestors = num;
                            }
                        }

                        break;
                    case "DESC":
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            int num = 0;
                            if (int.TryParse(lineValue, out num))
                            {
                                submissionRecord.GenerationsOfDecendants = num;
                            }
                        }

                        break;
                    case "ORDI":
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            submissionRecord.OrdinanceProcessFlag = string.Compare(lineValue, "YES", true) == 0;
                        }

                        break;
                    case "RIN":
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            submissionRecord.AutomatedRecordId = lineValue;
                        }

                        break;
                    case "CHAN":
                        GedcomChangeDate date = new GedcomChangeDate(Database);
                        date.Level = level;
                        parseState.Records.Push(date);
                        break;
                    case "NOTE":
                        AddNoteRecord(submissionRecord);
                        break;
                }
            }
            else
            {
                // shouldn't be here
                Debug.WriteLine("Unknown state / tag parsing submission node: " + tag + "\t at level: " + level);
            }
        }

        private void ReadEventRecord()
        {
            GedcomEvent eventRecord;
            bool done = false;

            eventRecord = parseState.Records.Peek() as GedcomEvent;

            if (tag.StartsWith("_"))
            {
                switch (tag)
                {
                    default:
                        GedcomCustomRecord custom = new GedcomCustomRecord();
                        custom.Level = level;
                        custom.XRefID = xrefId;
                        custom.Tag = tag;

                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            custom.Classification = lineValue;
                        }

                        // TODO: may want to use customs at some point
                        parseState.Records.Push(custom);
                        break;
                }
            }

            switch (eventRecord.RecordType)
            {
                case GedcomRecordType.FamilyEvent:
                    GedcomFamilyEvent famEvent = eventRecord as GedcomFamilyEvent;
                    if (level == eventRecord.ParsingLevel + 2 && tag == "AGE")
                    {
                        if (parseState.PreviousTag == "HUSB")
                        {
                            GedcomAge age = GedcomAge.Parse(lineValue, Database);
                            famEvent.HusbandAge = age;
                            done = true;
                        }
                        else if (parseState.PreviousTag == "WIFE")
                        {
                            GedcomAge age = GedcomAge.Parse(lineValue, Database);
                            famEvent.WifeAge = age;
                            done = true;
                        }
                    }
                    else if (level == eventRecord.ParsingLevel + 1)
                    {
                        done = tag == "HUSB" || tag == "WIFE";
                    }

                    break;
                case GedcomRecordType.IndividualEvent:
                    GedcomIndividualEvent individualEvent = eventRecord as GedcomIndividualEvent;
                    if (level == eventRecord.ParsingLevel + 1)
                    {
                        if (tag == "AGE")
                        {
                            GedcomAge age = GedcomAge.Parse(lineValue, Database);
                            individualEvent.Age = age;
                            done = true;
                        }
                        else if (tag == "FAMC" &&
                                   (eventRecord.EventType == GedcomEventType.Birth ||
                                     eventRecord.EventType == GedcomEventType.CHR ||
                                     eventRecord.EventType == GedcomEventType.ADOP))
                        {
                            if (lineValueType == GedcomLineValueType.PointerType)
                            {
                                individualEvent.Famc = lineValue;
                                missingReferences.Add(lineValue);
                            }

                            done = true;
                        }
                        else if (tag == "CONT" &&
                                 eventRecord.EventType == GedcomEventType.DSCRFact)
                        {
                            eventRecord.Classification += newlineDelimiter;
                            eventRecord.Classification += lineValue;
                        }
                        else if (tag == "CONC" &&
                                 eventRecord.EventType == GedcomEventType.DSCRFact)
                        {
                            eventRecord.Classification += lineValue;
                        }
                    }
                    else if (level == eventRecord.ParsingLevel + 2)
                    {
                        if (tag == "ADOP" &&
                            eventRecord.EventType == GedcomEventType.ADOP)
                        {
                            if (lineValueType == GedcomLineValueType.DataType)
                            {
                                if (lineValue == "HUSB")
                                {
                                    individualEvent.AdoptedBy = GedcomAdoptionType.Husband;
                                }
                                else if (lineValue == "WIFE")
                                {
                                    individualEvent.AdoptedBy = GedcomAdoptionType.Wife;
                                }
                                else if (lineValue == "BOTH")
                                {
                                    individualEvent.AdoptedBy = GedcomAdoptionType.HusbandAndWife;
                                }
                            }

                            done = true;
                        }
                    }

                    break;
            }

            if (!done)
            {
                if (level == eventRecord.ParsingLevel + 1)
                {
                    switch (tag)
                    {
                        case "TYPE":
                            if (lineValueType == GedcomLineValueType.DataType)
                            {
                                // if the event is generic, but the type
                                // can be mapped to an actual event type
                                // convert it.
                                bool convertedEventType = false;
                                if ((eventRecord.EventType == GedcomEventType.GenericEvent ||
                                     eventRecord.EventType == GedcomEventType.GenericFact)
                                    && string.IsNullOrEmpty(eventRecord.EventName))
                                {
                                    GedcomEventType type = GedcomEvent.ReadableToType(lineValue);
                                    if (type != GedcomEventType.GenericEvent)
                                    {
                                        eventRecord.EventType = type;
                                        convertedEventType = true;
                                    }
                                }

                                if (!convertedEventType)
                                {
                                    // in TGC551LF  (torture test gedcom file) TYPE is set
                                    // to the same as the event tag name in some instances
                                    // this is stupid, so if _lineValue is the same
                                    // as the event tag, don't set it.
                                    string eventTag = parseState.ParentTag(level);
                                    if (lineValue != eventTag)
                                    {
                                        eventRecord.Classification = lineValue;
                                    }
                                }
                            }

                            break;
                        case "DATE":
                            GedcomDate date = new GedcomDate(Database);
                            date.Database = Database;
                            date.Level = level;
                            parseState.Records.Push(date);
                            eventRecord.Date = date;
                            level++;
                            ReadDateRecord();
                            level--;
                            parseState.Records.Pop();
                            break;
                        case "PLAC":
                            GedcomPlace place = new GedcomPlace();
                            place.Database = Database;
                            place.Level = level;

                            eventRecord.Place = place;

                            if (lineValueType == GedcomLineValueType.DataType)
                            {
                                place.Name = lineValue;
                            }
                            else
                            {
                                // invalid, provide a name anyway
                                place.Name = string.Empty;
                                Debug.WriteLine("invalid place node, no name at level: " + level);
                            }

                            parseState.Records.Push(place);
                            break;
                        case "ADDR":
                            if (eventRecord.Address == null)
                            {
                                eventRecord.Address = new GedcomAddress();
                                eventRecord.Address.Database = Database;
                            }

                            if (lineValueType == GedcomLineValueType.DataType)
                            {
                                eventRecord.Address.AddressLine = lineValue;
                            }

                            break;
                        case "PHON":
                            if (eventRecord.Address == null)
                            {
                                eventRecord.Address = new GedcomAddress();
                                eventRecord.Address.Database = Database;
                            }

                            if (lineValueType == GedcomLineValueType.DataType)
                            {
                                if (string.IsNullOrEmpty(eventRecord.Address.Phone1))
                                {
                                    eventRecord.Address.Phone1 = lineValue;
                                }
                                else if (string.IsNullOrEmpty(eventRecord.Address.Phone2))
                                {
                                    eventRecord.Address.Phone2 = lineValue;
                                }
                                else if (string.IsNullOrEmpty(eventRecord.Address.Phone3))
                                {
                                    eventRecord.Address.Phone3 = lineValue;
                                }
                                else
                                {
                                    // should never occur only 3 phone numbers are allowed
                                }
                            }

                            break;
                        case "EMAIL":
                            if (eventRecord.Address == null)
                            {
                                eventRecord.Address = new GedcomAddress();
                                eventRecord.Address.Database = Database;
                            }

                            if (lineValueType == GedcomLineValueType.DataType)
                            {
                                if (string.IsNullOrEmpty(eventRecord.Address.Email1))
                                {
                                    eventRecord.Address.Email1 = lineValue;
                                }
                                else if (string.IsNullOrEmpty(eventRecord.Address.Email2))
                                {
                                    eventRecord.Address.Email2 = lineValue;
                                }
                                else if (string.IsNullOrEmpty(eventRecord.Address.Email3))
                                {
                                    eventRecord.Address.Email3 = lineValue;
                                }
                                else
                                {
                                    // should never occur only 3 emails are allowed
                                }
                            }

                            break;
                        case "FAX":
                            if (eventRecord.Address == null)
                            {
                                eventRecord.Address = new GedcomAddress();
                                eventRecord.Address.Database = Database;
                            }

                            if (lineValueType == GedcomLineValueType.DataType)
                            {
                                if (string.IsNullOrEmpty(eventRecord.Address.Fax1))
                                {
                                    eventRecord.Address.Fax1 = lineValue;
                                }
                                else if (string.IsNullOrEmpty(eventRecord.Address.Fax2))
                                {
                                    eventRecord.Address.Fax2 = lineValue;
                                }
                                else if (string.IsNullOrEmpty(eventRecord.Address.Fax3))
                                {
                                    eventRecord.Address.Fax3 = lineValue;
                                }
                                else
                                {
                                    // should never occur only 3 fax numbers are allowed
                                }
                            }

                            break;
                        case "WWW":
                            if (eventRecord.Address == null)
                            {
                                eventRecord.Address = new GedcomAddress();
                                eventRecord.Address.Database = Database;
                            }

                            if (lineValueType == GedcomLineValueType.DataType)
                            {
                                if (string.IsNullOrEmpty(eventRecord.Address.Www1))
                                {
                                    eventRecord.Address.Www1 = lineValue;
                                }
                                else if (string.IsNullOrEmpty(eventRecord.Address.Www2))
                                {
                                    eventRecord.Address.Www2 = lineValue;
                                }
                                else if (string.IsNullOrEmpty(eventRecord.Address.Www3))
                                {
                                    eventRecord.Address.Www3 = lineValue;
                                }
                                else
                                {
                                    // should never occur only 3 urls are allowed
                                }
                            }

                            break;
                        case "AGNC":
                            if (lineValueType == GedcomLineValueType.DataType)
                            {
                                eventRecord.ResponsibleAgency = lineValue;
                            }

                            break;
                        case "RELI":
                            if (lineValueType == GedcomLineValueType.DataType)
                            {
                                eventRecord.ReligiousAffiliation = lineValue;
                            }

                            break;
                        case "CAUS":
                            if (lineValueType == GedcomLineValueType.DataType)
                            {
                                eventRecord.Cause = lineValue;
                            }

                            break;
                        case "RESN":
                            // restriction notice
                            if (lineValueType == GedcomLineValueType.DataType)
                            {
                                try
                                {
                                    eventRecord.RestrictionNotice = EnumHelper.Parse<GedcomRestrictionNotice>(lineValue, true);
                                }
                                catch
                                {
                                    Debug.WriteLine("Invalid restriction type: " + lineValue);

                                    // default to confidential to protect privacy
                                    eventRecord.RestrictionNotice = GedcomRestrictionNotice.Confidential;
                                }
                            }

                            break;
                        case "NOTE":
                            AddNoteRecord(eventRecord);
                            break;
                        case "SOUR":
                            AddSourceCitation(eventRecord);
                            break;
                        case "OBJE":
                            AddMultimediaRecord(eventRecord);
                            break;
                        case "QUAY":
                            if (lineValueType == GedcomLineValueType.DataType)
                            {
                                int certainty = Convert.ToInt32(lineValue);
                                if ((certainty > (int)GedcomCertainty.Primary) ||
                                    (certainty < (int)GedcomCertainty.Unreliable))
                                {
                                    certainty = (int)GedcomCertainty.Unreliable;
                                }

                                eventRecord.Certainty = (GedcomCertainty)certainty;
                            }

                            break;
                    }
                }
                else if (parseState.PreviousTag != string.Empty && level == eventRecord.ParsingLevel + 2)
                {
                    AddressParse(eventRecord.Address, tag, lineValue, lineValueType);
                }
            }
        }

        private void ReadPlaceRecord()
        {
            GedcomPlace place;

            place = parseState.Records.Peek() as GedcomPlace;

            if (level == place.ParsingLevel + 1)
            {
                switch (tag)
                {
                    case "FORM":
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            place.Form = lineValue;
                        }

                        break;
                    case "FONE":
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            GedcomVariation variation = new GedcomVariation();
                            variation.Database = Database;
                            variation.Value = lineValue;

                            place.PhoneticVariations.Add(variation);
                        }

                        break;
                    case "ROMN":
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            GedcomVariation variation = new GedcomVariation();
                            variation.Database = Database;
                            variation.Value = lineValue;

                            place.RomanizedVariations.Add(variation);
                        }

                        break;
                    case "MAP":
                        // map, longitude / latitude stored as child nodes
                        break;
                    case "NOTE":
                        AddNoteRecord(place);
                        break;
                }
            }
            else if (parseState.PreviousTag != string.Empty && level == place.ParsingLevel + 2)
            {
                if (tag == "TYPE")
                {
                    if (parseState.PreviousTag == "FONE")
                    {
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            GedcomVariation variation = place.PhoneticVariations[place.PhoneticVariations.Count - 1];
                            variation.VariationType = lineValue;
                        }
                    }
                    else if (parseState.PreviousTag == "ROMN")
                    {
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            GedcomVariation variation = place.RomanizedVariations[place.RomanizedVariations.Count - 1];
                            variation.VariationType = lineValue;
                        }
                    }
                }
                else if (parseState.PreviousTag == "MAP")
                {
                    if (tag == "LATI")
                    {
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            place.Latitude = lineValue;
                        }
                    }
                    else if (tag == "LONG")
                    {
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            place.Longitude = lineValue;
                        }
                    }
                }
            }
            else
            {
                // shouldn't be here
                Debug.WriteLine("Unknown state / tag parsing place node: " + tag + "\t at level: " + level);
            }
        }

        private void ReadSourceCitationRecord()
        {
            GedcomSourceCitation sourceCitation;

            sourceCitation = parseState.Records.Peek() as GedcomSourceCitation;

            GedcomSourceRecord sourceRecord = null;

            if (parseState.Database.Contains(sourceCitation.Source))
            {
                sourceRecord = parseState.Database[sourceCitation.Source] as GedcomSourceRecord;
            }

            if (level == sourceCitation.ParsingLevel + 1)
            {
                switch (tag)
                {
                    case "PAGE":
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            sourceCitation.Page = lineValue;
                        }

                        break;
                    case "CONT":
                        if (sourceRecord != null)
                        {
                            sourceRecord.Title += newlineDelimiter;
                            sourceRecord.Title += lineValue;
                        }

                        break;
                    case "CONC":
                        if (sourceRecord != null)
                        {
                            sourceRecord.Title += lineValue;
                        }

                        break;
                    case "TEXT":
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            if (sourceCitation.ParsedText == null)
                            {
                                int capacity = lineValue.Length;
                                if (!string.IsNullOrEmpty(sourceCitation.Text))
                                {
                                    capacity += sourceCitation.Text.Length;
                                    capacity += newlineDelimiter.Length;
                                }

                                sourceCitation.ParsedText = new StringBuilder(capacity);
                            }

                            if (!string.IsNullOrEmpty(sourceCitation.Text))
                            {
                                sourceCitation.ParsedText.Append(newlineDelimiter);
                            }

                            sourceCitation.ParsedText.Append(lineValue);
                        }

                        break;
                    case "DATA":
                        // data tag, just contains child tags
                        break;
                    case "EVEN":
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            sourceCitation.EventType = lineValue;
                        }

                        break;
                    case "OBJE":
                        AddMultimediaRecord(sourceCitation);
                        break;
                    case "NOTE":
                        AddNoteRecord(sourceCitation);
                        break;
                    case "QUAY":
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            int certainty = Convert.ToInt32(lineValue);
                            if ((certainty > (int)GedcomCertainty.Primary) ||
                                (certainty < (int)GedcomCertainty.Unreliable))
                            {
                                certainty = (int)GedcomCertainty.Unreliable;
                            }

                            sourceCitation.Certainty = (GedcomCertainty)certainty;
                        }

                        break;
                }
            }
            else if (parseState.PreviousTag != string.Empty && level == sourceCitation.ParsingLevel + 2)
            {
                if (parseState.PreviousTag == "EVEN" && tag == "ROLE")
                {
                    if (lineValueType == GedcomLineValueType.DataType)
                    {
                        sourceCitation.Role = lineValue;
                    }
                }
                else
                {
                    if (tag == "DATE")
                    {
                        GedcomDate date = new GedcomDate(Database);
                        date.Level = level;
                        parseState.Records.Push(date);
                        sourceCitation.Date = date;
                        level++;
                        ReadDateRecord();
                        level--;
                        parseState.Records.Pop();
                    }
                    else if (tag == "TEXT")
                    {
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            if (sourceCitation.ParsedText == null)
                            {
                                int capacity = lineValue.Length;
                                if (!string.IsNullOrEmpty(sourceCitation.Text))
                                {
                                    capacity += sourceCitation.Text.Length;
                                    capacity += newlineDelimiter.Length;
                                }

                                sourceCitation.ParsedText = new StringBuilder(capacity);
                            }

                            if (!string.IsNullOrEmpty(sourceCitation.Text))
                            {
                                sourceCitation.ParsedText.Append(newlineDelimiter);
                            }

                            sourceCitation.ParsedText.Append(lineValue);
                        }
                    }
                    else if (tag == "CONC")
                    {
                        if (sourceCitation.ParsedText == null)
                        {
                            sourceCitation.ParsedText = new StringBuilder(lineValue.Length);
                        }

                        sourceCitation.ParsedText.Append(lineValue);
                    }
                    else if (tag == "CONT")
                    {
                        if (sourceCitation.ParsedText == null)
                        {
                            int capacity = lineValue.Length + newlineDelimiter.Length;
                            sourceCitation.ParsedText = new StringBuilder(capacity);
                        }

                        sourceCitation.ParsedText.Append(newlineDelimiter);
                        sourceCitation.ParsedText.Append(lineValue);
                    }
                }
            }
            else if (parseState.PreviousTag != string.Empty && level == sourceCitation.ParsingLevel + 3)
            {
                if (parseState.PreviousTag == "TEXT" || parseState.PreviousTag == "CONC" || parseState.PreviousTag == "CONT")
                {
                    if (tag == "CONC")
                    {
                        if (sourceCitation.ParsedText == null)
                        {
                            sourceCitation.ParsedText = new StringBuilder(lineValue.Length);
                        }

                        sourceCitation.ParsedText.Append(lineValue);
                    }
                    else if (tag == "CONT")
                    {
                        if (sourceCitation.ParsedText == null)
                        {
                            int capacity = lineValue.Length + newlineDelimiter.Length;
                            sourceCitation.ParsedText = new StringBuilder(capacity);
                        }

                        sourceCitation.ParsedText.Append(newlineDelimiter);
                        sourceCitation.ParsedText.Append(lineValue);
                    }
                }
            }
            else
            {
                // shouldn't be here
                Debug.WriteLine("Unknown state / tag parsing source citation node: " + tag + "\t at level: " + level);
            }
        }

        private void ReadSpouseSealingRecord()
        {
            var record = parseState.Records.Peek() as GedcomSpouseSealingRecord;

            if (level == record.ParsingLevel + 1)
            {
                switch (tag)
                {
                    case "DATE":
                        GedcomDate date = new GedcomDate(Database);
                        date.Level = level;
                        parseState.Records.Push(date);
                        record.Date = date;
                        level++;
                        ReadDateRecord();
                        level--;
                        parseState.Records.Pop();
                        break;
                    case "PLAC":
                        record.Place = new GedcomPlace();
                        record.Place.Database = Database;
                        record.Place.Level = level;

                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            record.Place.Name = Database.PlaceNameCollection[lineValue];
                        }
                        else
                        {
                            // invalid, provide a name anyway
                            record.Place.Name = "Unknown";
                            Debug.WriteLine("invalid place node, no name at level: " + level);
                        }

                        break;
                    case "NOTE":
                        AddNoteRecord(record);
                        break;
                    case "SOUR":
                        AddSourceCitation(record);
                        break;
                    case "STAT":
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            try
                            {
                                record.Status = EnumHelper.Parse<SpouseSealingDateStatus>(lineValue, true);
                            }
                            catch
                            {
                                Debug.WriteLine("Invalid spouse sealing date status value: " + lineValue);

                                record.Status = SpouseSealingDateStatus.NotSet;
                            }
                        }

                        break;
                    case "TEMP":
                        record.TempleCode = lineValue;
                        break;
                    default:
                        // TODO: Log unexpected tag below sealing.
                        break;
                }
            }
            else if ((!string.IsNullOrEmpty(parseState.PreviousTag)) &&
                        parseState.PreviousTag == "STAT" &&
                        level == parseState.PreviousLevel + 1)
            {
                if (tag == "DATE")
                {
                    if (lineValueType == GedcomLineValueType.DataType)
                    {
                        record.StatusChangeDate = new GedcomChangeDate(Database);
                        record.StatusChangeDate.ParseDateString(lineValue);
                        record.StatusChangeDate.Level = level;
                        parseState.Records.Push(record.StatusChangeDate);
                    }
                }
            }
            else
            {
                // shouldn't be here
                Debug.WriteLine("Unknown state / tag parsing note node: " + tag + "\t at level: " + level);
            }
        }

        private void ReadFamilyLinkRecord()
        {
            GedcomFamilyLink childOf;

            childOf = parseState.Records.Peek() as GedcomFamilyLink;

            if (level == childOf.ParsingLevel + 1)
            {
                switch (tag)
                {
                    case "PEDI":
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            try
                            {
                                childOf.Pedigree = EnumHelper.Parse<PedigreeLinkageType>(lineValue, true);
                            }
                            catch
                            {
                                Debug.WriteLine("Invalid pedegree linkage type: " + lineValue);

                                childOf.Pedigree = PedigreeLinkageType.Unknown;
                            }
                        }

                        break;
                    case "STAT":
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            try
                            {
                                childOf.Status = EnumHelper.Parse<ChildLinkageStatus>(lineValue, true);
                            }
                            catch
                            {
                                Debug.WriteLine("Invalid child linkage status type: " + lineValue);

                                childOf.Status = ChildLinkageStatus.Unknown;
                            }
                        }

                        break;
                    case "NOTE":
                        AddNoteRecord(childOf);
                        break;
                }
            }
            else
            {
                // shouldn't be here
                Debug.WriteLine("Unknown state / tag parsing family link node: " + tag + "\t at level: " + level);
            }
        }

        private void ReadAssociationRecord()
        {
            GedcomAssociation association;

            association = parseState.Records.Peek() as GedcomAssociation;

            if (level == association.ParsingLevel + 1)
            {
                switch (tag)
                {
                    case "RELA":
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            association.Description = lineValue;
                        }

                        break;
                    case "NOTE":
                        AddNoteRecord(association);
                        break;
                    case "SOUR":
                        AddSourceCitation(association);
                        break;
                }
            }
            else
            {
                // shouldn't be here
                Debug.WriteLine("Unknown state / tag parsing association node: " + tag + "\t at level: " + level);
            }
        }

        private void ReadNameRecord()
        {
            GedcomName name;

            name = parseState.Records.Peek() as GedcomName;

            if (level == name.ParsingLevel + 1)
            {
                switch (tag)
                {
                    case "TYPE":
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            name.Type = lineValue;
                        }

                        break;
                    case "FONE":
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            GedcomVariation variation = new GedcomVariation();
                            variation.Database = Database;
                            variation.Value = lineValue;

                            name.PhoneticVariations.Add(variation);
                        }

                        break;
                    case "ROMN":
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            GedcomVariation variation = new GedcomVariation();
                            variation.Database = Database;
                            variation.Value = lineValue;

                            name.RomanizedVariations.Add(variation);
                        }

                        break;
                    case "NPFX":
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            // Prefix from NAME has priority
                            if (string.IsNullOrEmpty(name.Prefix))
                            {
                                name.Prefix = lineValue;
                            }
                        }

                        break;
                    case "GIVN":
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            // Given name part has priority over parsed NAME tag if it is supplied.
                            if (!string.IsNullOrEmpty(lineValue))
                            {
                                name.Given = lineValue;
                            }
                        }

                        break;
                    case "NICK":
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            // Nickname part has priority over parsed NAME tag if it is supplied.
                            if (!string.IsNullOrEmpty(lineValue))
                            {
                                name.Nick = lineValue;
                            }
                        }

                        break;
                    case "SPFX":
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            // Surname prefix part has priority over parsed NAME tag if it is supplied.
                            if (!string.IsNullOrEmpty(lineValue))
                            {
                                name.SurnamePrefix = lineValue;
                            }
                        }

                        break;
                    case "SURN":
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            // Surname part has priority over parsed NAME tag if it is supplied.
                            if (!string.IsNullOrEmpty(lineValue))
                            {
                                name.Surname = lineValue;
                            }
                        }

                        break;
                    case "NSFX":
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            // Name suffix part has priority over parsed NAME tag if it is supplied.
                            if (!string.IsNullOrEmpty(lineValue))
                            {
                                name.Suffix = lineValue;
                            }
                        }

                        break;
                    case "NOTE":
                        AddNoteRecord(name);
                        break;
                    case "SOUR":
                        AddSourceCitation(name);
                        break;
                }
            }
            else if (parseState.PreviousTag != string.Empty && level == name.ParsingLevel + 2)
            {
                if (tag == "TYPE")
                {
                    if (parseState.PreviousTag == "FONE")
                    {
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            GedcomVariation variation = name.PhoneticVariations[name.PhoneticVariations.Count - 1];
                            variation.VariationType = lineValue;
                        }
                    }
                    else if (parseState.PreviousTag == "ROMN")
                    {
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            GedcomVariation variation = name.RomanizedVariations[name.RomanizedVariations.Count - 1];
                            variation.VariationType = lineValue;
                        }
                    }
                }
            }
            else
            {
                // shouldn't be here
                Debug.WriteLine("Unknown state / tag parsing name node: " + tag + "\t at level: " + level);
            }
        }

        private void ReadDateRecord()
        {
            GedcomDate date;

            date = parseState.Records.Peek() as GedcomDate;

            if (level == date.ParsingLevel + 1)
            {
                switch (tag)
                {
                    // Yes this does seem odd a DATE when we are already parsing
                    //  a GedcomDateRecord.  The reason for this is that
                    // we treat a CHAN as a GedcomDate as that is all it really is
                    // and it contains the DATE as a child tag, so at level + 1
                    case "DATE":
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            DateParse(date, lineValue);
                        }

                        break;

                    // Again, CHAN can have notes
                    case "NOTE":
                        AddNoteRecord(date);
                        break;

                    // for a normal DATE +1 is correct, for a CHAN, +2
                    case "TIME":
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            date.Time = lineValue;
                        }

                        break;

                    // sources aren't allowed on change dates, however family tree maker
                    // is known to put them in, we won't bother differentiating
                    // dates and change dates so we will just allow on either
                    case "SOUR":
                        AddSourceCitation(date);
                        break;
                }
            }
            else if (level == date.ParsingLevel + 2)
            {
                switch (tag)
                {
                    // Time for a CHAN
                    case "TIME":
                        if (lineValueType == GedcomLineValueType.DataType)
                        {
                            date.Time = lineValue;
                        }

                        break;
                }
            }
            else
            {
                // shouldn't be here
                Debug.WriteLine("Unknown state / tag parsing date node: " + tag + "\t at level: " + level);
            }
        }

        private void ReadRepositoryCitation()
        {
            GedcomRepositoryCitation citation;

            citation = parseState.Records.Peek() as GedcomRepositoryCitation;

            if (level == citation.ParsingLevel + 1)
            {
                switch (tag)
                {
                    case "NOTE":
                        AddNoteRecord(citation);
                        break;
                    case "CALN":
                        citation.CallNumbers.Add(lineValue);
                        citation.MediaTypes.Add(SourceMediaType.None);
                        break;
                }
            }
            else if (parseState.PreviousTag == "CALN" &&
                     level == citation.ParsingLevel + 2)
            {
                if (tag == "MEDI" &&
                    lineValueType == GedcomLineValueType.DataType)
                {
                    SourceMediaType sourceMediaType = SourceMediaType.None;
                    try
                    {
                        string val = lineValue.Replace(" ", "_");
                        sourceMediaType = EnumHelper.Parse<SourceMediaType>(val, true);

                        // Parsed as "Other" but the type isn't specified (see comment below)
                        if (sourceMediaType == SourceMediaType.Other)
                        {
                            citation.OtherMediaTypes.Add(lineValue);
                        }
                    }
                    catch
                    {
                        // TGC551LF.GED has an invalid MEDI value
                        // "Book (or other description of this source)"
                        //
                        // Spec says:
                        //
                        // [ audio | book | card | electronic | fiche | film | magazine |
                        // manuscript | map | newspaper | photo | tombstone | video ]
                        // A code, selected from one of the media classifications choices above,
                        // that indicates the type of material in which the referenced source is stored.
                        //
                        // We support other types as well as defined by SourceMediaType,
                        // and other types should be added as needed.  This field is NOT for a
                        // description.
                        // Set to other and set other field for this media type to hold the
                        // value entered.
                        sourceMediaType = SourceMediaType.Other;
                        citation.OtherMediaTypes.Add(lineValue);
                    }

                    citation.MediaTypes[citation.MediaTypes.Count - 1] = sourceMediaType;
                }
            }
            else
            {
                // shouldn't be here
                Debug.WriteLine("Unknown state / tag parsing repo node: " + tag + "\t at level: " + level);
            }
        }

        private void AddSourceCitation(GedcomRecord record)
        {
            GedcomSourceCitation sourceCitation = new GedcomSourceCitation();
            sourceCitation.Level = level;
            sourceCitation.Database = parseState.Database;

            if (lineValueType == GedcomLineValueType.PointerType)
            {
                sourceCitation.Source = lineValue;
                missingReferences.Add(lineValue);
            }
            else
            {
                GedcomSourceRecord source = new GedcomSourceRecord();
                source.Level = 0; // new top level source, always 0
                source.ParsingLevel = level;
                source.XRefID = Database.GenerateXref("SOUR");

                if (lineValue != string.Empty)
                {
                    source.Title = lineValue;
                }

                sourceCitation.Source = source.XRefID;

                parseState.Database.Add(source.XRefID, source);
            }

            record.Sources.Add(sourceCitation);
            parseState.Records.Push(sourceCitation);

            sourceCitations.Add(sourceCitation);
        }

        private string AddNoteRecord(GedcomRecord record)
        {
            string xref = string.Empty;

            if (lineValueType == GedcomLineValueType.PointerType)
            {
                if (!removedNotes.Contains(lineValue))
                {
                    record.Notes.Add(lineValue);
                    xref = lineValue;
                    missingReferences.Add(lineValue);
                }
            }
            else
            {
                GedcomNoteRecord note = new GedcomNoteRecord();
                note.Level = 0; // new top level note, always 0 (not true, 1 in header, fixed up later)
                note.ParsingLevel = level;
                note.XRefID = Database.GenerateXref("NOTE");

                if (lineValue != string.Empty)
                {
                    note.ParsedText.Append(lineValue);
                }

                parseState.Records.Push(note);

                record.Notes.Add(note.XRefID);
                xref = note.XRefID;
            }

            return xref;
        }

        private void AddMultimediaRecord(GedcomRecord record)
        {
            if (lineValueType == GedcomLineValueType.PointerType)
            {
                record.Multimedia.Add(lineValue);
                missingReferences.Add(lineValue);
            }
            else
            {
                GedcomMultimediaRecord multimedia = new GedcomMultimediaRecord();
                multimedia.Level = 0; // new top level multimedia, always 0
                multimedia.ParsingLevel = level;
                multimedia.XRefID = Database.GenerateXref("OBJE");

                record.Multimedia.Add(multimedia.XRefID);
                parseState.Records.Push(multimedia);
            }
        }

        private string AddSubmitterRecord(GedcomRecord record)
        {
            string xref;

            if (lineValueType == GedcomLineValueType.PointerType)
            {
                xref = lineValue;
                missingReferences.Add(xref);
            }
            else
            {
                GedcomSubmitterRecord submitter = new GedcomSubmitterRecord();
                submitter.Level = 0; // always level 0
                submitter.ParsingLevel = level + 1;
                submitter.XRefID = Database.GenerateXref("S");
                parseState.Records.Push(submitter);

                xref = submitter.XRefID;
            }

            return xref;
        }

        // tag mapping for broken GEDCOM, if it is just
        // a matter of the tag name not being valid we can
        // map to the correct one (or the one we support)
        private string TagMap(string tag)
        {
            string ret = tag;
            switch (tag)
            {
                // we convert _AKA to the admitedly invalid AKA, but we deal
                // with that as a valid tag as it is known to occur in some
                // files.  Ends up adding a name with a type of aka
                case "_AKA":
                    ret = "AKA";
                    break;

                // we convert _DEG to GRAD, could possibly be EDUC
                case "_DEG":
                    this.tag = "GRAD";
                    break;
                case "_EMAIL":
                case "EMAL": // seen from Generations
                    ret = "EMAIL";
                    break;
                case "_URL":
                case "URL":
                    ret = "WWW";
                    break;
            }

            return ret;
        }
    }
}