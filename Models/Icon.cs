using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace Kaenx.Creator.Models
{
    public class Icon: INotifyPropertyChanged
    {
        
        private int _uid = -1;
        public int UId
        {
            get { return _uid; }
            set { _uid = value; Changed("UId"); }
        }

        private string _name = "dummy";
        public string Name
        {
            get { return _name; }
            set { _name = value; Changed("Name"); LastModified = DateTime.Now; }
        }

        private byte[] _data;
        public byte[] Data
        {
            get { return _data; }
            set { _data = value; LastModified = DateTime.Now; }
        }

        private DateTime _modified;
        public DateTime LastModified
        {
            get { return _modified; }
            set { _modified = value; Changed("LastModified"); Changed("LastModifiedDisplay"); }
        }

        public string LastModifiedDisplay
        {
            get { return LastModified.ToString("G", System.Threading.Thread.CurrentThread.CurrentCulture); }
        }


        //TODO implement converter
        // BitmapImage image;
        // MemoryStream ms = null;

        // public ImageSource Source
        // {
        //     get {
        //         if(image == null || _dataChanged)
        //         {
        //             if(ms != null) ms.Dispose();
        //             ms = new MemoryStream(Data);
        //             image = new BitmapImage();
        //             image.BeginInit();
        //             image.StreamSource = ms;
        //             image.EndInit();
        //             _dataChanged = false;
        //         }
                
        //         return image;
        //     }
        // }


        public event PropertyChangedEventHandler PropertyChanged;
        private void Changed(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}