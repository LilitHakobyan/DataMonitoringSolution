namespace ExternalMessageHandling.Models
{
    /// <summary>
    /// The Data model.
    /// </summary>
    public class DataDynamicModel
    {
        /// <summary>
        /// The Data status.
        /// </summary>
        public string? Status { get; set; }

        /// <summary>
        /// The Data id.
        /// </summary>
        public int DataId { get; set; }

        /// <summary>
        /// The start time.
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// The completion time.
        /// </summary>
        public DateTime? CompletionTime { get; set; }
    }
}
