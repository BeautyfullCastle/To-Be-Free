using System;

namespace Language
{
    [Serializable]
    public class EventData : IData
    {
        public int index;
        public string script;
    }
}
