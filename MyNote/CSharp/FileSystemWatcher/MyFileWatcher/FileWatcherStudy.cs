using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MyFileWatcher
{
    class FileWatcherStudy
    {       
        private static FileSystemWatcher  fsWatcher;
        private static string filePath = @"D:\CodeStudy\FileWatcer\result.txt";
        private static int _num=0;
        private static int _errorNum=0;
        private static int _creatNum = 0;
        private static int _changeNum = 0;
        private static int _deleteNum = 0;
        public static void FileWatcher()
        {
            //if (File.Exists(filePath)) Console.WriteLine("FileContent: " + File.ReadAllText(filePath));

            //创建文件监听对象（带参）
            //FileSystemWatcher fsWatcher = new FileSystemWatcher(@"D:\CodeStudy\FileWatcer\","result.txt");
            
            //创建文件监听对象（无参）
            fsWatcher = new FileSystemWatcher();
            //设置要监听的文件夹！
            fsWatcher.Path = @"D:\CodeStudy\FileWatcer\txtFile\";
            //设置要监听的单个文件
           //fsWatcher.Filter = "*.txt";
            fsWatcher.Filter = "*.txt";
            //子文件夹是否要加入监听范围
            fsWatcher.IncludeSubdirectories = true;
            //设置要监听的文件属性
            //fsWatcher.NotifyFilter = NotifyFilters.LastWrite|NotifyFilters.Attributes|NotifyFilters.Size;
            fsWatcher.NotifyFilter = NotifyFilters.Attributes|NotifyFilters.FileName|NotifyFilters.LastWrite|NotifyFilters.CreationTime;
           //fsWatcher.NotifyFilter = NotifyFilters.Security;

            //添加事件处理,"Changed"事件会有一次触发执行两次的bug
            fsWatcher.Changed += new FileSystemEventHandler(OnProcess);
            fsWatcher.Created += new FileSystemEventHandler(OnProcess);
            fsWatcher.Deleted += new FileSystemEventHandler(OnProcess);
            fsWatcher.Renamed += new RenamedEventHandler(OnRenamed);

            //开始监听事件,设置为true后会一直监听事件，直到程序结束
            //方法注释上定义：设置监听事件是否启动，默认为false，不启动
            fsWatcher.EnableRaisingEvents = true;

            //在当前代码处等待，指定事件触发一次后继续往下执行
            //EnableRaisingEvents = false;时也会等待触发一次事件
            fsWatcher.WaitForChanged(WatcherChangeTypes.Changed);

            //fsWatcher.BeginInit();

            //设置读取按键防止程序结束
            Console.WriteLine("Press \'q\' to quit the sample program");
            while (Console.Read() != 'q') ;
        }
        private static void OnProcess(object sender, FileSystemEventArgs e)
        {
            switch (e.ChangeType)
            {
                case WatcherChangeTypes.Created:
                    //每次创建文件会有一个Created事件和一个Changed事件
                    //所以需要执行关闭和打开 EnableRaisingEvents 的操作
                    fsWatcher.EnableRaisingEvents = false;
                    OnCreated(sender, e);
                    fsWatcher.EnableRaisingEvents = true;
                    break;
                case WatcherChangeTypes.Deleted:
                    OnDeleted(sender,e);
                    break;
                case WatcherChangeTypes.Changed:
                    //每次触发后先关闭监听事件，执行方法后再打开监听事件，可以避免多次触发的问题
                    fsWatcher.EnableRaisingEvents = false;
                    OnChanged(sender, e);
                    fsWatcher.EnableRaisingEvents = true;
                    break;
                default:
                    break;
            }
        }
        private static void OnChanged(object sender,FileSystemEventArgs e) 
        {
            
            //fsWatcher.EnableRaisingEvents = false;
            OnChangedOperation(sender,  e);           
            //fsWatcher.EnableRaisingEvents = true;
        }

        private static void OnChangedOperation(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Changed)
            {
                Console.WriteLine($"File: {e.FullPath}---{e.ChangeType}---{System.DateTime.Now.ToString("HH:mm:ss:ff")}");     
            }
            //读取文件内容时会有进程冲突的问题
            //使用异常和线程等待解决文件被占用的问题
            while (true)
            {
                try
                {
                    using (Stream stream = File.Open(e.FullPath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        if (stream != null)
                        {
                            break;
                        }
                    }
                }
                catch (Exception ex1)
                {
                    System.Threading.Thread.Sleep(500);
                }
            }
            FileInfo fi = new FileInfo(e.FullPath);
            Console.WriteLine("File is read only? : " +fi.IsReadOnly);
            if (!fi.IsReadOnly)
            {
                try
                {
                    using (FileStream fs = new FileStream(e.FullPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        Console.WriteLine("Content: " + File.ReadAllText(e.FullPath));
                    };
                    //fi.OpenRead();
                }
                catch (Exception ex1)
                {
                    Console.WriteLine("Exception: " + ex1.Message);
                    _errorNum++;
                    //throw;
                }
            }
            _changeNum++;_num++;
            Console.WriteLine("ErrorCount: " + (_errorNum));
            Console.WriteLine("CreatCount: " + (_creatNum));
            Console.WriteLine("ChangeCount: " + (_changeNum));
            Console.WriteLine("DeleteCount: " + (_deleteNum));
            Console.WriteLine("Count: " + (_num));
            Console.WriteLine("EndTime:"+System.DateTime.Now.ToString("HH:mm:ss:ff"));
            Console.WriteLine();
        }

        private static void OnCreated(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Created)
            {
                Console.WriteLine($"File: {e.FullPath}---{e.ChangeType}---{System.DateTime.Now.ToString("HH:mm:ss:ff")}");
            }
            while (true)
            {
                try
                {
                    using (Stream stream = File.Open(e.FullPath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        if (stream != null) break;
                    }
                }
                catch (Exception ex)
                {
                    System.Threading.Thread.Sleep(500);
                    //throw;
                }
            }
            try
            {
                Console.WriteLine("Content:" +File.ReadAllText(e.FullPath));
            }
            catch (Exception ex)
            {
                Console.WriteLine("ExceptionInfo:" + ex.Message);
                _errorNum++;
                //throw;
            }
            _creatNum++;  _num++;
            Console.WriteLine("ErrorCount: " + (_errorNum));
            Console.WriteLine("CreatCount: " + (_creatNum));
            Console.WriteLine("ChangeCount: " + (_changeNum));
            Console.WriteLine("DeleteCount: " + (_deleteNum));
            Console.WriteLine("Count: " + (_num));
            Console.WriteLine("EndTime:" + System.DateTime.Now.ToString("HH:mm:ss:ff"));
            Console.WriteLine();
        }
        private static void OnDeleted(object sender, FileSystemEventArgs e)
        { 
            Console.WriteLine($"File: {e.FullPath}---{e.ChangeType}---{System.DateTime.Now.ToString()}");
            Console.WriteLine("File is deleted?: "+File.Exists(e.FullPath));
            _num++;_deleteNum++;
            Console.WriteLine("ErrorCount: " + (_errorNum));
            Console.WriteLine("CreatCount: " + (_creatNum));
            Console.WriteLine("ChangeCount: " + (_changeNum));
            Console.WriteLine("DeleteCount: " + (_deleteNum));
            Console.WriteLine("Count: " + (_num) + "\n");

        }
        private static void OnRenamed(object sender, RenamedEventArgs e)
        {
            Console.WriteLine($"File: {e.FullPath}---{e.ChangeType}---{e.OldName}---{System.DateTime.Now.ToString()}");
            while (true)
            {
                try
                {
                    using (Stream stream = File.Open(e.FullPath,FileMode.Open,FileAccess.ReadWrite,FileShare.ReadWrite))
                    {
                        break;
                    }
                }
                catch (Exception ex)
                {
                    System.Threading.Thread.Sleep(500);
                    //throw;
                }
            }
            try
            {
                Console.WriteLine("Content: "+File.ReadAllText(e.FullPath));
            }
            catch (Exception ex)
            {
                Console.WriteLine("ExceptionInfo:" + ex.Message);
                _errorNum++;
                //throw;
            }
            Console.WriteLine("ErrorCount: "+(_errorNum));
            Console.WriteLine("Count: " + (++_num) + "\n");
        }
    }
}
