using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Eb
{
    public enum EbFieldType
    {
        None = 0,
        Int,
        Float,
        String
    }

    [JsonObject(MemberSerialization.OptOut)]
    public struct EbForeignKeyInfo
    {
        public string Table;
        public string Key;
        public string ForeignTable;
        public string ForeignKey;
    }

    [JsonObject(MemberSerialization.OptOut)]
    public struct EbFieldInfo
    {
        public string FieldName;
        public EbFieldType FieldType;
    }

    [JsonObject(MemberSerialization.OptOut)]
    public struct EbTableInfo
    {
        public string TableName;
        public List<EbFieldInfo> ListFieldInfo;
    }

    public class EbDbInfo
    {
        public List<EbTableInfo> ListTable;
        public List<EbForeignKeyInfo> ListForeignKeyInfo;
    }
}
