using System;
using FakeMessageBus.Benchmark.Utilities;
using UnityEngine;

namespace FakeEventBus.Benchmark
{
    public class CSharpEventBenchmark : MonoProfiler
    {
        private class Message { }
        
        private class Publisher
        {
            public event Action<Message> Event;

            public void Send(Message args)
            {
                Event.Invoke(args);
            }
        }

        private class Observer
        {
            public void On(Message message) { }
        }

        [Min(1)]
        [SerializeField] private int m_ObserverCount = 1;
        
        private Message m_Message;
        private Publisher m_Publisher;

        private void Start()
        {
            m_Publisher = new Publisher();
            m_Message = new Message();

            for (int i = 0; i < m_ObserverCount; i++)
            {
                var observer = new Observer();
                m_Publisher.Event += observer.On;
            }
        }
        
        protected override int Order => 3;
        
        protected override void OnBeginSample() { }

        protected override void Sample(int i)
        {
            m_Publisher.Send(m_Message);
        }
    }
}
