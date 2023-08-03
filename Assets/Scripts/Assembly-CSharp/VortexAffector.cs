using UnityEngine;

public class VortexAffector : Affector
{
	protected Vector3 Direction;

	private float Magnitude;

	private bool UseCurve;

	private AnimationCurve VortexCurve;

	public VortexAffector(float mag, Vector3 dir, EffectNode node)
		: base(node)
	{
		Magnitude = mag;
		Direction = dir;
		UseCurve = false;
	}

	public VortexAffector(AnimationCurve vortexCurve, Vector3 dir, EffectNode node)
		: base(node)
	{
		VortexCurve = vortexCurve;
		Direction = dir;
		UseCurve = true;
	}

	public override void Update()
	{
		Vector3 vector = Node.GetLocalPosition() - Node.Owner.EmitPoint;
		if (vector.magnitude != 0f)
		{
			float num = Vector3.Dot(Direction, vector);
			vector -= num * Direction;
			Vector3 zero = Vector3.zero;
			zero = ((!(vector == Vector3.zero)) ? Vector3.Cross(Direction, vector).normalized : vector);
			float elapsedTime = Node.GetElapsedTime();
			float num2 = ((!UseCurve) ? Magnitude : VortexCurve.Evaluate(elapsedTime));
			zero *= num2 * Time.deltaTime;
			Node.Position += zero;
		}
	}
}
