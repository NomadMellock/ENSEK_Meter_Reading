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
        private readonly ILogger<FileService> logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="MeterReadingContext"></param>
        public FileService(MeterReadingContext MeterReadingContext, ILogger<FileService> logger)
        {
            this.MeterReadingContext = MeterReadingContext;
            this.logger = logger;
        }

        /// <summary>
        /// Post a Single File
        /// </summary>
        /// <param name="fileData"></param>
        /// <returns></returns>
        public async Task<MeterResults> ProcessMeterReadingsAsync(IFormFile fileData)
        {
            var returnMeterResults = new MeterResults();
            logger.LogInformation($"Processing File: {fileData.FileName}");
            try
            {
                
                var fileDetails = new FileDetails()
                {
                    id = 0,
                    FileName = fileData.FileName,
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

                        var meterAvailable = await MeterReadingContext.FindAsync<MeterReadings>(record.AccountId);
                        
                        if (meterAvailable != null) 
                        {

                            logger.LogInformation($"[{record.AccountId}] Found existing meter reading");

                            // No need to check if account exists in accounts table as we can only add meter readings to accounts that do exist

                            if (DateTime.Parse(record.MeterReadingDateTime).CompareTo(meterAvailable.MeterReadingDateTime) >= 0)
                            {

                                meterAvailable.MeterReadingDateTime = DateTime.Parse(record.MeterReadingDateTime);

                                if (Math.Abs(record.MeterReadValue) < meterAvailable.MeterReadValue) {
                                    logger.LogInformation($"[{record.AccountId}] - Current meter reading is less than current meter reading");
                                    returnMeterResults.SuspiciousReadings++;
                                }
                                else 
                                {
                                    if (record.MeterReadValue.ToString().Length <= 5)
                                    {
                                        meterAvailable.MeterReadValue = Math.Abs(record.MeterReadValue); // Assuming all meter readings are positive, any readings with a - or negative would be counted as a 0 or prefixed with 0

                                        MeterReadingContext.Entry(meterAvailable).State = EntityState.Modified; // Prevents locking for the entry in ef core
                                        MeterReadingContext.MeterReadings.Update(meterAvailable);
                                        await MeterReadingContext.SaveChangesAsync();


                                        logger.LogInformation($"[{record.AccountId}] - Updated successfully");
                                        returnMeterResults.UpdatedMeterReadings++;
                                    }
                                    else
                                    {
                                        logger.LogInformation($"[{record.AccountId}] - Invalid meter reading value of : {record.MeterReadValue} should be in format: NNNNN");
                                        returnMeterResults.InvalidMeterReadings++;
                                    }
                                }
                            }
                            else
                            {
                                logger.LogInformation($"[{record.AccountId}] - Date {record.MeterReadingDateTime} is less than current date: {meterAvailable.MeterReadingDateTime}" );
                                returnMeterResults.Outdated++;
                            }
                        } 
                        else
                        {

                            logger.LogInformation($"[{record.AccountId}] - Unable to find meter reading");

                            //Check account Exists in accounts table
                            var accounts = MeterReadingContext.Find<Accounts>(record.AccountId);

                            if (accounts != null)
                            {
                                logger.LogInformation($"[{record.AccountId}] - Creating new meter reading");

                                var meterReading = new MeterReadings()
                                {
                                    AccountId = record.AccountId,
                                    MeterReadingDateTime = DateTime.Parse(record.MeterReadingDateTime),
                                    MeterReadValue = Math.Abs(record.MeterReadValue) // Assuming all meter readings are positive, any readings with a - or negative would be counted as a 0 or prefixed with 0
                                };


                                if (meterReading != null && meterReading.MeterReadValue.ToString().Length <= 5)
                                {
                                    MeterReadingContext.MeterReadings.Add(meterReading);

                                    logger.LogInformation($"[{record.AccountId}] - Created new meter reading");
                                    returnMeterResults.NewMeterReadings++;
                                }
                                else
                                {
                                    logger.LogInformation($"[{record.AccountId}] - Invalid meter reading value of : {record.MeterReadValue} should be in format: NNNNN");
                                    returnMeterResults.InvalidMeterReadings++;
                                }
                            }
                            else
                            {
                                logger.LogInformation($"[{record.AccountId}] - Account is missing");
                                returnMeterResults.MissingAccounts++;
                            }
                        }

                        
                        returnMeterResults.TotalMeterReadings++;
                        await MeterReadingContext.SaveChangesAsync();
                    }
                }
                
                logger.LogInformation($"Results:");
                logger.LogInformation($"   * {returnMeterResults.TotalMeterReadings} Total");
                logger.LogInformation($"   * {returnMeterResults.NewMeterReadings} New");
                logger.LogInformation($"   * {returnMeterResults.UpdatedMeterReadings} Updated");
                logger.LogInformation($"   * {returnMeterResults.InvalidMeterReadings} Invalid");
                logger.LogInformation($"   * {returnMeterResults.MissingAccounts} Missing Accounts");
                logger.LogInformation($"   * {returnMeterResults.SuspiciousReadings} Suspicious (meter readings are less than current)");
                logger.LogInformation($"   * {returnMeterResults.Outdated} OutDated (Date is less than current)");
                logger.LogInformation($"Processing file Complete: {fileData.FileName}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message,$"Processing file: {fileData.FileName} - {ex.StackTrace}");
                throw;
            }

            // File must of been processed successfully so lets tell the consumer
            returnMeterResults.FileProcessed = true;

            return returnMeterResults;
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
