using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Eb
{
    public class EbScriptMgr : CSLE.ICLS_Logger
    {
        //---------------------------------------------------------------------
        static EbScriptMgr mScriptMgr;
        CSLE.CLS_Environment mEnvironment;
        CSLE.CLS_Content mContent;
        // key=file_name，短文件名，不含扩展名，value=文件内容
        Dictionary<string, string> mMapFile = new Dictionary<string, string>();

        //---------------------------------------------------------------------
        public CSLE.CLS_Environment Env { get { return mEnvironment; } }

        //-------------------------------------------------------------------------
        static public EbScriptMgr Instance
        {
            get { return mScriptMgr; }
        }

        //-------------------------------------------------------------------------
        public EbScriptMgr()
        {
            mScriptMgr = this;
        }

        //---------------------------------------------------------------------
        public void Log(string str)
        {
            EbLog.Note(str);
        }

        //---------------------------------------------------------------------
        public void Log_Warn(string str)
        {
            EbLog.Warning(str);
        }

        //---------------------------------------------------------------------
        public void Log_Error(string str)
        {
            EbLog.Error(str);
        }

        //---------------------------------------------------------------------
        public void create(string dir_script)
        {
            mEnvironment = new CSLE.CLS_Environment((CSLE.ICLS_Logger)this);

            mEnvironment.RegType(new CSLE.RegHelper_Type(typeof(EbLog)));
            mEnvironment.RegType(new CSLE.RegHelper_Type(typeof(EntityMgr)));
            mEnvironment.RegType(new CSLE.RegHelper_Type(typeof(Entity)));
            mEnvironment.RegType(new CSLE.RegHelper_Type(typeof(IComponent)));

            string[] list_script = Directory.GetFiles(dir_script, "*.txt", System.IO.SearchOption.AllDirectories);

            // Build
            Dictionary<string, IList<CSLE.Token>> project = new Dictionary<string, IList<CSLE.Token>>();
            IList<CSLE.Token> tokens;
            foreach (var i in list_script)
            {
                string c = System.IO.File.ReadAllText(i);
                mMapFile[Path.GetFileNameWithoutExtension(i)] = c;
                EbLog.Note("解析脚本 file_name=" + Path.GetFileNameWithoutExtension(i));
                try
                {
                    tokens = mEnvironment.ParserToken(c);
                    project[Path.GetFileNameWithoutExtension(i)] = tokens;
                }
                catch (Exception ec)
                {
                    EbLog.Error("EbScriptMgr Build Script Error! File=" + i);
                    EbLog.Error(ec.ToString());
                    continue;
                }
            }

            // Compiler
            try
            {
                mEnvironment.Project_Compiler(project, false);
            }
            catch (Exception ec)
            {
                EbLog.Error("EbScriptMgr Compiler Script Error!");
                EbLog.Error(ec.ToString());
            }
        }

        //---------------------------------------------------------------------
        public void destroy()
        {
        }

        //---------------------------------------------------------------------
        public void setValue(string name, object v)
        {
            if (mContent == null)
            {
                mContent = mEnvironment.CreateContent();
            }
            mContent.DefineAndSet(name, v.GetType(), v);
        }

        //---------------------------------------------------------------------
        public void clearAllValue()
        {
            mContent = null;
        }

        //---------------------------------------------------------------------
        public object doFile(string file_name)
        {
            string code;
            mMapFile.TryGetValue(file_name, out code);
            if (code == null)
            {
                EbLog.Error("EbScriptMgr.runScript() Error! 读取文件失败,File=" + file_name);
                return null;
            }

            if (mContent == null)
            {
                mContent = mEnvironment.CreateContent();
            }

            try
            {
                var tokens = mEnvironment.tokenParser.Parse(code);
                var expr = mEnvironment.Expr_CompilerToken(tokens);
                return mEnvironment.Expr_Execute(expr, mContent);
            }
            catch (Exception ec)
            {
                EbLog.Error("EbScriptMgr.runScript() Error!");
                EbLog.Error(ec.ToString());
                EbLog.Error(mContent.DumpValue());
                EbLog.Error(mContent.DumpStack(null));
            }

            return null;
        }
    }
}
