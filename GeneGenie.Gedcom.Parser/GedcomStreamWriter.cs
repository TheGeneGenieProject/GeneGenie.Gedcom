// <copyright file="GedcomStreamWriter.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>
// <author> Copyright (C) 2016 Ryan O'Neill r@genegenie.com </author>
// <author> Copyright (C) 2007-2008 David A Knight david@ritter.demon.co.uk </author>

namespace GeneGenie.Gedcom.Parser
{
    using System.IO;
    using System.Text;

    /// <summary>
    /// Stream writer for outputting GEDCOM files. Can be used to output to both memory and files.
    /// </summary>
    /// <seealso cref="System.IO.StreamWriter" />
    internal class GedcomStreamWriter : StreamWriter
    {
        // private int tabSize = 4;
        private string tab = "    ";

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomStreamWriter"/> class.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        public GedcomStreamWriter(Stream stream)
            : base(stream)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomStreamWriter"/> class.
        /// </summary>
        /// <param name="path">The complete file path to write to. <paramref name="path" /> can be a file name.</param>
        public GedcomStreamWriter(string path)
            : base(path)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomStreamWriter"/> class.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        /// <param name="encoding">The character encoding to use.</param>
        public GedcomStreamWriter(Stream stream, Encoding encoding)
            : base(stream, encoding)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomStreamWriter"/> class.
        /// </summary>
        /// <param name="path">The complete file path to write to.</param>
        /// <param name="append">true to append data to the file; false to overwrite the file. If the specified file does not exist, this parameter has no effect, and the constructor creates a new file.</param>
        public GedcomStreamWriter(string path, bool append)
            : base(path, append)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomStreamWriter" /> class.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <param name="bufferSize">The buffer size, in bytes.</param>
        /// <param name="leaveOpen">true to leave the stream open after the <see cref="T:System.IO.StreamWriter" /> object is disposed; otherwise, false.</param>
        public GedcomStreamWriter(Stream stream, Encoding encoding, int bufferSize, bool leaveOpen)
            : base(stream, encoding, bufferSize, leaveOpen)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomStreamWriter"/> class.
        /// </summary>
        /// <param name="path">The complete file path to write to.</param>
        /// <param name="append">true to append data to the file; false to overwrite the file. If the specified file does not exist, this parameter has no effect, and the constructor creates a new file.</param>
        /// <param name="encoding">The character encoding to use.</param>
        public GedcomStreamWriter(string path, bool append, Encoding encoding)
            : base(path, append, encoding)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomStreamWriter"/> class.
        /// </summary>
        /// <param name="path">The complete file path to write to.</param>
        /// <param name="append">true to append data to the file; false to overwrite the file. If the specified file does not exist, this parameter has no effect, and the constructor creates a new file.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <param name="bufferSize">The buffer size, in bytes.</param>
        public GedcomStreamWriter(string path, bool append, Encoding encoding, int bufferSize)
            : base(path, append, encoding, bufferSize)
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether to allow the ASCII information separator character to be written.
        /// </summary>
        public bool AllowInformationSeparatorOnSave { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to replace line tabs (ASCII vertical tabs) as spaces when written.
        /// </summary>
        public bool AllowLineTabsSave { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to replace tabs (ASCII horizontal tabs) as spaces when written.
        /// </summary>
        public bool AllowTabsSave { get; set; }

        /// <summary>
        /// Writes a character to the stream.
        /// </summary>
        /// <param name="value">The character to write to the stream.</param>
        public override void Write(char value)
        {
            if (!AllowInformationSeparatorOnSave && value == 0x1f)
            {
                base.Write(" ");
            }
            else if ((!AllowLineTabsSave && value == 0x0b) || (!AllowTabsSave && value == 0x09))
            {
                base.Write(tab);
            }
            else
            {
                base.Write(value);
            }
        }

        /// <summary>
        /// Writes a subarray of characters to the stream.
        /// </summary>
        /// <param name="buffer">A character array that contains the data to write.</param>
        /// <param name="index">The character position in the buffer at which to start reading data.</param>
        /// <param name="count">The maximum number of characters to write.</param>
        public override void Write(char[] buffer, int index, int count)
        {
            base.Write(buffer, index, count);
        }

        /// <summary>
        /// Writes the text representation of an object to the text string or stream by calling the ToString method on that object.
        /// </summary>
        /// <param name="value">The object to write.</param>
        public override void Write(object value)
        {
            Write(value.ToString());
        }

        /// <summary>
        /// Writes a string to the stream.
        /// </summary>
        /// <param name="value">The string to write to the stream. If <paramref name="value" /> is null, nothing is written.</param>
        public override void Write(string value)
        {
            string tmp = value;

            if (!AllowInformationSeparatorOnSave)
            {
                tmp = tmp.Replace("\x001f", " ");
            }

            if (!AllowLineTabsSave)
            {
                tmp = tmp.Replace("\x000b", tab);
            }

            if (!AllowTabsSave)
            {
                tmp = tmp.Replace("\x0009", tab);
            }

            base.Write(value);
        }

        /// <summary>
        /// Writes a formatted string to the text string or stream, using the same semantics as the <see cref="M:System.String.Format(System.String,System.Object)" /> method.
        /// </summary>
        /// <param name="format">A composite format string (see Remarks).</param>
        /// <param name="arg0">The object to format and write.</param>
        public override void Write(string format, object arg0)
        {
            Write(string.Format(format, arg0));
        }

        /// <summary>
        /// Writes a formatted string to the text string or stream, using the same semantics as the <see cref="M:System.String.Format(System.String,System.Object,System.Object)" /> method.
        /// </summary>
        /// <param name="format">A composite format string (see Remarks).</param>
        /// <param name="arg0">The first object to format and write.</param>
        /// <param name="arg1">The second object to format and write.</param>
        public override void Write(string format, object arg0, object arg1)
        {
            Write(string.Format(format, arg0, arg1));
        }

        /// <summary>
        /// Writes a formatted string to the text string or stream, using the same semantics as the <see cref="M:System.String.Format(System.String,System.Object,System.Object,System.Object)" /> method.
        /// </summary>
        /// <param name="format">A composite format string (see Remarks).</param>
        /// <param name="arg0">The first object to format and write.</param>
        /// <param name="arg1">The second object to format and write.</param>
        /// <param name="arg2">The third object to format and write.</param>
        public override void Write(string format, object arg0, object arg1, object arg2)
        {
            Write(string.Format(format, arg0, arg1, arg2));
        }

        /// <summary>
        /// Writes a character followed by a line terminator to the text string or stream.
        /// </summary>
        /// <param name="value">The character to write to the text stream.</param>
        public override void WriteLine(char value)
        {
            if (!AllowInformationSeparatorOnSave && value == 0x1f)
            {
                base.WriteLine(" ");
            }
            else if ((!AllowLineTabsSave && value == 0x0b) || (!AllowTabsSave && value == 0x09))
            {
                base.WriteLine(tab);
            }
            else
            {
                base.WriteLine(value);
            }
        }

        /// <summary>
        /// Writes a subarray of characters followed by a line terminator to the text string or stream.
        /// </summary>
        /// <param name="buffer">The character array from which data is read.</param>
        /// <param name="index">The character position in <paramref name="buffer" /> at which to start reading data.</param>
        /// <param name="count">The maximum number of characters to write.</param>
        public override void WriteLine(char[] buffer, int index, int count)
        {
            base.WriteLine(buffer, index, count);
        }

        /// <summary>
        /// Writes the text representation of an object by calling the ToString method on that object, followed by a line terminator to the text string or stream.
        /// </summary>
        /// <param name="value">The object to write. If <paramref name="value" /> is null, only the line terminator is written.</param>
        public override void WriteLine(object value)
        {
            WriteLine(value.ToString());
        }

        /// <summary>
        /// Writes a string followed by a line terminator to the text string or stream.
        /// </summary>
        /// <param name="value">The string to write. If <paramref name="value" /> is null, only the line terminator is written.</param>
        public override void WriteLine(string value)
        {
            string tmp = value;

            if (!AllowInformationSeparatorOnSave)
            {
                tmp = tmp.Replace("\x001f", " ");
            }

            if (!AllowLineTabsSave)
            {
                tmp = tmp.Replace("\x000b", tab);
            }

            if (!AllowTabsSave)
            {
                tmp = tmp.Replace("\x0009", tab);
            }

            base.WriteLine(tmp);
        }

        /// <summary>
        /// Writes a formatted string and a new line to the text string or stream, using the same semantics as the <see cref="M:System.String.Format(System.String,System.Object)" /> method.
        /// </summary>
        /// <param name="format">A composite format string (see Remarks).</param>
        /// <param name="arg0">The object to format and write.</param>
        public override void WriteLine(string format, object arg0)
        {
            WriteLine(string.Format(format, arg0));
        }

        /// <summary>
        /// Writes a formatted string and a new line to the text string or stream, using the same semantics as the <see cref="M:System.String.Format(System.String,System.Object,System.Object)" /> method.
        /// </summary>
        /// <param name="format">A composite format string (see Remarks).</param>
        /// <param name="arg0">The first object to format and write.</param>
        /// <param name="arg1">The second object to format and write.</param>
        public override void WriteLine(string format, object arg0, object arg1)
        {
            WriteLine(string.Format(format, arg0, arg1));
        }

        /// <summary>
        /// Writes out a formatted string and a new line, using the same semantics as <see cref="M:System.String.Format(System.String,System.Object)" />.
        /// </summary>
        /// <param name="format">A composite format string (see Remarks).</param>
        /// <param name="arg0">The first object to format and write.</param>
        /// <param name="arg1">The second object to format and write.</param>
        /// <param name="arg2">The third object to format and write.</param>
        public override void WriteLine(string format, object arg0, object arg1, object arg2)
        {
            WriteLine(string.Format(format, arg0, arg1, arg2));
        }
    }
}
