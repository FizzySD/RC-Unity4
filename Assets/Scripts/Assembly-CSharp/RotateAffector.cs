using UnityEngine;

public class RotateAffector : Affector
{
	protected float Delta;

	protected AnimationCurve RotateCurve;

	protected RSTYPE Type;

	public RotateAffector(float delta, EffectNode node)
		: base(node)
	{
		Type = RSTYPE.SIMPLE;
		Delta = delta;
	}

	public RotateAffector(AnimationCurve curve, EffectNode node)
		: base(node)
	{
		Type = RSTYPE.CURVE;
		RotateCurve = curve;
	}

	public override void Update()
	{
		float elapsedTime = Node.GetElapsedTime();
		if (Type == RSTYPE.CURVE)
		{
			Node.RotateAngle = (int)RotateCurve.Evaluate(elapsedTime);
		}
		else if (Type == RSTYPE.SIMPLE)
		{
			float rotateAngle = Node.RotateAngle + Delta * Time.deltaTime;
			Node.RotateAngle = rotateAngle;
		}
	}
}
