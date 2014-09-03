using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Eb
{
    class Program
    {
        //---------------------------------------------------------------------
        static EntityMgr mEntityMgr = null;

        //---------------------------------------------------------------------
        static void Main(string[] args)
        {
            Console.Title = "EcTest";
            Console.ForegroundColor = ConsoleColor.Green;
            EbLog.NoteCallback = Console.WriteLine;
            EbLog.WarningCallback = Console.WriteLine;
            EbLog.ErrorCallback = Console.WriteLine;

            mEntityMgr = new EntityMgr();
            mEntityMgr.regComponentFactory(new ComponentFactory<EcApp<DefApp>>(mEntityMgr));
            mEntityMgr.create((byte)_eNodeType.Ec, _eNodeType.Ec.ToString(), 0, null);

            Entity et = mEntityMgr.createEmptyEntity("EtApp", null);
            et.addComponent<EcApp<DefApp>>();

            EbLog.Note("按任意键退出。。。");

            while (true)
            {
                mEntityMgr.update(0.0f);

                Thread.Sleep(100);

                if (Console.KeyAvailable) break;
            }

            mEntityMgr.Dispose();
        }
    }
}
