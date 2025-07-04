using System;
using FakeMessageBus.Benchmark.Utilities;

namespace FakeMessageBus.Benchmark
{
    public class RegistrationBenchmark : MonoProfiler
    {
        private class Message { }

        private class Observer
        {
            [ObserveMessage]
            public void On(Message message) {}
        }


        private MessageBus m_MessageBus;
        private Observer[] m_Observers;

        protected override int Order => 0;

        private void Start()
        {
            m_MessageBus = new MessageBus();
            m_Observers = new Observer[Iterations];

            Array.Fill(m_Observers, new Observer());
        }

        protected override void OnBeginSample()
        {
            m_MessageBus.Clear();
        }

        protected override void Sample(int i)
        {
            m_MessageBus.Register(m_Observers[i]);
        }
    }
}
