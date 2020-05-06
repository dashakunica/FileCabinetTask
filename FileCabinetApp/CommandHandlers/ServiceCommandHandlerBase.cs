namespace FileCabinetApp
{
    /// <summary>
    /// Base service command handler.
    /// </summary>
    public abstract class ServiceCommandHandlerBase : CommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceCommandHandlerBase"/> class.
        /// </summary>
        /// <param name="fileCabinetService">Service.</param>
        protected ServiceCommandHandlerBase(IFileCabinetService fileCabinetService)
        {
            this.Service = fileCabinetService;
        }

        /// <summary>
        /// Gets service.
        /// </summary>
        /// <value>
        /// Service.
        /// </value>
        protected IFileCabinetService Service { get; private set; }

        /// <summary>
        /// Handler.
        /// </summary>
        /// <param name="commandRequest">Command request.</param>
        public override void Handle(AppCommandRequest commandRequest)
        {
            base.Handle(commandRequest);
        }
    }
}
