using NUnit.Framework;
using UnityEngine;

namespace FakeMessageBus.EditModeTests
{
    [TestFixture]
    public class AutoRegistrationTests
    {
        private class Message { }

        private class Observer : MonoBehaviour
        {
            public int CallbackCount;

            [ObserveMessage]
            private void On(Message message)
            {
                CallbackCount++;
            }
        }

        [Test]
        public void Test_MessageBusDecorator_SingleRegistration()
        {
            var observerGameObject = new GameObject();
            _ = observerGameObject.AddComponent<Observer>();

            MessageBusProxy.Clear();

            Assert.That(MessageBusProxy.GetActiveObserverCount<Message>(), Is.EqualTo(0));

            MessageBusProxy.RegisterSingle(observerGameObject);

            Assert.That(MessageBusProxy.GetActiveObserverCount<Message>(), Is.EqualTo(1));
        }

        [Test]
        public void Test_MessageBusDecorator_ObjectRegistration()
        {
            var observerGameObject = new GameObject();

            _ = observerGameObject.AddComponent<Observer>();
            _ = observerGameObject.AddComponent<Observer>();

            MessageBusProxy.Clear();

            Assert.That(MessageBusProxy.GetActiveObserverCount<Message>(), Is.EqualTo(0));

            MessageBusProxy.RegisterObject(observerGameObject);

            Assert.That(MessageBusProxy.GetActiveObserverCount<Message>(), Is.EqualTo(2));
        }

        [Test]
        public void Test_MessageBusDecorator_RecursiveRegistration()
        {
            var observerGameObject = new GameObject();
            var nestedGameObject = new GameObject();

            nestedGameObject.transform.parent = observerGameObject.transform;
            
            _ = observerGameObject.AddComponent<Observer>();
            _ = nestedGameObject.AddComponent<Observer>();

            MessageBusProxy.Clear();

            Assert.That(MessageBusProxy.GetActiveObserverCount<Message>(), Is.EqualTo(0));

            MessageBusProxy.RegisterRecursive(observerGameObject);

            Assert.That(MessageBusProxy.GetActiveObserverCount<Message>(), Is.EqualTo(2));
        }

        [Test]
        public void Test_MessageBusDecorator_SingleUnregistration()
        {
            var observerGameObject = new GameObject();
            _ = observerGameObject.AddComponent<Observer>();

            MessageBusProxy.Clear();
            MessageBusProxy.RegisterRecursive(observerGameObject);

            Assert.That(MessageBusProxy.GetActiveObserverCount<Message>(), Is.EqualTo(1));

            MessageBusProxy.UnregisterSingle(observerGameObject);

            Assert.That(MessageBusProxy.GetActiveObserverCount<Message>(), Is.EqualTo(0));
        }

        [Test]
        public void Test_MessageBusDecorator_ObjectUnregistration()
        {
            var observerGameObject = new GameObject();

            _ = observerGameObject.AddComponent<Observer>();
            _ = observerGameObject.AddComponent<Observer>();

            MessageBusProxy.Clear();
            MessageBusProxy.RegisterObject(observerGameObject);

            Assert.That(MessageBusProxy.GetActiveObserverCount<Message>(), Is.EqualTo(2));

            MessageBusProxy.UnregisterObject(observerGameObject);

            Assert.That(MessageBusProxy.GetActiveObserverCount<Message>(), Is.EqualTo(0));
        }

        [Test]
        public void Test_MessageBusDecorator_RecursiveUnregistration()
        {
            var observerGameObject = new GameObject();
            var nestedGameObject = new GameObject();

            nestedGameObject.transform.parent = observerGameObject.transform;
            
            _ = observerGameObject.AddComponent<Observer>();
            _ = nestedGameObject.AddComponent<Observer>();

            MessageBusProxy.Clear();
            MessageBusProxy.RegisterRecursive(observerGameObject);

            Assert.That(MessageBusProxy.GetActiveObserverCount<Message>(), Is.EqualTo(2));

            MessageBusProxy.UnregisterRecursive(observerGameObject);

            Assert.That(MessageBusProxy.GetActiveObserverCount<Message>(), Is.EqualTo(0));
        }
    }
}
