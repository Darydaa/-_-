using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Ninject;
using Ninject.Modules;
using System.Threading;

namespace AwesomeLibrary
{
    public class Reflector : IReflector
    {
        public object Create(Type t) 
        {
            return Activator.CreateInstance(t);
        }
        public void CopyCode(MethodInfo[] me, object[] list,string name)
        {
            try
            {
                ParameterInfo[] pa = me.FirstOrDefault(x => x.Name == name).GetParameters();
                if (pa.Length != list.Length)
                    throw new Exception("Несовпадение количества параметров");
                for (int i = 0; i < pa.Length; i++)
                {
                    if (pa[i].GetType() != list[i].GetType())
                    {
                        throw new Exception("Такого метода c переданными параметрами");
                    }
                }
            }
            catch (Exception e) { Console.WriteLine(e.Message); }


        }
         public void Invoke(object obj,string name,object[] list)
        {
            Type t = obj.GetType();
            MethodInfo[] me = t.GetMethods();
            CopyCode(me,list,name);
            me.FirstOrDefault(x=>x.Name==name)?.Invoke(obj,list);
            

        }
        public void Invoke(Type t,string name, object[] list) 
        {
            object ob = Create(t);
            MethodInfo[] me = t.GetMethods();
            CopyCode(me, list, name);
            me.FirstOrDefault(x => x.Name == name)?.Invoke(ob, list);
         
        }


        public void Invoke<T>(T obj, BindingFlags flags, string name, object[] list)
        {
            Type t = obj.GetType();
            MethodInfo[] me = t.GetMethods();
            CopyCode(me, list, name);
            me.FirstOrDefault(x => x.Name == name)?.Invoke(obj, flags, null, list, null);

                
        }
        public void Invoke(Type t, string name, object[] list, BindingFlags flags) { 

            object ob = Create(t);
            MethodInfo[] me = t.GetMethods();
            CopyCode(me, list, name);
            me.FirstOrDefault(x => x.Name == name)?.Invoke(ob, flags, null, list, null);
        }






        public ILogger logger;
        Info info;
        //public Reflector(ILogger log)
        //{
        //    logger = log;
        //}
      public Info Analyze(object obj)
        {
            Type t = obj.GetType();
            info.TypeName = t.ToString();
            ConstructorInfo[] ci = t.GetConstructors();
            info.IsPublicConstructor = ci.Length >= 1;
            info.assemblyName = t.Module.Assembly.ToString();
            FieldInfo[] fi = t.GetFields(
            BindingFlags.NonPublic | BindingFlags.Instance);
            info.privateFields = new List<string>();
            for (int i = 0; i < fi.Length; i++)
            {
                info.privateFields.Add(fi[i].Name);
            }
            FieldInfo[] fi1 = t.GetFields(
            );
            info.publicFields = new List<string>();
            for (int i = 0; i < fi1.Length; i++)
            {
                info.publicFields.Add(fi1[i].Name);
            }
            PropertyInfo[] pr = t.GetProperties();
            info.publicProperties = new List<string>();
            for (int i = 0; i < pr.Length; i++)
            {
                info.publicProperties.Add(pr[i].Name);
            }
            PropertyInfo[] pr1 = t.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance);
            info.privateProperties = new List<string>();
            for (int i = 0; i < pr1.Length; i++)
            {
                info.privateProperties.Add(pr1[i].Name);
            }
            MethodInfo[] me = t.GetMethods();
            info.publicMethods = new List<string>();
            for (int i = 0; i < me.Length; i++)
            {
                info.publicMethods.Add(me[i].Name);
            }
            MethodInfo[] me1 =

            t.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);
            info.privateMethods = new List<string>();
            for (int i = 0; i < me1.Length; i++)
            {
                info.privateMethods.Add(me1[i].Name);
            }
            return info;

        }
        public void Save()
        {
            using (StreamWriter sw = new StreamWriter(@"clasInfo.json", true, System.Text.Encoding.Default))
            {
                sw.WriteLine(JsonConvert.SerializeObject(info));
            }
        }



    }
}