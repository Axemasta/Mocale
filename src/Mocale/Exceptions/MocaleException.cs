namespace Mocale.Exceptions;

internal class MocaleException : Exception
{
    public MocaleException(string message)
        : base(message)
    {
    }

    public MocaleException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
