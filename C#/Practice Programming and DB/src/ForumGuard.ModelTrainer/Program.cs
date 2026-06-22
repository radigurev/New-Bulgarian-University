using ForumGuard.ModelTrainer.Cli;

using CancellationTokenSource cancellationTokenSource = new();
Console.CancelKeyPress += (_, eventArgs) =>
{
    eventArgs.Cancel = true;
    cancellationTokenSource.Cancel();
};

TrainerRunner runner = new(Console.Out, Console.Error);
return runner.Run(args, cancellationTokenSource.Token);
