using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InvBaseItem
{
	public enum Slot
	{
		None = 0,
		Weapon = 1,
		Shield = 2,
		Body = 3,
		Shoulders = 4,
		Bracers = 5,
		Boots = 6,
		Trinket = 7,
		_LastDoNotUse = 8
	}

	public GameObject attachment;

	public Color color = Color.white;

	public string description;

	public UIAtlas iconAtlas;

	public string iconName = string.Empty;

	public int id16;

	public int maxItemLevel = 50;

	public int minItemLevel = 1;

	public string name;

	public Slot slot;

	public List<InvStat> stats = new List<InvStat>();
}
