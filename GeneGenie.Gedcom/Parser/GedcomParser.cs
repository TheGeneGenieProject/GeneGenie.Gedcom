// <copyright file="GedcomParser.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>
// <author> Copyright (C) 2016 Ryan O'Neill r@genegenie.com </author>
// <author> Copyright (C) 2007 David A Knight david@ritter.demon.co.uk </author>

namespace GeneGenie.Gedcom.Parser
{
    using System;
    using GeneGenie.Gedcom.Enums;
    using GeneGenie.Gedcom.Helpers;

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

        private static char[] validOtherChar = new char[]
        {
            '!', '\"', '$', '%', '&', '\'', '(',
            ')', '*', '+', ',', '-', '.', '/', ':',
            ';', '<', '=', '>', '?', '[', ']', '\\',
            '|', '^', '`', '{', '|', '}', '~',
        };

        private int previousLevel = -1;
        private string previousTag = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomParser"/> class.
        /// </summary>
        public GedcomParser()
        {
            ResetParseState();
        }

        /// <summary>
        /// Fired if the parser encounters an error.
        /// </summary>
        public event EventHandler ParserError;

        /// <summary>
        /// Fired whenever a full GEDCOM line has been parsed.
        /// </summary>
        public event EventHandler TagFound;

        /// <summary>
        /// Gets the current state the parser is in.
        /// </summary>
        public GedcomState State { get; private set; } = GedcomState.Level;

        /// <summary>
        /// Gets or sets the error state, if any, the parser is in.
        /// </summary>
        public GedcomErrorState ErrorState { get; set; } = GedcomErrorState.NoError;

        /// <summary>
        /// Gets the level of the current GEDCOM tag.
        /// </summary>
        public int Level { get; private set; } = -1;

