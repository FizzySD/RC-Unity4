using System.Collections.Generic;

namespace ExitGames.Client.Photon
{
	public class EventData
	{
		public byte Code;

		public Dictionary<byte, object> Parameters;

		public byte SenderKey = 254;

		private int sender = -1;

		public byte CustomDataKey = 245;

		private object customData;

		public object this[byte key]
		{
			get
			{
				if (Parameters == null)
				{
					return null;
				}
				object value;
				Parameters.TryGetValue(key, out value);
				return value;
			}
			internal set
			{
				if (Parameters == null)
				{
					Parameters = new Dictionary<byte, object>();
				}
				Parameters[key] = value;
			}
		}

		public int Sender
		{
			get
			{
				if (sender == -1)
				{
					object obj = this[SenderKey];
					sender = ((obj != null) ? ((int)obj) : (-1));
				}
				return sender;
			}
			internal set
			{
				sender = value;
			}
		}

		public object CustomData
		{
			get
			{
				if (customData == null)
				{
					customData = this[CustomDataKey];
				}
				return customData;
			}
			internal set
			{
				customData = value;
			}
		}

		internal void Reset()
		{
			Code = 0;
			Parameters.Clear();
			sender = -1;
			customData = null;
		}

		public override string ToString()
		{
			return string.Format("Event {0}.", Code.ToString());
		}

		public string ToStringFull()
		{
			return string.Format("Event {0}: {1}", Code, SupportClass.DictionaryToString(Parameters));
		}
	}
}
