using System;
using System.Collections.Generic;
using System.Text;
using Hakoniwa.Core.Utils;

namespace HakoniwaCoreTest
{
    class LineBinaryStorageTest
    {
        private LineBinaryStorage obj;
        public LineBinaryStorageTest()
        {
        }

        private void Prepare()
        {
            this.obj = new LineBinaryStorage(2, 8);
            this.obj.SetReallocLines(2);
        }

        private bool TestGet(int line, int column_id, UInt64 expect_value)
        {
            UInt64 ret_value;
            if (obj.GetData(line, column_id, out ret_value))
            {
                if (ret_value == expect_value)
                {
                    Console.WriteLine("PASSED: line=" + line.ToString() + " column_id=" + column_id.ToString() + " expect_value=" + expect_value.ToString() + " real_value=" + ret_value.ToString());
                    return true;
                }
                else
                {
                    Console.WriteLine("ERROR: line=" + line.ToString() + " column_id=" + column_id.ToString() + " expect_value=" + expect_value.ToString() + " real_value=" + ret_value.ToString());
                }

            }
            else
            {
                Console.WriteLine("ERROR: line=" + line.ToString() + " column_id=" + column_id.ToString());
            }
            return false;
        }

        private void DoTest1()
        {
            UInt64 c1_value = 1;
            UInt64 c2_value = 10001;
            obj.SetData(0, c1_value);
            obj.SetData(1, c2_value);

            this.TestGet(0, 0, 1);
            this.TestGet(0, 1, 10001);
        }
        private void DoTest2()
        {
            obj.Next();
            UInt64 c1_value = 2;
            UInt64 c2_value = 10002;
            obj.SetData(0, c1_value);
            obj.SetData(1, c2_value);

            this.TestGet(0, 0, 1);
            this.TestGet(0, 1, 10001);
            this.TestGet(1, 0, 2);
            this.TestGet(1, 1, 10002);
        }
        private void DoTest3()
        {
            obj.Next();
            UInt64 c1_value = 3;
            UInt64 c2_value = 10003;
            obj.SetData(0, c1_value);
            obj.SetData(1, c2_value);

            this.TestGet(0, 0, 1);
            this.TestGet(0, 1, 10001);
            this.TestGet(1, 0, 2);
            this.TestGet(1, 1, 10002);
            this.TestGet(2, 0, 3);
            this.TestGet(2, 1, 10003);
        }

        public void DoTestFile(string path)
        {
            SimulationTimeStorage storage = new SimulationTimeStorage(2);

            string[] names = new string[2];
            names[0] = "Unity";
            names[1] = "Athrill";
            storage.SetAssetNames(names);

            storage.SetSimTime(0, 10001);
            storage.SetSimTime(1, 10002);
            storage.Next();

            storage.SetSimTime(0, 20001);
            storage.SetSimTime(1, 20002);
            storage.Next();

            storage.SetSimTime(0, 30001);
            storage.SetSimTime(1, 30002);
            storage.Next();


            storage.Flush(path);
        }


        public void DoTest()
        {
            this.Prepare();
            this.DoTest1();
            this.DoTest2();
            this.DoTest3();
            this.DoTestFile(@"C:\project\oss\hakoniwa-core\impl\asset\server\csharp\HakoniwaCoreTest\HakoniwaCoreTest\test1.csv");
        }
    }
}