        /// <summary>
        /// Gets the Xref of the current GEDCOM tag (if any).
        /// </summary>
        public string XrefID { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the current GEDCOM tag.
        /// </summary>
        public string Tag { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the value of the current GEDCOM tag, or a pointer to
        /// another record.
        /// </summary>
        public string LineValue { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the type of the line value, data or pointer.
        /// </summary>
        public GedcomLineValueType LineValueType { get; private set; } = GedcomLineValueType.NoType;

        /// <summary>
        /// Gets or sets the collection of xrefs used in the gedcom file.
        /// This is used as a cache to lower memory consumption.
        /// If not set one will be created when parsing.
        /// </summary>
        public IndexedKeyCollection TagCollection { get; set; }

        /// <summary>
        /// Gets or sets the collection of xrefs used in the gedcom file.
        /// This is used as a cache to lower memory consumption.
        /// Setting this to an XRefIndexedKeyCollection will
        /// allow on the fly replacement of xrefs.
        /// If not set one will be created when parsing.
        /// </summary>
        public IndexedKeyCollection XrefCollection { get; set; }

        /// <summary>
        /// Gets or sets the character set used to encode the GEDCOM file.
        /// </summary>
        public GedcomCharset Charset { get; set; } = GedcomCharset.Unknown;

        /// <summary>
        /// Gets or sets a value indicating whether invalid delimiters cause errors.
        /// If set to true invalid GEDCOM files that use multiple spaces
        /// to separate level / xrefid / tags will be processed without
        /// generating an error.  royal.ged from Gedcom.pm (http://www.pjcj.net)
        /// needs this as the tags are indented presumably for readability.
        /// </summary>
        public bool IgnoreInvalidDelim { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether missing terminators cause an error.
        /// If set to true GEDCOM lines that do not contain a terminator
        /// on tags / line values will not cause the parser to generate an error.
        /// </summary>
        public bool IgnoreMissingTerms { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether to leave newlines in.
        /// Some broken apps (Reunion for instance) omit
        /// CONT (and maybe CONC) values, leading to broken GEDCOM.
        /// When set to true the parser will deal with this by converting
        /// the broken line to another CONC.
        /// </summary>
        public bool ApplyConcContOnNewLineHack { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether to allow tabs as OtherChar.
        /// </summary>
        public bool AllowTabs { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether to allow line tabs as OtherChar.
        /// </summary>
        public bool AllowLineTabs { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether to allow Information Separator One (0x1f) as OtherChar.
        /// </summary>
        public bool AllowInformationSeparatorOne { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether to allow - or _ in tag names.
        /// </summary>
        public bool AllowHyphenOrUnderscoreInTag { get; set; } = false;

        /// <summary>
        /// Obtain a human readable error message for the given error state.
        /// </summary>
        /// <param name="state">The error state.</param>
        /// <returns>A friendlier string for the error.</returns>
        public static string GedcomErrorString(GedcomErrorState state)
        {
            return StaticData.ParseErrorDescriptions[(int)state];
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
        /// <param name="data">Data to parse, expected to be unicode.</param>
        /// <returns>The last error encountered.</returns>
        public GedcomErrorState GedcomParse(string data)
        {
            ErrorState = GedcomErrorState.NoError;

            int i = 0;

            int len = data.Length;

            // Tags are always the same, data.Substring was allocating lots
            // of memory, instead use a special collection which matches via
            // array index, e.g   tagCollection[str, index, length] to avoid
            // the extra allocations, and caches the resulting string for
            // use again without having to substring
            if (TagCollection == null)
            {
                TagCollection = new IndexedKeyCollection();
            }

            // same for Xrefs
            if (XrefCollection == null)
            {
                XrefCollection = new IndexedKeyCollection();
            }

            while (i < len)
            {
                int temp = i;

                switch (State)
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
                            lvl += data[temp++] - '0';
                        }

                        // possible we had data after eating white space
                        // but that it wasn't a digit
                        if (!hadLevel)
                        {
                            if (ApplyConcContOnNewLineHack &&
                                (previousTag == "CONC" || previousTag == "CONT"))
                            {
                                Level = previousLevel;
                                Tag = "CONC";
                                State = GedcomState.LineValue;
                            }
                            else
                            {
                                ErrorState = GedcomErrorState.LevelExpected;
                            }
                        }
                        else if (temp == i)
                        {
                            if (!char.IsWhiteSpace(data[i]))
                            {
                                ErrorState = GedcomErrorState.LevelExpected;
                            }
                            else
                            {
                                i++;
                            }
                        }
                        else
                        {
                            Level = lvl;

                            if (Level > MaxLevel)
                            {
                                ErrorState = GedcomErrorState.LevelInvalid;
                                Level = -1;
                            }
                            else
                            {
                                i = temp;
                            }
                        }

                        // move to next state if we have a level
                        // and we are still in a level state (may not be
                        // if we have some hacks active)
                        if (Level != -1 && State == GedcomState.Level)
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

                                    State = GedcomState.XrefID;
                                }
                                else if (IsDelim(data[i]))
                                {
                                    ErrorState = GedcomErrorState.InvalidDelim;
                                }
                                else
                                {
                                    State = GedcomState.XrefID;
                                }
                            }
                            else
                            {
                                ErrorState = GedcomErrorState.LevelMissingDelim;
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
                                ErrorState = GedcomErrorState.XrefIDTooLong;
                            }
                            else
                            {
                                XrefID = XrefCollection[data, i, temp - i];

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

                                        State = GedcomState.Tag;
                                    }
                                    else if (IsDelim(data[i]))
                                    {
                                        ErrorState = GedcomErrorState.InvalidDelim;
                                    }
                                    else
                                    {
                                        State = GedcomState.Tag;
                                    }
                                }
                                else
                                {
                                    ErrorState = GedcomErrorState.XrefIDMissingDelim;
                                }
                            }
                        }
                        else
                        {
                            State = GedcomState.Tag;
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
                            ErrorState = GedcomErrorState.TagExpected;
                        }
                        else
                        {
                            Tag = TagCollection[data, i, temp - i];

                            i = temp;
                        }

                        if (Tag != string.Empty)
                        {
                            if (Tag == "TRLR" && i == len)
                            {
                                FoundTag();
                            }
                            else
                            {
                                if (i < len && IsDelim(data[i]))
                                {
                                    i++;

                                    State = GedcomState.LineValue;
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
                                else if (State != GedcomState.LineValue && !IgnoreMissingTerms)
                                {
                                    ErrorState = GedcomErrorState.TagMissingDelimOrTerm;
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
                                LineValue = XrefCollection[data, i, temp - i];
                                i = temp + 1;
                                LineValueType = GedcomLineValueType.PointerType;
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
                                    ErrorState = GedcomErrorState.LineValueInvalid;
                                }
                            }
                        }
                        else
                        {
                            while (ErrorState == GedcomErrorState.NoError && LineValue == string.Empty)
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
                                        ErrorState = GedcomErrorState.LineValueInvalid;
                                    }
                                    else
                                    {
                                        temp = Math.Min(temp, len);
                                        string dup = data.Substring(i, temp - i);

                                        // unescape @@
                                        LineValue = dup.Replace("@@", "@");

                                        LineValueType = GedcomLineValueType.DataType;
                                    }

                                    i = temp;
                                }

                                // TODO: no line value, but have hit the terminator
                                // what should this be allowed for?
                                // Family Tree Maker outputs emtpy CONT (and CONC?)
                                else if (Tag == "CONT" || Tag == "CONC")
                                {
                                    LineValue = " ";
                                }
                                else
                                {
                                    // hit a terminator
                                    break;
                                }
                            }
                        }

                        if (ErrorState == GedcomErrorState.NoError)
                        {
                            // can't use FoundTag here, may not want to reset
                            previousLevel = Level;
                            previousTag = Tag;

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
                                ErrorState = GedcomErrorState.LineValueMissingTerm;
                            }
                        }

                        break;
                }

                if (ErrorState != GedcomErrorState.NoError)
                {
                    ParserError?.Invoke(this, EventArgs.Empty);
                    break;
                }
            }

            // reset parse status for more input
            ResetParseState(false);

            return ErrorState;
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

        private static bool IsAlpha(char c)
        {
            return char.IsLetter(c) || c == '_';
        }

        private static bool IsDelim(char c)
        {
            return c == ' ';
        }

        private static bool IsDigit(char c)
        {
            return char.IsDigit(c);
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

        private static bool IsTerminator(char c)
        {
            return c == '\r' || c == '\n';
        }

        private bool IsOtherChar(char c)
        {
            // input to GedcomParser should be unicode so anything > 0x80 is ok as other char?
            // big hack breaking the strictness of the parser but works
            // should be >= 0x80 <= 0xfe
            bool ret = c >= 0x80;

            ret |= AllowTabs && c == 0x09;
            ret |= AllowInformationSeparatorOne && c == 0x1f;
            ret |= AllowLineTabs && c == 0x0b;

            if (!ret && c != ' ')
            {
                for (int i = 0; i < validOtherChar.Length; i++)
                {
                    if (c == validOtherChar[i])
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

                    // TODO: strictly speaking this is an error if
                    // this condition isn't met
                    if (i < data.Length && data[i] == '@')
                    {
                        ret = true;
                    }
                }
            }

            return ret;
        }

        private bool IsXrefID(string data, int i)
        {
            return IsPointer(data, i);
        }

        private void FoundTag()
        {
            // end of data + end tag
            TagFound?.Invoke(this, EventArgs.Empty);

            previousLevel = Level;
            previousTag = Tag;

            // reset states
            ResetParseState(false);
        }

        private void ResetParseState(bool resetLevel)
        {
            ResetParseState(resetLevel, Charset);
        }

        private void ResetParseState(bool resetLevel, GedcomCharset charset)
        {
            Charset = charset;

            XrefID = string.Empty;
            Tag = string.Empty;
            LineValue = string.Empty;
            State = GedcomState.Level;
            if (resetLevel)
            {
                Level = -1;
                previousLevel = -1;
                previousTag = string.Empty;
            }

            LineValueType = GedcomLineValueType.NoType;
        }
    }
}
