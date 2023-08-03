#define DEBUG
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;

namespace ExitGames.Client.Photon
{
	public class SupportClass
	{
		public delegate int IntegerMillisecondsDelegate();

		public class ThreadSafeRandom
		{
			private static readonly Random _r = new Random();

			public static int Next()
			{
				lock (_r)
				{
					return _r.Next();
				}
			}
		}

		private static List<Thread> threadList;

		protected internal static IntegerMillisecondsDelegate IntegerMilliseconds = () => Environment.TickCount;

		private static uint[] crcLookupTable;

		public static List<MethodInfo> GetMethods(Type type, Type attribute)
		{
			List<MethodInfo> list = new List<MethodInfo>();
			if (type == null)
			{
				return list;
			}
			MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			MethodInfo[] array = methods;
			foreach (MethodInfo methodInfo in array)
			{
				if (attribute == null || methodInfo.IsDefined(attribute, false))
				{
					list.Add(methodInfo);
				}
			}
			return list;
		}

		public static int GetTickCount()
		{
			return IntegerMilliseconds();
		}

		[Obsolete("Use StartBackgroundCalls() instead. It works with StopBackgroundCalls().")]
		public static byte CallInBackground(Func<bool> myThread, int millisecondsInterval = 100, string taskName = "")
		{
			return StartBackgroundCalls(myThread, millisecondsInterval, null);
		}

		public static byte StartBackgroundCalls(Func<bool> myThread, int millisecondsInterval = 100, string taskName = "")
		{
			if (threadList == null)
			{
				threadList = new List<Thread>();
			}
			Thread thread = new Thread((ThreadStart)delegate
			{
				try
				{
					while (myThread())
					{
						Thread.Sleep(millisecondsInterval);
					}
				}
				catch (ThreadAbortException)
				{
				}
			});
			if (!string.IsNullOrEmpty(taskName))
			{
				thread.Name = taskName;
			}
			thread.IsBackground = true;
			thread.Start();
			threadList.Add(thread);
			return (byte)(threadList.Count - 1);
		}

		public static bool StopBackgroundCalls(byte id)
		{
			if (threadList == null || id > threadList.Count || threadList[id] == null)
			{
				return false;
			}
			threadList[id].Abort();
			return true;
		}

		public static bool StopAllBackgroundCalls()
		{
			if (threadList == null)
			{
				return false;
			}
			foreach (Thread thread in threadList)
			{
				thread.Abort();
			}
			return true;
		}

		public static void WriteStackTrace(Exception throwable, TextWriter stream)
		{
			if (stream != null)
			{
				stream.WriteLine(throwable.ToString());
				stream.WriteLine(throwable.StackTrace);
				stream.Flush();
			}
			else
			{
				Debug.WriteLine(throwable.ToString());
				Debug.WriteLine(throwable.StackTrace);
			}
		}

		public static void WriteStackTrace(Exception throwable)
		{
			WriteStackTrace(throwable, null);
		}

		public static string DictionaryToString(IDictionary dictionary)
		{
			return DictionaryToString(dictionary, true);
		}

		public static string DictionaryToString(IDictionary dictionary, bool includeTypes)
		{
			if (dictionary == null)
			{
				return "null";
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("{");
			foreach (object key in dictionary.Keys)
			{
				if (stringBuilder.Length > 1)
				{
					stringBuilder.Append(", ");
				}
				Type type;
				string text;
				if (dictionary[key] == null)
				{
					type = typeof(object);
					text = "null";
				}
				else
				{
					type = dictionary[key].GetType();
					text = dictionary[key].ToString();
				}
				if (typeof(IDictionary) == type || typeof(Hashtable) == type)
				{
					text = DictionaryToString((IDictionary)dictionary[key]);
				}
				if (typeof(string[]) == type)
				{
					text = string.Format("{{{0}}}", string.Join(",", (string[])dictionary[key]));
				}
				if (typeof(byte[]) == type)
				{
					text = string.Format("byte[{0}]", ((byte[])dictionary[key]).Length);
				}
				if (includeTypes)
				{
					stringBuilder.AppendFormat("({0}){1}=({2}){3}", key.GetType().Name, key, type.Name, text);
				}
				else
				{
					stringBuilder.AppendFormat("{0}={1}", key, text);
				}
			}
			stringBuilder.Append("}");
			return stringBuilder.ToString();
		}

		[Obsolete("Use DictionaryToString() instead.")]
		public static string HashtableToString(Hashtable hash)
		{
			return DictionaryToString(hash);
		}

		public static string ByteArrayToString(byte[] list)
		{
			if (list == null)
			{
				return string.Empty;
			}
			return BitConverter.ToString(list);
		}

		private static uint[] InitializeTable(uint polynomial)
		{
			uint[] array = new uint[256];
			for (int i = 0; i < 256; i++)
			{
				uint num = (uint)i;
				for (int j = 0; j < 8; j++)
				{
					num = (((num & 1) != 1) ? (num >> 1) : ((num >> 1) ^ polynomial));
				}
				array[i] = num;
			}
			return array;
		}

		public static uint CalculateCrc(byte[] buffer, int length)
		{
			uint num = uint.MaxValue;
			uint polynomial = 3988292384u;
			if (crcLookupTable == null)
			{
				crcLookupTable = InitializeTable(polynomial);
			}
			for (int i = 0; i < length; i++)
			{
				num = (num >> 8) ^ crcLookupTable[buffer[i] ^ (num & 0xFF)];
			}
			return num;
		}
	}
}
