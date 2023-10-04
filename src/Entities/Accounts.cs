using CsvHelper.Configuration.Attributes;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ENSEK_Meter_Reading.Entities
{
    /// <summary>
    /// DB Entity Meter Reading
    /// </summary>
    public class Accounts
    {
        /// <summary>
        /// Account Id
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Name("AccountId")]
        public int AccountId { get; set; }

        /// <summary>
        /// First Name
        /// </summary>
        [Name("FirstName")]
        public string? FirstName { get; set; }

        /// <summary>
        /// Last Name
        /// </summary>
        [Name("LastName")]
        public string? LastName { get; set; }
    }
}
