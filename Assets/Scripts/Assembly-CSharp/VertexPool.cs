using UnityEngine;

public class VertexPool
{
	public class VertexSegment
	{
		public int IndexCount;

		public int IndexStart;

		public VertexPool Pool;

		public int VertCount;

		public int VertStart;

		public VertexSegment(int start, int count, int istart, int icount, VertexPool pool)
		{
			VertStart = start;
			VertCount = count;
			IndexCount = icount;
			IndexStart = istart;
			Pool = pool;
		}
	}

	public const int BlockSize = 36;

	public float BoundsScheduleTime = 1f;

	public bool ColorChanged;

	public Color[] Colors;

	public float ElapsedTime;

	protected bool FirstUpdate = true;

	protected int IndexTotal;

	protected int IndexUsed;

	public bool IndiceChanged;

	public int[] Indices;

	public Material Material;

	public Mesh Mesh;

	public bool UVChanged;

	public Vector2[] UVs;

	public bool VertChanged;

	protected bool VertCountChanged;

	protected int VertexTotal;

	protected int VertexUsed;

	public Vector3[] Vertices;

	public VertexPool(Mesh mesh, Material material)
	{
		VertexTotal = (VertexUsed = 0);
		VertCountChanged = false;
		Mesh = mesh;
		Material = material;
		InitArrays();
		Vertices = Mesh.vertices;
		Indices = Mesh.triangles;
		Colors = Mesh.colors;
		UVs = Mesh.uv;
		IndiceChanged = (ColorChanged = (UVChanged = (VertChanged = true)));
	}

	public RibbonTrail AddRibbonTrail(float width, int maxelemnt, float len, Vector3 pos, int stretchType, float maxFps)
	{
		return new RibbonTrail(GetVertices(maxelemnt * 2, (maxelemnt - 1) * 6), width, maxelemnt, len, pos, stretchType, maxFps);
	}

	public Sprite AddSprite(float width, float height, STYPE type, ORIPOINT ori, Camera cam, int uvStretch, float maxFps)
	{
		return new Sprite(GetVertices(4, 6), width, height, type, ori, cam, uvStretch, maxFps);
	}

	public void EnlargeArrays(int count, int icount)
	{
		Vector3[] vertices = Vertices;
		Vertices = new Vector3[Vertices.Length + count];
		vertices.CopyTo(Vertices, 0);
		Vector2[] uVs = UVs;
		UVs = new Vector2[UVs.Length + count];
		uVs.CopyTo(UVs, 0);
		Color[] colors = Colors;
		Colors = new Color[Colors.Length + count];
		colors.CopyTo(Colors, 0);
		int[] indices = Indices;
		Indices = new int[Indices.Length + icount];
		indices.CopyTo(Indices, 0);
		VertCountChanged = true;
		IndiceChanged = true;
		ColorChanged = true;
		UVChanged = true;
		VertChanged = true;
	}

	public Material GetMaterial()
	{
		return Material;
	}

	public VertexSegment GetVertices(int vcount, int icount)
	{
		int num = 0;
		int num2 = 0;
		if (VertexUsed + vcount >= VertexTotal)
		{
			num = (vcount / 36 + 1) * 36;
		}
		if (IndexUsed + icount >= IndexTotal)
		{
			num2 = (icount / 36 + 1) * 36;
		}
		VertexUsed += vcount;
		IndexUsed += icount;
		if (num != 0 || num2 != 0)
		{
			EnlargeArrays(num, num2);
			VertexTotal += num;
			IndexTotal += num2;
		}
		return new VertexSegment(VertexUsed - vcount, vcount, IndexUsed - icount, icount, this);
	}

	protected void InitArrays()
	{
		Vertices = new Vector3[4];
		UVs = new Vector2[4];
		Colors = new Color[4];
		Indices = new int[6];
		VertexTotal = 4;
		IndexTotal = 6;
	}

	public void LateUpdate()
	{
		if (VertCountChanged)
		{
			Mesh.Clear();
		}
		Mesh.vertices = Vertices;
		if (UVChanged)
		{
			Mesh.uv = UVs;
		}
		if (ColorChanged)
		{
			Mesh.colors = Colors;
		}
		if (IndiceChanged)
		{
			Mesh.triangles = Indices;
		}
		ElapsedTime += Time.deltaTime;
		if (ElapsedTime > BoundsScheduleTime || FirstUpdate)
		{
			RecalculateBounds();
			ElapsedTime = 0f;
		}
		if (ElapsedTime > BoundsScheduleTime)
		{
			FirstUpdate = false;
		}
		VertCountChanged = false;
		IndiceChanged = false;
		ColorChanged = false;
		UVChanged = false;
		VertChanged = false;
	}

	public void RecalculateBounds()
	{
		Mesh.RecalculateBounds();
	}
}
