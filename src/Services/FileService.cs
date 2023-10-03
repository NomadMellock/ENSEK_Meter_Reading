using CsvHelper;
using ENSEK_Meter_Reading.Data;
using ENSEK_Meter_Reading.Entities;
using ENSEK_Meter_Reading.Enums;
using ENSEK_Meter_Reading.Models;
using ENSEK_Meter_Reading.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.VisualBasic.FileIO;
using System.Globalization;
using System.IO;
using System.Text;

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
        public async Task<MeterResults> PostFileAsync(IFormFile fileData, FileTypes fileType)
        {
            var returnMeterResults = new MeterResults();

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

                if(fileDetails.FileData.Length > 0)
                {
                    using var reader = new StreamReader(new MemoryStream(fileDetails.FileData));
                    using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

                    var records = csv.GetRecords<MeterReading>().ToArray();

                    foreach (var record in records)
                    {

                        var account = await MeterReadingContext.FindAsync<MeterReadings>(record.AccountId);
                        
                        if (account != null) 
                        {

                            // No need to check if account exists in accounts table as we can only add meter readings to accounts that do exist

                            if (DateTime.Parse(record.MeterReadingDateTime).CompareTo(account.MeterReadingDateTime) > 0)
                            {

                                account.MeterReadingDateTime = DateTime.Parse(record.MeterReadingDateTime);

                                if (Math.Abs(record.MeterReadValue) < account.MeterReadValue) {
                                    returnMeterResults.SuspicioudReadings++;
                                }
                                else 
                                {
                                    if (record.MeterReadValue.ToString().Length <= 5)
                                    {
                                        account.MeterReadValue = Math.Abs(record.MeterReadValue); // Assuming all meter readings are positive, any readings with a - or negative would be counted as a 0 or prefixed with 0

                                        MeterReadingContext.Entry(account).State = EntityState.Modified; // Prevents locking for the entry in ef core
                                        MeterReadingContext.MeterReadings.Update(account);
                                        await MeterReadingContext.SaveChangesAsync();
                                        
                                        returnMeterResults.UpdatedMeterReadings++;
                                    }
                                    else
                                    {
                                        returnMeterResults.InvalidMeterReadings++;
                                    }

                                    
                                }
                            }
                            else
                            {
                                returnMeterResults.Outdated++;
                            }
                        } 
                        else
                        {

                            //Check account Exists in accounts table

                            var meterReading = new MeterReadings()
                            {
                                AccountId = record.AccountId,
                                MeterReadingDateTime = DateTime.Parse(record.MeterReadingDateTime),
                                MeterReadValue = Math.Abs(record.MeterReadValue) // Assuming all meter readings are positive, any readings with a - or negative would be counted as a 0 or prefixed with 0
                            };


                            if (meterReading != null && meterReading.MeterReadValue.ToString().Length <= 5)
                            {
                                MeterReadingContext.MeterReadings.Add(meterReading);
                                returnMeterResults.NewMeterReadings++;
                            } else
                            {
                                returnMeterResults.InvalidMeterReadings++;
                            }
                        }
                        
                        returnMeterResults.TotalMeterReadings++;
                        await MeterReadingContext.SaveChangesAsync();
                    }
                }

                var result = MeterReadingContext.FileDetails.Add(fileDetails);
                await MeterReadingContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }

            // File must of been processed successfully so lets tell the consumer
            returnMeterResults.FileProcessed = true;

            return returnMeterResults;
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
