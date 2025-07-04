using System;
using System.Reflection;
using System.Threading.Tasks;

namespace FakeMessageBus
{
	public interface IObserverBindings
	{
		public int CallbackCount { get; }

		public void Add(Delegate callback);

		public void Remove(object observer);

		public bool Contains(MethodInfo methodInfo, object observer);
	}
    
    public class ObserverBindings<TMessage> : IObserverBindings
    {
        private Action<TMessage>[] m_Callbacks = new Action<TMessage>[4];
        private int m_Count;
        
        public int CallbackCount => m_Count;

        public void Add(Delegate callback)
        {
            Add((Action<TMessage>)callback);
        }

        public void Add(Action<TMessage> callback)
        {
            if (m_Count == m_Callbacks.Length)
            {
                Array.Resize(ref m_Callbacks, m_Callbacks.Length * 2);
            }

            m_Callbacks[m_Count] = callback;
            m_Count++;
        }

        public void Remove(object observer)
        {
            for (int i = 0; i < m_Count; i++)
            {
                if (m_Callbacks[i].Target == observer)
                {
                    m_Count--;
                    m_Callbacks[i] = m_Callbacks[m_Count];
                    m_Callbacks[m_Count] = null;
                    break;
                }
            }
        }

        public void Remove(Action<TMessage> callback)
        {
            for (int i = 0; i < m_Count; i++)
            {
                if (m_Callbacks[i] == callback)
                {
                    m_Count--;
                    m_Callbacks[i] = m_Callbacks[m_Count];
                    m_Callbacks[m_Count] = null;
                    break;
                }
            }
        }

        public bool Contains(MethodInfo methodInfo, object observer)
        {
            for (int i = 0; i < m_Count; i++)
            {
                if (m_Callbacks[i].Method == methodInfo && ReferenceEquals(m_Callbacks[i].Target, observer))
                {
                    return true;
                }
            }

            return false;
        }

        public void Invoke(TMessage message)
        {
            var callbacks = m_Callbacks;
            int count = m_Count;

            for (int i = 0; i < count; i++)
            {
                callbacks[i].Invoke(message);
            }
        }

        public async Task InvokeAsync(TMessage message)
        {
            for (int i = 0; i < m_Count; i++)
            {
                m_Callbacks[i].Invoke(message);

                await Task.Yield();
            }
        }
    }
}