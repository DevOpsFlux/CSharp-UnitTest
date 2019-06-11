/*
 * This Solution is given "as is" without any warranties
 * and licensed under the Code Project Open License (CPOL) 1.02
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UserLibrary
{
    /// <summary>
    /// First class implementing IUserInterface
    /// </summary>
    public class UserClass1 : Interfaces.IUserInterface
    {
        public String GetName() { return "Add"; }
        public int Funktion(int a, int b) { return a + b; }

    }

    /// <summary>
    /// Second class implementing IUserInterface
    /// </summary>
    public class UserClass2 : Interfaces.IUserInterface
    {
        public String GetName() { return "Sub"; }
        public int Funktion(int a, int b) { return a - b; }

    }

    /// <summary>
    /// Dummy class, not used!
    /// </summary>
    public class UserClass3 
    {
        public String GetName() { return "Sub"; }
        public int Funktion(int a, int b) { return a - b; }

    }
}
