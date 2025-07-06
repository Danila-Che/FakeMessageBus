using FakeMessageBus.Benchmark.Utilities;
using UnityEngine;

namespace FakeMessageBus.Benchmark
{
    public class NotificationBenchmark : MonoProfiler
    {
        private class Message { }

        private class Observer
        {
            [ObserveMessage]
            public void On(Message message) {}
        }

        [Min(1)]
        [SerializeField] private int m_ObserverCount = 1;
        [SerializeField] private bool m_Concurrent;

        private IMessageBus m_MessageBus;
        private Message m_Message;

        protected override int Order => 1;

        private void Start()
        {
            if (m_Concurrent)
            {
                m_MessageBus = new ConcurrentMessageBus();
            }
            else
            {
                m_MessageBus = new MessageBus();
            }
            m_Message = new Message();

            for (int i = 0; i < m_ObserverCount; i++)
            {
                var observer = new Observer();
                m_MessageBus.Register(observer);
            }
        }

        protected override void OnBeginSample() { }

        protected override void Sample(int i)
        {
            m_MessageBus.Send(m_Message);
        }
    }
}
