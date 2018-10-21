// <copyright file="Util.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>
// <author> Copyright (C) 2007 David A Knight david@ritter.demon.co.uk </author>
// <author> Copyright (C) 2016 Ryan O'Neill r@genegenie.com </author>

namespace GeneGenie.Gedcom
{
    using System;
    using System.IO;
    using System.Text;

    /// <summary>
    /// TODO: Doc
    /// </summary>
    public static class Util
    {
        private static readonly string[] NewLineArray = { Environment.NewLine };

        /// <summary>
        /// Generates a soundex string for the passed value.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns>A string of characters representing the soundex value.</returns>
        public static string GenerateSoundex(string str)
        {
            StringBuilder sb = new StringBuilder(str.Length);

            if (!string.IsNullOrEmpty(str))
            {
                sb.Append(char.ToLower(str[0]));

                for (int i = 1; i < str.Length; i++)
                {
                    sb.Append(SoundExEncodeChar(str[i]));
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Encodes the passed character to it's soundex value.
        /// </summary>
        /// <param name="c">The c.</param>
        /// <returns>The soundex character (as a string).</returns>
        public static string SoundExEncodeChar(char c)
        {
            switch (char.ToLower(c))
            {
                case 'b':
                case 'f':
                case 'p':
                case 'v':
                    return "1";
                case 'c':
                case 'g':
                case 'j':
                case 'k':
                case 'q':
                case 's':
                case 'x':
                case 'z':
                    return "2";
                case 'd':
                case 't':
                    return "3";
                case 'l':
                    return "4";
                case 'm':
                case 'n':
                    return "5";
                case 'r':
                    return "6";
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Escapes "at" sign.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns>
        /// The input string with the @ symbol escaped, or the original string if no @ symbol present.
        /// </returns>
        public static string EscapeAtSign(string str)
        {
            if (str.IndexOf("@") != -1)
            {
                str = str.Replace("@", "@@");
            }

            return str;
        }

        /// <summary>
        /// Returns the position of the escaped character in the passed string.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns>
        /// A count of the total number of escaped characters (the "at" sign) found in the passed string, or 0 if none.
        /// </returns>
        public static int EscapedAtLength(string str)
        {
            int i = 0;
            int count = 0;

            while (i < str.Length && i != -1)
            {
                i = str.IndexOf("@", i);
                if (i != -1)
                {
                    i++;
                    count++;
                }
            }

            return count;
        }

        /// <summary>
        /// Splits the line text.
        /// </summary>
        /// <param name="sw">The writer.</param>
        /// <param name="text">The text.</param>
        /// <param name="level">The level.</param>
        /// <param name="maxLen">The maximum length.</param>
        public static void SplitLineText(TextWriter sw, string text, int level, int maxLen)
        {
            SplitLineText(sw, text, level, maxLen, int.MaxValue, false);
        }

        /// <summary>
        /// Splits the line text.
        /// </summary>
        /// <param name="sw">The writer.</param>
        /// <param name="text">The text.</param>
        /// <param name="level">The level.</param>
        /// <param name="maxLen">The maximum length.</param>
        /// <param name="maxSplits">The maximum splits.</param>
        /// <param name="cont">if set to <c>true</c> [cont].</param>
        public static void SplitLineText(TextWriter sw, string text, int level, int maxLen, int maxSplits, bool cont)
        {
            string line = text.Replace("@", "@@");

            string[] lines = line.Split(NewLineArray, StringSplitOptions.None);
            bool first = true;

            string levelPlusOne = null;

            foreach (string l in lines)
            {
                if (first)
                {
                    if (l.Length <= maxLen)
                    {
                        sw.Write(l);
                    }
                    else
                    {
                        SplitText(sw, l, level + 1, maxLen, maxSplits, cont);
                    }

                    first = false;
                }
                else
                {
                    if (levelPlusOne == null)
                    {
                        levelPlusOne = (level + 1).ToString();
                    }

                    sw.Write(Environment.NewLine);
                    sw.Write(levelPlusOne);
                    sw.Write(" CONT ");
                    if (l.Length <= maxLen)
                    {
                        sw.Write(l);
                    }
                    else
                    {
                        SplitText(sw, l, level + 1, maxLen, maxSplits, cont);
                    }
                }
            }
        }

        /// <summary>
        /// Splits the text.
        /// </summary>
        /// <param name="sw">The writer.</param>
        /// <param name="line">The line.</param>
        /// <param name="level">The level.</param>
        /// <param name="maxLen">The maximum length.</param>
        public static void SplitText(TextWriter sw, string line, int level, int maxLen)
        {
            SplitText(sw, line, level, maxLen, int.MaxValue, false);
        }

        // TODO: this is potentially bad, lots of substrings, only potentially though, large notes for example
        // in practice probably not that bad.

        /// <summary>
        /// Splits the text.
        /// </summary>
        /// <param name="sw">The writer.</param>
        /// <param name="line">The line.</param>
        /// <param name="level">The level.</param>
        /// <param name="maxLen">The maximum length.</param>
        /// <param name="maxSplits">The maximum splits.</param>
        /// <param name="cont">if set to <c>true</c> [cont].</param>
        public static void SplitText(TextWriter sw, string line, int level, int maxLen, int maxSplits, bool cont)
        {
            bool firstSplit = true;
            int splits = 0;

            string splitTag = " CONC ";
            if (cont)
            {
                splitTag = " CONT ";
            }

            string levelStr = null;

            while (line.Length > maxLen)
            {
                int space = line.LastIndexOf(" ", maxLen - 1, maxLen);

                // can't split it, just output the line,
                // TODO: this becomes invalid gedcom, should split in a word
                // according to the spec
                if (space == -1)
                {
                    if (firstSplit)
                    {
                        sw.Write(line);
                        firstSplit = false;
                    }
                    else
                    {
                        if (levelStr == null)
                        {
                            levelStr = level.ToString();
                        }

                        sw.Write(Environment.NewLine);
                        sw.Write(levelStr);
                        sw.Write(splitTag);
                        sw.Write(line);
                    }

                    line = string.Empty;

                    splits++;

                    break;
                }
                else
                {
                    string tmp = line.Substring(0, space);
                    if (firstSplit)
                    {
                        sw.Write(tmp);
                        firstSplit = false;
                    }
                    else
                    {
                        if (levelStr == null)
                        {
                            levelStr = level.ToString();
                        }

                        sw.Write(Environment.NewLine);
                        sw.Write(levelStr);
                        sw.Write(splitTag);
                        sw.Write(tmp);
                    }

                    line = line.Substring(space + 1);

                    splits++;
                }

                if (splits == maxSplits)
                {
                    System.Diagnostics.Debug.WriteLine("Text too long in SplitText, truncating");
                    break;
                }
            }

            if (line != string.Empty)
            {
                if (levelStr == null)
                {
                    levelStr = level.ToString();
                }

                sw.Write(Environment.NewLine);
                sw.Write(levelStr);
                sw.Write(" CONC ");
                sw.Write(line);
            }
        }
    }
}
