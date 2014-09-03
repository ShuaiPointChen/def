using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

public enum ViSealDataState
{
    ACTIVE,
    DEACTIVE,
}

public class ViSealedData
{
    public Int32 ID = -1;
    public string Name = string.Empty;
    public string Note = string.Empty;
    public ViEnum32<ViSealDataState> State;

    public virtual void Fomat() { }
    public virtual void Start() { }
}

public class ViSealedFile<T> where T : ViSealedData, new()
{
    public static string Name = string.Empty;
}

public static class ViSealedDB<T> where T : ViSealedData, new()
{
    static Dictionary<string, T> _idxDatas = new Dictionary<string, T>();
    static Dictionary<string, T> _nameDatas = new Dictionary<string, T>();
    static T _defaultData = new T();
    public static Dictionary<string, T> Datas { get { return _idxDatas; } }

    public static T Data(Int32 ID)
    {
        T data = default(T);
        if (_idxDatas.TryGetValue(ID.ToString(), out data))
        {
            return data;
        }
        //ViDebuger.Warning("There is no data<" + typeof(T).Name + ">[" + ID + "]");
        if (_idxDatas.TryGetValue("0", out data))
        {
            return data;
        }
        return _defaultData;
    }

    public static T Data(UInt32 ID)
    {
        return Data((Int32)ID);
    }

    public static T GetData(Int32 ID)
    {
        T data = default(T);
        bool exist = _idxDatas.TryGetValue(ID.ToString(), out data);
        if (exist == false)
        {
            //ViDebuger.Warning("There is no data<" + typeof(T).Name + ">[" + ID + "]");
        }
        return data;
    }

    public static T GetData(UInt32 ID)
    {
        return GetData((Int32)ID);
    }

    public static T Data(string name)
    {
        T data = default(T);
        if (_nameDatas.TryGetValue(name, out data))
        {
            return data;
        }
        if (_idxDatas.TryGetValue("0", out data))
        {
            return data;
        }
        return _defaultData;
    }

    public static T GetData(string name)
    {
        T data = default(T);
        _nameDatas.TryGetValue(name, out data);
        return data;
    }

    public static void AddData(T data)
    {
        //ViDebuger.Note("ViSealedDB<" + typeof(T).Name + ">AddData(ID = " + data.ID + ", Name = " + data.Name + ")");
        if (_idxDatas.ContainsKey(data.ID.ToString()))
        {
            ViDebuger.Warning("ID重复<" + data.ID + ", " + data.Name + ">");
            return;
        }
        data.Fomat();
        _idxDatas.Add(data.ID.ToString(), data);
        if (_nameDatas.ContainsKey(data.Name))
        {

        }
        else
        {
            _nameDatas.Add(data.Name, data);
        }
    }

    public static void Clear()
    {
        _idxDatas.Clear();
        _nameDatas.Clear();
    }

    public static void Load(string file)
    {
        Clear();
        if (File.Exists(file) == false)
        {
            ViDebuger.Warning("没有发现指定文件" + file);
            return;
        }
        FileStream br = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read);
        if (br == null)
        {
            return;
        }
        byte[] bufferArray = new byte[br.Length];
        br.Read(bufferArray, 0, bufferArray.Length);
        ViIStream IS = new ViIStream();
        IS.Init(bufferArray, bufferArray.Length);
        Load(IS);
        br.Close();
    }

    public static void Load(ViIStream IS)
    {
        Clear();
        while (IS.RemainLength > 0)
        {
            T data;
            if (ViBinaryReader.ReadSealedData(IS, out data) == false)
            {
                break;
            }
            if (data.State == (Int32)ViSealDataState.ACTIVE)
            {
                AddData(data);
            }
        }
    }
}

public static class ViSealedDataAssisstant
{
    public static readonly BindingFlags BindingFlag = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    public static FieldInfo[] GetFeilds(Type type)
    {
        if (type.IsSubclassOf(typeof(ViSealedData)))
        {
            FieldInfo[] fields = type.GetFields(BindingFlag);
            // 修正继承属性的位置
            FieldInfo[] fieldList = new FieldInfo[fields.Length];
            int headSize = 4;
            for (int idx = headSize; idx < fields.Length; ++idx)
            {
                fieldList[idx] = fields[idx - headSize];
            }
            for (int idx = 0; idx < headSize; ++idx)
            {
                fieldList[idx] = fields[fields.Length - headSize + idx];
            }
            return fieldList;
        }
        else
        {
            FieldInfo[] fieldList = type.GetFields(BindingFlag);
            return fieldList;
        }
    }
}
