using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rehawk.Signals
{
    public static class Signals
    {
        public delegate void SignalListener();
        public delegate void SignalListener<in TSignal>(TSignal signal);
        
        private static readonly Dictionary<Type, List<Delegate>> listeners = new Dictionary<Type, List<Delegate>>();
        private static readonly Dictionary<Type, List<Delegate>> payloadListeners = new Dictionary<Type, List<Delegate>>();

        static Signals()
        {
            listeners.Clear();
            payloadListeners.Clear();
        }
        
        public static void Publish<T>()
        {
            Publish<T>(Activator.CreateInstance<T>());
        }
        
        public static void Publish<T>(T signal)
        {
            if (listeners.TryGetValue(typeof(T), out List<Delegate> resultListenersA))
            {
                foreach (Delegate listener in resultListenersA)
                {
                    ((SignalListener) listener).Invoke();
                }
            }
            
            if (payloadListeners.TryGetValue(typeof(T), out List<Delegate> resultListenersB))
            {
                foreach (Delegate listener in resultListenersB)
                {
                    ((SignalListener<T>) listener).Invoke(signal);
                }
            }
        }
        
        public static bool IsSubscribed<T>(SignalListener callback)
        {
            if (listeners.TryGetValue(typeof(T), out List<Delegate> resultListeners))
            {
                return resultListeners.Contains(callback);
            }

            return false;
        }
        
        public static bool IsSubscribed<T>(SignalListener<T> callback)
        {
            if (payloadListeners.TryGetValue(typeof(T), out List<Delegate> resultListeners))
            {
                return resultListeners.Contains(callback);
            }

            return false;
        }
        
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

        public static void Unsubscribe<T>(SignalListener callback)
        {
            if (listeners.TryGetValue(typeof(T), out List<Delegate> resultListeners))
            {
                resultListeners.Remove(callback);
            }
        }

        public static void Unsubscribe<T>(SignalListener<T> callback)
        {
            if (payloadListeners.TryGetValue(typeof(T), out List<Delegate> resultListeners))
            {
                resultListeners.Remove(callback);
            }
        }

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
