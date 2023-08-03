using UnityEngine;

internal class RegionTrigger : MonoBehaviour
{
	public string myName;

	public RCEvent playerEventEnter;

	public RCEvent playerEventExit;

	public RCEvent titanEventEnter;

	public RCEvent titanEventExit;

	public void CopyTrigger(RegionTrigger copyTrigger)
	{
		playerEventEnter = copyTrigger.playerEventEnter;
		titanEventEnter = copyTrigger.titanEventEnter;
		playerEventExit = copyTrigger.playerEventExit;
		titanEventExit = copyTrigger.titanEventExit;
		myName = copyTrigger.myName;
	}

	private void OnTriggerEnter(Collider other)
	{
		GameObject gameObject = other.transform.gameObject;
		if (gameObject.layer == 8)
		{
			if (playerEventEnter == null)
			{
				return;
			}
			HERO component = gameObject.GetComponent<HERO>();
			if (component != null)
			{
				string key = (string)FengGameManagerMKII.RCVariableNames["OnPlayerEnterRegion[" + myName + "]"];
				if (FengGameManagerMKII.playerVariables.ContainsKey(key))
				{
					FengGameManagerMKII.playerVariables[key] = component.photonView.owner;
				}
				else
				{
					FengGameManagerMKII.playerVariables.Add(key, component.photonView.owner);
				}
				playerEventEnter.checkEvent();
			}
		}
		else
		{
			if (gameObject.layer != 11 || titanEventEnter == null)
			{
				return;
			}
			TITAN component2 = gameObject.transform.root.gameObject.GetComponent<TITAN>();
			if (component2 != null)
			{
				string key = (string)FengGameManagerMKII.RCVariableNames["OnTitanEnterRegion[" + myName + "]"];
				if (FengGameManagerMKII.titanVariables.ContainsKey(key))
				{
					FengGameManagerMKII.titanVariables[key] = component2;
				}
				else
				{
					FengGameManagerMKII.titanVariables.Add(key, component2);
				}
				titanEventEnter.checkEvent();
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		GameObject gameObject = other.transform.root.gameObject;
		if (gameObject.layer == 8)
		{
			if (playerEventExit == null)
			{
				return;
			}
			HERO component = gameObject.GetComponent<HERO>();
			if (component != null)
			{
				string key = (string)FengGameManagerMKII.RCVariableNames["OnPlayerLeaveRegion[" + myName + "]"];
				if (FengGameManagerMKII.playerVariables.ContainsKey(key))
				{
					FengGameManagerMKII.playerVariables[key] = component.photonView.owner;
				}
				else
				{
					FengGameManagerMKII.playerVariables.Add(key, component.photonView.owner);
				}
			}
		}
		else
		{
			if (gameObject.layer != 11 || titanEventExit == null)
			{
				return;
			}
			TITAN component2 = gameObject.GetComponent<TITAN>();
			if (component2 != null)
			{
				string key = (string)FengGameManagerMKII.RCVariableNames["OnTitanLeaveRegion[" + myName + "]"];
				if (FengGameManagerMKII.titanVariables.ContainsKey(key))
				{
					FengGameManagerMKII.titanVariables[key] = component2;
				}
				else
				{
					FengGameManagerMKII.titanVariables.Add(key, component2);
				}
				titanEventExit.checkEvent();
			}
		}
	}
}
