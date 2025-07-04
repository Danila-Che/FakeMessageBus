namespace FakeMessageBus
{
    public interface IMessageBus
    {
        int GetActiveObserverCount<TMessage>();

		void Clear(bool includeCache = false);

		void Register(object observer);
        
        void Unregister(object observer);

        void Send<TMessage>(TMessage message);
    }
}
