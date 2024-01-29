using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Globalization;
using System.ComponentModel;
using System.Linq;

namespace Kaenx.Creator.Models
{
    public class MemoryByte : INotifyPropertyChanged
    {   
        public MemoryByteUsage Usage { get; set; }
        public int Address { get; set; }
        public int Offset { get; set; }

        public string Name
        {
            get { return $"0x{Address:X4}"; }
        }

        public MemoryByte(int address, int offset, MemoryByteUsage usage = MemoryByteUsage.Free)
        {
            Address = address;
            Offset = offset;
            Usage = usage;
        }

        public MemoryByte(int address, int offset, Parameter para, int usedBits, MemoryByteUsage usage = MemoryByteUsage.Free)
        {
            Address = address;
            Offset = offset;

            for(int x = 0; x < usedBits; x++)
                Bits[x] += 1;
                
            Usage = usage;
            ParameterList.Add(para);
        }

        public object UsedBy { get; set; }
        public List<MemoryUnion> UnionList { get; set; } = new List<MemoryUnion>();
        public List<Parameter> ParameterList { get; set; } = new List<Parameter>();

        public List<uint> Bits {get;set;} = new List<uint>() { 0,0,0,0,0,0,0,0 };

        public int CheckFreeBits()
        {
            if(!Bits.Any(b => b != 0))
                return 0;
            if(Bits.Any(b => b > 1))
                return -1;
            if(Bits.Count(b => b > 0) == 8)
                return 1;
            return 2;
        }

        public (int size, int offset) GetFreeBits()
        {
            int maxSize = 0;
            int offset = 0;
            int currentSize = 0;
            int currentOffset = -1;

            for(int i = 0; i < 8; i++)
            {
                if(Bits[i] == 0)
                {
                    currentSize++;
                    if(currentOffset == -1) currentOffset = i;
                } else {
                    if(currentSize > maxSize)
                    {
                        maxSize = currentSize;
                        offset = currentOffset;
                    }
                    currentSize = 0;
                    currentOffset = -1;
                }
            }
            if(currentSize > maxSize)
            {
                maxSize = currentSize;
                offset = currentOffset;
            }
            return (maxSize, offset);
        }

        public void SetByteUsed(MemoryByteUsage usage, object usedBy)
        {
            UsedBy = usedBy;
            Usage = usage;
            for(int x = 0; x < 8; x++)
                Bits[x] += 1;
        }

        public void SetBitsUsed(Parameter para, int size, int offset)
        {
            for(int x = 0; x < size; x++)
            {
                Bits[offset + x] += 1;
            }
            if(!ParameterList.Contains(para))
                ParameterList.Add(para);
        }

        public void SetBitsUsed(Union union, List<Parameter> paras, int size, int offset)
        {

            UnionList.Add(new MemoryUnion() {
                UnionObject = union,
                ParameterList = paras
            });

            for(int x = 0; x < size; x++)
            {
                Bits[offset + x] += 1;
            }
        }

        public int SetBytesUsed(Parameter para, int size)
        {
            int offset = 0;

            for(int i = 0; i < 8; i++)
            {
                bool flag = true;
                for(int x = 0; x < size; x++)
                {
                    if(Bits[i+x] != 0)
                    {
                        flag = false;
                        break;
                    }

                }
                if(flag) break;
                offset++;
            }
            if((offset + size) > 8) throw new Exception("Kein freier Speicherplatz in Byte");

            for(int x = 0; x < size; x++)
                Bits[offset+x] += 1;

            if(!ParameterList.Contains(para))
                ParameterList.Add(para);

            return offset;
        }

        
        private List<int> fillColor;
        public List<int> FillColor
        {
            get{
                CalculateFillColors();
                return fillColor;
            }
        }

        private void CalculateFillColors()
        {
            if(fillColor != null) return;
            fillColor = new List<int>();
            foreach(uint c in Bits)
            {
                if(c == 0)
                    fillColor.Add(7);
                else if(c == 1)
                    fillColor.Add(6);
                else
                    fillColor.Add(5);
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void Changed(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public enum MemoryByteUsage
    {
        Used,
        Free,
        GroupAddress,
        Association,
        Coms,
        Module
    }
}