using System.Collections.Generic;

namespace ExitGames.Client.Photon
{
	public class NonallocBytesWrapper
	{
		public byte[] buffer;

		public int bytecount;

		private static readonly Stack<NonallocBytesWrapper> unusedPool = new Stack<NonallocBytesWrapper>();

		private static readonly Stack<NonallocBytesWrapper> usedPool = new Stack<NonallocBytesWrapper>();

		public void ReturnToPool()
		{
			buffer = null;
			unusedPool.Push(this);
		}

		public static NonallocBytesWrapper GetFromPool(byte[] buffer, int bytecount)
		{
			NonallocBytesWrapper nonallocBytesWrapper = ((unusedPool.Count <= 0) ? new NonallocBytesWrapper() : unusedPool.Pop());
			usedPool.Push(nonallocBytesWrapper);
			nonallocBytesWrapper.buffer = buffer;
			nonallocBytesWrapper.bytecount = bytecount;
			return nonallocBytesWrapper;
		}

		public static void ReturnAllToPool()
		{
			while (usedPool.Count > 0)
			{
				unusedPool.Push(usedPool.Pop());
			}
		}
	}
}
