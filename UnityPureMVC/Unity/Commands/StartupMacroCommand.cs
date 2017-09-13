using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityPureMVC.Interfaces;
using UnityPureMVC.Patterns;

namespace UnityPureMVC.Unity.Commands
{
    public class StartupMacroCommand : MacroCommand
    {
        public override void Execute(INotification notification)
        {
            base.Execute(notification);
        }
    }
}
