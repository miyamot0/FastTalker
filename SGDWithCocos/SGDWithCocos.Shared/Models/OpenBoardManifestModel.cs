//----------------------------------------------------------------------------------------------
// <copyright file="OpenBoardManifestModel.cs" 
// Copyright November 6, 2016 Shawn Gilroy
//
// This file is part of Fast Talker
//
// Fast Talker is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, version 3.
//
// Fast Talker is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Fast Talker.  If not, see http://www.gnu.org/licenses/. 
// </copyright>
//
// <summary>
// The Fast Talker is a tool to assist clinicans and researchers in the treatment of communication disorders.
// 
// Email: shawn(dot)gilroy(at)temple.edu
//
// </summary>
//----------------------------------------------------------------------------------------------

using System.Collections.Generic;

namespace SGDWithCocos.Models
{
    /// <summary>
    /// Open board manifest model
    /// </summary>
    public class OpenBoardManifestModel
    {
        public string format { get; set; }
        public string root { get; set; }
        public OpenBoardManifestPathModel paths { get; set; }
    }

    /// <summary>
    /// Open board paths
    /// </summary>
    public class OpenBoardManifestPathModel
    {
        public Dictionary<string, string> images { get; set; }
        public Dictionary<string, string> sounds { get; set; }
        public Dictionary<string, string> boards { get; set; }
    }
}
