using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Eb
{
    class Program
    {
        //---------------------------------------------------------------------
        static void Main(string[] args)
        {
            Console.Title = "EbTest";
            Console.ForegroundColor = ConsoleColor.Green;
            EbLog.NoteCallback = Console.WriteLine;
            EbLog.WarningCallback = Console.WriteLine;
            EbLog.ErrorCallback = Console.WriteLine;

            TestMgr test_mgr = new TestMgr();

            //test_mgr.regTest(new TestDummy());
            //test_mgr.regTest(new TestEntity());
            test_mgr.regTest(new TestDataMgr());

            test_mgr.create();
            EbLog.Note("按任意键退出。。。");

            while (true)
            {
                test_mgr.update();

                Thread.Sleep(100);

                if (Console.KeyAvailable) break;
            }

            test_mgr.destroy();
        }
    }
}
