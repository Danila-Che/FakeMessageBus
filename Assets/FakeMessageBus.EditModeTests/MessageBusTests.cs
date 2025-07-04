using NUnit.Framework;

namespace FakeMessageBus.EditModeTests
{
    [TestFixture]
    public class MessageBusTests
    {
        private class Message { }

        private class ObserverWithoutCallback
        {
            public void NotCallback() { }
        }

        private class OnePublicCallbackStub
        {
            [ObserveMessage]
            public void Callback(Message message) { }
        }

        private class OnePrivateCallbackStub
        {
            [ObserveMessage]
            private void Callback(Message message) { }
        }

        private class OneInvalidCallbackStub
        {
            [ObserveMessage]
            private void Callback(int arg0, int arg1) { }
        }

        private class OneCallbackStub
        {
            public int CallbackCount = 0;

            [ObserveMessage]
            private void Callback(Message message)
            {
                CallbackCount++;
            }
        }

        private class Observer<T>
        {
            public T Value;

            [ObserveMessage]
            private void On(T arg)
            {
                Value = arg;
            }
        }
        
        /// <summary>
        /// Given an message bus without a observer
        /// When a observer without a callback tries to register on the message bus
        /// Then the message bus still has no observer
        /// </summary>
        [Test]
        public void Test_TryRegisterObserverWithoutCallback()
        {
            var messageBus = new MessageBus();
            var observer = new ObserverWithoutCallback();
            messageBus.Register(observer);

            Assert.That(messageBus.GetActiveObserverCount<Message>(), Is.EqualTo(0));
        }

        /// <summary>
        /// Given an message bus without a observer
        /// When a invalid callback observer tries to register on the message bus
        /// Then the message bus throws an exeption
        /// And the message bus still has no observer
        /// </summary>
        [Test]
        public void Test_TryRegisterObserverWithInvalidCallback()
        {
            var messageBus = new MessageBus();
            var observer = new OneInvalidCallbackStub();

            Assert.Throws<InvalidCallbackException>(() => messageBus.Register(observer));
            Assert.That(messageBus.GetActiveObserverCount<Message>(), Is.EqualTo(0));
        }

        /// <summary>
        /// Given an message bus without a observer
        /// When a public callback observer tries to register on the message bus
        /// Then the message bus has one observer
        /// </summary>
        [Test]
        public void Test_RegisterObserverWithOnePublicCallback()
        {
            var messageBus = new MessageBus();
            var observer = new OnePublicCallbackStub();
            messageBus.Register(observer);

            Assert.That(messageBus.GetActiveObserverCount<Message>(), Is.EqualTo(1));
        }

        /// <summary>
        /// Given an message bus without a observer
        /// When a private callback observer tries to register on the message bus
        /// Then the message bus has one observer
        /// </summary>
        [Test]
        public void Test_RegisterObserverWithOnePrivateCallback()
        {
            var messageBus = new MessageBus();
            var observer = new OnePrivateCallbackStub();
            messageBus.Register(observer);

            Assert.That(messageBus.GetActiveObserverCount<Message>(), Is.EqualTo(1));
        }

        /// <summary>
        /// Given an message bus with one observer
        /// When this observer tries to unregister from the message bus
        /// Then the message bus has no observer
        /// </summary>
        [Test]
        public void Test_UnregisterObserverWithOneCallback()
        {
            var messageBus = new MessageBus();
            var observer = new OneCallbackStub();
            messageBus.Register(observer);

            Assert.That(messageBus.GetActiveObserverCount<Message>(), Is.EqualTo(1));

            messageBus.Unregister(observer);
            
            Assert.That(messageBus.GetActiveObserverCount<Message>(), Is.EqualTo(0));
        }

        /// <summary>
        /// Given an message bus with one observer
        /// When the message bus raises an message
        /// Then the observer will perform a callback 
        /// </summary>
        [Test]
        public void Test_NotifyRegisteredObservers()
        {
            var messageBus = new MessageBus();
            var observer = new OneCallbackStub();
            messageBus.Register(observer);
            messageBus.Send(new Message());

            Assert.That(observer.CallbackCount, Is.EqualTo(1));
        }

        [Test]
        public void Test_AnyTypeOfMessage()
        {
            var messageBus = new MessageBus();
            var observer0 = new Observer<int>();
            var observer1 = new Observer<string>();
            
            messageBus.Register(observer0);
            messageBus.Register(observer1);
            
            observer0.Value = 0;
            observer1.Value = string.Empty;
            
            messageBus.Send(42);
            Assert.That(observer0.Value, Is.EqualTo(42));
            Assert.That(observer1.Value, Is.EqualTo(string.Empty));

            observer0.Value = 0;
            observer1.Value = string.Empty;
            
            messageBus.Send("42");
            Assert.That(observer0.Value, Is.EqualTo(0));
            Assert.That(observer1.Value, Is.EqualTo("42"));
        }
    }
}
