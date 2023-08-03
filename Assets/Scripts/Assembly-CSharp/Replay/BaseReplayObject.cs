using UnityEngine;

namespace Replay
{
	internal class BaseReplayObject : MonoBehaviour
	{
		public int ObjectId;

		public virtual void SetState(BaseReplayState state)
		{
		}
	}
}
