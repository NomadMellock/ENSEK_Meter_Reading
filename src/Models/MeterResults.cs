namespace ENSEK_Meter_Reading.Models
{
    /// <summary>
    /// Return results based on current uploaded file
    /// </summary>
    public class MeterResults
    {
        /// <summary>
        /// File has been fully processed for all meter readings
        /// </summary>
        public bool FileProcessed { get; set; }

        /// <summary>
        /// Total number of meter readings
        /// </summary>
        public int TotalMeterReadings { get; set; }

        /// <summary>
        /// Number of new meter readings added
        /// </summary>
        public int NewMeterReadings { get; set; }

        /// <summary>
        /// Number of updated meter readings
        /// </summary>
        public int UpdatedMeterReadings { get; set; }

        /// <summary>
        /// Number of invlaid meter readings
        /// </summary>
        public int InvalidMeterReadings { get; set; }

        /// <summary>
        /// Number of suspicious readings
        /// </summary>
        public int SuspicioudReadings { get; set; }

        /// <summary>
        /// Entries that are already existing, however, date is less than the one on file
        /// </summary>
        public int Outdated { get; set; }
        
    }
}
