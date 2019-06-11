/*
 * This Solution is given "as is" without any warranties
 * and licensed under the Code Project Open License (CPOL) 1.02
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Interfaces
{
    /// <summary>
    /// Interface used by the users to implement the dynamic loaded functions.
    /// </summary>
    public interface IUserInterface
    {
        String GetName();
        int Funktion(int a, int b);
    }
}
