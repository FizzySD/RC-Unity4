using Photon;
using UnityEngine;

public class ReplayObject : Photon.MonoBehaviour
{
	private GameObject lineRenderer = new GameObject();

	private GameObject lineRenderer1 = new GameObject();

	public void Awake()
	{
		Material material = new Material(Shader.Find("Specular"));
		lineRenderer.AddComponent<LineRenderer>();
		lineRenderer.GetComponent<LineRenderer>().SetWidth(0.2f, 0.2f);
		lineRenderer.GetComponent<LineRenderer>().material = material;
		lineRenderer.GetComponent<LineRenderer>().material.color = Color.cyan;
		lineRenderer.GetComponent<LineRenderer>().SetColors(Color.blue, Color.blue);
		lineRenderer.GetComponent<LineRenderer>().SetVertexCount(2);
		lineRenderer1.AddComponent<LineRenderer>();
		lineRenderer1.GetComponent<LineRenderer>().SetWidth(0.2f, 0.2f);
		lineRenderer1.GetComponent<LineRenderer>().material = material;
		lineRenderer1.GetComponent<LineRenderer>().material.color = Color.red;
		lineRenderer1.GetComponent<LineRenderer>().SetColors(Color.blue, Color.blue);
		lineRenderer1.GetComponent<LineRenderer>().SetVertexCount(2);
	}

	public void SetDataForFrame(ReplayData data)
	{
		base.transform.position = data.position;
		base.transform.rotation = data.rotation;
		GetComponent<HERO>().animation.Play(data.animId);
		if (data.LeftHookPos != new Vector3(0f, 0f, 0f))
		{
			lineRenderer.GetComponent<LineRenderer>().enabled = true;
			lineRenderer.GetComponent<LineRenderer>().SetPosition(0, base.transform.position);
			lineRenderer.GetComponent<LineRenderer>().SetPosition(1, data.LeftHookPos);
		}
		else
		{
			lineRenderer.GetComponent<LineRenderer>().enabled = false;
		}
		if (data.RightHookPos != new Vector3(0f, 0f, 0f))
		{
			lineRenderer1.GetComponent<LineRenderer>().enabled = true;
			lineRenderer1.GetComponent<LineRenderer>().SetPosition(0, base.transform.position);
			lineRenderer1.GetComponent<LineRenderer>().SetPosition(1, data.RightHookPos);
		}
		else
		{
			lineRenderer1.GetComponent<LineRenderer>().enabled = false;
		}
		if (data.isDashing)
		{
			PhotonNetwork.Instantiate("FX/boost_smoke", base.transform.position, base.transform.rotation, 0);
		}
		if (data.isGhostBoosting)
		{
			GetComponent<HERO>().SetGhostSmoke(1);
		}
		else
		{
			GetComponent<HERO>().SetGhostSmoke(0);
		}
	}
}
