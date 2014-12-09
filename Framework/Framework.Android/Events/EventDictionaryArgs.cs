using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.Events
{
    public class EventDictionaryArgs : EventArgs
    {
        public Dictionary<string, object> DictionaryValue { get; set; }

        public EventDictionaryArgs(Dictionary<string, object> p_objDictionary)
        {
            DictionaryValue = p_objDictionary;
        }
    }
}
