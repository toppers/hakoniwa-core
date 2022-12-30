using Hakoniwa.Core.Utils.Logger;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Hakoniwa.PluggableAsset.Communication.Pdu.Raw
{
    public class PduBinOffsetElmInfo
    {
        public bool is_array;
        public bool is_primitive;
        public string field_name;
        public string type_name;
        public int offset;
        public int elm_size;
        public int array_size;
    }
    public class PduBinOffsetInfo
    {
        public string package_name;
        public string type_name;
        public int size;
        public List<PduBinOffsetElmInfo> elms;
    }

    public class PduOffset
    {
        /*
         *  string: <package_name>/<type_name>
         */ 
        static private Dictionary<string, PduBinOffsetInfo> map = new Dictionary<string, PduBinOffsetInfo>();
        static private bool IsPrimitive(string type_name)
        {
            switch (type_name)
            {
                case "int8":
                    return true;
                case "int16":
                    return true;
                case "int32":
                    return true;
                case "int64":
                    return true;
                case "uint8":
                    return true;
                case "uint16":
                    return true;
                case "uint32":
                    return true;
                case "uint64":
                    return true;
                case "float32":
                    return true;
                case "float64":
                    return true;
                case "bool":
                    return true;
                case "string":
                    return true;
                default:
                    return false;
            }
        }

        static private void Put(string package_name, string type_name, PduBinOffsetInfo elm)
        {
            if (type_name.Contains("/"))
            {
                SimpleLogger.Get().Log(Level.INFO, "PduOffset key=" + type_name);
                map[type_name] = elm;
            }
            else
            {
                SimpleLogger.Get().Log(Level.INFO, "PduOffset key=" + package_name + "/" + type_name);
                map[package_name + "/" + type_name] = elm;
            }
        }

        static public PduBinOffsetInfo Get(string type_name)
        {
            return map[type_name];
        }

        static public void Parse(string filepath)
        {
            PduBinOffsetInfo off_info = new PduBinOffsetInfo();

            var tmp = filepath.Split('/');
            int len = tmp.Length;
            var filename = tmp[len - 1];
            var package_name = tmp[len - 2];
            var type_name = filename.Split('.')[0];
            off_info.package_name = package_name;
            off_info.type_name = type_name;
            off_info.elms = new List<PduBinOffsetElmInfo>();
            off_info.size = 0;
            SimpleLogger.Get().Log(Level.INFO, "Parse filepath=" + filepath);
            foreach (string line in File.ReadLines(filepath))
            {
                SimpleLogger.Get().Log(Level.INFO, "PduOffset line=" + line);
                string[] attr = line.Split(':');
                PduBinOffsetElmInfo elm = new PduBinOffsetElmInfo();
                elm.is_array = attr[0].Equals("array");
                elm.is_primitive = attr[1].Equals("primitive");
                elm.field_name = attr[2];
                if (attr[3].Contains("/") || IsPrimitive(attr[3]))
                {
                    elm.type_name = attr[3];
                }
                else
                {
                    elm.type_name = package_name + "/" + attr[3];
                }
                elm.offset = int.Parse(attr[4]);
                off_info.size = elm.offset + int.Parse(attr[5]);
                if (attr.Length >= 7)
                {
                    elm.array_size = int.Parse(attr[6]);
                    elm.elm_size = int.Parse(attr[5]) / elm.array_size;
                }
                else
                {
                    elm.array_size = 1;
                    elm.elm_size = int.Parse(attr[5]);
                }
                off_info.elms.Add(elm);
            }
            Put(package_name, type_name, off_info);
            return;
        }
        static public void ParseAll(string package_dirs)
        {
            string[] package_names = Directory.GetDirectories(package_dirs);
            SimpleLogger.Get().Log(Level.INFO, "ParseAll() package_dirs=" + package_dirs);
            foreach (var package_path in package_names)
            {
                var tmp = package_path.Split(Path.DirectorySeparatorChar);
                var package_name = tmp[tmp.Length - 1];
                SimpleLogger.Get().Log(Level.INFO, "package_name=" + package_name);
                var tmp_dir = package_dirs + "/" + package_name;
                string[] filepaths = Directory.GetFiles(tmp_dir);
                SimpleLogger.Get().Log(Level.INFO, "tmp_dir=" + tmp_dir);
                SimpleLogger.Get().Log(Level.INFO, "filepaths.Length=" + filepaths.Length);
                foreach (var filepath in filepaths)
                {
                    Parse(filepath.Replace(Path.DirectorySeparatorChar, '/'));
                }
            }
        }
    }
}
