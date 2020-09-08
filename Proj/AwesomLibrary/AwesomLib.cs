using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Ninject;
using Ninject.Modules;
using System.Threading;


namespace AwesomeLibrary
{
    public interface IThreadManager
    { void StartInNewThread(Action act); }
    public class ThreadManager : IThreadManager
    {
        public void StartInNewThread(Action a)
        {

            Thread myThread = new Thread(new ThreadStart(a));
            myThread.Start();
            Console.WriteLine(myThread.ManagedThreadId);

        }
        public void StartInNewThread(Action<object> action, object obj)
        {
            Thread myThread = new Thread(new ParameterizedThreadStart(action));
            myThread.Start(obj);
            Console.WriteLine(myThread.ManagedThreadId);

        }
        public void StartInNewThreadDynamivInvoke(Action<object> action, object obj)
        {
            Thread myThread = new Thread(x => { if (x is Action<object> t) { t.DynamicInvoke(obj); } });
            myThread.Start(action);
            Console.WriteLine(myThread.ManagedThreadId);
        }


    }

    public interface ILogger
    {
        void Log(Exception r);
        void Log(string massage, Messages type);
    }

  public  static class ModuleLoader
    {
        static public IKernel Load(IKernel kernel, LoggerType type)
        {

            if (type == LoggerType.Console)
                kernel.Bind<ILogger>().To<ConsoleLogger>().InSingletonScope();
            else kernel.Bind<ILogger>().To<FileLogger>().InSingletonScope();

            kernel.Bind<IReflector>().To<Reflector>();
            kernel.Bind<IThreadManager>().To<ThreadManager>().InSingletonScope();
            return kernel;
        }
    }



    public static class LoggerFactory
    {
        public static ILogger GetLogge(LoggerType loggerType)
        {
            if (loggerType == LoggerType.File)
                return new FileLogger();
            else return new ConsoleLogger();
        }

    }
    class ConsoleLogger : Logger, ILogger
    {
        public void Log(string str, Messages t)
        {
            Console.WriteLine($"{WriteLog(t)} {str}");
            Console.ResetColor();
        }
        public void Log(Exception t)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{DateTime.Now} , ERROR : {t.Message}");
            Console.ResetColor();
        }
    }
    class FileLogger : Logger, ILogger
    {

        public string WritePath { get; set; } = "log.txt";
        public void Log(string str, Messages t)
        {
            string text = WriteLog(t) + ' ' + str;

            using (StreamWriter sw = new StreamWriter(WritePath, true, System.Text.Encoding.Default))
            {
                sw.WriteLine(text);
            }
        }
        public void Log(Exception t)
        {
            using (StreamWriter sw = new StreamWriter(WritePath, true, System.Text.Encoding.Default))
            {
                sw.WriteLine($"{DateTime.Now} , ERROR : {t.Message}");
            }
        }
    }
    public enum LoggerType
    {
        File,
        Console
    }
    public enum Messages
    {
        ERROR = 1,
        INFO,
        WARNING
    }



    abstract class Logger
    {
        protected static string WriteLog(Messages type)
        {
            string typeOfMessage = "";
            if (type == Messages.ERROR) { typeOfMessage = "ERROR"; Console.ForegroundColor = ConsoleColor.Red; };
            if (type == Messages.INFO) { typeOfMessage = "INFO"; Console.ForegroundColor = ConsoleColor.Green; };
            if (type == Messages.WARNING) { typeOfMessage = " WARNING"; Console.ForegroundColor = ConsoleColor.Yellow; };
            return $"{DateTime.Now} , {typeOfMessage} :";
        }
    }
    public struct Info
    {
        public bool IsPublicConstructor;
        public string TypeName;
        public string assemblyName;
        public List<string> publicFields;
        public List<string> publicProperties;
        public List<string> privateFields;
        public List<string> privateProperties;
        public List<string> publicMethods;
        public List<string> privateMethods;
    }



}

