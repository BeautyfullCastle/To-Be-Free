using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Language
{
    [Serializable]
    public class EventData : IData
    {
        public int index;
        public string script;
    }
}
