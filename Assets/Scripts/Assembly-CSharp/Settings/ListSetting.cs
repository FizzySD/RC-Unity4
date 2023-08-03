using System.Collections.Generic;
using SimpleJSONFixed;

namespace Settings
{
	internal class ListSetting<T> : TypedSetting<List<T>>, IListSetting where T : BaseSetting, new()
	{
		public ListSetting(List<T> defaultValue)
			: base(defaultValue)
		{
		}

		public ListSetting(T defaultValue)
		{
			DefaultValue = new List<T> { defaultValue };
			SetDefault();
		}

		public ListSetting(T defaultValue, int count)
		{
			List<T> list = new List<T>();
			JSONNode json = defaultValue.SerializeToJsonObject();
			for (int i = 0; i < count; i++)
			{
				T val = new T();
				CopyLimits(defaultValue, val);
				val.DeserializeFromJsonObject(json);
				list.Add(val);
			}
			DefaultValue = list;
			SetDefault();
		}

		public ListSetting()
		{
			DefaultValue = new List<T>();
			SetDefault();
		}

		public override void SetDefault()
		{
			List<T> list = new List<T>();
			foreach (T item in DefaultValue)
			{
				T val = new T();
				CopyDefaultLimits(val);
				val.DeserializeFromJsonObject(item.SerializeToJsonObject());
				list.Add(val);
			}
			base.Value = list;
		}

		public override void DeserializeFromJsonObject(JSONNode json)
		{
			List<T> list = new List<T>();
			JSONArray asArray = json.AsArray;
			JSONNode.Enumerator enumerator = asArray.GetEnumerator();
			while (enumerator.MoveNext())
			{
				JSONNode json2 = enumerator.Current;
				T val = new T();
				CopyDefaultLimits(val);
				val.DeserializeFromJsonObject(json2);
				list.Add(val);
			}
			base.Value = list;
		}

		public override JSONNode SerializeToJsonObject()
		{
			JSONArray jSONArray = new JSONArray();
			foreach (T item in base.Value)
			{
				jSONArray.Add(item.SerializeToJsonObject());
			}
			return jSONArray;
		}

		public int GetCount()
		{
			return base.Value.Count;
		}

		public BaseSetting GetItemAt(int index)
		{
			return base.Value[index];
		}

		public List<BaseSetting> GetItems()
		{
			List<BaseSetting> list = new List<BaseSetting>();
			foreach (T item in base.Value)
			{
				list.Add(item);
			}
			return list;
		}

		public void AddItem(BaseSetting item)
		{
			base.Value.Add((T)item);
		}

		public void Clear()
		{
			base.Value.Clear();
		}

		private void CopyLimits(T from, T to)
		{
			if (from is IntSetting)
			{
				((IntSetting)(object)to).MinValue = ((IntSetting)(object)from).MinValue;
				((IntSetting)(object)to).MaxValue = ((IntSetting)(object)from).MaxValue;
			}
			else if (from is ColorSetting)
			{
				((ColorSetting)(object)to).MinAlpha = ((ColorSetting)(object)from).MinAlpha;
			}
			else if (from is FloatSetting)
			{
				((FloatSetting)(object)to).MinValue = ((FloatSetting)(object)from).MinValue;
				((FloatSetting)(object)to).MaxValue = ((FloatSetting)(object)from).MaxValue;
			}
			else if (from is StringSetting)
			{
				((StringSetting)(object)to).MaxLength = ((StringSetting)(object)from).MaxLength;
			}
		}

		private void CopyDefaultLimits(T to)
		{
			if (DefaultValue.Count > 0)
			{
				CopyLimits(DefaultValue[0], to);
			}
		}
	}
}
