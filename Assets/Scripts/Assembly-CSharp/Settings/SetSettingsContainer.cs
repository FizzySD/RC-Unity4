using System;
using System.Collections.Generic;

namespace Settings
{
	internal class SetSettingsContainer<T> : BaseSettingsContainer, ISetSettingsContainer where T : BaseSetSetting, new()
	{
		public IntSetting SelectedSetIndex = new IntSetting(0, 0);

		public ListSetting<T> Sets = new ListSetting<T>(new T());

		protected override bool Validate()
		{
			return Sets.GetCount() > 0;
		}

		public BaseSetSetting GetSelectedSet()
		{
			int value = SelectedSetIndex.Value;
			value = Math.Min(value, Sets.GetCount() - 1);
			value = Math.Max(value, 0);
			return (BaseSetSetting)Sets.GetItemAt(value);
		}

		public IntSetting GetSelectedSetIndex()
		{
			return SelectedSetIndex;
		}

		public void CreateSet(string name)
		{
			T item = new T
			{
				Name = 
				{
					Value = name
				}
			};
			Sets.Value.Add(item);
		}

		public void CopySelectedSet(string name)
		{
			T val = new T();
			val.Copy(GetSelectedSet());
			val.Name.Value = name;
			val.Preset.Value = false;
			Sets.Value.Add(val);
		}

		public bool CanDeleteSelectedSet()
		{
			if (Sets.GetCount() > 1)
			{
				return CanEditSelectedSet();
			}
			return false;
		}

		public bool CanEditSelectedSet()
		{
			return !GetSelectedSet().Preset.Value;
		}

		public void DeleteSelectedSet()
		{
			Sets.Value.Remove((T)GetSelectedSet());
		}

		public IListSetting GetSets()
		{
			return Sets;
		}

		public void SetPresetsFromJsonString(string json)
		{
			SetSettingsContainer<T> setSettingsContainer = new SetSettingsContainer<T>();
			setSettingsContainer.DeserializeFromJsonString(json);
			Sets.Value.RemoveAll((T x) => x.Preset.Value);
			for (int i = 0; i < setSettingsContainer.Sets.Value.Count; i++)
			{
				setSettingsContainer.Sets.Value[i].Preset.Value = true;
				Sets.Value.Insert(i, setSettingsContainer.Sets.Value[i]);
			}
		}

		public string[] GetSetNames()
		{
			List<string> list = new List<string>();
			foreach (BaseSetSetting item in Sets.GetItems())
			{
				list.Add(item.Name.Value);
			}
			return list.ToArray();
		}
	}
}
