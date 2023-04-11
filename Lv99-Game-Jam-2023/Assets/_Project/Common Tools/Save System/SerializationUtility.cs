using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;

namespace Tensori.SaveSystem
{
    public static class SerializationUtility
    {
        public enum SizeUnits
        {
            Byte, KB, MB, GB, TB, PB, EB, ZB, YB
        }

        public static string ToSize(this long value, SizeUnits unit)
        {
            return (value / (double)Math.Pow(1024, (long)unit)).ToString("0.00");
        }

        public static byte[] SerializeToByteArray(this object obj, bool compress = false)
        {
            if (obj == null)
                return null;

            BinaryFormatter _binaryFormatter = new BinaryFormatter();

            using (MemoryStream _memStream = new MemoryStream())
            {
                _binaryFormatter.Serialize(_memStream, obj);

                if (compress)
                    return _memStream.ToArray().Compress();
                else
                    return _memStream.ToArray();
            }
        }

        public static T DeserializeFromByteArray<T>(this byte[] byteArray, bool isCompressed = false) where T : class
        {
            if (byteArray == null)
                return null;

            try
            {
                if (isCompressed)
                    byteArray = byteArray.Decompress();

                using (MemoryStream _memStream = new MemoryStream())
                {
                    BinaryFormatter _binaryFormatter = new BinaryFormatter();
                    _memStream.Write(byteArray, 0, byteArray.Length);
                    _memStream.Seek(0, SeekOrigin.Begin);
                    T obj = (T)_binaryFormatter.Deserialize(_memStream);
                    return obj;
                }
            }
            catch (Exception _exception)
            {
                UnityEngine.Debug.LogError($"SerializationUtility.DeserializeFromByteArray(): deserializing type {nameof(T)} exception: {_exception.ToString()}");
            }

            return null;
        }

        public static byte[] Compress(this byte[] data)
        {
            MemoryStream _output = new MemoryStream();

            using (DeflateStream _dstream = new DeflateStream(_output, CompressionLevel.Optimal))
            {
                _dstream.Write(data, 0, data.Length);
            }

            return _output.ToArray();
        }

        public static byte[] Decompress(this byte[] data)
        {
            MemoryStream input = new MemoryStream(data);
            MemoryStream output = new MemoryStream();

            using (DeflateStream dstream = new DeflateStream(input, CompressionMode.Decompress))
            {
                dstream.CopyTo(output);
            }

            return output.ToArray();
        }
    }
}

[System.Serializable]
public struct SerializableVector3
{
    /// <summary>
    /// x component
    /// </summary>
    public float x;

    /// <summary>
    /// y component
    /// </summary>
    public float y;

    /// <summary>
    /// z component
    /// </summary>
    public float z;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="rX"></param>
    /// <param name="rY"></param>
    /// <param name="rZ"></param>
    public SerializableVector3(float rX, float rY, float rZ)
    {
        x = rX;
        y = rY;
        z = rZ;
    }

    /// <summary>
    /// Returns a string representation of the object
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return String.Format("[{0}, {1}, {2}]", x, y, z);
    }

    /// <summary>
    /// Automatic conversion from SerializableVector3 to Vector3
    /// </summary>
    /// <param name="rValue"></param>
    /// <returns></returns>
    public static implicit operator UnityEngine.Vector3(SerializableVector3 rValue)
    {
        return new UnityEngine.Vector3(rValue.x, rValue.y, rValue.z);
    }

    /// <summary>
    /// Automatic conversion from Vector3 to SerializableVector3
    /// </summary>
    /// <param name="rValue"></param>
    /// <returns></returns>
    public static implicit operator SerializableVector3(UnityEngine.Vector3 rValue)
    {
        return new SerializableVector3(rValue.x, rValue.y, rValue.z);
    }
}

[System.Serializable]
public struct SerializableVector3Int
{
    /// <summary>
    /// x component
    /// </summary>
    public int x;

    /// <summary>
    /// y component
    /// </summary>
    public int y;

    /// <summary>
    /// z component
    /// </summary>
    public int z;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="rX"></param>
    /// <param name="rY"></param>
    /// <param name="rZ"></param>
    public SerializableVector3Int(int rX, int rY, int rZ)
    {
        x = rX;
        y = rY;
        z = rZ;
    }

    /// <summary>
    /// Returns a string representation of the object
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return String.Format("[{0}, {1}, {2}]", x.ToStringMinimalAlloc(), y.ToStringMinimalAlloc(), z.ToStringMinimalAlloc());
    }

    /// <summary>
    /// Automatic conversion from SerializableVector3 to Vector3
    /// </summary>
    /// <param name="rValue"></param>
    /// <returns></returns>
    public static implicit operator UnityEngine.Vector3Int(SerializableVector3Int rValue)
    {
        return new UnityEngine.Vector3Int(rValue.x, rValue.y, rValue.z);
    }

    /// <summary>
    /// Automatic conversion from Vector3 to SerializableVector3
    /// </summary>
    /// <param name="rValue"></param>
    /// <returns></returns>
    public static implicit operator SerializableVector3Int(UnityEngine.Vector3Int rValue)
    {
        return new SerializableVector3Int(rValue.x, rValue.y, rValue.z);
    }
}
