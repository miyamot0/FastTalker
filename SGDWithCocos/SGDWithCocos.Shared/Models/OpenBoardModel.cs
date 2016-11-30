//----------------------------------------------------------------------------------------------
// <copyright file="OpenBoardModel.cs" 
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
    /// Deserialize OBF, OBZ file
    /// </summary>
    public class OpenBoardModel
    {
        public string format { get; set; }
        public string id { get; set; }
        public string locale { get; set; }
        public string name { get; set; }
        public string description_html { get; set; }

        public OpenBoardModelGrid grid { get; set; }
        public List<OpenBoardModelButton> buttons { get; set; }
        public List<OpenBoardModelImage> images { get; set; }
        public List<OpenBoardModelSound> sounds { get; set; }
    }

    /// <summary>
    /// Deserialize Grid
    /// </summary>
    public class OpenBoardModelGrid
    {
        public string rows { get; set; }
        public string columns { get; set; }
        public List<string[]> order { get; set; }
    }

    /// <summary>
    /// Deserialize Buttons
    /// </summary>
    public class OpenBoardModelButton
    {
        public string id { get; set; }
        public string label { get; set; }
        public string image_id { get; set; }
        public string background_color { get; set; }
        public string border_color { get; set; }
        public OpenBoardModelBoards load_board { get; set; }
    }

    /// <summary>
    /// Deserialize Images 
    /// </summary>
    public class OpenBoardModelImage
    {
        public string id { get; set; }
        public string width { get; set; }
        public string height { get; set; }
        public string data { get; set; }

        public string url { get; set; }
        public string content_type { get; set; }

        public string ext_speaker_freshness { get; set; }

        public string path { get; set; }

        public OpenBoardModelLicense license { get; set; }
    }

    /// <summary>
    /// Extended OBF model for parsing of base64 through LINQ
    /// </summary>
    public class OpenBoardModelImageReference
    {
        public string id { get; set; }
        public string width { get; set; }
        public string height { get; set; }
        public string data { get; set; }

        public string url { get; set; }
        public string content_type { get; set; }

        public string ext_speaker_freshness { get; set; }

        public string path { get; set; }

        // New Here
        public string base64 { get; set; }

        public OpenBoardModelLicense license { get; set; }
    }

    /// <summary>
    /// Deserialize Sounds (not actively used)
    /// </summary>
    public class OpenBoardModelSound
    {
        public string id { get; set; }
        public string data { get; set; }

        public string url { get; set; }
        public string content_type { get; set; }

        public OpenBoardModelLicense license { get; set; }
    }

    /// <summary>
    /// Licensing information for embedded links
    /// </summary>
    public class OpenBoardModelLicense
    {
        public string type { get; set; }
        public string copyright_notice_url { get; set; }
        public string source_url { get; set; }
        public string author_name { get; set; }
        public string author_url { get; set; }
    }

    /// <summary>
    /// Deserialize links to other boards
    /// </summary>
    public class OpenBoardModelBoards
    {
        public string name { get; set; }
        public string url { get; set; }
        public string data_url { get; set; }

    }
}
