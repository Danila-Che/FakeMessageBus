using System;

namespace FakeMessageBus
{
	[AttributeUsage(AttributeTargets.Method)]
	public class ObserveMessageAttribute : Attribute { }
}
