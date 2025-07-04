using NUnit.Framework;

namespace FakeMessageBus.EditModeTests
{
    [TestFixture]
    public class SeveralCallbackTests
    {
        private class FirstMessage { }

        private class SecondsMessage { }

        private class ObserverWithIdenticalCallbacks
        {
            public int FirstCallbackCount;
            public int SecondCallbackCount;

            [ObserveMessage]
            public void FirstCallback(FirstMessage message)
            {
                FirstCallbackCount++;
            }
            
            [ObserveMessage]
            public void SecondCallback(FirstMessage message)
            {
                SecondCallbackCount++;
            }
        }

        private class ObserverWithDifferentCallbacks
        {
            public int FirstCallbackCount;
            public int SecondCallbackCount;

            [ObserveMessage]
            public void FirstCallback(FirstMessage message)
            {
                FirstCallbackCount++;
            }

            [ObserveMessage]
            public void SecondCallback(SecondsMessage message)
            {
                SecondCallbackCount++;
            }
        }

        private class ObserverWithValidAndOneInvalidCallback
        {
            [ObserveMessage]
            public void FirstCallback(FirstMessage validMessage) { }

            [ObserveMessage]
            public void SecondCallback(int arg0, int arg1) { }
        }

        [Test]
        public void Test_NotifyObserverWithIdenticalCallbacks()
        {
            var messageBus = new MessageBus();
            var observer = new ObserverWithIdenticalCallbacks();
            messageBus.Register(observer);

            Assert.That(observer.FirstCallbackCount, Is.EqualTo(0));
            Assert.That(observer.SecondCallbackCount, Is.EqualTo(0));

            messageBus.Send(new FirstMessage());

            Assert.That(observer.FirstCallbackCount, Is.EqualTo(1));
            Assert.That(observer.SecondCallbackCount, Is.EqualTo(1));
        }

        [Test]
        public void Test_NotifyObserverWithDifferentCallbacks()
        {
            var messageBus = new MessageBus();
            var observer = new ObserverWithDifferentCallbacks();
            messageBus.Register(observer);

            Assert.That(observer.FirstCallbackCount, Is.EqualTo(0));
            Assert.That(observer.SecondCallbackCount, Is.EqualTo(0));

            messageBus.Send(new FirstMessage());

            Assert.That(observer.FirstCallbackCount, Is.EqualTo(1));
            Assert.That(observer.SecondCallbackCount, Is.EqualTo(0));

            messageBus.Send(new SecondsMessage());

            Assert.That(observer.FirstCallbackCount, Is.EqualTo(1));
            Assert.That(observer.SecondCallbackCount, Is.EqualTo(1));
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
            var observer = new ObserverWithValidAndOneInvalidCallback();

            Assert.Throws<InvalidCallbackException>(() => messageBus.Register(observer));
            Assert.That(messageBus.GetActiveObserverCount<FirstMessage>(), Is.EqualTo(0));
            Assert.That(messageBus.GetActiveObserverCount<SecondsMessage>(), Is.EqualTo(0));
        }
    }
}
