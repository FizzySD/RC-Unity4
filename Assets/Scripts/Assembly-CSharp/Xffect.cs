using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Xffect")]
public class Xffect : MonoBehaviour
{
	private List<EffectLayer> EflList = new List<EffectLayer>();

	protected float ElapsedTime;

	public float LifeTime = -1f;

	private Dictionary<string, VertexPool> MatDic = new Dictionary<string, VertexPool>();

	public void Active()
	{
		foreach (Transform item in base.transform)
		{
			item.gameObject.SetActive(true);
		}
		base.gameObject.SetActive(true);
		ElapsedTime = 0f;
	}

	private void Awake()
	{
		Initialize();
	}

	public void DeActive()
	{
		foreach (Transform item in base.transform)
		{
			item.gameObject.SetActive(false);
		}
		base.gameObject.SetActive(false);
	}

	public void Initialize()
	{
		if (EflList.Count > 0)
		{
			return;
		}
		foreach (Transform item in base.transform)
		{
			EffectLayer effectLayer = (EffectLayer)item.GetComponent(typeof(EffectLayer));
			if (effectLayer != null && effectLayer.Material != null)
			{
				Material material = effectLayer.Material;
				EflList.Add(effectLayer);
				Transform transform2 = base.transform.Find("mesh " + material.name);
				if (transform2 != null)
				{
					MeshFilter meshFilter = (MeshFilter)transform2.GetComponent(typeof(MeshFilter));
					MeshRenderer meshRenderer = (MeshRenderer)transform2.GetComponent(typeof(MeshRenderer));
					meshFilter.mesh.Clear();
					MatDic[material.name] = new VertexPool(meshFilter.mesh, material);
				}
				if (!MatDic.ContainsKey(material.name))
				{
					GameObject obj = new GameObject("mesh " + material.name);
					obj.transform.parent = base.transform;
					GameObject gameObject = obj;
					gameObject.AddComponent("MeshFilter");
					gameObject.AddComponent("MeshRenderer");
					MeshFilter meshFilter = (MeshFilter)gameObject.GetComponent(typeof(MeshFilter));
					MeshRenderer meshRenderer = (MeshRenderer)gameObject.GetComponent(typeof(MeshRenderer));
					meshRenderer.castShadows = false;
					meshRenderer.receiveShadows = false;
					meshRenderer.renderer.material = material;
					MatDic[material.name] = new VertexPool(meshFilter.mesh, material);
				}
			}
		}
		foreach (EffectLayer efl in EflList)
		{
			efl.Vertexpool = MatDic[efl.Material.name];
		}
	}

	private void LateUpdate()
	{
		foreach (KeyValuePair<string, VertexPool> item in MatDic)
		{
			item.Value.LateUpdate();
		}
		if (!(ElapsedTime > LifeTime) || !(LifeTime >= 0f))
		{
			return;
		}
		foreach (EffectLayer efl in EflList)
		{
			efl.Reset();
		}
		DeActive();
		ElapsedTime = 0f;
	}

	private void OnDrawGizmosSelected()
	{
	}

	public void SetClient(Transform client)
	{
		foreach (EffectLayer efl in EflList)
		{
			efl.ClientTransform = client;
		}
	}

	public void SetDirectionAxis(Vector3 axis)
	{
		foreach (EffectLayer efl in EflList)
		{
			efl.OriVelocityAxis = axis;
		}
	}

	public void SetEmitPosition(Vector3 pos)
	{
		foreach (EffectLayer efl in EflList)
		{
			efl.EmitPoint = pos;
		}
	}

	private void Start()
	{
		base.transform.position = Vector3.zero;
		base.transform.rotation = Quaternion.identity;
		base.transform.localScale = Vector3.one;
		foreach (Transform item in base.transform)
		{
			item.transform.position = Vector3.zero;
			item.transform.rotation = Quaternion.identity;
			item.transform.localScale = Vector3.one;
		}
		foreach (EffectLayer efl in EflList)
		{
			efl.StartCustom();
		}
	}

	private void Update()
	{
		ElapsedTime += Time.deltaTime;
		foreach (EffectLayer efl in EflList)
		{
			if (ElapsedTime > efl.StartTime)
			{
				efl.FixedUpdateCustom();
			}
		}
	}
}
