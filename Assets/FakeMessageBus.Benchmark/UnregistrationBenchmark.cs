using System;
using FakeMessageBus.Benchmark.Utilities;

namespace FakeMessageBus.Benchmark
{
    public class UnregistrationBenchmark : MonoProfiler
    {
        private class Message { }

        private class Observer
        {
            [ObserveMessage]
            public void On(Message message) {}
        }

        private MessageBus m_MessageBus;
        private Observer[] m_Observers;

        protected override int Order => 2;

        private void Start()
        {
            m_MessageBus = new MessageBus();
            m_Observers = new Observer[Iterations];

            Array.Fill(m_Observers, new Observer());
        }

        protected override void OnBeginSample()
        {
            Array.ForEach(m_Observers, observer => m_MessageBus.Register(observer));
        }

        protected override void Sample(int i)
        {
            m_MessageBus.Unregister(m_Observers[i]);
        }
    }
}
