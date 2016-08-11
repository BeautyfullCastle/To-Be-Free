using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Language
{
    [Serializable]
    public class CityData : IData
    {
        public int index;
        public string name;
        public string area;
    }
}
