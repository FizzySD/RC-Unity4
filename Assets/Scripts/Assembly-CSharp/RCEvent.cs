using System.Collections.Generic;

internal class RCEvent
{
	public enum foreachType
	{
		titan = 0,
		player = 1
	}

	public enum loopType
	{
		noLoop = 0,
		ifLoop = 1,
		foreachLoop = 2,
		whileLoop = 3
	}

	private RCCondition condition;

	private RCAction elseAction;

	private int eventClass;

	private int eventType;

	public string foreachVariableName;

	public List<RCAction> trueActions;

	public RCEvent(RCCondition sentCondition, List<RCAction> sentTrueActions, int sentClass, int sentType)
	{
		condition = sentCondition;
		trueActions = sentTrueActions;
		eventClass = sentClass;
		eventType = sentType;
	}

	public void checkEvent()
	{
		switch (eventClass)
		{
		default:
			return;
		case 0:
		{
			for (int j = 0; j < trueActions.Count; j++)
			{
				trueActions[j].doAction();
			}
			return;
		}
		case 1:
			if (!condition.checkCondition())
			{
				if (elseAction != null)
				{
					elseAction.doAction();
				}
			}
			else
			{
				for (int j = 0; j < trueActions.Count; j++)
				{
					trueActions[j].doAction();
				}
			}
			return;
		case 2:
			switch (eventType)
			{
			case 0:
			{
				foreach (TITAN titan in FengGameManagerMKII.instance.getTitans())
				{
					if (FengGameManagerMKII.titanVariables.ContainsKey(foreachVariableName))
					{
						FengGameManagerMKII.titanVariables[foreachVariableName] = titan;
					}
					else
					{
						FengGameManagerMKII.titanVariables.Add(foreachVariableName, titan);
					}
					foreach (RCAction trueAction in trueActions)
					{
						trueAction.doAction();
					}
				}
				break;
			}
			case 1:
			{
				PhotonPlayer[] playerList = PhotonNetwork.playerList;
				foreach (PhotonPlayer value in playerList)
				{
					if (FengGameManagerMKII.playerVariables.ContainsKey(foreachVariableName))
					{
						FengGameManagerMKII.playerVariables[foreachVariableName] = value;
					}
					else
					{
						FengGameManagerMKII.titanVariables.Add(foreachVariableName, value);
					}
					foreach (RCAction trueAction2 in trueActions)
					{
						trueAction2.doAction();
					}
				}
				break;
			}
			}
			return;
		case 3:
			break;
		}
		while (condition.checkCondition())
		{
			foreach (RCAction trueAction3 in trueActions)
			{
				trueAction3.doAction();
			}
		}
	}

	public void setElse(RCAction sentElse)
	{
		elseAction = sentElse;
	}
}
