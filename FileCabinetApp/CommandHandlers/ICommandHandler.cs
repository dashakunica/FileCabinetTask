namespace FileCabinetApp
{
    /// <summary>
    /// Command handler interface.
    /// </summary>
    public interface ICommandHandler
    {
        /// <summary>
        /// Sets next handle.
        /// </summary>
        /// <param name="commandHandler">Command handler.</param>
        /// <returns>Next command handle.</returns>
        ICommandHandler SetNext(ICommandHandler commandHandler);

        /// <summary>
        /// Handle request.
        /// </summary>
        /// <param name="commandRequest">Source command request.</param>
        void Handle(AppCommandRequest commandRequest);
    }
}
