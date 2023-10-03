using ENSEK_Meter_Reading.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ENSEK_Meter_Reading.Models
{
    /// <summary>
    /// Details of the incomming file
    /// </summary>
    [Table("FileDetails")]
    public class FileDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string FileName { get; set; }
        public byte[] FileData { get; set; }
        public FileTypes FileType { get; set; }
    }
}
