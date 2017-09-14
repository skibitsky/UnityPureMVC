using UnityPureMVC.Interfaces;
using UnityPureMVC.Patterns;

namespace UnityPureMVC
{
    public class StartupMacroCommand : MacroCommand
    {
        /// <summary>
        /// Initialize the <c>MacroCommand</c>.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         In your subclass, override this method to 
        ///         initialize the <c>MacroCommand</c>'s <i>SubCommand</i>  
        ///         list with <c>ICommand</c> class references like
        ///         this:
        ///     </para>
        ///     <example>
        ///         <code>
        ///             override void InitializeMacroCommand() 
        ///             {
        ///                 AddSubCommand(() => new com.me.myapp.controller.FirstCommand());
        ///                 AddSubCommand(() => new com.me.myapp.controller.SecondCommand());
        ///                 AddSubCommand(() => new com.me.myapp.controller.ThirdCommand());
        ///             }
        ///         </code>
        ///     </example>
        ///     <para>
        ///         Note that <i>SubCommand</i>s may be any <c>ICommand</c> implementor,
        ///         <c>MacroCommand</c>s or <c>SimpleCommands</c> are both acceptable.
        ///     </para>
        /// </remarks>
        protected override void InitializeMacroCommand()
        {
            base.InitializeMacroCommand();
        }

        public override void Execute(INotification notification)
        {
            base.Execute(notification);
        }
    }
}
