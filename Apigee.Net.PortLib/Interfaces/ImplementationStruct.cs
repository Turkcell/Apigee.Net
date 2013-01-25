using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apigee.Net.PortLib
{
    /// <summary>
    /// Apigee.Net Portable Library contains Interfaces which must be implemeneted per Plaftorm (.Net4.5, Win8, WP7, WP8).
    /// you must supply all required implementations for those interfaces - in order to use the Library on your platform. 
    /// </summary>
    public class ImplementationStruct
    {
        public IHttpTools iHttpTools;
     //   public IJsonTools iJsonTools;e
    }
}
