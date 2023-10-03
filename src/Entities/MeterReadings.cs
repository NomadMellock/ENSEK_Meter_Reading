using CsvHelper.Configuration.Attributes;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ENSEK_Meter_Reading.Entities
{
    /// <summary>
    /// DB Entity Meter Reading
    /// </summary>
    public class MeterReadings
    {
        /// <summary>
        /// Account Id
        /// </summary>
        [Key]
        [Name("AccountId")]
        public int AccountId { get; set; }

        /// <summary>
        /// Meter Date
        /// </summary>
        [Name("MeterReadingDateTime")]
        public DateTime MeterReadingDateTime { get; set; }

        /// <summary>
        /// Meter Value
        /// </summary>
        [Name("MeterReadValue")]
        public int MeterReadValue { get; set; }
    }
}