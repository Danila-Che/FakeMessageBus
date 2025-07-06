using System.Threading;

namespace FakeMessageBus
{
    public class ConcurrentMessageBus : IMessageBus
    {
        private readonly ReaderWriterLockSlim m_Lock = new();
        private readonly MessageBus m_MessageBus = new();
        
        public int GetActiveObserverCount<TMessage>()
        {
            m_Lock.EnterReadLock();
            try
            {
                return m_MessageBus.GetActiveObserverCount<TMessage>();
            }
            finally
            {
                m_Lock.ExitReadLock();
            }
        }

        public void Clear(bool includeCache = false)
        {
            m_Lock.EnterWriteLock();
            try
            {
                m_MessageBus.Clear();
            }
            finally
            {
                m_Lock.ExitWriteLock();
            }
        }

        public void Register(object observer)
        {
            m_Lock.EnterWriteLock();
            try
            {
                m_MessageBus.Register(observer);
            }
            finally
            {
                m_Lock.ExitWriteLock();
            }
        }

        public void Unregister(object observer)
        {
            m_Lock.EnterWriteLock();
            try
            {
                m_MessageBus.Unregister(observer);
            }
            finally
            {
                m_Lock.ExitWriteLock();
            }
        }

        public void Send<TMessage>(TMessage message)
        {
            m_Lock.EnterReadLock();
            try
            {
                m_MessageBus.Send(message);
            }
            finally
            {
                m_Lock.ExitReadLock();
            }
        }
    }
}