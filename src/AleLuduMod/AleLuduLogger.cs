using Reactor.Utilities;

namespace AleLuduMod;

public static class AleLuduLogger
{
    public static void Debug(string message)
    {
        Logger<AleLuduModPlugin>.Debug(message);
    }

    public static void Info(string message)
    {
        Logger<AleLuduModPlugin>.Info(message);
    }

    public static void Warn(string message)
    {
        Logger<AleLuduModPlugin>.Warning(message);
    }

    public static void Error(string message)
    {
        Logger<AleLuduModPlugin>.Error(message);
    }

    public static void Fatal(string message)
    {
        Logger<AleLuduModPlugin>.Fatal(message);
    }
}