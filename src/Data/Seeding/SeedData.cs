using CsvHelper;
using ENSEK_Meter_Reading.Entities;
using ENSEK_Meter_Reading.Models;
using System.Globalization;

namespace ENSEK_Meter_Reading.Data.Seeding
{
    public class SeedData
    {
        private readonly MeterReadingContext meterReadingContext;

        public SeedData(MeterReadingContext meterReadingContext)
        {
            this.meterReadingContext = meterReadingContext;
        }

        public void Seed()
        {
            if (!meterReadingContext.Accounts.Any())
            {
                using var reader = new StreamReader("Data\\Seeding\\Test_Accounts.csv");
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

                var records = csv.GetRecords<Accounts>().ToArray();
                meterReadingContext.Accounts.AddRange(records);
                meterReadingContext.SaveChanges();

                // You could read all the CSV and do additional validation
                //foreach (var record in records)
                //{

                //    var account = meterReadingContext.Find<Accounts>(record.AccountId);

                //    if (account == null)
                //    {
                //        meterReadingContext.Accounts.Add(new Accounts()
                //        {
                //            AccountId = record.AccountId,
                //            FirstName = record.FirstName,
                //            LastName = record.LastName
                //        });

                //        meterReadingContext.SaveChanges();
                //    }
                //}
                
            }
        }
    }
}
