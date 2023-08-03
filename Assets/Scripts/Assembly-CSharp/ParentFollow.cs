using UnityEngine;

public class ParentFollow : MonoBehaviour
{
	private Transform bTransform;

	public bool isActiveInScene;

	private Transform parent;

	private void Awake()
	{
		bTransform = base.transform;
		isActiveInScene = true;
	}

	public void RemoveParent()
	{
		parent = null;
	}

	public void SetParent(Transform transform)
	{
		parent = transform;
		bTransform.rotation = transform.rotation;
	}

	private void Update()
	{
		if (isActiveInScene && parent != null)
		{
			bTransform.position = parent.position;
		}
	}
}
