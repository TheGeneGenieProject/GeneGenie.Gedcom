/*
 *  $Id: GedcomParser.cs 198 2008-11-15 15:18:04Z davek $
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

    /// <summary>
    /// Defines the parse states for GEDCOM file
    /// </summary>
    public enum GedcomState
    {
        /// <summary>
        /// Reading the current level 
        /// </summary>
        Level,
        /// <summary>
        /// Reading the current ID
        /// </summary>
        XrefID,
        /// <summary>
        /// Reading the current tag name
        /// </summary>
        Tag,
        /// <summary>
        /// Reading the value for the current tag
        /// </summary>
        LineValue
    }

    /// <summary>
    /// Defines the current error status the parser is in
    /// </summary>
    public enum GedcomErrorState
    {
        /// <summary>
        /// No error has occured
        /// </summary>
        NoError = 0,

        /// <summary>
        /// A level value was expected but not found
        /// </summary>
        LevelExpected,
        /// <summary>
        /// Delimeter after level not found
        /// </summary>
        LevelMissingDelim,
        /// <summary>
        /// The level value is invalid
        /// </summary>
        LevelInvalid,

        /// <summary>
        /// Delimeter after XrefID not found
        /// </summary>
        XrefIDMissingDelim,
        /// <summary>
        /// The ID is too long, can be at most 22 characters
        /// </summary>
        XrefIDTooLong,

        /// <summary>
        /// A GEDCOM tag name (or custom tag name) was expected but not found
        /// </summary>
        TagExpected,
        /// <summary>
        /// Delimeter, or newline after the tag was not found 
        /// </summary>
        TagMissingDelimOrTerm,

        /// <summary>
        /// Value expected but not found
        /// </summary>
        LineValueExpected,
        /// <summary>
        /// newline after line value not found
        /// </summary>
        LineValueMissingTerm,
        /// <summary>
        /// The line value is invalid
        /// </summary>
        LineValueInvalid,

        /// <summary>
        /// Deliminator in GEDCOM is a single space, this error will occur
        /// when a multi space delimiter is detected
        /// </summary>
        InvalidDelim,

        /// <summary>
        /// An unknown error has occured while parsing
        /// </summary>
        UnknownError
    }

    /// <summary>
    /// Line values in GEDCOM can either be a pointer to another record, or the data itself.
    /// </summary>
    public enum GedcomLineValueType
    {
        /// <summary>
        /// No line value
        /// </summary>
        NoType,
        /// <summary>
        /// Line value is a pointer to another record
        /// </summary>
        PointerType,
        /// <summary>
        /// Line value is the actual data
        /// </summary>
        DataType
    }

    public enum GedcomCharset
    {
        Unknown,
        Ansel,
        Ansi,
        Ascii,
        UTF8,
        UTF16BE,
        UTF16LE,
        UTF32BE,
        UTF32LE,
        UnSupported
    };

    /// <summary>
    /// GedcomParser is responsible for parsing GEDCOM files.
    /// This class implements GEDCOM 5.5 grammar rules.
    /// This is probably not the class you want to use unless writing a
    /// validator application.  GedcomRecordReader makes use of this
    /// class for building up a GedcomDatabase.
    /// </summary>
    public class GedcomParser
    {

        // arbitary magic max level number
        private const int MaxLevel = 99;
        private const int MaxXRefLength = 22;

        private GedcomCharset _charset = GedcomCharset.Unknown;

        private static string[] ErrorStrings = new string[]
        {
            "No Error",

            "Level expected but not found",
            "Level needs trailing delimeter",
            "Level is invalid",

            "Xref id needs trailing delimeter",
            "Xref too long",

            "Tag expected",
            "Tag needs trailing delimeter or newline",

            "Line value expected",
            "Line value needs trailing newline",
            "Line value invalid",

            "Unknown Error"
        };

        private GedcomState _State = GedcomState.Level;
        private GedcomErrorState _Error = GedcomErrorState.NoError;
        private int _Level = -1;
        private string _XrefID = string.Empty;
        private string _Tag = string.Empty;
        private string _LineValue = string.Empty;
        private GedcomLineValueType _LineValueType = GedcomLineValueType.NoType;

        private int _previousLevel = -1;
        private string _previousTag = string.Empty;

        static char[] ValidOtherChar = new char[] {
            '!', '\"', '$', '%', '&', '\'', '(',
            ')', '*', '+', ',', '-', '.', '/', ':',
            ';', '<', '=', '>', '?', '[', ']', '\\',
            '|', '^', '`', '{', '|', '}', '~'
        };

        private Utility.IndexedKeyCollection _tagCollection;
        private Utility.IndexedKeyCollection _xrefCollection;

        private bool _ignoreInvalidDelim = false;
        private bool _ignoreMissingTerms = false;
        private bool _applyConcContOnNewLineHack = false;
        private bool _allowTabs = false;
        private bool _allowLineTabs = false;
        private bool _allowInformationSeparatorOne = false;
        private bool _allowHyphenOrUnderscoreInTag = false;




        /// <summary>
        /// Create a GEDCOM parser.
        /// </summary>
        public GedcomParser()
        {
            ResetParseState();
        }




        /// <value>
        /// The current state the parser is in.
        /// </value>
        public GedcomState State
        {
            get { return _State; }
        }

        /// <value>
        /// The error state, if any, the parser is in
        /// </value>
        public GedcomErrorState ErrorState
        {
            get { return _Error; }
            set { _Error = value; }
        }

        /// <value>
        /// The level of the current GEDCOM tag
        /// </value>
        public int Level
        {
            get { return _Level; }
        }

        /// <value>
        /// The Xref of the current GEDCOM tag (if any)
        /// </value>
        public string XrefID
        {
            get { return _XrefID; }
        }

        /// <value>
        /// The current GEDCOM tag
        /// </value>
        public string Tag
        {
            get { return _Tag; }
        }

        /// <value>
        /// The value of the current GEDCOM tag, or a pointer to
        /// another record
        /// </value>
        public string LineValue
        {
            get { return _LineValue; }
        }

        /// <value>
        /// The type of the line value, data or pointer.
        /// </value>
        public GedcomLineValueType LineValueType
        {
            get { return _LineValueType; }
        }

        /// <value>
        /// A collection of xrefs used in the gedcom file.
        /// This is used as a cache to lower memory consumption.
        /// If not set one will be created when parsing.
        /// </value>
        public Utility.IndexedKeyCollection TagCollection
        {
            get { return _tagCollection; }
            set { _tagCollection = value; }
        }

        /// <value>
        /// A collection of xrefs used in the gedcom file.
        /// This is used as a cache to lower memory consumption.
        /// Setting this to an XRefIndexedKeyCollection will
        /// allow on the fly replacement of xrefs.
        /// If not set one will be created when parsing.
        /// </value>
        public Utility.IndexedKeyCollection XrefCollection
        {
            get { return _xrefCollection; }
            set { _xrefCollection = value; }
        }

        public GedcomCharset Charset
        {
            get { return _charset; }
            set { _charset = value; }
        }

        //// <value>
        /// If set to true invalid GEDCOM files that use multiple spaces
        /// to separate level / xrefid / tags will be processed without
        /// generating an error.  royal.ged from Gedcom.pm (http://www.pjcj.net)
        /// needs this as the tags are indented presumably for readability
        /// </value>
        public bool IgnoreInvalidDelim
        {
            get { return _ignoreInvalidDelim; }
            set { _ignoreInvalidDelim = value; }
        }

        //// <value>
        /// If set to true GEDCOM lines that do not contain a terminator
        /// on tags / line values will not cause the parser to generate an error.
        /// </value>
        public bool IgnoreMissingTerms
        {
            get { return _ignoreMissingTerms; }
            set { _ignoreMissingTerms = value; }
        }

        //// <value>
        /// Some broken apps (Reunion for instance) leave newlines in
        /// CONT (and maybe CONC) values, leading to broken GEDCOM.
        /// When set to true the parser will deal with this by converting
        /// the broken line to another CONC
        /// </value>
        public bool ApplyConcContOnNewLineHack
        {
            get { return _applyConcContOnNewLineHack; }
            set { _applyConcContOnNewLineHack = value; }
        }

        /// <summary>
        /// Allow tabs as OtherChar
        /// </summary>
        public bool AllowTabs
        {
            get { return _allowTabs; }
            set { _allowTabs = value; }
        }

        /// <summary>
        /// Allow line tabs as OtherChar
        /// </summary>
        public bool AllowLineTabs
        {
            get { return _allowLineTabs; }
            set { _allowLineTabs = value; }
        }

        /// <summary>
        /// Allow Information Separator One (0x1f) as OtherChar
        /// </summary>
        public bool AllowInformationSeparatorOne
        {
            get { return _allowInformationSeparatorOne; }
            set { _allowInformationSeparatorOne = value; }
        }

        /// <summary>
        /// Allow - or _ in tag names
        // </summary>
        public bool AllowHyphenOrUnderscoreInTag
        {
            get { return _allowHyphenOrUnderscoreInTag; }
            set { _allowHyphenOrUnderscoreInTag = value; }
        }




        /// <summary>
        /// Fired if the parser encounters an error 
        /// </summary>
        public event EventHandler ParserError;

        /// <summary>
        /// Fired whenever a full GEDCOM line has been parsed
        /// </summary>
        public event EventHandler TagFound;




        private static bool IsAlpha(char c)
        {
            return (Char.IsLetter(c) || c == '_');
        }

        private static bool IsDelim(char c)
        {
            return (c == ' ');
        }

        private static bool IsDigit(char c)
        {
            return (Char.IsDigit(c));
        }

        private static bool IsAlphaNum(char c)
        {
            bool ret = IsAlpha(c);
            if (!ret)
            {
                ret = IsDigit(c);
            }

            return ret;
        }

        private bool IsOtherChar(char c)
        {
            // input to GedcomParser should be unicode so anything > 0x80 is ok as other char?
            // big hack breaking the strictness of the parser but works
            // should be >= 0x80 <= 0xfe
            bool ret = (c >= 0x80);

            ret |= (AllowTabs && c == 0x09);
            ret |= (AllowInformationSeparatorOne && c == 0x1f);
            ret |= (AllowLineTabs && c == 0x0b);

            if (!ret && c != ' ')
            {
                for (int i = 0; i < ValidOtherChar.Length; i++)
                {
                    if (c == ValidOtherChar[i])
                    {
                        ret = true;
                        break;
                    }
                }
            }

            return ret;
        }

        private bool IsAnyChar(string data, int i)
        {
            bool ret = false;
            char c = data[i];

            ret = IsAlpha(c);
            if (!ret)
            {
                ret = IsDigit(c);
            }
            if (!ret)
            {
                ret = IsOtherChar(c);
            }
            if ((!ret) && (c == '#' || c == ' '))
            {
                ret = true;
            }
            if ((!ret) && c == '@')
            {
                if (((i < data.Length - 1) && data[i + 1] == '@') || (i > 0 && data[i - 1] == '@'))
                {
                    ret = true;
                }
            }

            return ret;
        }

        private bool IsEscapeText(string data, int i)
        {
            return IsAnyChar(data, i);
        }

        private bool IsEscape(string data, int i)
        {
            bool ret = false;

            if (data[i] == '@' && (i < data.Length - 1) && data[i + 1] == '#')
            {
                i += 2;

                while (i < data.Length && (data[i] != '@'))
                {
                    if (!IsEscapeText(data, i))
                    {
                        break;
                    }
                    i++;
                }
                if (data[i] == '@')
                {
                    ret = true;
                }
            }

            return ret;
        }

        private bool IsNonAt(char c)
        {
            bool ret = IsAlpha(c);
            if (!ret)
            {
                ret = IsDigit(c);
            }
            if (!ret)
            {
                ret = IsOtherChar(c);
            }
            if ((!ret) && (c == ' ' || c == '#'))
            {
                ret = true;
            }

            return ret;
        }

        private bool IsPointerChar(char c)
        {
            return IsNonAt(c);
        }

        private bool IsPointerString(char c)
        {
            return IsPointerChar(c);
        }

        private bool IsPointer(string data, int i)
        {
            bool ret = false;

            if (!IsEscape(data, i))
            {
                if (data[i] == '@')
                {
                    i++;
                    while (i < data.Length && data[i] != '@')
                    {
                        char c = data[i];
                        if (!(IsAlphaNum(c) || IsPointerString(c)))
                        {
                            break;
                        }
                        i++;
                    }
                    // FIXME: strictly speaking this is an error if
                    // this condition isn't met
                    if (i < data.Length && data[i] == '@')
                    {
                        ret = true;
                    }
                }
            }

            return ret;
        }

        private static bool IsTerminator(char c)
        {
            return (c == '\r' || c == '\n');
        }

        private bool IsXrefID(string data, int i)
        {
            return IsPointer(data, i);
        }

        /// <summary>
        /// Parses the given data, which should be 1 or more lines, multiple
        /// calls can be made with multiple lines.
        /// Events are triggered upon reading a line, or on an error.
        /// If TagCollection and XrefTagCollection haven't been set
        /// prior to calling default IndexedKeyCollection objects will be
        /// used.  To support replacing XRefs you need to set XrefTagCollection
        /// to an instance of XRefIndexedKeyCollection before calling. 
        /// </summary>
        /// <param name="data">Data to parse, expected to be unicode</param>
        public GedcomErrorState GedcomParse(string data)
        {
            _Error = GedcomErrorState.NoError;

            int i = 0;

            int len = data.Length;

            // Tags are always the same, data.Substring was allocating lots
            // of memory, instead use a special collection which matches via
            // array index, e.g   tagCollection[str, index, length] to avoid
            // the extra allocations, and caches the resulting string for
            // use again without having to substring
            if (_tagCollection == null)
            {
                _tagCollection = new Utility.IndexedKeyCollection();
            }

            // same for Xrefs
            if (_xrefCollection == null)
            {
                _xrefCollection = new Utility.IndexedKeyCollection();
            }

            while (i < len)
            {
                int temp = i;

                switch (_State)
                {
                    case GedcomState.Level:
                        // eat up leading white space
                        while (temp < len && char.IsWhiteSpace(data[temp]))
                        {
                            temp++;
                        }

                        bool hadLevel = false;
                        int lvl = 0;
                        while (temp < len && IsDigit(data[temp]))
                        {
                            hadLevel = true;
                            lvl *= 10;
                            lvl += (((int)data[temp++]) - (int)'0');
                        }

                        // possible we had data after eating white space
                        // but that it wasn't a digit
                        if (!hadLevel)
                        {
                            if (ApplyConcContOnNewLineHack &&
                                (_previousTag == "CONC" || _previousTag == "CONT"))
                            {
                                _Level = _previousLevel;
                                _Tag = "CONC";
                                _State = GedcomState.LineValue;
                            }
                            else
                            {
                                _Error = GedcomErrorState.LevelExpected;
                            }
                        }
                        else if (temp == i)
                        {
                            if (!char.IsWhiteSpace(data[i]))
                            {
                                _Error = GedcomErrorState.LevelExpected;
                            }
                            else
                            {
                                i++;
                            }
                        }
                        else
                        {
                            _Level = lvl;

                            if (_Level > MaxLevel)
                            {
                                _Error = GedcomErrorState.LevelInvalid;
                                _Level = -1;
                            }
                            else
                            {
                                i = temp;
                            }
                        }

                        // move to next state if we have a level
                        // and we are still in a level state (may not be
                        // if we have some hacks active)
                        if (_Level != -1 && _State == GedcomState.Level)
                        {
                            if (IsDelim(data[i]))
                            {
                                i++;
                                if (IgnoreInvalidDelim)
                                {
                                    while (i < len && IsDelim(data[i]))
                                    {
                                        i++;
                                    }
                                    _State = GedcomState.XrefID;
                                }
                                else if (IsDelim(data[i]))
                                {
                                    _Error = GedcomErrorState.InvalidDelim;
                                }
                                else
                                {
                                    _State = GedcomState.XrefID;
                                }
                            }
                            else
                            {
                                _Error = GedcomErrorState.LevelMissingDelim;
                            }
                        }

                        break;

                    case GedcomState.XrefID:

                        // no optional xref id just move to next state
                        // otherwise extract pointer

                        if (IsXrefID(data, temp))
                        {
                            // bypass first @
                            i++;
                            temp = i;

                            while (temp < len && data[temp] != '@')
                            {
                                temp++;
                            }

                            if ((temp - i) > MaxXRefLength)
                            {
                                _Error = GedcomErrorState.XrefIDTooLong;
                            }
                            else
                            {
                                _XrefID = _xrefCollection[data, i, temp - i];

                                i = temp + 1;

                                if (IsDelim(data[i]))
                                {
                                    i++;
                                    if (IgnoreInvalidDelim)
                                    {
                                        while (i < len && IsDelim(data[i]))
                                        {
                                            i++;
                                        }
                                        _State = GedcomState.Tag;
                                    }
                                    else if (IsDelim(data[i]))
                                    {
                                        _Error = GedcomErrorState.InvalidDelim;
                                    }
                                    else
                                    {
                                        _State = GedcomState.Tag;
                                    }
                                }
                                else
                                {
                                    _Error = GedcomErrorState.XrefIDMissingDelim;
                                }
                            }
                        }
                        else
                        {
                            _State = GedcomState.Tag;
                        }

                        break;

                    case GedcomState.Tag:
                        while (temp < len &&
                               (IsAlphaNum(data[temp]) ||
                                (AllowHyphenOrUnderscoreInTag &&
                                 (data[temp] == '-' || data[temp] == '_'))))
                        {
                            temp++;
                        }

                        if (temp == i)
                        {
                            _Error = GedcomErrorState.TagExpected;
                        }
                        else
                        {
                            _Tag = _tagCollection[data, i, temp - i];

                            i = temp;
                        }

                        if (_Tag != string.Empty)
                        {
                            if (_Tag == "TRLR" && i == len)
                            {
                                FoundTag();
                            }
                            else
                            {
                                if (i < len && IsDelim(data[i]))
                                {
                                    i++;

                                    _State = GedcomState.LineValue;
                                }
                                // not else if so we can handle tags with a trailing space but no line value
                                if (i == len || IsTerminator(data[i]))
                                {
                                    FoundTag();

                                    while (i < len && IsTerminator(data[i]))
                                    {
                                        i++;
                                    }
                                }
                                else if (_State != GedcomState.LineValue && !IgnoreMissingTerms)
                                {
                                    _Error = GedcomErrorState.TagMissingDelimOrTerm;
                                }
                            }
                        }

                        break;

                    case GedcomState.LineValue:
                        if (IsPointer(data, temp))
                        {
                            // bypass first @
                            i++;
                            temp = i;

                            while (temp < len && data[temp] != '@')
                            {
                                temp++;
                            }

                            if ((temp - i) > 0)
                            {
                                _LineValue = _xrefCollection[data, i, temp - i];
                                i = temp + 1;
                                _LineValueType = GedcomLineValueType.PointerType;
                            }

                            // GEDCOM only allows a single XREF for a pointer
                            // Genopro ignores this and puts a comma separated
                            // list of XREFs in the mess it pretends is GEDCOM.
                            // This causes us to get stuck in the LineValue state
                            // (this could of cause happen with anything after the
                            //  pointer)
                            if (i < len)
                            {
                                // we will allow white space, but nothing else
                                while (i < len && IsDelim(data[i]))
                                {
                                    i++;
                                }

                                if (i < len && !IsTerminator(data[i]))
                                {
                                    _Error = GedcomErrorState.LineValueInvalid;
                                }
                            }
                        }
                        else
                        {
                            while (_Error == GedcomErrorState.NoError && _LineValue == string.Empty)
                            {
                                if (temp < len && IsAnyChar(data, temp))
                                {
                                    temp++;
                                }
                                else if (temp < len && IsEscape(data, temp))
                                {
                                    // bypass @#

                                    temp += 2;

                                    while (temp < len && data[temp] != '@')
                                    {
                                        temp++;
                                    }
                                    temp++;
                                }
                                // hack for presidents.ged, email address
                                // is used in PHON on line 13 with a single @
                                // this isn't valid GEDCOM
                                // Should be escaped as @@ but handle it anyway
                                // Same thing occurs in user supplied file TOUT200801_unicode.ged
                                // with RELA @INDI:BAPM
                                else if (temp < len && data[temp] == '@')
                                {
                                    temp++;
                                }
                                else if (temp != i)
                                {
                                    if ((temp < len) && !IsTerminator(data[temp]))
                                    {
                                        _Error = GedcomErrorState.LineValueInvalid;
                                    }
                                    else
                                    {
                                        temp = Math.Min(temp, len);
                                        string dup = data.Substring(i, temp - i);
                                        // unescape @@ 
                                        _LineValue = dup.Replace("@@", "@");

                                        _LineValueType = GedcomLineValueType.DataType;
                                    }
                                    i = temp;
                                }
                                // FIXME: no line value, but have hit the terminator
                                // what should this be allowed for?
                                // Family Tree Maker outputs emtpy CONT (and CONC?)
                                else if (_Tag == "CONT" || _Tag == "CONC")
                                {
                                    _LineValue = " ";
                                }
                                else
                                {
                                    // hit a terminator
                                    break;
                                }
                            }
                        }

                        if (_Error == GedcomErrorState.NoError)
                        {
                            // can't use FoundTag here, may not want to reset
                            _previousLevel = _Level;
                            _previousTag = _Tag;

                            if (TagFound != null)
                            {
                                TagFound(this, EventArgs.Empty);
                            }
                            if (i == len || IsTerminator(data[i]))
                            {
                                while (i < len && IsTerminator(data[i]))
                                {
                                    i++;
                                }

                                // reset states
                                ResetParseState(false);
                            }
                            else if (!IgnoreMissingTerms)
                            {
                                _Error = GedcomErrorState.LineValueMissingTerm;
                            }
                        }
                        break;
                }

                if (_Error != GedcomErrorState.NoError)
                {
                    ParserError?.Invoke(this, EventArgs.Empty);
                    break;
                }
            }

            // reset parse status for more input
            ResetParseState(false);

            return _Error;
        }

        private void FoundTag()
        {
            // end of data + end tag
            TagFound?.Invoke(this, EventArgs.Empty);

            _previousLevel = _Level;
            _previousTag = _Tag;

            // reset states
            ResetParseState(false);
        }

        /// <summary>
        /// Resets the parser states, use this if the same GedcomParser is
        /// ready to parse a different gedcom file, or if recovering from
        /// an error the parser has hit.
        /// </summary>
        public void ResetParseState()
        {
            ResetParseState(true, GedcomCharset.Unknown);
        }

        private void ResetParseState(bool resetLevel)
        {
            ResetParseState(resetLevel, _charset);
        }

        private void ResetParseState(bool resetLevel, GedcomCharset charset)
        {
            _charset = charset;

            _XrefID = string.Empty;
            _Tag = string.Empty;
            _LineValue = string.Empty;
            _State = GedcomState.Level;
            if (resetLevel)
            {
                _Level = -1;
                _previousLevel = -1;
                _previousTag = string.Empty;
            }
            _LineValueType = GedcomLineValueType.NoType;
        }

        /// <summary>
        /// Obtain a human readable error message for the given error state
        /// </summary>
        /// <param name="state">The error state</param>
        public static string GedcomErrorString(GedcomErrorState state)
        {
            return ErrorStrings[(int)state];
        }
    }
}
