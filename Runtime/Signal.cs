using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rehawk.Signals
{
    /// <summary>
    /// Provides a static implementation of a signal/pub-sub system, where events of different types
    /// can be published or subscribed to, with or without attached payloads.
    /// </summary>
    public static class Signal
    {
        public delegate void SignalListener();
        public delegate void SignalListener<in TSignal>(TSignal signal);
        
        private static readonly Dictionary<Type, List<Delegate>> listeners = new Dictionary<Type, List<Delegate>>();
        private static readonly Dictionary<Type, List<Delegate>> payloadListeners = new Dictionary<Type, List<Delegate>>();

        static Signal()
        {
            listeners.Clear();
            payloadListeners.Clear();
        }

        /// <summary>
        /// Publishes a signal of the specified type.
        /// This method triggers all listeners subscribed to the signal type without requiring additional payload data.
        /// </summary>
        /// <typeparam name="T">The type of the signal to be published.</typeparam>
        public static void Publish<T>()
        {
            Publish(Activator.CreateInstance<T>());
        }

        /// <summary>
        /// Publishes a signal of the specified type with the provided payload.
        /// This method triggers all listeners subscribed to the signal type,
        /// passing the provided signal instance as payload data.
        /// </summary>
        /// <typeparam name="T">The type of the signal to be published.</typeparam>
        /// <param name="signal">The signal instance containing the payload data to be sent to listeners.</param>
        public static void Publish<T>(T signal)
        {
            if (listeners.TryGetValue(typeof(T), out List<Delegate> resultListenersA))
            {
                foreach (Delegate listener in resultListenersA.ToArray())
                {
                    ((SignalListener) listener).Invoke();
                }
            }
            
            if (payloadListeners.TryGetValue(typeof(T), out List<Delegate> resultListenersB))
            {
                foreach (Delegate listener in resultListenersB.ToArray())
                {
                    ((SignalListener<T>) listener).Invoke(signal);
                }
            }
        }

        /// <summary>
        /// Determines whether a specific listener is subscribed to signals of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the signal to check for subscriptions.</typeparam>
        /// <param name="callback">The callback method representing the listener to check.</param>
        /// <returns>True if the specified listener is subscribed to the signal type; otherwise, false.</returns>
        public static bool IsSubscribed<T>(SignalListener callback)
        {
            if (listeners.TryGetValue(typeof(T), out List<Delegate> resultListeners))
            {
                return resultListeners.Contains(callback);
            }

            return false;
        }

        /// <summary>
        /// Checks if the specified callback is subscribed to signals of the given type.
        /// </summary>
        /// <typeparam name="T">The type of the signal to check for subscription.</typeparam>
        /// <param name="callback">The callback instance to verify for subscription.</param>
        /// <returns>True if the callback is subscribed to the signal type; otherwise, false.</returns>
        public static bool IsSubscribed<T>(SignalListener<T> callback)
        {
            if (payloadListeners.TryGetValue(typeof(T), out List<Delegate> resultListeners))
            {
                return resultListeners.Contains(callback);
            }

            return false;
        }

        /// <summary>
        /// Subscribes a listener to signals of the specified type.
        /// This method adds the provided listener to the list of listeners for the given signal type.
        /// </summary>
        /// <typeparam name="T">The type of the signal to subscribe to.</typeparam>
        /// <param name="callback">The listener to be invoked when the specified signal is published.</param>
        public static void Subscribe<T>(SignalListener callback)
        {
            if (!listeners.ContainsKey(typeof(T)))
            {
                listeners.Add(typeof(T), new List<Delegate>
                {
                    callback
                });   
            }
            else
            {
                listeners[typeof(T)].Add(callback);
            }
        }

        /// <summary>
        /// Subscribes a listener to signals of the specified type with payload data.
        /// The subscribed listener will be invoked with the corresponding payload whenever the signal is published.
        /// </summary>
        /// <typeparam name="T">The type of the signal to subscribe to.</typeparam>
        /// <param name="callback">The listener callback to invoke when the signal of the specified type is published.</param>
        public static void Subscribe<T>(SignalListener<T> callback)
        {
            if (!payloadListeners.ContainsKey(typeof(T)))
            {
                payloadListeners.Add(typeof(T), new List<Delegate>
                {
                    callback
                });   
            }
            else
            {
                payloadListeners[typeof(T)].Add(callback);
            }
        }

        /// <summary>
        /// Unsubscribes a specified listener from a signal of the given type.
        /// This method removes a listener previously subscribed to a signal type with no payload.
        /// </summary>
        /// <typeparam name="T">The type of the signal from which the listener will be unsubscribed.</typeparam>
        /// <param name="callback">The listener's callback method to be removed.</param>
        public static void Unsubscribe<T>(SignalListener callback)
        {
            if (listeners.TryGetValue(typeof(T), out List<Delegate> resultListeners))
            {
                resultListeners.Remove(callback);
            }
        }

        /// <summary>
        /// Unsubscribes a callback from listening to signals of the specified type.
        /// Removes the provided callback from the list of listeners for the given signal type.
        /// </summary>
        /// <typeparam name="T">The type of the signal to unsubscribe from.</typeparam>
        /// <param name="callback">The callback to unsubscribe from the signal.</param>
        public static void Unsubscribe<T>(SignalListener<T> callback)
        {
            if (payloadListeners.TryGetValue(typeof(T), out List<Delegate> resultListeners))
            {
                resultListeners.Remove(callback);
            }
        }

        /// <summary>
        /// Unsubscribes all listeners from a specific signal type.
        /// This method removes all delegates attached to the specified signal type,
        /// including both standard and payload-based listeners.
        /// </summary>
        /// <typeparam name="T">The type of the signal from which all listeners will be unsubscribed.</typeparam>
        public static void UnsubscribeAll<T>()
        {
            if (listeners.ContainsKey(typeof(T)))
            {
                listeners.Remove(typeof(T));
            }
    
            if (payloadListeners.ContainsKey(typeof(T)))
            {
                listeners.Remove(typeof(T));
            }
        }

        /// <summary>
        /// Unsubscribes all listeners from all signal types.
        /// This method clears all registered listeners across all signal types,
        /// including listeners registered with and without payload data.
        /// </summary>
        public static void UnsubscribeAll()
        {
            listeners.Clear();
            payloadListeners.Clear();
        }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void OnSubsystemRegistration()
        {
            listeners.Clear();
            payloadListeners.Clear();
        }
    }
}
