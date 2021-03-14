using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.Core.Communication.Pdu
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
    class PduConfig
    {
        private int base_offset;
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
    }
}
