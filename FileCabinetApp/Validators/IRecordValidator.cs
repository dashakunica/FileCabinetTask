namespace FileCabinetApp
{
    /// <summary>
    /// Record validator interface.
    /// </summary>
    public interface IRecordValidator
    {
        /// <summary>
        /// Validate parameters.
        /// </summary>
        /// <param name="data">Validate data.</param>
        void ValidateParameters(ValidateParametersData data);
    }
}
