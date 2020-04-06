namespace FileCabinetApp
{
    public abstract class ServiceCommandHandlerBase : CommandHandlerBase
    {
        protected ServiceCommandHandlerBase(IFileCabinetService fileCabinetService)
        {
            this.Service = fileCabinetService;
        }

        protected IFileCabinetService Service { get; private set; }

        public override void Handle(AppCommandRequest commandRequest)
        {
            base.Handle(commandRequest);
        }
    }
}
