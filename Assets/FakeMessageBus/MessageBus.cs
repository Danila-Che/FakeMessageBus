using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace FakeMessageBus
{
	public class MessageBus : IMessageBus
	{
		private struct CallbackCache
		{
			public Type MessageArgType;
			public MethodInfo MethodInfo;
			public Type DelegateType;
		}

		private const BindingFlags k_Scope =
			BindingFlags.NonPublic |
			BindingFlags.Public |
			BindingFlags.Instance;
		
		private readonly Dictionary<Type, IObserverBindings> m_ObserverBindings = new(); // Type is message arg type
		private readonly Dictionary<Type, List<CallbackCache>> m_ObserverCallbackCache = new(); // Type is observer type

		public int GetActiveObserverCount<TMessage>()
		{
			if (m_ObserverBindings.TryGetValue(typeof(TMessage), out var bindings))
			{
				return bindings.CallbackCount;
			}

			return 0;
		}

		public void Clear(bool includeCache = false)
		{
			m_ObserverBindings.Clear();

			if (includeCache)
			{
				m_ObserverCallbackCache.Clear();
			}
		}

		public void Register(object observer)
		{
			CacheIfNecessary(observer);
			AddObserver(observer);
		}

		public void Unregister(object observer)
		{
			foreach (var binding in m_ObserverBindings)
			{
				binding.Value.Remove(observer);
			}
		}
		
		public void Send<TMessage>(TMessage message)
		{
			if (m_ObserverBindings.TryGetValue(typeof(TMessage), out var bindings))
			{
				((ObserverBindings<TMessage>)bindings).Invoke(message);
			}
		}

		public async Task SendAsync<TMessage>(TMessage message)
		{
			if (m_ObserverBindings.TryGetValue(typeof(TMessage), out var bindings))
			{
				await ((ObserverBindings<TMessage>)bindings).InvokeAsync(message);
			}
		}

		private void CacheIfNecessary(object observer)
		{
			if (!m_ObserverCallbackCache.ContainsKey(observer.GetType()))
			{
				var cache = new List<CallbackCache>();

				Cache(observer.GetType(), cache);
				m_ObserverCallbackCache[observer.GetType()] = cache;
			}
		}

		private static void Cache(Type type, List<CallbackCache> messageArgTypes)
		{
			while (type != null)
			{
				foreach (var methodInfo in type.GetMethods(k_Scope))
				{
					if (TryGetMessageArgType(methodInfo, out Type messageArgType))
					{
						messageArgTypes.Add(new CallbackCache
						{
							MessageArgType = messageArgType,
							MethodInfo = methodInfo,
							DelegateType = typeof(Action<>).MakeGenericType(messageArgType)
						});
					}
				}

				type = type.BaseType;
			}
		}

		private void AddObserver(object observer)
		{
			if (!m_ObserverCallbackCache.TryGetValue(observer.GetType(), out var callbacks))
			{
				return;
			}
			
			foreach (var messageCache in callbacks)
			{
				if (!m_ObserverBindings.TryGetValue(messageCache.MessageArgType, out var bindings))
				{
					bindings = CreateBinding(messageCache.MessageArgType);
					m_ObserverBindings[messageCache.MessageArgType] = bindings;
				}

				if (!bindings.Contains(messageCache.MethodInfo, observer))
				{
					var callback = CreateDelegate(messageCache, observer);
					bindings.Add(callback);
				}
			}
		}

		private static bool TryGetMessageArgType(MethodInfo methodInfo, out Type messageArgType)
		{
			if (HasValidAttribute(methodInfo))
			{
				if (HasValidParameter(methodInfo, out messageArgType))
				{
					return true;
				}

				throw new InvalidCallbackException($"The callback {methodInfo.Name} of {methodInfo.DeclaringType.Name} must contain only one parameter");
			}

			messageArgType = null;
			return false;
		}

		private static bool HasValidAttribute(MethodInfo methodInfo)
		{
			return methodInfo.IsDefined(typeof(ObserveMessageAttribute), inherit: false);
		}

		private static bool HasValidParameter(MethodInfo methodInfo, out Type messageArgType)
		{
			var parameters = methodInfo.GetParameters();

			if (parameters.Length == 1)
			{
				messageArgType = parameters[0].ParameterType;
				return true;
			}

			messageArgType = null;
			return false;
		}

		private static IObserverBindings CreateBinding(Type callbackType)
		{
			var constructed = typeof(ObserverBindings<>).MakeGenericType(callbackType);
			return (IObserverBindings)Activator.CreateInstance(constructed);
		}
		
		private static Delegate CreateDelegate(CallbackCache cache, object target)
		{
			return cache.MethodInfo.CreateDelegate(cache.DelegateType, target);
		}
	}
}
