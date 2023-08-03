using System.Collections.Generic;

namespace Settings
{
	internal interface IListSetting
	{
		int GetCount();

		BaseSetting GetItemAt(int index);

		List<BaseSetting> GetItems();

		void AddItem(BaseSetting item);

		void Clear();
	}
}
