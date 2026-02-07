using System.Threading;

public static class CancellationUtils
{
    public static void CancelAndDispose(ref CancellationTokenSource cts)
    {
        if (cts == null)
            return;

        if (!cts.IsCancellationRequested)
            cts.Cancel();

        cts.Dispose();
        cts = null;
    }
}