using System;
using System.Reflection;

namespace HWdTech.Common
{
    public class Singleton<T>
    {
        static public T Instance
        {
            get
            {
                if (null == instance)
                {
                    lock (lockObject)
                    {
                        if (null == instance)
                        {
                            try
                            {
                                typeof (T).InvokeMember("Init", BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod, null, null, null);
                            }
                            catch (Exception)
                            {
                                throw new Exception(
                                    String.Format(
                                        "Type {0} should have public static method Init to initialize an instance of the singleton object.", 
                                        typeof(T).ToString()
                                ));
                            }

                        }
                    }
                }
                return instance;
            }
            set
            {
                instance = value;
            }
        }
        static private T instance;
        private static object lockObject = new object();
    }
}
