using System;
using System.IO;
using Newtonsoft.Json;

public static class EbJsonHelper
{
    //-------------------------------------------------------------------------
    //public static string serialize(object obj)
    //{
    //    JsonSerializer serializer = new JsonSerializer();
    //    StringWriter sw = new StringWriter();
    //    serializer.Serialize(new JsonTextWriter(sw), obj);
    //    return sw.ToString();
    //}

    ////-------------------------------------------------------------------------
    //public static T deserialize<T>(string str_json)
    //{
    //    JsonReader reader = new JsonTextReader(new StringReader(str_json));
    //    JsonSerializer serializer = new JsonSerializer();
    //    return serializer.Deserialize<T>(reader);
    //}

    //-------------------------------------------------------------------------
    public static string serialize(object obj)
    {
        return JsonConvert.SerializeObject(obj);
    }

    //-------------------------------------------------------------------------
    public static T deserialize<T>(string str_json)
    {
        return (T)JsonConvert.DeserializeObject<T>(str_json);
    }
}