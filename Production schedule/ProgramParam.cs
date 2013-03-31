using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;

namespace Production_schedule
{
    [Serializable]
    public class ProgramParam
    {
        public bool DrawItemText { get; set; }
        public Color TaskColorFill { get; set; }
        public Color TaskColorFillOrder { get; set; }
        public Color TaskColorPerimeter { get; set; }
        public Color TaskColorFillAlert { get; set; }
        public Color ConfColor1 { get; set; }
        public Color ConfColor2 { get; set; }
        public Color ServColor1 { get; set; }
        public Color ServColor2 { get; set; }
        public Color TaskColorFillShadow { get; set; }
        public Color TaskColorFillOrderShadow { get; set; }
        public Color ConfColor1Shadow { get; set; }
        public Color ConfColor2Shadow { get; set; }
        public Color ServColor1Shadow { get; set; }
        public Color ServColor2Shadow { get; set; }
        public Color AllPerimeterColorShadow { get; set; }

        public bool GraphAutoSize { get; set; }
        public int GraphWidth { get; set; }
        public int GraphHeight { get; set; }

        public int LastWindowPosX { get; set; }
        public int LastWindowPosY { get; set; }

        public int LastWindowWidth { get; set; }
        public int LastWindowHeight { get; set; }
        public FormWindowState FormSate { get; set; }
    }

    
    public class ProgramParamSerializer
    {
        public static void Serialize(ProgramParam data, string FileName)
        {
            FileStream fs = new FileStream(FileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            BinaryFormatter bf = new BinaryFormatter();

            //сериализация
            bf.Serialize(fs, data);
            fs.Close();
        }

        public static void Deserialize(out ProgramParam data, string FileName)
        {
            FileStream fs = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            BinaryFormatter bf = new BinaryFormatter();
            data = (ProgramParam)bf.Deserialize(fs);
            fs.Close();
        } 
    }
}
