using System;

namespace FileCabinetApp
{
    /// <summary>
    /// Application command request.
    /// </summary>
    public class AppCommandRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppCommandRequest"/> class.
        /// Application command request.
        /// </summary>
        /// <param name="command">Command input.</param>
        /// <param name="parameters">Parameters input.</param>
        public AppCommandRequest(string command, string parameters)
        {
            this.Command = command ?? throw new ArgumentNullException(nameof(command));
            this.Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
        }

        /// <summary>
        /// Gets command value.
        /// </summary>
        /// <value>
        /// Command value.
        /// </value>
        public string Command { get; }

        /// <summary>
        /// Gets parameters.
        /// </summary>
        /// <value>
        /// Parameters.
        /// </value>
        public string Parameters { get; }
    }
}
