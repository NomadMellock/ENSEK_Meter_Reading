using ENSEK_Meter_Reading.Enums;
using ENSEK_Meter_Reading.Models;

namespace ENSEK_Meter_Reading.Services.Interfaces
{
    /// <summary>
    /// Interface for File Service
    /// </summary>
    public interface IFileService
    {
        /// <summary>
        /// Process a meter reading uploaded file
        /// </summary>
        /// <param name="fileData"></param>
        /// <returns>Task</returns>
        public Task<MeterResults> ProcessMeterReadingsAsync(IFormFile fileData);
        
    }
}
