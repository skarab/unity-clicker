using UnityEngine;
using System.Collections;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Numerics;

public class Helpers
{
    public static string GetDataPath()
    {
#if UNITY_EDITOR
        return "../Build/Data/";
#else
        return Application.dataPath+"/../Data/";
#endif
    }

    public static System.Object Load(string path)
    {
        if (!File.Exists(path))
            return null;

        System.Object obj = null;

        try
        {
            IFormatter formatter = new BinaryFormatter();
            SurrogateSelector ss = new SurrogateSelector();
            ss.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), new Vector3SerializationSurrogate());
            ss.AddSurrogate(typeof(Quaternion), new StreamingContext(StreamingContextStates.All), new QuaternionSerializationSurrogate());
            ss.AddSurrogate(typeof(BigInteger), new StreamingContext(StreamingContextStates.All), new BigIntegerSerializationSurrogate());
            formatter.SurrogateSelector = ss;

            Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None);
            obj = formatter.Deserialize(stream);
            stream.Close();
        }
        catch(System.Exception e)
        {
            obj = null;
            Debug.Log(e.ToString());
            try
            {
                File.Delete(path);
            }
            catch(System.Exception f)
            {
            }
        }
        
        return obj;
    }

    public static void Save(System.Object obj, string path)
    {
        try
        {
            IFormatter formatter = new BinaryFormatter();
            SurrogateSelector ss = new SurrogateSelector();
            ss.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), new Vector3SerializationSurrogate());
            ss.AddSurrogate(typeof(Quaternion), new StreamingContext(StreamingContextStates.All), new QuaternionSerializationSurrogate());
            ss.AddSurrogate(typeof(BigInteger), new StreamingContext(StreamingContextStates.All), new BigIntegerSerializationSurrogate());
            formatter.SurrogateSelector = ss;

            Stream stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, obj);
            stream.Close();
        }
        catch(System.Exception e)
        {
            Debug.Log(e.ToString());
            try
            {
                File.Delete(path);
            }
            catch(System.Exception f)
            {
            }
        }
    }
}

sealed class Vector3SerializationSurrogate : ISerializationSurrogate
{
    public void GetObjectData(System.Object obj, SerializationInfo info, StreamingContext context)
    {
        Vector3 v3 = (Vector3) obj;
        info.AddValue("x", v3.x);
        info.AddValue("y", v3.y);
        info.AddValue("z", v3.z);
    }
 
    public System.Object SetObjectData(System.Object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
    {
        Vector3 v3 = (Vector3) obj;
        v3.x = (float)info.GetValue("x", typeof(float));
        v3.y = (float)info.GetValue("y", typeof(float));
        v3.z = (float)info.GetValue("z", typeof(float));
        obj = v3;
        return obj;
    }
}

sealed class QuaternionSerializationSurrogate : ISerializationSurrogate
{
    public void GetObjectData(System.Object obj, SerializationInfo info, StreamingContext context)
    {
        Quaternion q = (Quaternion) obj;
        info.AddValue("x", q.x);
        info.AddValue("y", q.y);
        info.AddValue("z", q.z);
        info.AddValue("w", q.w);
    }
 
    public System.Object SetObjectData(System.Object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
    {
        Quaternion q = (Quaternion) obj;
        q.x = (float)info.GetValue("x", typeof(float));
        q.y = (float)info.GetValue("y", typeof(float));
        q.z = (float)info.GetValue("z", typeof(float));
        q.w = (float)info.GetValue("w", typeof(float));
        obj = q;
        return obj;
    }
}

sealed class BigIntegerSerializationSurrogate : ISerializationSurrogate
{
    public void GetObjectData(System.Object obj, SerializationInfo info, StreamingContext context)
    {
        BigInteger i = (BigInteger) obj;
        info.AddValue("v", i.ToByteArray());
    }
 
    public System.Object SetObjectData(System.Object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
    {
        BigInteger i = (BigInteger) obj;
        i = new BigInteger((byte[])info.GetValue("v", typeof(byte[])));
        obj = i;
        return obj;
    }
}

