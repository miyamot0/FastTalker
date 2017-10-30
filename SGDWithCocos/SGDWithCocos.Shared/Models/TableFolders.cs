using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace SGDWithCocos.Models
{
    public class TableFolders
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        public string Text { get; set; }
        public string AssetName { get; set; }
        public string Base64 { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        //public CCColor3B TextColor { get; set; }
        public float Scale { get; set; }
        public int Tag { get; set; }
        public float TextScale { get; set; }
        public bool TextVisible { get; set; }

        public TableFolders() { }

        public TableFolders(string text, string assetName, string base64, float x, float y, int tag, float scale)
        {
            Text = text;
            AssetName = assetName;
            Base64 = base64;
            X = x;
            Y = y;
            Tag = tag;
            Scale = scale;
            TextScale = 1f;
            TextVisible = true;
            //TextColor = color;
        }
    }
}
