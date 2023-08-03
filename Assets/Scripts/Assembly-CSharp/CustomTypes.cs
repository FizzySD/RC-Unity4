using ExitGames.Client.Photon;
using UnityEngine;

internal static class CustomTypes
{
	public const byte PhotonPlayerCode = 80;

	public const short PhotonPlayerLength = 4;

	public const byte QuaternionCode = 81;

	public const short QuaternionLength = 16;

	public const byte Vector2Code = 87;

	public const short Vector2Length = 8;

	public const byte Vector3Code = 86;

	public const short Vector3Length = 12;

	private static object DeserializePhotonPlayer(StreamBuffer buff, short length)
	{
		int offset;
		byte[] bufferAndAdvance = buff.GetBufferAndAdvance(length, out offset);
		int value;
		Protocol.Deserialize(out value, bufferAndAdvance, ref offset);
		PhotonPlayer photonPlayer = PhotonPlayer.Find(value);
		if (photonPlayer == null)
		{
			return PhotonNetwork.player;
		}
		return photonPlayer;
	}

	private static object DeserializeQuaternion(StreamBuffer buff, short length)
	{
		Quaternion quaternion = default(Quaternion);
		int offset;
		byte[] bufferAndAdvance = buff.GetBufferAndAdvance(length, out offset);
		Protocol.Deserialize(out quaternion.w, bufferAndAdvance, ref offset);
		Protocol.Deserialize(out quaternion.x, bufferAndAdvance, ref offset);
		Protocol.Deserialize(out quaternion.y, bufferAndAdvance, ref offset);
		Protocol.Deserialize(out quaternion.z, bufferAndAdvance, ref offset);
		return quaternion;
	}

	private static object DeserializeVector2(StreamBuffer buff, short length)
	{
		Vector2 vector = default(Vector2);
		int offset;
		byte[] bufferAndAdvance = buff.GetBufferAndAdvance(length, out offset);
		Protocol.Deserialize(out vector.x, bufferAndAdvance, ref offset);
		Protocol.Deserialize(out vector.y, bufferAndAdvance, ref offset);
		return vector;
	}

	private static object DeserializeVector3(StreamBuffer buff, short length)
	{
		Vector3 vector = default(Vector3);
		int offset;
		byte[] bufferAndAdvance = buff.GetBufferAndAdvance(length, out offset);
		Protocol.Deserialize(out vector.x, bufferAndAdvance, ref offset);
		Protocol.Deserialize(out vector.y, bufferAndAdvance, ref offset);
		Protocol.Deserialize(out vector.z, bufferAndAdvance, ref offset);
		return vector;
	}

	private static short SerializePhotonPlayer(StreamBuffer buff, object customobject)
	{
		int iD = ((PhotonPlayer)customobject).ID;
		int offset;
		byte[] bufferAndAdvance = buff.GetBufferAndAdvance(4, out offset);
		Protocol.Serialize(iD, bufferAndAdvance, ref offset);
		return 4;
	}

	private static short SerializeQuaternion(StreamBuffer buff, object obj)
	{
		Quaternion quaternion = (Quaternion)obj;
		int offset;
		byte[] bufferAndAdvance = buff.GetBufferAndAdvance(16, out offset);
		Protocol.Serialize(quaternion.w, bufferAndAdvance, ref offset);
		Protocol.Serialize(quaternion.x, bufferAndAdvance, ref offset);
		Protocol.Serialize(quaternion.y, bufferAndAdvance, ref offset);
		Protocol.Serialize(quaternion.z, bufferAndAdvance, ref offset);
		return 16;
	}

	private static short SerializeVector2(StreamBuffer buff, object customobject)
	{
		Vector2 vector = (Vector2)customobject;
		int offset;
		byte[] bufferAndAdvance = buff.GetBufferAndAdvance(8, out offset);
		Protocol.Serialize(vector.x, bufferAndAdvance, ref offset);
		Protocol.Serialize(vector.y, bufferAndAdvance, ref offset);
		return 8;
	}

	private static short SerializeVector3(StreamBuffer buff, object customobject)
	{
		Vector3 vector = (Vector3)customobject;
		int offset;
		byte[] bufferAndAdvance = buff.GetBufferAndAdvance(12, out offset);
		Protocol.Serialize(vector.x, bufferAndAdvance, ref offset);
		Protocol.Serialize(vector.y, bufferAndAdvance, ref offset);
		Protocol.Serialize(vector.z, bufferAndAdvance, ref offset);
		return 12;
	}

	public static void Register()
	{
		PhotonPeer.RegisterType(typeof(Quaternion), 81, SerializeQuaternion, DeserializeQuaternion);
		PhotonPeer.RegisterType(typeof(Vector2), 87, SerializeVector2, DeserializeVector2);
		PhotonPeer.RegisterType(typeof(Vector3), 86, SerializeVector3, DeserializeVector3);
		PhotonPeer.RegisterType(typeof(PhotonPlayer), 80, SerializePhotonPlayer, DeserializePhotonPlayer);
	}
}
