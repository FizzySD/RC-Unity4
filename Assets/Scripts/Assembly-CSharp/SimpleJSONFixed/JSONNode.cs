using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SimpleJSONFixed
{
	public abstract class JSONNode
	{
		public struct Enumerator
		{
			private enum Type
			{
				None = 0,
				Array = 1,
				Object = 2
			}

			private Type type;

			private Dictionary<string, JSONNode>.Enumerator m_Object;

			private List<JSONNode>.Enumerator m_Array;

			public bool IsValid
			{
				get
				{
					return type != Type.None;
				}
			}

			public KeyValuePair<string, JSONNode> Current
			{
				get
				{
					if (type == Type.Array)
					{
						return new KeyValuePair<string, JSONNode>(string.Empty, m_Array.Current);
					}
					if (type == Type.Object)
					{
						return m_Object.Current;
					}
					return new KeyValuePair<string, JSONNode>(string.Empty, null);
				}
			}

			public Enumerator(List<JSONNode>.Enumerator aArrayEnum)
			{
				type = Type.Array;
				m_Object = default(Dictionary<string, JSONNode>.Enumerator);
				m_Array = aArrayEnum;
			}

			public Enumerator(Dictionary<string, JSONNode>.Enumerator aDictEnum)
			{
				type = Type.Object;
				m_Object = aDictEnum;
				m_Array = default(List<JSONNode>.Enumerator);
			}

			public bool MoveNext()
			{
				if (type == Type.Array)
				{
					return m_Array.MoveNext();
				}
				if (type == Type.Object)
				{
					return m_Object.MoveNext();
				}
				return false;
			}
		}

		public struct ValueEnumerator
		{
			private Enumerator m_Enumerator;

			public JSONNode Current
			{
				get
				{
					return m_Enumerator.Current.Value;
				}
			}

			public ValueEnumerator(List<JSONNode>.Enumerator aArrayEnum)
				: this(new Enumerator(aArrayEnum))
			{
			}

			public ValueEnumerator(Dictionary<string, JSONNode>.Enumerator aDictEnum)
				: this(new Enumerator(aDictEnum))
			{
			}

			public ValueEnumerator(Enumerator aEnumerator)
			{
				m_Enumerator = aEnumerator;
			}

			public bool MoveNext()
			{
				return m_Enumerator.MoveNext();
			}

			public ValueEnumerator GetEnumerator()
			{
				return this;
			}
		}

		public struct KeyEnumerator
		{
			private Enumerator m_Enumerator;

			public string Current
			{
				get
				{
					return m_Enumerator.Current.Key;
				}
			}

			public KeyEnumerator(List<JSONNode>.Enumerator aArrayEnum)
				: this(new Enumerator(aArrayEnum))
			{
			}

			public KeyEnumerator(Dictionary<string, JSONNode>.Enumerator aDictEnum)
				: this(new Enumerator(aDictEnum))
			{
			}

			public KeyEnumerator(Enumerator aEnumerator)
			{
				m_Enumerator = aEnumerator;
			}

			public bool MoveNext()
			{
				return m_Enumerator.MoveNext();
			}

			public KeyEnumerator GetEnumerator()
			{
				return this;
			}
		}

		public class LinqEnumerator : IEnumerator<KeyValuePair<string, JSONNode>>, IDisposable, IEnumerator, IEnumerable<KeyValuePair<string, JSONNode>>, IEnumerable
		{
			private JSONNode m_Node;

			private Enumerator m_Enumerator;

			public KeyValuePair<string, JSONNode> Current
			{
				get
				{
					return m_Enumerator.Current;
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return m_Enumerator.Current;
				}
			}

			internal LinqEnumerator(JSONNode aNode)
			{
				m_Node = aNode;
				if (m_Node != null)
				{
					m_Enumerator = m_Node.GetEnumerator();
				}
			}

			public bool MoveNext()
			{
				return m_Enumerator.MoveNext();
			}

			public void Dispose()
			{
				m_Node = null;
				m_Enumerator = default(Enumerator);
			}

			public IEnumerator<KeyValuePair<string, JSONNode>> GetEnumerator()
			{
				return new LinqEnumerator(m_Node);
			}

			public void Reset()
			{
				if (m_Node != null)
				{
					m_Enumerator = m_Node.GetEnumerator();
				}
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return new LinqEnumerator(m_Node);
			}
		}

		public static bool forceASCII = false;

		public static bool longAsString = false;

		public static bool allowLineComments = true;

		[ThreadStatic]
		private static StringBuilder m_EscapeBuilder;

		public abstract JSONNodeType Tag { get; }

		public virtual JSONNode this[int aIndex]
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		public virtual JSONNode this[string aKey]
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		public virtual string Value
		{
			get
			{
				return "";
			}
			set
			{
			}
		}

		public virtual int Count
		{
			get
			{
				return 0;
			}
		}

		public virtual bool IsNumber
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsString
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsBoolean
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsNull
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsArray
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsObject
		{
			get
			{
				return false;
			}
		}

		public virtual bool Inline
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		public virtual IEnumerable<JSONNode> Children
		{
			get
			{
				yield break;
			}
		}

		public IEnumerable<JSONNode> DeepChildren
		{
			get
			{
				foreach (JSONNode child in Children)
				{
					foreach (JSONNode deepChild in child.DeepChildren)
					{
						yield return deepChild;
					}
				}
			}
		}

		public IEnumerable<KeyValuePair<string, JSONNode>> Linq
		{
			get
			{
				return new LinqEnumerator(this);
			}
		}

		public KeyEnumerator Keys
		{
			get
			{
				return new KeyEnumerator(GetEnumerator());
			}
		}

		public ValueEnumerator Values
		{
			get
			{
				return new ValueEnumerator(GetEnumerator());
			}
		}

		public virtual double AsDouble
		{
			get
			{
				double result = 0.0;
				if (double.TryParse(Value, NumberStyles.Float, CultureInfo.InvariantCulture, out result))
				{
					return result;
				}
				return 0.0;
			}
			set
			{
				Value = value.ToString(CultureInfo.InvariantCulture);
			}
		}

		public virtual int AsInt
		{
			get
			{
				return (int)AsDouble;
			}
			set
			{
				AsDouble = value;
			}
		}

		public virtual float AsFloat
		{
			get
			{
				return (float)AsDouble;
			}
			set
			{
				AsDouble = value;
			}
		}

		public virtual bool AsBool
		{
			get
			{
				bool result = false;
				if (bool.TryParse(Value, out result))
				{
					return result;
				}
				return !string.IsNullOrEmpty(Value);
			}
			set
			{
				Value = (value ? "true" : "false");
			}
		}

		public virtual long AsLong
		{
			get
			{
				long result = 0L;
				if (long.TryParse(Value, out result))
				{
					return result;
				}
				return 0L;
			}
			set
			{
				Value = value.ToString();
			}
		}

		public virtual ulong AsULong
		{
			get
			{
				ulong result = 0uL;
				if (ulong.TryParse(Value, out result))
				{
					return result;
				}
				return 0uL;
			}
			set
			{
				Value = value.ToString();
			}
		}

		public virtual JSONArray AsArray
		{
			get
			{
				return this as JSONArray;
			}
		}

		public virtual JSONObject AsObject
		{
			get
			{
				return this as JSONObject;
			}
		}

		internal static StringBuilder EscapeBuilder
		{
			get
			{
				if (m_EscapeBuilder == null)
				{
					m_EscapeBuilder = new StringBuilder();
				}
				return m_EscapeBuilder;
			}
		}

		public virtual void Add(string aKey, JSONNode aItem)
		{
		}

		public virtual void Add(JSONNode aItem)
		{
			Add("", aItem);
		}

		public virtual JSONNode Remove(string aKey)
		{
			return null;
		}

		public virtual JSONNode Remove(int aIndex)
		{
			return null;
		}

		public virtual JSONNode Remove(JSONNode aNode)
		{
			return aNode;
		}

		public virtual void Clear()
		{
		}

		public virtual JSONNode Clone()
		{
			return null;
		}

		public virtual bool HasKey(string aKey)
		{
			return false;
		}

		public virtual JSONNode GetValueOrDefault(string aKey, JSONNode aDefault)
		{
			return aDefault;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			WriteToStringBuilder(stringBuilder, 0, 0, JSONTextMode.Compact);
			return stringBuilder.ToString();
		}

		public virtual string ToString(int aIndent)
		{
			StringBuilder stringBuilder = new StringBuilder();
			WriteToStringBuilder(stringBuilder, 0, aIndent, JSONTextMode.Indent);
			return stringBuilder.ToString();
		}

		internal abstract void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode);

		public abstract Enumerator GetEnumerator();

		public static implicit operator JSONNode(string s)
		{
			if (s != null)
			{
				return new JSONString(s);
			}
			return JSONNull.CreateOrGet();
		}

		public static implicit operator string(JSONNode d)
		{
			if (!(d == null))
			{
				return d.Value;
			}
			return null;
		}

		public static implicit operator JSONNode(double n)
		{
			return new JSONNumber(n);
		}

		public static implicit operator double(JSONNode d)
		{
			if (!(d == null))
			{
				return d.AsDouble;
			}
			return 0.0;
		}

		public static implicit operator JSONNode(float n)
		{
			return new JSONNumber(n);
		}

		public static implicit operator float(JSONNode d)
		{
			if (!(d == null))
			{
				return d.AsFloat;
			}
			return 0f;
		}

		public static implicit operator JSONNode(int n)
		{
			return new JSONNumber(n);
		}

		public static implicit operator int(JSONNode d)
		{
			if (!(d == null))
			{
				return d.AsInt;
			}
			return 0;
		}

		public static implicit operator JSONNode(long n)
		{
			if (longAsString)
			{
				return new JSONString(n.ToString());
			}
			return new JSONNumber(n);
		}

		public static implicit operator long(JSONNode d)
		{
			if (!(d == null))
			{
				return d.AsLong;
			}
			return 0L;
		}

		public static implicit operator JSONNode(ulong n)
		{
			if (longAsString)
			{
				return new JSONString(n.ToString());
			}
			return new JSONNumber(n);
		}

		public static implicit operator ulong(JSONNode d)
		{
			if (!(d == null))
			{
				return d.AsULong;
			}
			return 0uL;
		}

		public static implicit operator JSONNode(bool b)
		{
			return new JSONBool(b);
		}

		public static implicit operator bool(JSONNode d)
		{
			if (!(d == null))
			{
				return d.AsBool;
			}
			return false;
		}

		public static implicit operator JSONNode(KeyValuePair<string, JSONNode> aKeyValue)
		{
			return aKeyValue.Value;
		}

		public static bool operator ==(JSONNode a, object b)
		{
			if ((object)a == b)
			{
				return true;
			}
			bool flag = a is JSONNull || (object)a == null || a is JSONLazyCreator;
			bool flag2 = b is JSONNull || b == null || b is JSONLazyCreator;
			if (flag && flag2)
			{
				return true;
			}
			if (!flag)
			{
				return a.Equals(b);
			}
			return false;
		}

		public static bool operator !=(JSONNode a, object b)
		{
			return !(a == b);
		}

		public override bool Equals(object obj)
		{
			return (object)this == obj;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		internal static string Escape(string aText)
		{
			StringBuilder escapeBuilder = EscapeBuilder;
			escapeBuilder.Length = 0;
			if (escapeBuilder.Capacity < aText.Length + aText.Length / 10)
			{
				escapeBuilder.Capacity = aText.Length + aText.Length / 10;
			}
			foreach (char c in aText)
			{
				switch (c)
				{
				case '\\':
					escapeBuilder.Append("\\\\");
					continue;
				case '"':
					escapeBuilder.Append("\\\"");
					continue;
				case '\n':
					escapeBuilder.Append("\\n");
					continue;
				case '\r':
					escapeBuilder.Append("\\r");
					continue;
				case '\t':
					escapeBuilder.Append("\\t");
					continue;
				case '\b':
					escapeBuilder.Append("\\b");
					continue;
				case '\f':
					escapeBuilder.Append("\\f");
					continue;
				}
				if (c < ' ' || (forceASCII && c > '\u007f'))
				{
					ushort num = c;
					escapeBuilder.Append("\\u").Append(num.ToString("X4"));
				}
				else
				{
					escapeBuilder.Append(c);
				}
			}
			string result = escapeBuilder.ToString();
			escapeBuilder.Length = 0;
			return result;
		}

		private static JSONNode ParseElement(string token, bool quoted)
		{
			if (quoted)
			{
				return token;
			}
			if (token.Length <= 5)
			{
				string text = token.ToLower();
				switch (text)
				{
				case "false":
				case "true":
					return text == "true";
				case "null":
					return JSONNull.CreateOrGet();
				}
			}
			double result;
			if (double.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out result))
			{
				return result;
			}
			return token;
		}

		public static JSONNode Parse(string aJSON)
		{
			Stack<JSONNode> stack = new Stack<JSONNode>();
			JSONNode jSONNode = null;
			int i = 0;
			StringBuilder stringBuilder = new StringBuilder();
			string aKey = "";
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			for (; i < aJSON.Length; i++)
			{
				switch (aJSON[i])
				{
				case '{':
					if (flag)
					{
						stringBuilder.Append(aJSON[i]);
						break;
					}
					stack.Push(new JSONObject());
					if (jSONNode != null)
					{
						jSONNode.Add(aKey, stack.Peek());
					}
					aKey = "";
					stringBuilder.Length = 0;
					jSONNode = stack.Peek();
					flag3 = false;
					break;
				case '[':
					if (flag)
					{
						stringBuilder.Append(aJSON[i]);
						break;
					}
					stack.Push(new JSONArray());
					if (jSONNode != null)
					{
						jSONNode.Add(aKey, stack.Peek());
					}
					aKey = "";
					stringBuilder.Length = 0;
					jSONNode = stack.Peek();
					flag3 = false;
					break;
				case ']':
				case '}':
					if (flag)
					{
						stringBuilder.Append(aJSON[i]);
						break;
					}
					if (stack.Count == 0)
					{
						throw new Exception("JSON Parse: Too many closing brackets");
					}
					stack.Pop();
					if (stringBuilder.Length > 0 || flag2)
					{
						jSONNode.Add(aKey, ParseElement(stringBuilder.ToString(), flag2));
					}
					if (jSONNode != null)
					{
						jSONNode.Inline = !flag3;
					}
					flag2 = false;
					aKey = "";
					stringBuilder.Length = 0;
					if (stack.Count > 0)
					{
						jSONNode = stack.Peek();
					}
					break;
				case ':':
					if (flag)
					{
						stringBuilder.Append(aJSON[i]);
						break;
					}
					aKey = stringBuilder.ToString();
					stringBuilder.Length = 0;
					flag2 = false;
					break;
				case '"':
					flag = !flag;
					flag2 = flag2 || flag;
					break;
				case ',':
					if (flag)
					{
						stringBuilder.Append(aJSON[i]);
						break;
					}
					if (stringBuilder.Length > 0 || flag2)
					{
						jSONNode.Add(aKey, ParseElement(stringBuilder.ToString(), flag2));
					}
					flag2 = false;
					aKey = "";
					stringBuilder.Length = 0;
					flag2 = false;
					break;
				case '\n':
				case '\r':
					flag3 = true;
					break;
				case '\t':
				case ' ':
					if (flag)
					{
						stringBuilder.Append(aJSON[i]);
					}
					break;
				case '\\':
					i++;
					if (flag)
					{
						char c = aJSON[i];
						switch (c)
						{
						case 't':
							stringBuilder.Append('\t');
							break;
						case 'r':
							stringBuilder.Append('\r');
							break;
						case 'n':
							stringBuilder.Append('\n');
							break;
						case 'b':
							stringBuilder.Append('\b');
							break;
						case 'f':
							stringBuilder.Append('\f');
							break;
						case 'u':
						{
							string s = aJSON.Substring(i + 1, 4);
							stringBuilder.Append((char)int.Parse(s, NumberStyles.AllowHexSpecifier));
							i += 4;
							break;
						}
						default:
							stringBuilder.Append(c);
							break;
						}
					}
					break;
				case '/':
					if (allowLineComments && !flag && i + 1 < aJSON.Length && aJSON[i + 1] == '/')
					{
						while (++i < aJSON.Length && aJSON[i] != '\n' && aJSON[i] != '\r')
						{
						}
					}
					else
					{
						stringBuilder.Append(aJSON[i]);
					}
					break;
				default:
					stringBuilder.Append(aJSON[i]);
					break;
				case '\ufeff':
					break;
				}
			}
			if (flag)
			{
				throw new Exception("JSON Parse: Quotation marks seems to be messed up.");
			}
			if (jSONNode == null)
			{
				return ParseElement(stringBuilder.ToString(), flag2);
			}
			return jSONNode;
		}
	}
}
