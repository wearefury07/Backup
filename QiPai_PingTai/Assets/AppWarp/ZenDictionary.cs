using MsgPack;
using MsgPack.Serialization;
using System.IO;
using System.Text;
using UnityEngine;

public static class ZenMessagePack
{
    public static MemoryStream SerializeObject(string message)
    {
        var stream = new MemoryStream();
        var serializer = MessagePackSerializer.Get<string>();
		serializer.Pack(stream, message);
		Debug.Log (message);
        return stream;
    }

    public static MemoryStream SerializeObject<T>(T obj)
    {
        var message = "";
        message = JsonUtility.ToJson(obj);
		Debug.Log (message);
        var stream = SerializeObject(message);
        return stream;
    }

    public static JSONObject DeserializeObject(byte[] payLoadBytes)
    {
        try
        {
            var raw = Unpacking.UnpackObject(new MemoryStream(payLoadBytes));
            string payloadString = raw.ToString();
            var rawObject = new JSONObject(payloadString);
            return rawObject;
        }
        catch (System.Exception ex)
        {
            Debug.Log("DeserializeObject (1 param): throw Ex: " + ex.Message);
        }

        return null;
    }

    public static T DeserializeObject<T>(byte[] payLoadBytes, WarpContentTypeCode payloadType)
    {
        if (payloadType == WarpContentTypeCode.JSON)
        {
            var payLoad = Encoding.UTF8.GetString(payLoadBytes, 0, payLoadBytes.Length);
#if DEBUG
            //UILogView.Log(payLoad);
#endif
            return JsonUtility.FromJson<T>(payLoad);
        }
        else if (payloadType == WarpContentTypeCode.MESSAGE_PACK)
        {
            try
            {
                var raw = Unpacking.UnpackObject(payLoadBytes).Value.ToString();
                string payloadString = raw.ToString();
#if DEBUG
               UILogView.Log(payloadString);
#endif
                var rawObject = JsonUtility.FromJson<T>(payloadString);
                return rawObject;
            }
            catch (System.Exception ex)
            {
                UILogView.Log("DeserializeObject: +\n" + ex.Message, true);
            }
        }
        return default(T);
    }
}

public class ZenDictionary : JSONObject
{
    public ZenDictionary() : base(Type.OBJECT)
    {
    }
    public MemoryStream ToStream()
    {
        return ZenMessagePack.SerializeObject(this.Print());
    }
    public void Add(string key, string value)
    {
        AddField(key, value);
    }
    public void Add(string key, int value)
    {
        AddField(key, value);
    }
    public void Add(string key, long value)
    {
        AddField(key, value);
    }
    public void Add(string key, double value)
    {
        Add(key, (float)value);
    }
    public void Add(string key, float value)
    {
        AddField(key, value);
    }
    public void Add(string key, bool value)
    {
        AddField(key, value);
    }
    public void Add(string key, int[] value)
    {
        JSONObject arr = new JSONObject(JSONObject.Type.ARRAY);
        foreach (var i in value)
            arr.Add(i);
        AddField(key, arr);
        //base.Add(new MessagePackObject(key), new MessagePackObject(value.Select(x => new MessagePackObject(x)).ToList()));
    }
}
