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

        static private void Put(string package_name, string type_name, PduBinOffsetInfo elm)
        {
            if (type_name.Contains("/"))
            {
                map[type_name] = elm;
            }
            else
            {
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

            var tmp = filepath.Split(Path.DirectorySeparatorChar);
            int len = tmp.Length;
            var filename = tmp[len - 1];
            var package_name = tmp[len - 2];
            var type_name = filename.Split('.')[0];
            off_info.package_name = package_name;
            off_info.type_name = type_name;
            off_info.elms = new List<PduBinOffsetElmInfo>();
            off_info.size = 0;
            foreach (string line in File.ReadLines(filepath))
            {
                string[] attr = line.Split(':');
                PduBinOffsetElmInfo elm = new PduBinOffsetElmInfo();
                elm.is_array = attr[0].Equals("array");
                elm.is_primitive = attr[1].Equals("primitive");
                elm.field_name = attr[2];
                if (attr[3].Contains("/"))
                {
                    elm.type_name = attr[3];
                }
                else
                {
                    elm.type_name = package_name + "/" + attr[3];
                }
                elm.offset = int.Parse(attr[4]);
                off_info.size += int.Parse(attr[5]);
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
            foreach (var package_name in package_names)
            {
                Parse(package_dirs + Path.DirectorySeparatorChar + package_name);
            }
        }
    }
}
