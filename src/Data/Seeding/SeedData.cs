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
            if (!meterReadingContext.MeterReadings.Any())
            {
                using var reader = new StreamReader("Data\\Seeding\\Test_Accounts.csv");
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

                var records = csv.GetRecords<MeterReading>().ToArray();

                foreach (var record in records)
                {

                }
            }
        }
    }
}
