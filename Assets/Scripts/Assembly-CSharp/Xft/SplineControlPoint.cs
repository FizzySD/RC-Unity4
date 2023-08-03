using UnityEngine;

namespace Xft
{
	public class SplineControlPoint
	{
		public int ControlPointIndex = -1;

		public float Dist;

		protected Spline mSpline;

		public Vector3 Normal;

		public Vector3 Position;

		public int SegmentIndex = -1;

		public bool IsValid
		{
			get
			{
				return NextControlPoint != null;
			}
		}

		public SplineControlPoint NextControlPoint
		{
			get
			{
				return mSpline.NextControlPoint(this);
			}
		}

		public Vector3 NextNormal
		{
			get
			{
				return mSpline.NextNormal(this);
			}
		}

		public Vector3 NextPosition
		{
			get
			{
				return mSpline.NextPosition(this);
			}
		}

		public SplineControlPoint PreviousControlPoint
		{
			get
			{
				return mSpline.PreviousControlPoint(this);
			}
		}

		public Vector3 PreviousNormal
		{
			get
			{
				return mSpline.PreviousNormal(this);
			}
		}

		public Vector3 PreviousPosition
		{
			get
			{
				return mSpline.PreviousPosition(this);
			}
		}

		private Vector3 GetNext2Normal()
		{
			SplineControlPoint nextControlPoint = NextControlPoint;
			if (nextControlPoint != null)
			{
				return nextControlPoint.NextNormal;
			}
			return Normal;
		}

		private Vector3 GetNext2Position()
		{
			SplineControlPoint nextControlPoint = NextControlPoint;
			if (nextControlPoint != null)
			{
				return nextControlPoint.NextPosition;
			}
			return NextPosition;
		}

		public void Init(Spline owner)
		{
			mSpline = owner;
			SegmentIndex = -1;
		}

		public Vector3 Interpolate(float localF)
		{
			localF = Mathf.Clamp01(localF);
			return Spline.CatmulRom(PreviousPosition, Position, NextPosition, GetNext2Position(), localF);
		}

		public Vector3 InterpolateNormal(float localF)
		{
			localF = Mathf.Clamp01(localF);
			return Spline.CatmulRom(PreviousNormal, Normal, NextNormal, GetNext2Normal(), localF);
		}
	}
}
