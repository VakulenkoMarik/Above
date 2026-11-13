using System;
using System.Collections.Generic;
using Extensions;

namespace DependencyControl
{
    public static class Dependency
    {
	    private static readonly Dictionary<Type, object> Instances = new Dictionary<Type, object>();
	    private static readonly Dictionary<Type, List<Action<object>>> PendingSubscribers = new Dictionary<Type, List<Action<object>>>();

		public static T Get<T>() where T : class
		{
			var type = typeof(T);

			if (Instances.TryGetValue(type, out var instance))
			{
				return instance as T;
			}

			return null;
		}

		public static void SubscribeOnAvailable<T>(Action<T> callback) where T : class
		{
			var type = typeof(T);

			if (Instances.TryGetValue(type, out var instance))
			{
				callback?.Invoke(instance as T);
				return;
			}

			if (!PendingSubscribers.TryGetValue(type, out var list))
			{
				list = new List<Action<object>>();
				PendingSubscribers[type] = list;
			}

			list.Add(obj => callback(obj as T));
		}

		public static void UnsubscribeOnAvailable<T>(Action<T> callback) where T : class
		{
			var type = typeof(T);

			if (PendingSubscribers.TryGetValue(type, out var callbacks))
			{
				callbacks.Remove(callback as Action<object>);

				if (callbacks.Count == 0)
				{
					PendingSubscribers.Remove(type);
				}
			}
		}

		public static void Register<T>(T instance) where T : class
		{
			Register(typeof(T), instance);
		}

		public static void Register(Type type, object instance)
		{
			if (instance == null)
			{
				throw new Exception($"Dependency Instance cannot be null.");
			}

			if (!Instances.TryAdd(type, instance))
			{
				throw new Exception($"{type.Name} is already registered!");
			}

			if (PendingSubscribers.TryGetValue(type, out var subscribers))
			{
				foreach (var action in subscribers)
				{
					action(instance);
				}

				PendingSubscribers.Remove(type);
			}
		}

		public static void UnRegister<T>(T instance) where T : class
		{
			UnRegister(typeof(T), instance);
		}

		public static void UnRegister(Type type, object instance)
		{
			if (!Instances.ContainsKey(type))
			{
				throw new Exception($"{type.Name} was not registered!");
			}

			Instances.Remove(type);
		}
	}
}

