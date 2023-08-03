internal class RCCondition
{
	public enum castTypes
	{
		typeInt = 0,
		typeBool = 1,
		typeString = 2,
		typeFloat = 3,
		typePlayer = 4,
		typeTitan = 5
	}

	public enum operands
	{
		lt = 0,
		lte = 1,
		e = 2,
		gte = 3,
		gt = 4,
		ne = 5
	}

	public enum stringOperands
	{
		equals = 0,
		notEquals = 1,
		contains = 2,
		notContains = 3,
		startsWith = 4,
		notStartsWith = 5,
		endsWith = 6,
		notEndsWith = 7
	}

	private int operand;

	private RCActionHelper parameter1;

	private RCActionHelper parameter2;

	private int type;

	public RCCondition(int sentOperand, int sentType, RCActionHelper sentParam1, RCActionHelper sentParam2)
	{
		operand = sentOperand;
		type = sentType;
		parameter1 = sentParam1;
		parameter2 = sentParam2;
	}

	private bool boolCompare(bool baseBool, bool compareBool)
	{
		switch (operand)
		{
		case 2:
			return baseBool == compareBool;
		case 5:
			return baseBool != compareBool;
		default:
			return false;
		}
	}

	public bool checkCondition()
	{
		switch (type)
		{
		case 0:
			return intCompare(parameter1.returnInt(null), parameter2.returnInt(null));
		case 1:
			return boolCompare(parameter1.returnBool(null), parameter2.returnBool(null));
		case 2:
			return stringCompare(parameter1.returnString(null), parameter2.returnString(null));
		case 3:
			return floatCompare(parameter1.returnFloat(null), parameter2.returnFloat(null));
		case 4:
			return playerCompare(parameter1.returnPlayer(null), parameter2.returnPlayer(null));
		case 5:
			return titanCompare(parameter1.returnTitan(null), parameter2.returnTitan(null));
		default:
			return false;
		}
	}

	private bool floatCompare(float baseFloat, float compareFloat)
	{
		switch (operand)
		{
		case 0:
			if (baseFloat >= compareFloat)
			{
				return false;
			}
			return true;
		case 1:
			if (baseFloat > compareFloat)
			{
				return false;
			}
			return true;
		case 2:
			if (baseFloat != compareFloat)
			{
				return false;
			}
			return true;
		case 3:
			if (baseFloat < compareFloat)
			{
				return false;
			}
			return true;
		case 4:
			if (baseFloat <= compareFloat)
			{
				return false;
			}
			return true;
		case 5:
			if (baseFloat == compareFloat)
			{
				return false;
			}
			return true;
		default:
			return false;
		}
	}

	private bool intCompare(int baseInt, int compareInt)
	{
		switch (operand)
		{
		case 0:
			if (baseInt >= compareInt)
			{
				return false;
			}
			return true;
		case 1:
			if (baseInt > compareInt)
			{
				return false;
			}
			return true;
		case 2:
			if (baseInt != compareInt)
			{
				return false;
			}
			return true;
		case 3:
			if (baseInt < compareInt)
			{
				return false;
			}
			return true;
		case 4:
			if (baseInt <= compareInt)
			{
				return false;
			}
			return true;
		case 5:
			if (baseInt == compareInt)
			{
				return false;
			}
			return true;
		default:
			return false;
		}
	}

	private bool playerCompare(PhotonPlayer basePlayer, PhotonPlayer comparePlayer)
	{
		switch (operand)
		{
		case 2:
			return basePlayer == comparePlayer;
		case 5:
			return basePlayer != comparePlayer;
		default:
			return false;
		}
	}

	private bool stringCompare(string baseString, string compareString)
	{
		switch (operand)
		{
		case 0:
			if (!(baseString == compareString))
			{
				return false;
			}
			return true;
		case 1:
			if (!(baseString != compareString))
			{
				return false;
			}
			return true;
		case 2:
			if (!baseString.Contains(compareString))
			{
				return false;
			}
			return true;
		case 3:
			if (baseString.Contains(compareString))
			{
				return false;
			}
			return true;
		case 4:
			if (!baseString.StartsWith(compareString))
			{
				return false;
			}
			return true;
		case 5:
			if (baseString.StartsWith(compareString))
			{
				return false;
			}
			return true;
		case 6:
			if (!baseString.EndsWith(compareString))
			{
				return false;
			}
			return true;
		case 7:
			if (baseString.EndsWith(compareString))
			{
				return false;
			}
			return true;
		default:
			return false;
		}
	}

	private bool titanCompare(TITAN baseTitan, TITAN compareTitan)
	{
		switch (operand)
		{
		case 2:
			return baseTitan == compareTitan;
		case 5:
			return baseTitan != compareTitan;
		default:
			return false;
		}
	}
}
