namespace ExternalMessaging
{
    public class IDataDynamic
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
        public DateTime? StartTimeUtc { get; set; }

        /// <summary>
        /// The completion time.
        /// </summary>
        public DateTime? CompletionTimeUtc { get; set; }
    }
}
