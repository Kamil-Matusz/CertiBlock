namespace CertiBlock.Shared.Messaging.Messages;

public abstract record BaseEvent(Guid Id, DateTime OccurredAt, Guid? CorrelationId, string Source, string SchemaVersion) : IMessage;