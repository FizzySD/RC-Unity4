using System.IO;
using System.Text;
using UnityEngine;

public class ReplaySaver : MonoBehaviour
{
	public static void SaveReplay(string text, string fileName)
	{
		byte[] bytes = Encoding.ASCII.GetBytes(text);
		if (!Directory.Exists(Application.dataPath + "/UserData/Replays"))
		{
			Directory.CreateDirectory(Application.dataPath + "/UserData/Replays");
		}
		File.WriteAllBytes(Application.dataPath + "/UserData/Replays/" + fileName, bytes);
	}
}
