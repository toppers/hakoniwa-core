using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.PluggableAsset.Communication.Pdu
{
    class PduFieldType
    {
        public string name;
        public int off;
        public int size;
        public PduFieldType(string name, int off, int size)
        {
            this.name = name;
            this.off = off;
            this.size = size;
        }
    }
    public class PduConfig
    {
        private int base_offset;
        private List<PduFieldType> header_fields = new List<PduFieldType>();
        private List<PduFieldType> fields = new List<PduFieldType>();

        public PduConfig(int base_off)
        {
            this.base_offset = base_off;
        }
        public int GetOffset(string name)
        {
            foreach (PduFieldType e in fields)
            {
                if (e.name == name)
                {
                    return e.off + base_offset;
                }
            }
            throw new FieldAccessException("ERROR: Invalid offset:" + name);
        }
        public int GetSize(string name)
        {
            foreach (PduFieldType e in fields)
            {
                if (e.name == name)
                {
                    return e.size;
                }
            }
            throw new FieldAccessException("ERROR: Invalid offset:" + name);
        }
        public void SetOffset(string name, int off, int size)
        {
            foreach (PduFieldType e in fields)
            {
                if (e.name.Equals(name))
                {
                    return;
                }
            }
            var new_entry = new PduFieldType(name, off, size);
            this.fields.Add(new_entry);
            return;
        }
        public int GetHeaderOffset(string name)
        {
            foreach (PduFieldType e in header_fields)
            {
                if (e.name == name)
                {
                    return e.off;
                }
            }
            throw new FieldAccessException("ERROR: Invalid offset:" + name);
        }
        public void SetHeaderOffset(string name, int off, int size)
        {
            foreach (PduFieldType e in fields)
            {
                if (e.name.Equals(name))
                {
                    return;
                }
            }
            var new_entry = new PduFieldType(name, off, size);
            this.header_fields.Add(new_entry);
        }
    }
}
