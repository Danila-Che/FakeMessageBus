using UnityEngine;
using UnityEngine.Pool;

namespace FakeMessageBus
{
	public static class MessageBusUtilities
	{
		public static void RegisterSingle(GameObject gameObject, MessageBus messageBus)
		{
			if (gameObject.TryGetComponent<MonoBehaviour>(out var monoBehaviour))
			{
				messageBus.Register(monoBehaviour);
			}
		}

		public static void RegisterObject(GameObject gameObject, MessageBus messageBus)
		{
			using var pooledObject = ListPool<MonoBehaviour>.Get(out var monoBehaviours);
			gameObject.GetComponents(monoBehaviours);

			for (int i = 0; i < monoBehaviours.Count; i++)
			{
				var monoBehaviour = monoBehaviours[i];

				if (monoBehaviour != null)
				{
					messageBus.Register(monoBehaviour);
				}
			}
		}

		public static void RegisterRecursive(GameObject gameObject, MessageBus messageBus)
		{
			using var pooledObject = ListPool<MonoBehaviour>.Get(out var monoBehaviours);
			gameObject.GetComponentsInChildren(includeInactive: true, monoBehaviours);

			for (int i = 0; i < monoBehaviours.Count; i++)
			{
				var monoBehaviour = monoBehaviours[i];

				if (monoBehaviour != null)
				{
					messageBus.Register(monoBehaviour);
				}
			}
		}

		public static void UnregisterSingle(GameObject gameObject, MessageBus messageBus)
		{
			if (gameObject.TryGetComponent<MonoBehaviour>(out var monoBehaviour))
			{
				messageBus.Unregister(monoBehaviour);
			}
		}

		public static void UnregisterObject(GameObject gameObject, MessageBus messageBus)
		{
			using var pooledObject = ListPool<MonoBehaviour>.Get(out var monoBehaviours);
			gameObject.GetComponents(monoBehaviours);

			for (int i = 0; i < monoBehaviours.Count; i++)
			{
				var monoBehaviour = monoBehaviours[i];

				if (monoBehaviour != null)
				{
					messageBus.Unregister(monoBehaviour);
				}
			}
		}

		public static void UnregisterRecursive(GameObject gameObject, MessageBus messageBus)
		{
			using var pooledObject = ListPool<MonoBehaviour>.Get(out var monoBehaviours);
			gameObject.GetComponentsInChildren(includeInactive: true, monoBehaviours);

			for (int i = 0; i < monoBehaviours.Count; i++)
			{
				var monoBehaviour = monoBehaviours[i];

				if (monoBehaviour != null)
				{
					messageBus.Unregister(monoBehaviour);
				}
			}
		}
	}
}
