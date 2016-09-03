/*
 *  $Id: Util.cs 200 2008-11-30 14:34:07Z davek $
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
    using System.IO;
    using System.Text;

    public static class Util
    {
        private static readonly char[] _newLineArray = Environment.NewLine.ToCharArray();

        public static string GenerateSoundex(string str)
        {
            StringBuilder sb = new StringBuilder(str.Length);

            if (!string.IsNullOrEmpty(str))
            {
                sb.Append(Char.ToLower(str[0]));

                for (int i = 1; i < str.Length; i++)
                {
                    sb.Append(SoundExEncodeChar(str[i]));
                }
            }

            return sb.ToString();
        }

        public static string SoundExEncodeChar(char c)
        {
            switch (Char.ToLower(c))
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

        public static string IntToString(int i)
        {
            bool neg = i < 0;
            i = Math.Abs(i);

            string ret;

            // quick hack, most of the time we will be < 10 for gedcom levels
            if (i < 10 && (!neg))
            {
                switch (i)
                {
                    case 1:
                        ret = "1";
                        break;
                    case 2:
                        ret = "2";
                        break;
                    case 3:
                        ret = "3";
                        break;
                    case 4:
                        ret = "4";
                        break;
                    case 5:
                        ret = "5";
                        break;
                    case 6:
                        ret = "6";
                        break;
                    case 7:
                        ret = "7";
                        break;
                    case 8:
                        ret = "8";
                        break;
                    case 9:
                        ret = "9";
                        break;
                    case 0:
                    default:
                        ret = "0";
                        break;
                }
            }
            else
            {
                int tmp = i;
                int digits = 1;
                while (((int)(tmp / 10)) > 0)
                {
                    digits++;
                    tmp = (int)(tmp / 10);
                }

                char[] buffer = null;

                if (neg)
                {
                    digits++;
                }

                buffer = new char[digits];

                while (digits > 0)
                {
                    digits--;
                    buffer[digits] = (char)(0x30 + (i % 10));
                    i = (int)(i / 10);
                }

                if (neg)
                {
                    buffer[0] = '-';
                }

                ret = new string(buffer);
            }

            return ret;
        }

        public static string EscapeAtSign(string str)
        {
            if (str.IndexOf("@") != -1)
            {
                str = str.Replace("@", "@@");
            }

            return str;
        }

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

        public static void SplitLineText(TextWriter sw, string text, int level, int maxLen)
        {
            SplitLineText(sw, text, level, maxLen, int.MaxValue, false);
        }

        public static void SplitLineText(TextWriter sw, string text, int level, int maxLen, int maxSplits, bool cont)
        {
            string line = text.Replace("@", "@@");

            string[] lines = line.Split(_newLineArray);
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
                        levelPlusOne = Util.IntToString(level + 1);
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

        public static void SplitText(TextWriter sw, string line, int level, int maxLen)
        {
            SplitText(sw, line, level, maxLen, int.MaxValue, false);
        }

        // FIXME: this is potentially bad, lots of substrings, only potentially though, large notes for example
        // in practice probably not that bad.
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
                // FIXME: this becomes invalid gedcom, should split in a word
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
                            levelStr = Util.IntToString(level);
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
                            levelStr = Util.IntToString(level);
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
                    levelStr = Util.IntToString(level);
                }

                sw.Write(Environment.NewLine);
                sw.Write(levelStr);
                sw.Write(" CONC ");
                sw.Write(line);
            }
        }
    }
}
