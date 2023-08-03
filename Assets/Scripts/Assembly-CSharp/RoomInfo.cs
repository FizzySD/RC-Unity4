using ExitGames.Client.Photon;

public class RoomInfo
{
	protected bool autoCleanUpField = PhotonNetwork.autoCleanUpPlayerObjects;

	private Hashtable customPropertiesField = new Hashtable();

	protected byte maxPlayersField;

	protected string nameField;

	protected bool openField = true;

	protected bool visibleField = true;

	public Hashtable customProperties
	{
		get
		{
			return customPropertiesField;
		}
	}

	public bool isLocalClientInside { get; set; }

	public byte maxPlayers
	{
		get
		{
			return maxPlayersField;
		}
	}

	public string name
	{
		get
		{
			return nameField;
		}
	}

	public bool open
	{
		get
		{
			return openField;
		}
	}

	public int playerCount { get; private set; }

	public bool removedFromList { get; internal set; }

	public bool visible
	{
		get
		{
			return visibleField;
		}
	}

	protected internal RoomInfo(string roomName, Hashtable properties)
	{
		CacheProperties(properties);
		nameField = roomName;
	}

	protected internal void CacheProperties(Hashtable propertiesToCache)
	{
		if (propertiesToCache == null || propertiesToCache.Count == 0 || customPropertiesField.Equals(propertiesToCache))
		{
			return;
		}
		if (propertiesToCache.ContainsKey((byte)251))
		{
			removedFromList = (bool)propertiesToCache[(byte)251];
			if (removedFromList)
			{
				return;
			}
		}
		if (propertiesToCache.ContainsKey(byte.MaxValue))
		{
			maxPlayersField = (byte)propertiesToCache[byte.MaxValue];
		}
		if (propertiesToCache.ContainsKey((byte)253))
		{
			openField = (bool)propertiesToCache[(byte)253];
		}
		if (propertiesToCache.ContainsKey((byte)254))
		{
			visibleField = (bool)propertiesToCache[(byte)254];
		}
		if (propertiesToCache.ContainsKey((byte)252))
		{
			playerCount = (byte)propertiesToCache[(byte)252];
		}
		if (propertiesToCache.ContainsKey((byte)249))
		{
			autoCleanUpField = (bool)propertiesToCache[(byte)249];
		}
		customPropertiesField.MergeStringKeys(propertiesToCache);
	}

	public override bool Equals(object p)
	{
		Room room = p as Room;
		if (room != null)
		{
			return nameField.Equals(room.nameField);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return nameField.GetHashCode();
	}

	public override string ToString()
	{
		object[] args = new object[5]
		{
			nameField,
			(!visibleField) ? "hidden" : "visible",
			(!openField) ? "closed" : "open",
			maxPlayersField,
			playerCount
		};
		return string.Format("Room: '{0}' {1},{2} {4}/{3} players.", args);
	}

	public string ToStringFull()
	{
		object[] args = new object[6]
		{
			nameField,
			(!visibleField) ? "hidden" : "visible",
			(!openField) ? "closed" : "open",
			maxPlayersField,
			playerCount,
			customPropertiesField.ToStringFull()
		};
		return string.Format("Room: '{0}' {1},{2} {4}/{3} players.\ncustomProps: {5}", args);
	}
}
