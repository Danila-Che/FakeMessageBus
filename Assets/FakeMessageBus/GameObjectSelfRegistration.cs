using System;
using UnityEngine;

namespace FakeMessageBus
{
    public enum RegistrationStrategy
    {
        Single,
        Object,
        Recursive
    }

    public class GameObjectSelfRegistration : MonoBehaviour
    {
        [SerializeField] private RegistrationStrategy m_RegistrationStrategy = RegistrationStrategy.Recursive;

        private bool m_WasRegistered;

        private void OnEnable()
        {
            if (m_WasRegistered) { return; }

            Register(m_RegistrationStrategy);

            m_WasRegistered = true;
        }

        private void OnDisable()
        {
			Unregister(m_RegistrationStrategy);

			m_WasRegistered = false;
        }

        protected virtual void Register(RegistrationStrategy registrationStrategy)
        {
			switch (registrationStrategy)
			{
				case RegistrationStrategy.Single:
					MessageBusProxy.RegisterSingle(gameObject);
					break;
				case RegistrationStrategy.Object:
					MessageBusProxy.RegisterObject(gameObject);
					break;
				case RegistrationStrategy.Recursive:
					MessageBusProxy.RegisterRecursive(gameObject);
					break;
				default:
					throw new ArgumentOutOfRangeException(registrationStrategy.ToString());
			}
		}

        protected virtual void Unregister(RegistrationStrategy registrationStrategy)
        {
			switch (registrationStrategy)
			{
				case RegistrationStrategy.Single:
					MessageBusProxy.UnregisterSingle(gameObject);
					break;
				case RegistrationStrategy.Object:
					MessageBusProxy.UnregisterObject(gameObject);
					break;
				case RegistrationStrategy.Recursive:
					MessageBusProxy.UnregisterRecursive(gameObject);
					break;
				default:
					throw new ArgumentOutOfRangeException(registrationStrategy.ToString());
			}
		}
    }
}
