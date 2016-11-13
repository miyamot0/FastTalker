//----------------------------------------------------------------------------------------------
// <copyright file="Storage.cs" 
// Copyright November 6, 2016 Shawn Gilroy
//
// This file is part of Cross Platform Communication App
//
// Cross Platform Communication App is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, version 3.
//
// Cross Platform Communication App is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Cross Platform Communication App.  If not, see http://www.gnu.org/licenses/. 
// </copyright>
//
// <summary>
// The Cross Platform Communication App is a tool to assist clinicans and researchers in the treatment of communication disorders.
// 
// Email: shawn(dot)gilroy(at)temple.edu
//
// </summary>
//----------------------------------------------------------------------------------------------

using System.Collections.Generic;

namespace SGDWithCocos.Models
{
    /// <summary>
    /// Class for serialization/parsing if Icon data
    /// </summary>
    public class Storage
    {
        public string Name { get; set; }
        public List<string> Tags { get; set; }
        public int Mature { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public Storage()
        {

        }
    }

    /// <summary>
    /// Container for serialized objects
    /// </summary>
    public class StorageContainer
    {
        public List<Storage> StoredIcons { get; set; }
    }
}
