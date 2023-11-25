namespace AiAssaultArena.Tanks.Common;

public static class TankRunner
{
    public static async Task RunAsync<TTank>(string url) where TTank : BaseTank, new()
    {
        var tank = new TTank();
        tank.Setup(url);
        await tank.StartAsync();

        using var cancellationTokenSource = new CancellationTokenSource();
        Console.CancelKeyPress += (sender, e) =>
        {
            e.Cancel = true;
            cancellationTokenSource.Cancel();
        };

        try
        {
            while (!cancellationTokenSource.Token.IsCancellationRequested)
            {
                await Task.Delay(1000);
            }
        }
        catch (TaskCanceledException)
        {
            Console.WriteLine("Terminating gracefully...");

        }

        Console.WriteLine("Application has stopped.");
    }
}