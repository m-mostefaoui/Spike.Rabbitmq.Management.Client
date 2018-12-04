namespace Console
{
    using PowerArgs;

    public class CommandLineArgs
    {
        [ArgRequired(PromptIfMissing = true)]
        public string QueueName { get; set; }

        [ArgRequired(PromptIfMissing = true)]
        public string Token { get; set; }
    }
}