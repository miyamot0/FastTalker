﻿//----------------------------------------------------------------------------------------------
// <copyright file="TableSettings.cs" 
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

using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace SGDWithCocos.Models
{
    public class TableSettings
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        public bool SingleMode { get; set; }
        public bool AutoUnselectSingleMode { get; set; }
    }
}
