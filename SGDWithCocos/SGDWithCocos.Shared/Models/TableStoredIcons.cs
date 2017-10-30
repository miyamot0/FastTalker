using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace SGDWithCocos.Models
{
    public class TableStoredIcons
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        public string Text { get; set; }
        public string Base64 { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Scale { get; set; }
        public int Tag { get; set; }
        public string Folder { get; set; }
        public float TextScale { get; set; }
        public bool TextVisible { get; set; }

        public TableStoredIcons() { }

        public TableStoredIcons(string text, string base64, float x, float y, int tag, string folder, float scale)
        {
            Text = text;
            Base64 = base64;
            X = x;
            Y = y;
            Tag = tag;
            Folder = folder;
            Scale = scale;
            TextScale = 1f;
            TextVisible = true;
        }
    }
}
