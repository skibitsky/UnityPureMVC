﻿//
//  UnityPureMVC C# Multicore
//
//  Copyright(c) 2017 Saad Shams <saad.shams@UnityPureMVC.org>
//  Your reuse is governed by the Creative Commons Attribution 3.0 License
//

using System;
using System.Collections.Generic;
using UnityPureMVC.Interfaces;
using UnityPureMVC.Patterns.Observer;

namespace UnityPureMVC.Core
{
    /// <summary>
    /// A Multiton <c>IView</c> implementation.
    /// </summary>
    /// <remarks>
    ///     <para>In UnityPureMVC, the <c>View</c> class assumes these responsibilities:</para>
    ///     <list type="bullet">
    ///         <item>Maintain a cache of <c>IMediator</c> instances</item>
    ///         <item>Provide methods for registering, retrieving, and removing <c>IMediators</c></item>
    ///         <item>Managing the observer lists for each <c>INotification</c> in the application</item>
    ///         <item>Providing a method for attaching <c>IObservers</c> to an <c>INotification</c>'s observer list</item>
    ///         <item>Providing a method for broadcasting an <c>INotification</c></item>
    ///         <item>Notifying the <c>IObservers</c> of a given <c>INotification</c> when it broadcast</item>
    ///     </list>
    /// </remarks>
    /// <seealso cref="UnityPureMVC.Patterns.Mediator.Mediator"/>
    /// <seealso cref="UnityPureMVC.Patterns.Observer.Observer"/>
    /// <seealso cref="UnityPureMVC.Patterns.Observer.Notification"/>
    public class View: IView
    {
        public static IView GetInstance
        {
            get
            {
                if (View.Instance != null) return View.Instance;
                return View.Instance ?? (View.Instance = (IView) new View());
            }
        }

        /// <summary>
        /// Constructs and initializes a new view
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         This <c>IView</c> implementation is a Multiton, 
        ///         so you should not call the constructor directly
        ///         Factory method <c>View.GetInstance</c>
        ///     </para>
        /// </remarks>
        public View()
        {
            MediatorMap = new Dictionary<string, IMediator>();
            ObserverMap = new Dictionary<string, IList<IObserver>>();
            InitializeView();
        }

        /// <summary>
        /// Initialize the Multiton View instance.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Called automatically by the constructor, this
        ///         is your opportunity to initialize the Multiton
        ///         instance in your subclass without overriding the
        ///         constructor.
        ///     </para>
        /// </remarks>
        protected virtual void InitializeView()
        {
        }

        /// <summary>
        ///     Register an <c>IObserver</c> to be notified
        ///     of <c>INotifications</c> with a given name.
        /// </summary>
        /// <param name="notificationName">the name of the <c>INotifications</c> to notify this <c>IObserver</c> of</param>
        /// <param name="observer">the <c>IObserver</c> to register</param>
        public virtual void RegisterObserver(string notificationName, IObserver observer)
        {
            if (!ObserverMap.ContainsKey(notificationName))
                ObserverMap.Add(notificationName, new List<IObserver>());
            ObserverMap[notificationName].Add(observer);
        }

        /// <summary>
        /// Notify the <c>IObservers</c> for a particular <c>INotification</c>.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         All previously attached <c>IObservers</c> for this <c>INotification</c>'s
        ///         list are notified and are passed a reference to the <c>INotification</c> in
        ///         the order in which they were registered.
        ///     </para>
        /// </remarks>
        /// <param name="notification"></param>
        public virtual void NotifyObservers(INotification notification)
        {
            // Get a reference to the observers list for this notification name
            if (!ObserverMap.TryGetValue(notification.Name, out IList<IObserver> observersRef)) return;
            // Copy observers from reference array to working array, 
            // since the reference array may change during the notification loop
            var observers = new List<IObserver>(observersRef);
            foreach (var observer in observers)
            {
                observer.NotifyObserver(notification);
            }
        }

