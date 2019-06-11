/*
 * This Solution is given "as is" without any warranties
 * and licensed under the Code Project Open License (CPOL) 1.02
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Interfaces;

namespace LoadFromDll
{
    /// <summary>
    /// Base class of the framework.
    /// Loading DLL from UserLibryry project by reflection.
    /// Checking if implement IUserInterface of Interfaces project.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Dictonary holding all Types imlementing the IUserInterface.
        /// Key is the Classname.
        /// Value is the Type.
        /// </summary>
        private static Dictionary<String, Type> nameTypeDict = new Dictionary<string, Type>();

        static void Main(string[] args)
        {
            
            //Load the assambly
            Assembly ass = Assembly.LoadFrom(@"..\..\..\UserLibrary\bin\Debug\UserLibrary.dll");
            foreach (Type t in ass.GetExportedTypes())
            {
                //Get all classes implement IUserInterface
                if (t.GetInterface("IUserInterface", true)!=null)
                {
                    Console.WriteLine("Found Type: {0}", t.Name);
                    nameTypeDict.Add(t.Name, t);//Add to Dictonary                    
                }
            }
            // Do something with the classes
            int a = 5;
            int b = 1;
            ExecuteByName("UserClass1", a, b);
            ExecuteByName("UserClass2", a, b);
            Console.ReadLine();
        }

        /// <summary>
        /// Create a instance of a type by its name and call its functions.
        /// </summary>
        /// <param name="typeName">Name of the type to load</param>
        /// <param name="a">value a for function call</param>
        /// <param name="b">value b for function call</param>
        private static void ExecuteByName(String typeName, int a, int b)
        {
            //Create Instance
            IUserInterface o = (IUserInterface)nameTypeDict[typeName].InvokeMember(null, BindingFlags.DeclaredOnly |
                        BindingFlags.Public | BindingFlags.NonPublic |
                        BindingFlags.Instance | BindingFlags.CreateInstance, null, null, null);
            //Print information and call function
            Console.WriteLine(o.GetName());
            Console.WriteLine(o.Funktion(a, b));
        }
    }
}
