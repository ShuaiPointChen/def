using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace Eb
{
    public class EbDataMgr
    {
        //---------------------------------------------------------------------
        static EbDataMgr mDataMgr;
        SQLiteDB mSqliteDb = null;
        EbFileStream mFileStream = new EbFileStreamDefault();
        EbDb mDb = new EbDb();
        EbDbInfo mDbInfo = new EbDbInfo();

        //---------------------------------------------------------------------
        static public EbDataMgr Instance
        {
            get { return mDataMgr; }
        }

        //---------------------------------------------------------------------
        public EbDataMgr()
        {
            mDataMgr = this;
        }

        //---------------------------------------------------------------------
        public void setup(string db_name, string db_filename, string dbinfo_dir, string dbforeignkeyinfo_filename)
        {
            // 加载Table描述文件
            mDbInfo.ListTable = new List<EbTableInfo>();
            string[] list_tableinfofile = Directory.GetFiles(dbinfo_dir, "*.json");
            foreach (var i in list_tableinfofile)
            {
                bool result = mFileStream.load(i);
                if (!result)
                {
                    EbLog.Note("EbDataMgr.setup() failed! tableinfo_filename=" + dbforeignkeyinfo_filename);
                    return;
                }
                string buf = mFileStream.getDataAsString();
                EbTableInfo table_info = JsonConvert.DeserializeObject<EbTableInfo>(buf);
                mDbInfo.ListTable.Add(table_info);
            }

            // 加载ForeignKey描述文件
            {
                bool result = mFileStream.load(dbforeignkeyinfo_filename);
                if (!result)
                {
                    EbLog.Note("EbDataMgr.setup() failed! dbforeignkeyinfo_filename=" + dbforeignkeyinfo_filename);
                    return;
                }
                string buf = mFileStream.getDataAsString();
                List<EbForeignKeyInfo> list_foreignkeyinfo = JsonConvert.DeserializeObject<List<EbForeignKeyInfo>>(buf);
                mDbInfo.ListForeignKeyInfo = list_foreignkeyinfo;
            }
            
            // 加载所有Db数据
            if (!mFileStream.load(db_filename))
            {
                EbLog.Note("EbDataMgr.setup() failed! db_filename=" + db_filename);
                return;
            }

            mSqliteDb = new SQLiteDB();

            try
            {
                byte[] bytes = mFileStream.getData();
                using (var mem_stream = new MemoryStream(bytes, 0, bytes.Length))
                {
                    mSqliteDb.OpenStream(db_name, mem_stream);

                    // 加载所有Table数据
                    foreach (var i in mDbInfo.ListTable)
                    {
                        _loadTable(i.TableName, i.ListFieldInfo);
                    }

                    mSqliteDb.Close();
                }
            }
            catch (Exception e)
            {
                EbLog.Note(e.ToString());
            }
        }

        //---------------------------------------------------------------------
        public EbTable getTable(string table_name)
        {
            return mDb._getTable(table_name);
        }

        //---------------------------------------------------------------------
        public void genTestDbDescFile(string dbinfo_filename)
        {
            EbDbInfo db_info = new EbDbInfo();

            // 所有表格信息
            db_info.ListTable = new List<EbTableInfo>();

            {
                EbTableInfo table_info;
                table_info.TableName = "Effect";
                table_info.ListFieldInfo = new List<EbFieldInfo>();

                EbFieldInfo fi1;
                fi1.FieldName = "Id";
                fi1.FieldType = EbFieldType.Int;
                table_info.ListFieldInfo.Add(fi1);

                EbFieldInfo fi2;
                fi2.FieldName = "Name";
                fi2.FieldType = EbFieldType.String;
                table_info.ListFieldInfo.Add(fi2);

                db_info.ListTable.Add(table_info);
            }

            {
                EbTableInfo table_info;
                table_info.TableName = "Item";
                table_info.ListFieldInfo = new List<EbFieldInfo>();

                EbFieldInfo fi1;
                fi1.FieldName = "Id";
                fi1.FieldType = EbFieldType.Int;
                table_info.ListFieldInfo.Add(fi1);

                EbFieldInfo fi2;
                fi2.FieldName = "Name";
                fi2.FieldType = EbFieldType.String;
                table_info.ListFieldInfo.Add(fi2);

                db_info.ListTable.Add(table_info);
            }

            // 所有外键信息
            List<EbForeignKeyInfo> list_foreignkeyinfo = new List<EbForeignKeyInfo>();

            {
                EbForeignKeyInfo fki;
                fki.Table = "Item";
                fki.Key = "Effect1";
                fki.ForeignTable = "Effect";
                fki.ForeignKey = "Id";
                list_foreignkeyinfo.Add(fki);
            }

            {
                EbForeignKeyInfo fki;
                fki.Table = "Item";
                fki.Key = "Effect2";
                fki.ForeignTable = "Effect";
                fki.ForeignKey = "Id";
                list_foreignkeyinfo.Add(fki);
            }

            // 序列化
            string str_json = JsonConvert.SerializeObject(list_foreignkeyinfo);
            byte[] str = System.Text.Encoding.Default.GetBytes(str_json);
            bool result = mFileStream.save(dbinfo_filename, str);
            if (!result)
            {
                EbLog.Note("EbDataMgr.genTestDbDescFile() failed! dbinfo_filename=" + dbinfo_filename);
            }
        }

        //---------------------------------------------------------------------
        void _loadTable(string table_name, List<EbFieldInfo> list_fieldinfo)
        {
            EbTable table = new EbTable();
            table.Name = table_name;
            foreach (var i in list_fieldinfo)
            {
                switch (i.FieldType)
                {
                    case EbFieldType.Int:// int
                        {
                            PropDef prop_def = new PropDef(i.FieldName, typeof(int));
                            table._addPropDef(prop_def);
                        }
                        break;
                    case EbFieldType.Float:// float
                        {
                            PropDef prop_def = new PropDef(i.FieldName, typeof(float));
                            table._addPropDef(prop_def);
                        }
                        break;
                    case EbFieldType.String:// string
                        {
                            PropDef prop_def = new PropDef(i.FieldName, typeof(string));
                            table._addPropDef(prop_def);
                        }
                        break;
                    default:
                        break;
                }
            }

            string str_query_select = string.Format("SELECT * FROM {0};", table_name);
            SQLiteQuery qr;
            qr = new SQLiteQuery(mSqliteDb, str_query_select);

            while (qr.Step())
            {
                EbPropSet prop_set = new EbPropSet();
                foreach (var i in list_fieldinfo)
                {
                    bool is_null = qr.IsNULL(i.FieldName);
                    if (is_null)
                    {
                        prop_set._addProp(i.FieldName, null);
                        continue;
                    }

                    switch (i.FieldType)
                    {
                        case EbFieldType.Int:// int
                            {
                                PropDef prop_def = table.getPropDef(i.FieldName);
                                Prop<int> prop = new Prop<int>(prop_def, 0);
                                prop.set(qr.GetInteger(i.FieldName));
                                prop_set._addProp(i.FieldName, prop);
                            }
                            break;
                        case EbFieldType.Float:// float
                            {
                                PropDef prop_def = table.getPropDef(i.FieldName);
                                Prop<float> prop = new Prop<float>(prop_def, 0f);
                                prop.set((float)qr.GetDouble(i.FieldName));
                                prop_set._addProp(i.FieldName, prop);
                            }
                            break;
                        case EbFieldType.String:// string
                            {
                                PropDef prop_def = table.getPropDef(i.FieldName);
                                Prop<string> prop = new Prop<string>(prop_def, "");
                                prop.set(qr.GetString(i.FieldName));
                                prop_set._addProp(i.FieldName, prop);
                            }
                            break;
                        default:
                            break;
                    }
                }

                IProp prop_id = prop_set.getProp("Id");
                if (prop_id == null)
                {
                    EbLog.Error("EbDataMgr1._loadTable() Error! Key=Id not exist, TableName=" + table_name);
                    continue;
                }
                Prop<int> p = (Prop<int>)prop_id;
                prop_set.Id = p.get();
                table._addPropSet(prop_set);
            }

            qr.Release();

            mDb._addTable(table);
        }
    }
}
