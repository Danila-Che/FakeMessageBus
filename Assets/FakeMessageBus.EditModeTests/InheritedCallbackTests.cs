using NUnit.Framework;

namespace FakeMessageBus.EditModeTests
{
    [TestFixture]
    public class InheritedCallbackTests
    {
        private class Message { }

        public class BaseObserver
        {
            public int BaseCallbackCount;

            [ObserveMessage]
            private void Callback(Message message)
            {
                BaseCallbackCount++;
            }
        }

        public class DerivedObserver : BaseObserver
        {
            public int DerivedCallbackCount;

            [ObserveMessage]
            private void Callback(Message message)
            {
                DerivedCallbackCount++;
            }
        }

        [Test]
        public void Test_NotifyObserverWithInheretedCallbacks()
        {
            var messageBus = new MessageBus();
            var observer = new DerivedObserver();
            messageBus.Register(observer);

            Assert.That(observer.BaseCallbackCount, Is.EqualTo(0));
            Assert.That(observer.DerivedCallbackCount, Is.EqualTo(0));

            messageBus.Send(new Message());

            Assert.That(observer.BaseCallbackCount, Is.EqualTo(1));
            Assert.That(observer.DerivedCallbackCount, Is.EqualTo(1));
        }

    }
}
