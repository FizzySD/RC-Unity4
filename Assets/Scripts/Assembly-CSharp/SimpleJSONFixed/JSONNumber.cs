using System;
using System.Globalization;
using System.Text;

namespace SimpleJSONFixed
{
	public class JSONNumber : JSONNode
	{
		private double m_Data;

		public override JSONNodeType Tag
		{
			get
			{
				return JSONNodeType.Number;
			}
		}

		public override bool IsNumber
		{
			get
			{
				return true;
			}
		}

		public override string Value
		{
			get
			{
				return m_Data.ToString(CultureInfo.InvariantCulture);
			}
			set
			{
				double result;
				if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out result))
				{
					m_Data = result;
				}
			}
		}

		public override double AsDouble
		{
			get
			{
				return m_Data;
			}
			set
			{
				m_Data = value;
			}
		}

		public override long AsLong
		{
			get
			{
				return (long)m_Data;
			}
			set
			{
				m_Data = value;
			}
		}

		public override ulong AsULong
		{
			get
			{
				return (ulong)m_Data;
			}
			set
			{
				m_Data = value;
			}
		}

		public override Enumerator GetEnumerator()
		{
			return default(Enumerator);
		}

		public JSONNumber(double aData)
		{
			m_Data = aData;
		}

		public JSONNumber(string aData)
		{
			Value = aData;
		}

		public override JSONNode Clone()
		{
			return new JSONNumber(m_Data);
		}

		internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode)
		{
			aSB.Append(Value);
		}

		private static bool IsNumeric(object value)
		{
			if (!(value is int) && !(value is uint) && !(value is float) && !(value is double) && !(value is decimal) && !(value is long) && !(value is ulong) && !(value is short) && !(value is ushort) && !(value is sbyte))
			{
				return value is byte;
			}
			return true;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (base.Equals(obj))
			{
				return true;
			}
			JSONNumber jSONNumber = obj as JSONNumber;
			if (jSONNumber != null)
			{
				return m_Data == jSONNumber.m_Data;
			}
			if (IsNumeric(obj))
			{
				return Convert.ToDouble(obj) == m_Data;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return m_Data.GetHashCode();
		}

		public override void Clear()
		{
			m_Data = 0.0;
		}
	}
}
