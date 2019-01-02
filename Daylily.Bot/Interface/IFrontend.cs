namespace Daylily.Bot.Interface
{
    public interface IFrontend : IConfigurable
    {
        event MessageEventHandler MessageReceived;
        event ExceptionEventHandler ErrorOccured;

        void RawObject_Received(object rawObject);
    }
}
