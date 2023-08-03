using System;
using System.Collections.Generic;
using System.Reflection;

namespace Utility
{
	internal class BaseCSVObject
	{
		private static Dictionary<Type, FieldInfo[]> _fields = new Dictionary<Type, FieldInfo[]>();

		protected virtual char Delimiter
		{
			get
			{
				return ',';
			}
		}

		protected virtual char ParamDelimiter
		{
			get
			{
				return ':';
			}
		}

		protected virtual bool NamedParams
		{
			get
			{
				return false;
			}
		}

		public virtual string Serialize()
		{
			List<string> list = new List<string>();
			FieldInfo[] fields = GetFields();
			for (int i = 0; i < fields.Length; i++)
			{
				string item = SerializeField(fields[i], this);
				list.Add(item);
			}
			return string.Join(Delimiter.ToString(), list.ToArray());
		}

		public virtual void Deserialize(string csv)
		{
			string[] array = csv.Split(Delimiter);
			FieldInfo[] fields = GetFields();
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = array[i].Trim();
			}
			if (NamedParams)
			{
				string[] array2 = array;
				foreach (string text in array2)
				{
					string[] array3 = text.Split(ParamDelimiter);
					FieldInfo fieldInfo = FindField(array3[0]);
					if (fieldInfo != null)
					{
						DeserializeField(fieldInfo, this, array3[1]);
					}
				}
				return;
			}
			for (int k = 0; k < fields.Length; k++)
			{
				if (IsList(fields[k]))
				{
					Type t = fields[k].FieldType.GetGenericArguments()[0];
					List<object> list = (List<object>)fields[k].GetValue(this);
					list.Clear();
					int num = k;
					while (num < array.Length)
					{
						list.Add(DeserializeValue(t, array[num]));
						k++;
					}
					break;
				}
				DeserializeField(fields[k], this, array[k]);
			}
		}

		protected virtual FieldInfo[] GetFields()
		{
			Type type = GetType();
			if (!_fields.ContainsKey(type))
			{
				_fields.Add(type, type.GetFields());
			}
			return _fields[type];
		}

		protected virtual FieldInfo FindField(string name)
		{
			FieldInfo[] array = _fields[GetType()];
			foreach (FieldInfo fieldInfo in array)
			{
				if (fieldInfo.Name == name)
				{
					return fieldInfo;
				}
			}
			return null;
		}

		protected virtual bool IsList(FieldInfo field)
		{
			if (field.FieldType.IsGenericType)
			{
				return field.FieldType.GetGenericTypeDefinition() == typeof(IList<>);
			}
			return false;
		}

		protected virtual string SerializeField(FieldInfo info, object instance)
		{
			string text = string.Empty;
			if (NamedParams)
			{
				text = info.Name + ParamDelimiter;
			}
			if (IsList(info))
			{
				List<string> list = new List<string>();
				Type t = info.FieldType.GetGenericArguments()[0];
				foreach (object item in (List<object>)info.GetValue(instance))
				{
					list.Add(SerializeValue(t, item));
				}
				return text + string.Join(Delimiter.ToString(), list.ToArray());
			}
			return text + SerializeValue(info.FieldType, info.GetValue(instance));
		}

		protected virtual void DeserializeField(FieldInfo info, object instance, string value)
		{
			info.SetValue(instance, DeserializeValue(info.FieldType, value));
		}

		protected virtual string SerializeValue(Type t, object value)
		{
			if (t == typeof(string))
			{
				return (string)value;
			}
			if (t == typeof(int) || t == typeof(float))
			{
				return value.ToString();
			}
			if (t == typeof(bool))
			{
				return Convert.ToInt32(value).ToString();
			}
			if (typeof(BaseCSVObject).IsAssignableFrom(t))
			{
				return ((BaseCSVObject)value).Serialize();
			}
			return string.Empty;
		}

		protected virtual object DeserializeValue(Type t, string value)
		{
			if (t == typeof(string))
			{
				return value;
			}
			if (t == typeof(int))
			{
				return int.Parse(value);
			}
			if (t == typeof(float))
			{
				return float.Parse(value);
			}
			if (t == typeof(bool))
			{
				return Convert.ToBoolean(int.Parse(value));
			}
			if (typeof(BaseCSVObject).IsAssignableFrom(t))
			{
				BaseCSVObject baseCSVObject = (BaseCSVObject)Activator.CreateInstance(t);
				baseCSVObject.Deserialize(value);
				return baseCSVObject;
			}
			return null;
		}
	}
}
