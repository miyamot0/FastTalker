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
