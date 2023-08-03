#define DEBUG
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ExitGames.Client.Photon
{
	public class Protocol16 : IProtocol
	{
		public enum GpType : byte
		{
			Unknown = 0,
			Array = 121,
			Boolean = 111,
			Byte = 98,
			ByteArray = 120,
			ObjectArray = 122,
			Short = 107,
			Float = 102,
			Dictionary = 68,
			Double = 100,
			Hashtable = 104,
			Integer = 105,
			IntegerArray = 110,
			Long = 108,
			String = 115,
			StringArray = 97,
			Custom = 99,
			Null = 42,
			EventData = 101,
			OperationRequest = 113,
			OperationResponse = 112
		}

		private readonly byte[] versionBytes = new byte[2] { 1, 6 };

		private readonly byte[] memShort = new byte[2];

		private readonly long[] memLongBlock = new long[1];

		private readonly byte[] memLongBlockBytes = new byte[8];

		private static readonly float[] memFloatBlock = new float[1];

		private static readonly byte[] memFloatBlockBytes = new byte[4];

		private readonly double[] memDoubleBlock = new double[1];

		private readonly byte[] memDoubleBlockBytes = new byte[8];

		private readonly byte[] memInteger = new byte[4];

		private readonly byte[] memLong = new byte[8];

		private readonly byte[] memFloat = new byte[4];

		private readonly byte[] memDouble = new byte[8];

		private byte[] memString;

		public override string ProtocolType
		{
			get
			{
				return "GpBinaryV16";
			}
		}

		public override byte[] VersionBytes
		{
			get
			{
				return versionBytes;
			}
		}

		private bool SerializeCustom(StreamBuffer dout, object serObject)
		{
			CustomType value;
			if (Protocol.TypeDict.TryGetValue(serObject.GetType(), out value))
			{
				if (value.SerializeStreamFunction == null)
				{
					byte[] array = value.SerializeFunction(serObject);
					dout.WriteByte(99);
					dout.WriteByte(value.Code);
					SerializeShort(dout, (short)array.Length, false);
					dout.Write(array, 0, array.Length);
					return true;
				}
				dout.WriteByte(99);
				dout.WriteByte(value.Code);
				int position = dout.Position;
				dout.Position += 2;
				short num = value.SerializeStreamFunction(dout, serObject);
				long num2 = dout.Position;
				dout.Position = position;
				SerializeShort(dout, num, false);
				dout.Position += num;
				if (dout.Position != num2)
				{
					throw new Exception("Serialization failed. Stream position corrupted. Should be " + num2 + " is now: " + dout.Position + " serializedLength: " + num);
				}
				return true;
			}
			return false;
		}

		private object DeserializeCustom(StreamBuffer din, byte customTypeCode)
		{
			short num = DeserializeShort(din);
			CustomType value;
			if (Protocol.CodeDict.TryGetValue(customTypeCode, out value))
			{
				if (value.DeserializeStreamFunction == null)
				{
					byte[] array = new byte[num];
					din.Read(array, 0, num);
					return value.DeserializeFunction(array);
				}
				int position = din.Position;
				object result = value.DeserializeStreamFunction(din, num);
				int num2 = din.Position - position;
				if (num2 != num)
				{
					din.Position = position + num;
				}
				return result;
			}
			byte[] array2 = new byte[num];
			din.Read(array2, 0, num);
			return array2;
		}

		private Type GetTypeOfCode(byte typeCode)
		{
			switch (typeCode)
			{
			case 105:
				return typeof(int);
			case 115:
				return typeof(string);
			case 97:
				return typeof(string[]);
			case 120:
				return typeof(byte[]);
			case 110:
				return typeof(int[]);
			case 104:
				return typeof(Hashtable);
			case 68:
				return typeof(IDictionary);
			case 111:
				return typeof(bool);
			case 107:
				return typeof(short);
			case 108:
				return typeof(long);
			case 98:
				return typeof(byte);
			case 102:
				return typeof(float);
			case 100:
				return typeof(double);
			case 121:
				return typeof(Array);
			case 99:
				return typeof(CustomType);
			case 122:
				return typeof(object[]);
			case 101:
				return typeof(EventData);
			case 113:
				return typeof(OperationRequest);
			case 112:
				return typeof(OperationResponse);
			case 0:
			case 42:
				return typeof(object);
			default:
				Debug.WriteLine("missing type: " + typeCode);
				throw new Exception("deserialize(): " + typeCode);
			}
		}

		private GpType GetCodeOfType(Type type)
		{
			switch (Type.GetTypeCode(type))
			{
			case TypeCode.Byte:
				return GpType.Byte;
			case TypeCode.String:
				return GpType.String;
			case TypeCode.Boolean:
				return GpType.Boolean;
			case TypeCode.Int16:
				return GpType.Short;
			case TypeCode.Int32:
				return GpType.Integer;
			case TypeCode.Int64:
				return GpType.Long;
			case TypeCode.Single:
				return GpType.Float;
			case TypeCode.Double:
				return GpType.Double;
			default:
				if (type.IsArray)
				{
					if (type == typeof(byte[]))
					{
						return GpType.ByteArray;
					}
					return GpType.Array;
				}
				if (type == typeof(Hashtable))
				{
					return GpType.Hashtable;
				}
				if (type == typeof(List<object>))
				{
					return GpType.ObjectArray;
				}
				if (type.IsGenericType && typeof(Dictionary<, >) == type.GetGenericTypeDefinition())
				{
					return GpType.Dictionary;
				}
				if (type == typeof(EventData))
				{
					return GpType.EventData;
				}
				if (type == typeof(OperationRequest))
				{
					return GpType.OperationRequest;
				}
				if (type == typeof(OperationResponse))
				{
					return GpType.OperationResponse;
				}
				return GpType.Unknown;
			}
		}

		private Array CreateArrayByType(byte arrayType, short length)
		{
			return Array.CreateInstance(GetTypeOfCode(arrayType), length);
		}

		private void SerializeOperationRequest(StreamBuffer stream, OperationRequest serObject, bool setType)
		{
			SerializeOperationRequest(stream, serObject.OperationCode, serObject.Parameters, setType);
		}

		public override void SerializeOperationRequest(StreamBuffer stream, byte operationCode, Dictionary<byte, object> parameters, bool setType)
		{
			if (setType)
			{
				stream.WriteByte(113);
			}
			stream.WriteByte(operationCode);
			SerializeParameterTable(stream, parameters);
		}

		public override OperationRequest DeserializeOperationRequest(StreamBuffer din)
		{
			OperationRequest operationRequest = new OperationRequest();
			operationRequest.OperationCode = DeserializeByte(din);
			operationRequest.Parameters = DeserializeParameterTable(din);
			return operationRequest;
		}

		public override void SerializeOperationResponse(StreamBuffer stream, OperationResponse serObject, bool setType)
		{
			if (setType)
			{
				stream.WriteByte(112);
			}
			stream.WriteByte(serObject.OperationCode);
			SerializeShort(stream, serObject.ReturnCode, false);
			if (string.IsNullOrEmpty(serObject.DebugMessage))
			{
				stream.WriteByte(42);
			}
			else
			{
				SerializeString(stream, serObject.DebugMessage, false);
			}
			SerializeParameterTable(stream, serObject.Parameters);
		}

		public override OperationResponse DeserializeOperationResponse(StreamBuffer stream)
		{
			OperationResponse operationResponse = new OperationResponse();
			operationResponse.OperationCode = DeserializeByte(stream);
			operationResponse.ReturnCode = DeserializeShort(stream);
			operationResponse.DebugMessage = Deserialize(stream, DeserializeByte(stream)) as string;
			operationResponse.Parameters = DeserializeParameterTable(stream);
			return operationResponse;
		}

		public override void SerializeEventData(StreamBuffer stream, EventData serObject, bool setType)
		{
			if (setType)
			{
				stream.WriteByte(101);
			}
			stream.WriteByte(serObject.Code);
			SerializeParameterTable(stream, serObject.Parameters);
		}

		public override EventData DeserializeEventData(StreamBuffer din, EventData target = null)
		{
			EventData eventData;
			if (target != null)
			{
				target.Reset();
				eventData = target;
			}
			else
			{
				eventData = new EventData();
			}
			eventData.Code = DeserializeByte(din);
			eventData.Parameters = DeserializeParameterTable(din, eventData.Parameters);
			return eventData;
		}

		private void SerializeParameterTable(StreamBuffer stream, Dictionary<byte, object> parameters)
		{
			if (parameters == null || parameters.Count == 0)
			{
				SerializeShort(stream, 0, false);
				return;
			}
			SerializeShort(stream, (short)parameters.Count, false);
			foreach (KeyValuePair<byte, object> parameter in parameters)
			{
				stream.WriteByte(parameter.Key);
				Serialize(stream, parameter.Value, true);
			}
		}

		private Dictionary<byte, object> DeserializeParameterTable(StreamBuffer stream, Dictionary<byte, object> target = null)
		{
			short num = DeserializeShort(stream);
			Dictionary<byte, object> dictionary = ((target != null) ? target : new Dictionary<byte, object>(num));
			for (int i = 0; i < num; i++)
			{
				byte key = stream.ReadByte();
				object value = Deserialize(stream, stream.ReadByte());
				dictionary[key] = value;
			}
			return dictionary;
		}

		public override void Serialize(StreamBuffer dout, object serObject, bool setType)
		{
			if (serObject == null)
			{
				if (setType)
				{
					dout.WriteByte(42);
				}
				return;
			}
			switch (GetCodeOfType(serObject.GetType()))
			{
			case GpType.Byte:
				SerializeByte(dout, (byte)serObject, setType);
				return;
			case GpType.String:
				SerializeString(dout, (string)serObject, setType);
				return;
			case GpType.Boolean:
				SerializeBoolean(dout, (bool)serObject, setType);
				return;
			case GpType.Short:
				SerializeShort(dout, (short)serObject, setType);
				return;
			case GpType.Integer:
				SerializeInteger(dout, (int)serObject, setType);
				return;
			case GpType.Long:
				SerializeLong(dout, (long)serObject, setType);
				return;
			case GpType.Float:
				SerializeFloat(dout, (float)serObject, setType);
				return;
			case GpType.Double:
				SerializeDouble(dout, (double)serObject, setType);
				return;
			case GpType.Hashtable:
				SerializeHashTable(dout, (Hashtable)serObject, setType);
				return;
			case GpType.ByteArray:
				SerializeByteArray(dout, (byte[])serObject, setType);
				return;
			case GpType.ObjectArray:
				SerializeObjectArray(dout, (IList)serObject, setType);
				return;
			case GpType.Array:
				if (serObject is int[])
				{
					SerializeIntArrayOptimized(dout, (int[])serObject, setType);
				}
				else if (serObject.GetType().GetElementType() == typeof(object))
				{
					SerializeObjectArray(dout, serObject as object[], setType);
				}
				else
				{
					SerializeArray(dout, (Array)serObject, setType);
				}
				return;
			case GpType.Dictionary:
				SerializeDictionary(dout, (IDictionary)serObject, setType);
				return;
			case GpType.EventData:
				SerializeEventData(dout, (EventData)serObject, setType);
				return;
			case GpType.OperationResponse:
				SerializeOperationResponse(dout, (OperationResponse)serObject, setType);
				return;
			case GpType.OperationRequest:
				SerializeOperationRequest(dout, (OperationRequest)serObject, setType);
				return;
			}
			if (serObject is ArraySegment<byte>)
			{
				ArraySegment<byte> arraySegment = (ArraySegment<byte>)serObject;
				SerializeByteArraySegment(dout, arraySegment.Array, arraySegment.Offset, arraySegment.Count, setType);
			}
			else if (!SerializeCustom(dout, serObject))
			{
				throw new Exception("cannot serialize(): " + serObject.GetType());
			}
		}

		private void SerializeByte(StreamBuffer dout, byte serObject, bool setType)
		{
			if (setType)
			{
				dout.WriteByte(98);
			}
			dout.WriteByte(serObject);
		}

		private void SerializeBoolean(StreamBuffer dout, bool serObject, bool setType)
		{
			if (setType)
			{
				dout.WriteByte(111);
			}
			dout.WriteByte((byte)(serObject ? 1 : 0));
		}

		public override void SerializeShort(StreamBuffer dout, short serObject, bool setType)
		{
			if (setType)
			{
				dout.WriteByte(107);
			}
			lock (memShort)
			{
				byte[] array = memShort;
				array[0] = (byte)(serObject >> 8);
				array[1] = (byte)serObject;
				dout.Write(array, 0, 2);
			}
		}

		private void SerializeInteger(StreamBuffer dout, int serObject, bool setType)
		{
			if (setType)
			{
				dout.WriteByte(105);
			}
			lock (memInteger)
			{
				byte[] array = memInteger;
				array[0] = (byte)(serObject >> 24);
				array[1] = (byte)(serObject >> 16);
				array[2] = (byte)(serObject >> 8);
				array[3] = (byte)serObject;
				dout.Write(array, 0, 4);
			}
		}

		private void SerializeLong(StreamBuffer dout, long serObject, bool setType)
		{
			if (setType)
			{
				dout.WriteByte(108);
			}
			lock (memLongBlock)
			{
				memLongBlock[0] = serObject;
				Buffer.BlockCopy(memLongBlock, 0, memLongBlockBytes, 0, 8);
				byte[] array = memLongBlockBytes;
				if (BitConverter.IsLittleEndian)
				{
					byte b = array[0];
					byte b2 = array[1];
					byte b3 = array[2];
					byte b4 = array[3];
					array[0] = array[7];
					array[1] = array[6];
					array[2] = array[5];
					array[3] = array[4];
					array[4] = b4;
					array[5] = b3;
					array[6] = b2;
					array[7] = b;
				}
				dout.Write(array, 0, 8);
			}
		}

		private void SerializeFloat(StreamBuffer dout, float serObject, bool setType)
		{
			if (setType)
			{
				dout.WriteByte(102);
			}
			lock (memFloatBlockBytes)
			{
				memFloatBlock[0] = serObject;
				Buffer.BlockCopy(memFloatBlock, 0, memFloatBlockBytes, 0, 4);
				if (BitConverter.IsLittleEndian)
				{
					byte b = memFloatBlockBytes[0];
					byte b2 = memFloatBlockBytes[1];
					memFloatBlockBytes[0] = memFloatBlockBytes[3];
					memFloatBlockBytes[1] = memFloatBlockBytes[2];
					memFloatBlockBytes[2] = b2;
					memFloatBlockBytes[3] = b;
				}
				dout.Write(memFloatBlockBytes, 0, 4);
			}
		}

		private void SerializeDouble(StreamBuffer dout, double serObject, bool setType)
		{
			if (setType)
			{
				dout.WriteByte(100);
			}
			lock (memDoubleBlockBytes)
			{
				memDoubleBlock[0] = serObject;
				Buffer.BlockCopy(memDoubleBlock, 0, memDoubleBlockBytes, 0, 8);
				byte[] array = memDoubleBlockBytes;
				if (BitConverter.IsLittleEndian)
				{
					byte b = array[0];
					byte b2 = array[1];
					byte b3 = array[2];
					byte b4 = array[3];
					array[0] = array[7];
					array[1] = array[6];
					array[2] = array[5];
					array[3] = array[4];
					array[4] = b4;
					array[5] = b3;
					array[6] = b2;
					array[7] = b;
				}
				dout.Write(array, 0, 8);
			}
		}

		public override void SerializeString(StreamBuffer stream, string value, bool setType)
		{
			if (setType)
			{
				stream.WriteByte(115);
			}
			int byteCount = Encoding.UTF8.GetByteCount(value);
			if (byteCount > 32767)
			{
				throw new NotSupportedException("Strings that exceed a UTF8-encoded byte-length of 32767 (short.MaxValue) are not supported. Yours is: " + byteCount);
			}
			SerializeShort(stream, (short)byteCount, false);
			int offset = 0;
			byte[] bufferAndAdvance = stream.GetBufferAndAdvance(byteCount, out offset);
			Encoding.UTF8.GetBytes(value, 0, value.Length, bufferAndAdvance, offset);
		}

		private void SerializeArray(StreamBuffer dout, Array serObject, bool setType)
		{
			if (setType)
			{
				dout.WriteByte(121);
			}
			if (serObject.Length > 32767)
			{
				throw new NotSupportedException("String[] that exceed 32767 (short.MaxValue) entries are not supported. Yours is: " + serObject.Length);
			}
			SerializeShort(dout, (short)serObject.Length, false);
			Type elementType = serObject.GetType().GetElementType();
			GpType codeOfType = GetCodeOfType(elementType);
			if (codeOfType != 0)
			{
				dout.WriteByte((byte)codeOfType);
				if (codeOfType == GpType.Dictionary)
				{
					bool setKeyType;
					bool setValueType;
					SerializeDictionaryHeader(dout, serObject, out setKeyType, out setValueType);
					for (int i = 0; i < serObject.Length; i++)
					{
						object value = serObject.GetValue(i);
						SerializeDictionaryElements(dout, value, setKeyType, setValueType);
					}
				}
				else
				{
					for (int j = 0; j < serObject.Length; j++)
					{
						object value2 = serObject.GetValue(j);
						Serialize(dout, value2, false);
					}
				}
				return;
			}
			CustomType value3;
			if (Protocol.TypeDict.TryGetValue(elementType, out value3))
			{
				dout.WriteByte(99);
				dout.WriteByte(value3.Code);
				for (int k = 0; k < serObject.Length; k++)
				{
					object value4 = serObject.GetValue(k);
					if (value3.SerializeStreamFunction == null)
					{
						byte[] array = value3.SerializeFunction(value4);
						SerializeShort(dout, (short)array.Length, false);
						dout.Write(array, 0, array.Length);
						continue;
					}
					int position = dout.Position;
					dout.Position += 2;
					short num = value3.SerializeStreamFunction(dout, value4);
					long num2 = dout.Position;
					dout.Position = position;
					SerializeShort(dout, num, false);
					dout.Position += num;
					if (dout.Position != num2)
					{
						throw new Exception("Serialization failed. Stream position corrupted. Should be " + num2 + " is now: " + dout.Position + " serializedLength: " + num);
					}
				}
				return;
			}
			throw new NotSupportedException("cannot serialize array of type " + elementType);
		}

		private void SerializeByteArray(StreamBuffer dout, byte[] serObject, bool setType)
		{
			if (setType)
			{
				dout.WriteByte(120);
			}
			SerializeInteger(dout, serObject.Length, false);
			dout.Write(serObject, 0, serObject.Length);
		}

		private void SerializeByteArraySegment(StreamBuffer dout, byte[] serObject, int offset, int count, bool setType)
		{
			if (setType)
			{
				dout.WriteByte(120);
			}
			SerializeInteger(dout, count, false);
			dout.Write(serObject, offset, count);
		}

		private void SerializeIntArrayOptimized(StreamBuffer inWriter, int[] serObject, bool setType)
		{
			if (setType)
			{
				inWriter.WriteByte(121);
			}
			SerializeShort(inWriter, (short)serObject.Length, false);
			inWriter.WriteByte(105);
			byte[] array = new byte[serObject.Length * 4];
			int num = 0;
			for (int i = 0; i < serObject.Length; i++)
			{
				array[num++] = (byte)(serObject[i] >> 24);
				array[num++] = (byte)(serObject[i] >> 16);
				array[num++] = (byte)(serObject[i] >> 8);
				array[num++] = (byte)serObject[i];
			}
			inWriter.Write(array, 0, array.Length);
		}

		private void SerializeStringArray(StreamBuffer dout, string[] serObject, bool setType)
		{
			if (setType)
			{
				dout.WriteByte(97);
			}
			SerializeShort(dout, (short)serObject.Length, false);
			for (int i = 0; i < serObject.Length; i++)
			{
				SerializeString(dout, serObject[i], false);
			}
		}

		private void SerializeObjectArray(StreamBuffer dout, IList objects, bool setType)
		{
			if (setType)
			{
				dout.WriteByte(122);
			}
			SerializeShort(dout, (short)objects.Count, false);
			for (int i = 0; i < objects.Count; i++)
			{
				object serObject = objects[i];
				Serialize(dout, serObject, true);
			}
		}

		private void SerializeHashTable(StreamBuffer dout, Hashtable serObject, bool setType)
		{
			if (setType)
			{
				dout.WriteByte(104);
			}
			SerializeShort(dout, (short)serObject.Count, false);
			Dictionary<object, object>.KeyCollection keys = serObject.Keys;
			foreach (object item in keys)
			{
				Serialize(dout, item, true);
				Serialize(dout, serObject[item], true);
			}
		}

		private void SerializeDictionary(StreamBuffer dout, IDictionary serObject, bool setType)
		{
			if (setType)
			{
				dout.WriteByte(68);
			}
			bool setKeyType;
			bool setValueType;
			SerializeDictionaryHeader(dout, serObject, out setKeyType, out setValueType);
			SerializeDictionaryElements(dout, serObject, setKeyType, setValueType);
		}

		private void SerializeDictionaryHeader(StreamBuffer writer, Type dictType)
		{
			bool setKeyType;
			bool setValueType;
			SerializeDictionaryHeader(writer, dictType, out setKeyType, out setValueType);
		}

		private void SerializeDictionaryHeader(StreamBuffer writer, object dict, out bool setKeyType, out bool setValueType)
		{
			Type[] genericArguments = dict.GetType().GetGenericArguments();
			setKeyType = genericArguments[0] == typeof(object);
			setValueType = genericArguments[1] == typeof(object);
			if (setKeyType)
			{
				writer.WriteByte(0);
			}
			else
			{
				GpType codeOfType = GetCodeOfType(genericArguments[0]);
				if (codeOfType == GpType.Unknown || codeOfType == GpType.Dictionary)
				{
					throw new Exception("Unexpected - cannot serialize Dictionary with key type: " + genericArguments[0]);
				}
				writer.WriteByte((byte)codeOfType);
			}
			if (setValueType)
			{
				writer.WriteByte(0);
				return;
			}
			GpType codeOfType2 = GetCodeOfType(genericArguments[1]);
			if (codeOfType2 == GpType.Unknown)
			{
				throw new Exception("Unexpected - cannot serialize Dictionary with value type: " + genericArguments[0]);
			}
			writer.WriteByte((byte)codeOfType2);
			if (codeOfType2 == GpType.Dictionary)
			{
				SerializeDictionaryHeader(writer, genericArguments[1]);
			}
		}

		private void SerializeDictionaryElements(StreamBuffer writer, object dict, bool setKeyType, bool setValueType)
		{
			IDictionary dictionary = (IDictionary)dict;
			SerializeShort(writer, (short)dictionary.Count, false);
			foreach (DictionaryEntry item in dictionary)
			{
				if (!setValueType && item.Value == null)
				{
					throw new Exception("Can't serialize null in Dictionary with specific value-type.");
				}
				if (!setKeyType && item.Key == null)
				{
					throw new Exception("Can't serialize null in Dictionary with specific key-type.");
				}
				Serialize(writer, item.Key, setKeyType);
				Serialize(writer, item.Value, setValueType);
			}
		}

		public override object Deserialize(StreamBuffer din, byte type)
		{
			switch (type)
			{
			case 105:
				return DeserializeInteger(din);
			case 115:
				return DeserializeString(din);
			case 97:
				return DeserializeStringArray(din);
			case 120:
				return DeserializeByteArray(din);
			case 110:
				return DeserializeIntArray(din);
			case 104:
				return DeserializeHashTable(din);
			case 68:
				return DeserializeDictionary(din);
			case 111:
				return DeserializeBoolean(din);
			case 107:
				return DeserializeShort(din);
			case 108:
				return DeserializeLong(din);
			case 98:
				return DeserializeByte(din);
			case 102:
				return DeserializeFloat(din);
			case 100:
				return DeserializeDouble(din);
			case 121:
				return DeserializeArray(din);
			case 99:
			{
				byte customTypeCode = din.ReadByte();
				return DeserializeCustom(din, customTypeCode);
			}
			case 122:
				return DeserializeObjectArray(din);
			case 101:
				return DeserializeEventData(din);
			case 113:
				return DeserializeOperationRequest(din);
			case 112:
				return DeserializeOperationResponse(din);
			case 0:
			case 42:
				return null;
			default:
				throw new Exception("Deserialize(): " + type + " pos: " + din.Position + " bytes: " + din.Length + ". " + SupportClass.ByteArrayToString(din.GetBuffer()));
			}
		}

		public override byte DeserializeByte(StreamBuffer din)
		{
			return din.ReadByte();
		}

		private bool DeserializeBoolean(StreamBuffer din)
		{
			return din.ReadByte() != 0;
		}

		public override short DeserializeShort(StreamBuffer din)
		{
			lock (memShort)
			{
				byte[] array = memShort;
				din.Read(array, 0, 2);
				return (short)((array[0] << 8) | array[1]);
			}
		}

		private int DeserializeInteger(StreamBuffer din)
		{
			lock (memInteger)
			{
				byte[] array = memInteger;
				din.Read(array, 0, 4);
				return (array[0] << 24) | (array[1] << 16) | (array[2] << 8) | array[3];
			}
		}

		private long DeserializeLong(StreamBuffer din)
		{
			lock (memLong)
			{
				byte[] array = memLong;
				din.Read(array, 0, 8);
				if (BitConverter.IsLittleEndian)
				{
					return (long)(((ulong)array[0] << 56) | ((ulong)array[1] << 48) | ((ulong)array[2] << 40) | ((ulong)array[3] << 32) | ((ulong)array[4] << 24) | ((ulong)array[5] << 16) | ((ulong)array[6] << 8) | array[7]);
				}
				return BitConverter.ToInt64(array, 0);
			}
		}

		private float DeserializeFloat(StreamBuffer din)
		{
			lock (memFloat)
			{
				byte[] array = memFloat;
				din.Read(array, 0, 4);
				if (BitConverter.IsLittleEndian)
				{
					byte b = array[0];
					byte b2 = array[1];
					array[0] = array[3];
					array[1] = array[2];
					array[2] = b2;
					array[3] = b;
				}
				return BitConverter.ToSingle(array, 0);
			}
		}

		private double DeserializeDouble(StreamBuffer din)
		{
			lock (memDouble)
			{
				byte[] array = memDouble;
				din.Read(array, 0, 8);
				if (BitConverter.IsLittleEndian)
				{
					byte b = array[0];
					byte b2 = array[1];
					byte b3 = array[2];
					byte b4 = array[3];
					array[0] = array[7];
					array[1] = array[6];
					array[2] = array[5];
					array[3] = array[4];
					array[4] = b4;
					array[5] = b3;
					array[6] = b2;
					array[7] = b;
				}
				return BitConverter.ToDouble(array, 0);
			}
		}

		private string DeserializeString(StreamBuffer din)
		{
			short num = DeserializeShort(din);
			if (num == 0)
			{
				return string.Empty;
			}
			if (memString == null || memString.Length < num)
			{
				memString = new byte[num];
			}
			din.Read(memString, 0, num);
			return Encoding.UTF8.GetString(memString, 0, num);
		}

		private Array DeserializeArray(StreamBuffer din)
		{
			short num = DeserializeShort(din);
			byte b = din.ReadByte();
			Array array = null;
			switch (b)
			{
			case 121:
			{
				Array array3 = DeserializeArray(din);
				Type type = array3.GetType();
				array = Array.CreateInstance(type, num);
				array.SetValue(array3, 0);
				for (short num4 = 1; num4 < num; num4 = (short)(num4 + 1))
				{
					array3 = DeserializeArray(din);
					array.SetValue(array3, num4);
				}
				break;
			}
			case 120:
			{
				array = Array.CreateInstance(typeof(byte[]), num);
				for (short num5 = 0; num5 < num; num5 = (short)(num5 + 1))
				{
					Array value2 = DeserializeByteArray(din);
					array.SetValue(value2, num5);
				}
				break;
			}
			case 98:
				array = DeserializeByteArray(din, num);
				break;
			case 105:
				array = DeserializeIntArray(din, num);
				break;
			case 99:
			{
				byte b2 = din.ReadByte();
				CustomType value;
				if (Protocol.CodeDict.TryGetValue(b2, out value))
				{
					array = Array.CreateInstance(value.Type, num);
					for (int i = 0; i < num; i++)
					{
						short num3 = DeserializeShort(din);
						if (value.DeserializeStreamFunction == null)
						{
							byte[] array2 = new byte[num3];
							din.Read(array2, 0, num3);
							array.SetValue(value.DeserializeFunction(array2), i);
						}
						else
						{
							array.SetValue(value.DeserializeStreamFunction(din, num3), i);
						}
					}
					break;
				}
				throw new Exception("Cannot find deserializer for custom type: " + b2);
			}
			case 68:
			{
				Array arrayResult = null;
				DeserializeDictionaryArray(din, num, out arrayResult);
				return arrayResult;
			}
			default:
			{
				array = CreateArrayByType(b, num);
				for (short num2 = 0; num2 < num; num2 = (short)(num2 + 1))
				{
					array.SetValue(Deserialize(din, b), num2);
				}
				break;
			}
			}
			return array;
		}

		private byte[] DeserializeByteArray(StreamBuffer din, int size = -1)
		{
			if (size == -1)
			{
				size = DeserializeInteger(din);
			}
			byte[] array = new byte[size];
			din.Read(array, 0, size);
			return array;
		}

		private int[] DeserializeIntArray(StreamBuffer din, int size = -1)
		{
			if (size == -1)
			{
				size = DeserializeInteger(din);
			}
			int[] array = new int[size];
			for (int i = 0; i < size; i++)
			{
				array[i] = DeserializeInteger(din);
			}
			return array;
		}

		private string[] DeserializeStringArray(StreamBuffer din)
		{
			int num = DeserializeShort(din);
			string[] array = new string[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = DeserializeString(din);
			}
			return array;
		}

		private object[] DeserializeObjectArray(StreamBuffer din)
		{
			short num = DeserializeShort(din);
			object[] array = new object[num];
			for (int i = 0; i < num; i++)
			{
				byte type = din.ReadByte();
				array[i] = Deserialize(din, type);
			}
			return array;
		}

		private Hashtable DeserializeHashTable(StreamBuffer din)
		{
			int num = DeserializeShort(din);
			Hashtable hashtable = new Hashtable(num);
			for (int i = 0; i < num; i++)
			{
				object obj = Deserialize(din, din.ReadByte());
				object value = Deserialize(din, din.ReadByte());
				if (obj != null)
				{
					hashtable[obj] = value;
				}
			}
			return hashtable;
		}

		private IDictionary DeserializeDictionary(StreamBuffer din)
		{
			byte b = din.ReadByte();
			byte b2 = din.ReadByte();
			int num = DeserializeShort(din);
			bool flag = b == 0 || b == 42;
			bool flag2 = b2 == 0 || b2 == 42;
			Type typeOfCode = GetTypeOfCode(b);
			Type typeOfCode2 = GetTypeOfCode(b2);
			Type type = typeof(Dictionary<, >).MakeGenericType(typeOfCode, typeOfCode2);
			IDictionary dictionary = Activator.CreateInstance(type) as IDictionary;
			for (int i = 0; i < num; i++)
			{
				object obj = Deserialize(din, flag ? din.ReadByte() : b);
				object value = Deserialize(din, flag2 ? din.ReadByte() : b2);
				if (obj != null)
				{
					dictionary.Add(obj, value);
				}
			}
			return dictionary;
		}

		private bool DeserializeDictionaryArray(StreamBuffer din, short size, out Array arrayResult)
		{
			byte keyTypeCode;
			byte valTypeCode;
			Type type = DeserializeDictionaryType(din, out keyTypeCode, out valTypeCode);
			arrayResult = Array.CreateInstance(type, size);
			for (short num = 0; num < size; num = (short)(num + 1))
			{
				IDictionary dictionary = Activator.CreateInstance(type) as IDictionary;
				if (dictionary == null)
				{
					return false;
				}
				short num2 = DeserializeShort(din);
				for (int i = 0; i < num2; i++)
				{
					object obj;
					if (keyTypeCode != 0)
					{
						obj = Deserialize(din, keyTypeCode);
					}
					else
					{
						byte type2 = din.ReadByte();
						obj = Deserialize(din, type2);
					}
					object value;
					if (valTypeCode != 0)
					{
						value = Deserialize(din, valTypeCode);
					}
					else
					{
						byte type3 = din.ReadByte();
						value = Deserialize(din, type3);
					}
					if (obj != null)
					{
						dictionary.Add(obj, value);
					}
				}
				arrayResult.SetValue(dictionary, num);
			}
			return true;
		}

		private Type DeserializeDictionaryType(StreamBuffer reader, out byte keyTypeCode, out byte valTypeCode)
		{
			keyTypeCode = reader.ReadByte();
			valTypeCode = reader.ReadByte();
			GpType gpType = (GpType)keyTypeCode;
			GpType gpType2 = (GpType)valTypeCode;
			Type type = ((gpType != 0) ? GetTypeOfCode(keyTypeCode) : typeof(object));
			Type type2 = ((gpType2 != 0) ? GetTypeOfCode(valTypeCode) : typeof(object));
			return typeof(Dictionary<, >).MakeGenericType(type, type2);
		}
	}
}
