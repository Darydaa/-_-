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
  public  interface IReflector
    {
        void Save();
        Info Analyze(object obj);
    }
}
