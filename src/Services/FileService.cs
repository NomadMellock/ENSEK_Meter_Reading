using ENSEK_Meter_Reading.Data;
using ENSEK_Meter_Reading.Enums;
using ENSEK_Meter_Reading.Models;
using ENSEK_Meter_Reading.Services.Interfaces;
using Microsoft.VisualBasic.FileIO;

namespace ENSEK_Meter_Reading.Services
{
    /// <summary>
    /// Service to retrieve a file
    /// </summary>
    public class FileService : IFileService
    {
        private readonly MeterReadingContext MeterReadingContext;
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="MeterReadingContext"></param>
        public FileService(MeterReadingContext MeterReadingContext)
        {
            this.MeterReadingContext = MeterReadingContext;
        }

        /// <summary>
        /// Post a Single File
        /// </summary>
        /// <param name="fileData"></param>
        /// <param name="fileType"></param>
        /// <returns></returns>
        public async Task PostFileAsync(IFormFile fileData, FileTypes fileType)
        {
            try
            {
                var fileDetails = new FileDetails()
                {
                    id = 0,
                    FileName = fileData.FileName,
                    FileType = fileType,
                };
                using (var stream = new MemoryStream())
                {
                    fileData.CopyTo(stream);
                    fileDetails.FileData = stream.ToArray();
                }
                var result = MeterReadingContext.FileDetails.Add(fileDetails);
                await MeterReadingContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Post Multiple Files
        /// </summary>
        /// <param name="fileData"></param>
        /// <returns></returns>
        public async Task PostMultiFileAsync(List<FileUpload> fileData)
        {
            try
            {
                foreach (FileUpload file in fileData)
                {
                    var fileDetails = new FileDetails()
                    {
                        id = 0,
                        FileName = file.FileDetails.FileName,
                        FileType = file.FileType,
                    };
                    using (var stream = new MemoryStream())
                    {
                        file.FileDetails.CopyTo(stream);
                        fileDetails.FileData = stream.ToArray();
                    }
                    var result = MeterReadingContext.FileDetails.Add(fileDetails);
                }
                await MeterReadingContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        /// <summary>
        /// Utility to Copy Stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="downloadPath"></param>
        /// <returns>Task</returns>
        /// <TODO>Move to a Utility Class</TODO>
        public async Task CopyStream(Stream stream, string downloadPath)
        {
            using (var fileStream = new FileStream(downloadPath, FileMode.Create, FileAccess.Write))
            {
                await stream.CopyToAsync(fileStream);
            }
        }
    }
}
