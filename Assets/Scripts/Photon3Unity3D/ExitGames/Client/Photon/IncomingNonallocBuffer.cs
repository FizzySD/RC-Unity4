using System.Collections.Generic;

namespace ExitGames.Client.Photon
{
	public class IncomingNonallocBuffer
	{
		public readonly byte[] buffer = new byte[1200];

		public int bytecount;

		private static readonly Stack<IncomingNonallocBuffer> unusedPool = new Stack<IncomingNonallocBuffer>();

		private static readonly Stack<IncomingNonallocBuffer> usedPool = new Stack<IncomingNonallocBuffer>();

		public static IncomingNonallocBuffer GetFromPool()
		{
			IncomingNonallocBuffer incomingNonallocBuffer;
			if (unusedPool.Count > 0)
			{
				incomingNonallocBuffer = unusedPool.Pop();
				incomingNonallocBuffer.bytecount = 0;
			}
			else
			{
				incomingNonallocBuffer = new IncomingNonallocBuffer();
			}
			usedPool.Push(incomingNonallocBuffer);
			return incomingNonallocBuffer;
		}

		public static void ReturnAllToPool()
		{
			while (usedPool.Count > 0)
			{
				unusedPool.Push(usedPool.Pop());
			}
		}

		public static implicit operator byte[](IncomingNonallocBuffer wrapper)
		{
			return wrapper.buffer;
		}
	}
}
