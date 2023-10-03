using CsvHelper.Configuration.Attributes;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;

namespace ENSEK_Meter_Reading.Models
{
    /// <summary>
    /// Meter Reading
    /// </summary>
    public class MeterReading
    {
        /// <summary>
        /// Account Id
        /// </summary>
        [Name("AccountId")]
        public int AccountId { get; set; }

        /// <summary>
        /// Meter Date
        /// </summary>
        [Name("MeterReadingDateTime")]
        public string MeterReadingDateTime { get; set; }

        /// <summary>
        /// Meter Value
        /// </summary>
        [Name("MeterReadValue")]
        public int MeterReadValue { get; set; }
    }
}
