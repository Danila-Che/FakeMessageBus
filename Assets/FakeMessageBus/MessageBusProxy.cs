using System;
using UnityEngine;

namespace FakeMessageBus
{
	public sealed class MessageBusProxy
	{
		private static readonly Lazy<MessageBus> m_Instance = new(() => new MessageBus());

		public static IMessageBus Bus => m_Instance.Value;

		public static int GetActiveObserverCount<T>()
		{
			return m_Instance.Value.GetActiveObserverCount<T>();
		}

		public static void Clear()
		{
			m_Instance.Value.Clear();
		}

		public static void Register(object observer)
		{
			m_Instance.Value.Register(observer);

		}

		public static void Unregister(object observer)
		{
			m_Instance.Value.Unregister(observer);
		}

		public static void Send<TMessage>(TMessage message)
		{
			m_Instance.Value.Send(message);
		}

		public static void RegisterSingle(GameObject gameObject)
		{
			MessageBusUtilities.RegisterSingle(gameObject, m_Instance.Value);
		}

		public static void RegisterObject(GameObject gameObject)
		{
			MessageBusUtilities.RegisterObject(gameObject, m_Instance.Value);
		}

		public static void RegisterRecursive(GameObject gameObject)
		{
			MessageBusUtilities.RegisterRecursive(gameObject, m_Instance.Value);
		}

		public static void UnregisterSingle(GameObject gameObject)
		{
			MessageBusUtilities.UnregisterSingle(gameObject, m_Instance.Value);
		}

		public static void UnregisterObject(GameObject gameObject)
		{
			MessageBusUtilities.UnregisterObject(gameObject, m_Instance.Value);
		}

		public static void UnregisterRecursive(GameObject gameObject)
		{
			MessageBusUtilities.UnregisterRecursive(gameObject, m_Instance.Value);
		}
	}
}
