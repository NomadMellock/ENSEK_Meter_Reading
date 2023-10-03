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
        /// Task to retrieve a single file
        /// </summary>
        /// <param name="fileData"></param>
        /// <param name="fileType"></param>
        /// <returns>Task</returns>
        public Task<MeterResults> PostFileAsync(IFormFile fileData, FileTypes fileType);

        /// <summary>
        /// Task to retrieve multiple files
        /// </summary>
        /// <param name="fileData"></param>
        /// <returns></returns>
        public Task PostMultiFileAsync(List<FileUpload> fileData);
    }
}
