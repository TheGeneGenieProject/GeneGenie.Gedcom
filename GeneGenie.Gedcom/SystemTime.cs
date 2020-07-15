// <copyright file="SystemTime.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>
// <author> Copyright (C) 2020 Ryan O'Neill r@genegenie.com </author>

namespace GeneGenie.Gedcom
{
    using System;

    /// <summary>
    /// Used to wrap DateTime.Now() so that it can be replaced for unit testing.
    /// </summary>
    public static class SystemTime
    {
        private static Func<DateTime> now = () => DateTime.Now;

        /// <summary>
        /// Gets the current time or the time under test for unit tests.
        /// </summary>
        public static DateTime Now
        {
            get
            {
                return now();
            }
        }

        /// <summary>
        /// Used to set the time to return when SystemTime.Now() is called.
        /// </summary>
        /// <param name="dateTimeNow">The time you want to return for the unit test.</param>
        public static void SetDateTime(DateTime dateTimeNow)
        {
            now = () => dateTimeNow;
        }

        /// <summary>
        /// Resets SystemTime.Now() to return the real time via DateTime.Now.
        /// </summary>
        public static void ResetDateTime()
        {
            now = () => DateTime.Now;
        }
    }
}
