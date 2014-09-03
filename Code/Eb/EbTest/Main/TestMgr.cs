using System;
using System.Collections.Generic;
using System.Text;

namespace Eb
{
    class TestMgr
    {
        //---------------------------------------------------------------------
        List<Test> mListTest = new List<Test>();

        //---------------------------------------------------------------------
        public void create()
        {
            foreach (var i in mListTest)
            {
                i.init();
            }
        }

        //---------------------------------------------------------------------
        public void destroy()
        {
            foreach (var i in mListTest)
            {
                i.release();
            }
            mListTest.Clear();
        }

        //---------------------------------------------------------------------
        public void update()
        {
            foreach (var i in mListTest)
            {
                i.update();
            }
        }

        //---------------------------------------------------------------------
        public void regTest(Test test)
        {
            mListTest.Add(test);
        }
    }
}
