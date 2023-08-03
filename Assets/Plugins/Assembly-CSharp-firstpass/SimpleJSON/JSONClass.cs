using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace SimpleJSON
{
    public class JSONClass : JSONNode, IEnumerable
    {
        private Dictionary<string, JSONNode> m_Dict = new Dictionary<string, JSONNode>();

        public override JSONNode this[string aKey]
        {
            get
            {
                if (m_Dict.ContainsKey(aKey))
                {
                    return m_Dict[aKey];
                }
                return new JSONLazyCreator(this, aKey);
            }
            set
            {
                if (m_Dict.ContainsKey(aKey))
                {
                    m_Dict[aKey] = value;
                }
                else
                {
                    m_Dict.Add(aKey, value);
                }
            }
        }

        public override JSONNode this[int aIndex]
        {
            get
            {
                if (aIndex < 0 || aIndex >= m_Dict.Count)
                {
                    return null;
                }
                int currentIndex = 0;
                foreach (KeyValuePair<string, JSONNode> kvp in m_Dict)
                {
                    if (currentIndex == aIndex)
                    {
                        return kvp.Value;
                    }
                    currentIndex++;
                }
                return null;
            }
            set
            {
                if (aIndex >= 0 && aIndex < m_Dict.Count)
                {
                    string key = GetKeyByIndex(aIndex);
                    if (!string.IsNullOrEmpty(key))
                    {
                        m_Dict[key] = value;
                    }
                }
            }
        }

        public override int Count
        {
            get { return m_Dict.Count; }
        }

        public override IEnumerable<JSONNode> Childs
        {
            get
            {
                foreach (KeyValuePair<string, JSONNode> item in m_Dict)
                {
                    yield return item.Value;
                }
            }
        }

        private string GetKeyByIndex(int index)
        {
            int currentIndex = 0;
            foreach (string key in m_Dict.Keys)
            {
                if (currentIndex == index)
                {
                    return key;
                }
                currentIndex++;
            }
            return null;
        }

        public override void Add(string aKey, JSONNode aItem)
        {
            if (!string.IsNullOrEmpty(aKey))
            {
                if (m_Dict.ContainsKey(aKey))
                {
                    m_Dict[aKey] = aItem;
                }
                else
                {
                    m_Dict.Add(aKey, aItem);
                }
            }
            else
            {
                m_Dict.Add(Guid.NewGuid().ToString(), aItem);
            }
        }

        public override JSONNode Remove(string aKey)
        {
            if (!m_Dict.ContainsKey(aKey))
            {
                return null;
            }
            JSONNode result = m_Dict[aKey];
            m_Dict.Remove(aKey);
            return result;
        }

        public override JSONNode Remove(int aIndex)
        {
            if (aIndex < 0 || aIndex >= m_Dict.Count)
            {
                return null;
            }
            string key = GetKeyByIndex(aIndex);
            if (!string.IsNullOrEmpty(key))
            {
                m_Dict.Remove(key);
                return m_Dict[key];
            }
            return null;
        }

        public override JSONNode Remove(JSONNode aNode)
        {
            List<string> keysToRemove = new List<string>();
            foreach (KeyValuePair<string, JSONNode> kvp in m_Dict)
            {
                if (kvp.Value == aNode)
                {
                    keysToRemove.Add(kvp.Key);
                }
            }
            foreach (string key in keysToRemove)
            {
                m_Dict.Remove(key);
            }
            return aNode;
        }

        public IEnumerator GetEnumerator()
        {
            foreach (KeyValuePair<string, JSONNode> N in m_Dict)
            {
                yield return N;
            }
        }

        public override string ToString()
        {
            string text = "{";
            foreach (KeyValuePair<string, JSONNode> item in m_Dict)
            {
                if (text.Length > 2)
                {
                    text += ", ";
                }
                string text2 = text;
                text = text2 + "\"" + JSONNode.Escape(item.Key) + "\":" + item.Value.ToString();
            }
            return text + "}";
        }

        public override string ToString(string aPrefix)
        {
            string text = "{ ";
            foreach (KeyValuePair<string, JSONNode> item in m_Dict)
            {
                if (text.Length > 3)
                {
                    text += ", ";
                }
                text = text + "\n" + aPrefix + "   ";
                string text2 = text;
                text = text2 + "\"" + JSONNode.Escape(item.Key) + "\" : " + item.Value.ToString(aPrefix + "   ");
            }
            return text + "\n" + aPrefix + "}";
        }
    }
}