        /// <summary>
        /// Remove the observer for a given notifyContext from an observer list for a given Notification name.
        /// </summary>
        /// <param name="notificationName">which observer list to remove from </param>
        /// <param name="notifyContext">remove the observer with this object as its notifyContext</param>
        public virtual void RemoveObserver(string notificationName, object notifyContext)
        {
            if (!ObserverMap.TryGetValue(notificationName, out IList<IObserver> observers)) return;
            for (var i = 0; i < observers.Count; i++)
            {
                if (!observers[i].CompareNotifyContext(notifyContext)) continue;
                observers.RemoveAt(i);
                break;
            }

            // Also, when a Notification's Observer list length falls to
            // zero, delete the notification key from the observer map
            if (observers.Count == 0 && ObserverMap.ContainsKey(notificationName))
                ObserverMap.Remove(notificationName);
        }

        /// <summary>
        /// Register an <c>IMediator</c> instance with the <c>View</c>.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Registers the <c>IMediator</c> so that it can be retrieved by name,
        ///         and further interrogates the <c>IMediator</c> for its 
        ///         <c>INotification</c> interests.
        ///     </para>
        ///     <para>
        ///         If the <c>IMediator</c> returns any <c>INotification</c>
        ///         names to be notified about, an <c>Observer</c> is created encapsulating 
        ///         the <c>IMediator</c> instance's <c>handleNotification</c> method 
        ///         and registering it as an <c>Observer</c> for all <c>INotifications</c> the
        ///         <c>IMediator</c> is interested in.
        ///     </para>
        /// </remarks>
        /// <param name="mediator">the name to associate with this <c>IMediator</c> instance</param>
        public virtual void RegisterMediator(IMediator mediator)
        {
            if (MediatorMap.ContainsKey(mediator.MediatorName)) return;

            MediatorMap[mediator.MediatorName] = mediator;
            var interests = mediator.ListNotificationInterests();
            if (interests.Length > 0)
            {
                var observer = new Observer(mediator.HandleNotification, mediator);
                foreach (var i in interests)
                {
                    RegisterObserver(i, observer);
                }
            }
            // alert the mediator that it has been registered
            mediator.OnRegister();
        }

        /// <summary>
        /// Retrieve an <c>IMediator</c> from the <c>View</c>.
        /// </summary>
        /// <param name="mediatorName">the name of the <c>IMediator</c> instance to retrieve.</param>
        /// <returns>the <c>IMediator</c> instance previously registered with the given <c>mediatorName</c>.</returns>
        public virtual IMediator RetrieveMediator(string mediatorName)
        {
            return MediatorMap.TryGetValue(mediatorName, out IMediator mediator) ? mediator : null;
        }

        /// <summary>
        /// Remove an <c>IMediator</c> from the <c>View</c>.
        /// </summary>
        /// <param name="mediatorName">name of the <c>IMediator</c> instance to be removed.</param>
        /// <returns>the <c>IMediator</c> that was removed from the <c>View</c></returns>
        public virtual IMediator RemoveMediator(string mediatorName)
        {
            if (!MediatorMap.ContainsKey(mediatorName)) return null;
            var mediator = MediatorMap[mediatorName];
            var interests = mediator.ListNotificationInterests();
            foreach (var i in interests)
            {
                RemoveObserver(i, mediator);
            }
            mediator.OnRemove();
            return mediator;
        }

        /// <summary>
        /// Check if a Mediator is registered or not
        /// </summary>
        /// <param name="mediatorName"></param>
        /// <returns>whether a Mediator is registered with the given <c>mediatorName</c>.</returns>
        public virtual bool HasMediator(string mediatorName)
        {
            return MediatorMap.ContainsKey(mediatorName);
        }   

        /// <summary> Singleton /// </summary>
        protected static IView Instance;

        /// <summary>Mapping of Mediator names to Mediator instances</summary>
        protected IDictionary<string, IMediator> MediatorMap;

        /// <summary>Mapping of Notification names to Observer lists</summary>
        protected IDictionary<string, IList<IObserver>> ObserverMap;

        /// <summary>Message Constants</summary>
        protected const string MULTITON_MSG = "View instance for this Multiton key already constructed!";
    }
}
