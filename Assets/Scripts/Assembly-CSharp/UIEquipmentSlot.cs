using UnityEngine;

[AddComponentMenu("NGUI/Examples/UI Equipment Slot")]
public class UIEquipmentSlot : UIItemSlot
{
	public InvEquipment equipment;

	public InvBaseItem.Slot slot;

	protected override InvGameItem observedItem
	{
		get
		{
			if (!(equipment == null))
			{
				return equipment.GetItem(slot);
			}
			return null;
		}
	}

	protected override InvGameItem Replace(InvGameItem item)
	{
		if (!(equipment == null))
		{
			return equipment.Replace(slot, item);
		}
		return item;
	}
}
