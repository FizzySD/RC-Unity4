using UnityEngine;

public class Emitter
{
	private float EmitDelayTime;

	private float EmitLoop;

	private float EmitterElapsedTime;

	private bool IsFirstEmit = true;

	private Vector3 LastClientPos = Vector3.zero;

	public EffectLayer Layer;

	public Emitter(EffectLayer owner)
	{
		Layer = owner;
		EmitLoop = Layer.EmitLoop;
		LastClientPos = Layer.ClientTransform.position;
	}

	protected int EmitByDistance()
	{
		if ((Layer.ClientTransform.position - LastClientPos).magnitude >= Layer.DiffDistance)
		{
			LastClientPos = Layer.ClientTransform.position;
			return 1;
		}
		return 0;
	}

	protected int EmitByRate()
	{
		int num = Random.Range(0, 100);
		if (num >= 0 && (float)num > Layer.ChanceToEmit)
		{
			return 0;
		}
		EmitDelayTime += Time.deltaTime;
		if (EmitDelayTime < Layer.EmitDelay && !IsFirstEmit)
		{
			return 0;
		}
		EmitterElapsedTime += Time.deltaTime;
		if (EmitterElapsedTime >= Layer.EmitDuration)
		{
			if (EmitLoop > 0f)
			{
				EmitLoop -= 1f;
			}
			EmitterElapsedTime = 0f;
			EmitDelayTime = 0f;
			IsFirstEmit = false;
		}
		if (EmitLoop == 0f)
		{
			return 0;
		}
		if (Layer.AvailableNodeCount == 0)
		{
			return 0;
		}
		int num2 = (int)(EmitterElapsedTime * (float)Layer.EmitRate) - (Layer.ActiveENodes.Length - Layer.AvailableNodeCount);
		int num3 = 0;
		num3 = ((num2 <= Layer.AvailableNodeCount) ? num2 : Layer.AvailableNodeCount);
		if (num3 <= 0)
		{
			return 0;
		}
		return num3;
	}

	public Vector3 GetEmitRotation(EffectNode node)
	{
		Vector3 zero = Vector3.zero;
		if (Layer.EmitType == 2)
		{
			if (!Layer.SyncClient)
			{
				return node.Position - (Layer.ClientTransform.position + Layer.EmitPoint);
			}
			return node.Position - Layer.EmitPoint;
		}
		if (Layer.EmitType == 3)
		{
			Vector3 vector = (Layer.SyncClient ? (node.Position - Layer.EmitPoint) : (node.Position - (Layer.ClientTransform.position + Layer.EmitPoint)));
			Vector3 toDirection = Vector3.RotateTowards(vector, Layer.CircleDir, (float)(90 - Layer.AngleAroundAxis) * 0.01745329f, 1f);
			return Quaternion.FromToRotation(vector, toDirection) * vector;
		}
		if (Layer.IsRandomDir)
		{
			Quaternion quaternion = Quaternion.Euler(0f, 0f, Layer.AngleAroundAxis);
			Quaternion quaternion2 = Quaternion.Euler(0f, Random.Range(0, 360), 0f);
			return Quaternion.FromToRotation(Vector3.up, Layer.OriVelocityAxis) * quaternion2 * quaternion * Vector3.up;
		}
		return Layer.OriVelocityAxis;
	}

	public int GetNodes()
	{
		if (Layer.IsEmitByDistance)
		{
			return EmitByDistance();
		}
		return EmitByRate();
	}

	public void Reset()
	{
		EmitterElapsedTime = 0f;
		EmitDelayTime = 0f;
		IsFirstEmit = true;
		EmitLoop = Layer.EmitLoop;
	}

	public void SetEmitPosition(EffectNode node)
	{
		Vector3 vector = Vector3.zero;
		if (Layer.EmitType == 1)
		{
			Vector3 emitPoint = Layer.EmitPoint;
			float x = Random.Range(emitPoint.x - Layer.BoxSize.x / 2f, emitPoint.x + Layer.BoxSize.x / 2f);
			float y = Random.Range(emitPoint.y - Layer.BoxSize.y / 2f, emitPoint.y + Layer.BoxSize.y / 2f);
			float z = Random.Range(emitPoint.z - Layer.BoxSize.z / 2f, emitPoint.z + Layer.BoxSize.z / 2f);
			vector.x = x;
			vector.y = y;
			vector.z = z;
			if (!Layer.SyncClient)
			{
				vector = Layer.ClientTransform.position + vector;
			}
		}
		else if (Layer.EmitType == 0)
		{
			vector = Layer.EmitPoint;
			if (!Layer.SyncClient)
			{
				vector = Layer.ClientTransform.position + Layer.EmitPoint;
			}
		}
		else if (Layer.EmitType == 2)
		{
			vector = Layer.EmitPoint;
			if (!Layer.SyncClient)
			{
				vector = Layer.ClientTransform.position + Layer.EmitPoint;
			}
			Vector3 vector2 = Vector3.up * Layer.Radius;
			vector = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)) * vector2 + vector;
		}
		else if (Layer.EmitType == 4)
		{
			Vector3 vector3 = Layer.EmitPoint + Layer.ClientTransform.localRotation * Vector3.forward * Layer.LineLengthLeft;
			Vector3 vector4 = Layer.EmitPoint + Layer.ClientTransform.localRotation * Vector3.forward * Layer.LineLengthRight;
			Vector3 vector5 = vector4 - vector3;
			float num = (float)(node.Index + 1) / (float)Layer.MaxENodes;
			float num2 = vector5.magnitude * num;
			vector = vector3 + vector5.normalized * num2;
			if (!Layer.SyncClient)
			{
				vector = Layer.ClientTransform.TransformPoint(vector);
			}
		}
		else if (Layer.EmitType == 3)
		{
			float num3 = (float)(node.Index + 1) / (float)Layer.MaxENodes;
			float y2 = 360f * num3;
			Vector3 vector6 = Quaternion.Euler(0f, y2, 0f) * (Vector3.right * Layer.Radius);
			vector = Quaternion.FromToRotation(Vector3.up, Layer.CircleDir) * vector6;
			if (!Layer.SyncClient)
			{
				vector = Layer.ClientTransform.position + vector + Layer.EmitPoint;
			}
			else
			{
				vector += Layer.EmitPoint;
			}
		}
		node.SetLocalPosition(vector);
	}
}
