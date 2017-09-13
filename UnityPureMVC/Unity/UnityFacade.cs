using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
// ReSharper disable ArrangeAccessorOwnerBody

namespace UnityPureMVC.Unity
{
    public class UnityFacade : Patterns.Facade
    {
        // ReSharper disable once InconsistentNaming
        public const string STARTUP = "UnityFacade.StartUp";

        static UnityFacade() { Instance = new UnityFacade(); }

        // Override 
#pragma warning disable 108,114
        public static UnityFacade GetInstance { get { return Instance as UnityFacade; } }
#pragma warning restore 108,114

        public virtual void Startup()
        {
            SendNotification(STARTUP);
        }

    }
}
