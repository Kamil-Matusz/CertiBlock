namespace CertiBlock.Shared.Observability;

public static class MessagingActivitySources
{
    public const string MessagingPublishSourceName = "CertiBlock.RabbitMQ.Publish";
    public const string MessagingConsumeSourceName = "CertiBlock.RabbitMQ.Consume";
    public const string DefaultSourceName = "CertiBlock";
}