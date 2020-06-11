using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Watched_It
{
    public enum Type
    {
        None, Movie, Series
    }

    public class Item
    {
        public Type type = Type.None;
        public string name = "None";
        public bool completed = false;
        public int season = 0;
        public int episode = 0;
        public string progress = "";


        public int LastEdited = 0;
        public Item(Type newtype, string newname)
        {
            name = newname;
            type = newtype;
            LastEdited = (int)DateTime.Now.Subtract(DateTime.MinValue.AddYears(1969)).TotalSeconds;
        }
    }
}
