using FakeMessageBus.Benchmark.Utilities;
using UnityEngine;

namespace FakeMessageBus.Benchmark
{
    public class ObserverBindingsBenchmark : MonoProfiler
    {
        private class Message { }

        private class Observer
        {
            [ObserveMessage]
            public void On(Message message) {}
        }

        [Min(1)]
        [SerializeField] private int m_ObserverCount = 1;

        private ObserverBindings<Message> m_Bindings;
        private Message m_Message;

        protected override int Order => 4;

        private void Start()
        {
            m_Bindings = new ObserverBindings<Message>();
            m_Message = new Message();

            for (int i = 0; i < m_ObserverCount; i++)
            {
                var observer = new Observer();
                m_Bindings.Add(observer.On);
            }
        }

        protected override void OnBeginSample() { }

        protected override void Sample(int i)
        {
            m_Bindings.Invoke(m_Message);
        }
    }
}
