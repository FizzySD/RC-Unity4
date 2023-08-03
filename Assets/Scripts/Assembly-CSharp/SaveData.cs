using System.IO;
using System.Text;
using UnityEngine;

public class SaveData
{
	public string final = "";

	public void SaveReplayData(Vector3 position, Quaternion rotation, string finalAnim, Vector3 LeftHookPos, Vector3 RightHookPos, bool isGhostBoosting, bool isDashing)
	{
		final += string.Format("{0},{1},{2}\t{3},{4},{5},{6}\t{7}\t{8},{9},{10}\t{11},{12},{13}\t{14}\t{15}\n", position.x, position.y, position.z, rotation.x, rotation.y, rotation.z, rotation.w, finalAnim, LeftHookPos.x, LeftHookPos.y, LeftHookPos.z, RightHookPos.x, RightHookPos.y, RightHookPos.z, isGhostBoosting, isDashing);
	}

	public void reset()
	{
		final = "";
	}

	public void Serialize()
	{
		byte[] bytes = Encoding.ASCII.GetBytes(final);
		if (!Directory.Exists(Application.dataPath + "/UserData/Replays"))
		{
			Directory.CreateDirectory(Application.dataPath + "/UserData/Replays");
		}
		File.WriteAllBytes(Application.dataPath + "/UserData/Replays/Replay1.txt", bytes);
	}
}
