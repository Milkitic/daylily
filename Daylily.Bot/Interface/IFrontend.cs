namespace Daylily.Bot.Interface
{
    public interface IFrontend : IMiddleware
    {
        event MessageEventHandler MessageReceived;
        event ExceptionEventHandler ErrorOccured;

        bool RawObject_Received(object rawObject);
    }
}
