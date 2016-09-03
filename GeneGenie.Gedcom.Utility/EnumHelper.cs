/*
 *  $Id: EnumHelper.cs 199 2008-11-15 15:20:44Z davek $
 * 
 *  Copyright (C) 2008 [name of author]
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

namespace Utility
{
    using System;

    public static class EnumHelper
    {
        public static T Parse<T>(string val)
        {
            return Parse<T>(val, false);
        }

        public static T Parse<T>(string val, bool ignoreCase)
        {
            return (T)Enum.Parse(typeof(T), val, ignoreCase);
        }

        public static T Parse<T>(string val, bool ignoreCase, T defaultValue)
        {
            T ret = defaultValue;
            try
            {
                ret = (T)Enum.Parse(typeof(T), val, ignoreCase);
            }
            catch
            {
            }

            return ret;
        }
    }
}
