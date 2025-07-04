using NUnit.Framework;

namespace FakeMessageBus.EditModeTests
{
    [TestFixture]
    public class ExtraTests
    {
        private class Message { }
        
        private class ObserverStub
        {
            public int CallbackCount;

            [ObserveMessage]
            private void Callback(Message message)
            {
                CallbackCount++;
            }
        }

        [Test]
        public void Test_TryRegisterObserverSeveralTimes()
        {
            var messageBus = new MessageBus();
            var observer = new ObserverStub();

            Assert.That(messageBus.GetActiveObserverCount<Message>(), Is.EqualTo(0));

            messageBus.Register(observer);
            messageBus.Register(observer);

            Assert.That(messageBus.GetActiveObserverCount<Message>(), Is.EqualTo(1));
        }

        [Test]
        public void Test_TryUnregisterObserverSeveralTimes()
        {
            var messageBus = new MessageBus();
            var observer = new ObserverStub();

            Assert.That(messageBus.GetActiveObserverCount<Message>(), Is.EqualTo(0));

            messageBus.Register(observer);

            Assert.That(messageBus.GetActiveObserverCount<Message>(), Is.EqualTo(1));

            messageBus.Unregister(observer);
            messageBus.Unregister(observer);

            Assert.That(messageBus.GetActiveObserverCount<Message>(), Is.EqualTo(0));
        }

        [Test]
        public void Test_TrySendMessageSeveralTimes()
        {
            var messageBus = new MessageBus();
            var observer = new ObserverStub();
            messageBus.Register(observer);

            Assert.That(observer.CallbackCount, Is.EqualTo(0));

            messageBus.Send(new Message());
            messageBus.Send(new Message());

            Assert.That(observer.CallbackCount, Is.EqualTo(2));
        }
    }
}
