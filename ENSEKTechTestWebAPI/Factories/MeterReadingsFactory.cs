using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ENSEKTechTestWebAPI.Models;

namespace ENSEKTechTestWebAPI.Factories
{
    public class MeterReadingsFactory
    {
        public const string MeterReadingExistsMessage = "This Meter Reading already exists";
        public const string AccountNotFoundMessage = "Account Not Found";
        public const string LaterReadingExistsMessage = "A later Meter Reading exists";
        public const string MeterReadingLowerThanPreviousMessage = "The Meter Reading is lower than an existing Reading";
        public const string InvalidMeterReadingMessage = "Invalid Meter Reading";

        public List<AccountDetails> GetAccounts()
        {
            using (var db = new ENSEKTechTestDBContext())
            {
                var accounts = (from acc in db.Accounts
                                let read = db.MeterReadings.Where(r => r.AccountId == acc.AccountId).OrderByDescending(r => r.MeterReadingDateTime).FirstOrDefault()
                                select new AccountDetails
                                {
                                    AccountId = acc.AccountId,
                                    FirstName = acc.FirstName,
                                    LastName = acc.LastName,
                                    MostRecentReading = (read == null ? null : read.MeterReadingDateTime),
                                    CurrentRead = (read == null ? null : read.MeterReadValue),
                                }).OrderBy(a => a.AccountId).ToList();

                return accounts;
            }
        }

        public List<UploadResults> SaveMeterReadings(List<MeterReading> meterReadings)
        {
            List<UploadResults> uploadResults = new List<UploadResults>();

            using (var db = new ENSEKTechTestDBContext())
            {
                foreach (var meterReading in meterReadings)
                {
                    try
                    {
                        AddMeterReading(db, meterReading);
                        uploadResults.Add(new Models.UploadResults
                        {
                            AccountId = meterReading.AccountId,
                            MeterReadingDateTime = meterReading.MeterReadingDateTime,
                            MeterReadValue = meterReading.MeterReadValue,
                            Result = "Reading added successfully"
                        });
                    }
                    catch (Exception ex)
                    {
                        uploadResults.Add(new Models.UploadResults
                        {
                            AccountId = meterReading.AccountId,
                            MeterReadingDateTime = meterReading.MeterReadingDateTime,
                            MeterReadValue = meterReading.MeterReadValue,
                            Result = ex.Message
                        });
                    }
                }
            }

            return uploadResults;
        }

        public void AddMeterReading(ENSEKTechTestDBContext db, MeterReading meterReading)
        {
            if (meterReading.MeterReadValue < 0 || meterReading.MeterReadValue > 99999)
            {
                throw new ArgumentOutOfRangeException("MeterReadValue", meterReading.MeterReadValue, InvalidMeterReadingMessage);
            }

            var thisReading = db.MeterReadings.FirstOrDefault(r => r.AccountId == meterReading.AccountId && r.MeterReadingDateTime == meterReading.MeterReadingDateTime && r.MeterReadValue == meterReading.MeterReadValue);
            if (thisReading != null)
            {
                throw new ArgumentOutOfRangeException("MeterReadingDateTime", meterReading.MeterReadingDateTime, MeterReadingExistsMessage);
            }

            var account = db.Accounts.FirstOrDefault(a => a.AccountId == meterReading.AccountId);
            if (account == null)
            {
                throw new ArgumentOutOfRangeException("AccountId", meterReading.AccountId, AccountNotFoundMessage);
            }

            var mostRecentReading = db.MeterReadings.Where(r => r.AccountId == meterReading.AccountId).OrderByDescending(r => r.MeterReadingDateTime).FirstOrDefault();
            if (mostRecentReading != null && mostRecentReading.MeterReadingDateTime > meterReading.MeterReadingDateTime)
            {
                throw new ArgumentOutOfRangeException("MeterReadingDateTime", meterReading.MeterReadingDateTime, LaterReadingExistsMessage);
            }

            var highestReading = db.MeterReadings.Where(r => r.AccountId == meterReading.AccountId).OrderByDescending(r => r.MeterReadValue).FirstOrDefault();
            if (highestReading != null && highestReading.MeterReadValue > meterReading.MeterReadValue)
            {
                throw new ArgumentOutOfRangeException("MeterReadingDateTime", meterReading.MeterReadingDateTime, MeterReadingLowerThanPreviousMessage);
            }
            
            db.MeterReadings.Add(new MeterReading
            {
                AccountId = meterReading.AccountId,
                MeterReadingDateTime = meterReading.MeterReadingDateTime,
                MeterReadValue = meterReading.MeterReadValue
            });
            db.SaveChanges();
        }
    }
}
