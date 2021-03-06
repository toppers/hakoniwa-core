using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.Core.Utils
{
    public class LineBinaryStorage
    {
        private int columns;
        private int current_line;
        private int max_line;
        private int column_data_size;
        private int base_realloc_lines = 1024;
        private byte[] buffer = null;

        public LineBinaryStorage(int columnum, int column_size)
        {
            this.columns = columnum;
            this.column_data_size = column_size;
            this.column_data_size = column_size;
            this.Clear();
            return;
        }
        public int GetColumns()
        {
            return this.columns;
        }
        public void SetReallocLines(int lines)
        {
            this.base_realloc_lines = lines;
        }
        public void Clear()
        {
            this.current_line = 0;
            this.max_line = base_realloc_lines;
            this.buffer = new byte[this.columns * this.column_data_size * this.max_line];
        }
        public int GetBufferSize()
        {
            return this.buffer.Length;
        }
        private void Realloc()
        {
            int next_max_line = this.max_line + base_realloc_lines;
            byte[] src = this.buffer;
            byte[] dst = new byte[this.columns * this.column_data_size * next_max_line];
            Buffer.BlockCopy(src, 0, dst, 0, src.Length);
            this.buffer = dst;
            this.max_line = next_max_line;
        }
        public byte[] GetBuffer()
        {
            byte[] dst = new byte[this.columns * this.column_data_size * this.GetLines()];
            Buffer.BlockCopy(this.buffer, 0, dst, 0, dst.Length);
            return dst;
        }
        public void Next()
        {
            this.current_line++;
            if (this.current_line >= this.max_line)
            {
                this.Realloc();
            }
        }
        public int GetLines()
        {
            return this.current_line;
        }
        public int GetCurrentLine()
        {
            return this.current_line;
        }
        public bool SetData(int column_id, byte[] data)
        {
            if (column_id >= this.columns)
            {
                return false;
            }
            if (this.column_data_size != data.Length)
            {
                return false;
            }
            int base_off = this.current_line * (this.columns * this.column_data_size);
            int inx = (column_id * this.column_data_size);
            Buffer.BlockCopy(data, 0, this.buffer, base_off + inx, this.column_data_size);
            return true;
        }
        public bool SetData(int column_id, UInt64 value)
        {
            byte[] data = BitConverter.GetBytes(value);
            return SetData(column_id, data);
        }
        public bool SetData(int column_id, double value)
        {
            byte[] data = BitConverter.GetBytes(value);
            return SetData(column_id, data);
        }

        public byte[] GetData(int line, int column_id)
        {
            if (column_id >= this.columns)
            {
                return null;
            }
            byte[] data = new byte[this.column_data_size];
            int base_off = line * (this.columns * this.column_data_size);
            int inx = (column_id * this.column_data_size);
            Buffer.BlockCopy(this.buffer, base_off + inx, data, 0, this.column_data_size);
            return data;
        }
        public bool GetData(int line, int column_id, out UInt64 ret_value)
        {
            byte[] data = this.GetData(line, column_id);
            ret_value = 0;
            if (data == null)
            {
                return false;
            }
            ret_value = BitConverter.ToUInt64(data, 0);
            return true;
        }
        public bool GetData(int line, int column_id, out double ret_value)
        {
            byte[] data = this.GetData(line, column_id);
            ret_value = 0;
            if (data == null)
            {
                return false;
            }
            ret_value = BitConverter.ToDouble(data, 0);
            return true;
        }

    }
}
