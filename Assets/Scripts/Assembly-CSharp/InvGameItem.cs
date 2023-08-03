using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InvGameItem
{
	public enum Quality
	{
		Broken = 0,
		Cursed = 1,
		Damaged = 2,
		Worn = 3,
		Sturdy = 4,
		Polished = 5,
		Improved = 6,
		Crafted = 7,
		Superior = 8,
		Enchanted = 9,
		Epic = 10,
		Legendary = 11,
		_LastDoNotUse = 12
	}

	public int itemLevel;

	private InvBaseItem mBaseItem;

	[SerializeField]
	private int mBaseItemID;

	public Quality quality;

	public InvBaseItem baseItem
	{
		get
		{
			if (mBaseItem == null)
			{
				mBaseItem = InvDatabase.FindByID(baseItemID);
			}
			return mBaseItem;
		}
	}

	public int baseItemID
	{
		get
		{
			return mBaseItemID;
		}
	}

	public Color color
	{
		get
		{
			Color white = Color.white;
			switch (quality)
			{
			case Quality.Broken:
				return new Color(0.4f, 0.2f, 0.2f);
			case Quality.Cursed:
				return Color.red;
			case Quality.Damaged:
				return new Color(0.4f, 0.4f, 0.4f);
			case Quality.Worn:
				return new Color(0.7f, 0.7f, 0.7f);
			case Quality.Sturdy:
				return new Color(1f, 1f, 1f);
			case Quality.Polished:
				return NGUIMath.HexToColor(3774856959u);
			case Quality.Improved:
				return NGUIMath.HexToColor(2480359935u);
			case Quality.Crafted:
				return NGUIMath.HexToColor(1325334783u);
			case Quality.Superior:
				return NGUIMath.HexToColor(12255231u);
			case Quality.Enchanted:
				return NGUIMath.HexToColor(1937178111u);
			case Quality.Epic:
				return NGUIMath.HexToColor(2516647935u);
			case Quality.Legendary:
				return NGUIMath.HexToColor(4287627519u);
			default:
				return white;
			}
		}
	}

	public string name
	{
		get
		{
			if (baseItem == null)
			{
				return null;
			}
			return quality.ToString() + " " + baseItem.name;
		}
	}

	public float statMultiplier
	{
		get
		{
			float num = 0f;
			switch (quality)
			{
			case Quality.Broken:
				num = 0f;
				break;
			case Quality.Cursed:
				num = -1f;
				break;
			case Quality.Damaged:
				num = 0.25f;
				break;
			case Quality.Worn:
				num = 0.9f;
				break;
			case Quality.Sturdy:
				num = 1f;
				break;
			case Quality.Polished:
				num = 1.1f;
				break;
			case Quality.Improved:
				num = 1.25f;
				break;
			case Quality.Crafted:
				num = 1.5f;
				break;
			case Quality.Superior:
				num = 1.75f;
				break;
			case Quality.Enchanted:
				num = 2f;
				break;
			case Quality.Epic:
				num = 2.5f;
				break;
			case Quality.Legendary:
				num = 3f;
				break;
			}
			float num2 = (float)itemLevel / 50f;
			return num * Mathf.Lerp(num2, num2 * num2, 0.5f);
		}
	}

	public InvGameItem(int id)
	{
		quality = Quality.Sturdy;
		itemLevel = 1;
		mBaseItemID = id;
	}

	public InvGameItem(int id, InvBaseItem bi)
	{
		quality = Quality.Sturdy;
		itemLevel = 1;
		mBaseItemID = id;
		mBaseItem = bi;
	}

	public List<InvStat> CalculateStats()
	{
		List<InvStat> list = new List<InvStat>();
		if (baseItem != null)
		{
			float num = statMultiplier;
			List<InvStat> stats = baseItem.stats;
			int i = 0;
			for (int count = stats.Count; i < count; i++)
			{
				InvStat invStat = stats[i];
				int num2 = Mathf.RoundToInt(num * (float)invStat.amount);
				if (num2 == 0)
				{
					continue;
				}
				bool flag = false;
				int j = 0;
				for (int count2 = list.Count; j < count2; j++)
				{
					InvStat invStat2 = list[j];
					if (invStat2.id == invStat.id && invStat2.modifier == invStat.modifier)
					{
						invStat2.amount += num2;
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					InvStat item = new InvStat
					{
						id = invStat.id,
						amount = num2,
						modifier = invStat.modifier
					};
					list.Add(item);
				}
			}
			list.Sort(InvStat.CompareArmor);
		}
		return list;
	}
}
