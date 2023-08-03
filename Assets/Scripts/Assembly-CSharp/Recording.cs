using System.Collections.Generic;
using UnityEngine;

public class Recording
{
	public GameObject replayobj2;

	public GameObject healthLabel = (GameObject)Object.Instantiate(Resources.Load("UI/LabelNameOverHead"));

	private Queue<ReplayData> originalQueue;

	private Queue<ReplayData> replayQueue;

	public ReplayObject replayObj { get; private set; }

	public Recording(Queue<ReplayData> recordingQueue)
	{
		originalQueue = new Queue<ReplayData>(recordingQueue);
		replayQueue = new Queue<ReplayData>(recordingQueue);
	}

	public void RestartReplay()
	{
		replayQueue = new Queue<ReplayData>(originalQueue);
	}

	public bool PlayNextFrame()
	{
		if (replayObj == null)
		{
			logger.addLINE("You have not instantiated replayGameobject");
		}
		bool result = false;
		if (replayQueue.Count != 0)
		{
			ReplayData dataForFrame = replayQueue.Dequeue();
			replayObj.SetDataForFrame(dataForFrame);
			result = true;
		}
		return result;
	}

	public void InstantiateReplayObject()
	{
		replayobj2 = (GameObject)Object.Instantiate(Resources.Load("AOTTG_HERO 1") as GameObject, new Vector3(0f, 0f, 0f), Quaternion.identity);
		HERO component = GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().main_object.GetComponent<HERO>();
		if (component != null)
		{
			HERO_SETUP component2 = component.GetComponent<HERO_SETUP>();
			HeroCostume myCostume = component2.myCostume;
			replayobj2.GetComponent<HERO>().GetComponent<HERO_SETUP>().init();
			replayobj2.GetComponent<HERO>().GetComponent<HERO_SETUP>().myCostume = myCostume;
			replayobj2.GetComponent<HERO>().GetComponent<HERO_SETUP>().myCostume.stat = myCostume.stat;
			replayobj2.GetComponent<HERO>().GetComponent<HERO_SETUP>().setCharacterComponent();
			replayobj2.GetComponent<HERO>().setStat2();
			replayobj2.GetComponent<HERO>().setSkillHUDPosition2();
			CostumeConeveter.HeroCostumeToPhotonData2(replayobj2.GetComponent<HERO>().GetComponent<HERO_SETUP>().myCostume, PhotonNetwork.player);
			replayobj2.GetComponent<HERO>().hasDied = true;
			Object.Destroy(replayobj2.GetComponent<PhotonView>());
			healthLabel.name = "LabelNameOverHead";
			healthLabel.transform.localPosition = new Vector3(0f, 5f, 0f);
			healthLabel.transform.localScale = new Vector3(5f, 5f, 5f);
			healthLabel.GetComponent<UILabel>().text = "Ghost";
			healthLabel.transform.parent = replayobj2.transform;
			healthLabel.transform.parent = replayobj2.transform;
			if (replayQueue.Count != 0)
			{
				ReplayData replayData = replayQueue.Peek();
				replayobj2.AddComponent<ReplayObject>();
				replayobj2.AddComponent<Rigidbody>();
				replayobj2.GetComponent<Rigidbody>().isKinematic = true;
				replayObj = replayobj2.GetComponent<ReplayObject>();
			}
		}
	}

	public void DestroyReplayObjectIfExists()
	{
		if (replayObj != null)
		{
			Object.Destroy(replayObj.gameObject);
			Object.Destroy(replayobj2);
		}
	}
}
