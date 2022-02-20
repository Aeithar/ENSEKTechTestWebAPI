using Microsoft.VisualStudio.TestTools.UnitTesting;
using ENSEKTechTestWebAPI.Factories;
using ENSEKTechTestWebAPI.Models;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;

namespace ENSEKTechTestUnitTests
{
    [TestClass]
    public class MeterReadingsTests
    {
        [TestMethod]
        public void MeterReadings_AccountNotFound_Test()
        {
            using (var db = new ENSEKTechTestDBContext())
            {
                // Arrange
                var factory = new MeterReadingsFactory();
                var reading = new MeterReading
                {
                    AccountId = 9999,
                    MeterReadingDateTime = System.DateTime.Now,
                    MeterReadValue = 12345
                };

                // Act
                try
                {
                    factory.AddMeterReading(db, reading);
                }
                catch (System.ArgumentOutOfRangeException e)
                {
                    // Assert
                    StringAssert.Contains(e.Message, MeterReadingsFactory.AccountNotFoundMessage);
                    return;
                }

                Assert.Fail("The expected exception was not thrown.");
            }
        }

        [TestMethod]
        public void MeterReadings_MeterReadingExists_Test()
        {
            using (var db = new ENSEKTechTestDBContext())
            {
                // Arrange
                var factory = new MeterReadingsFactory();
                var reading = new MeterReading
                {
                    AccountId = 1234,
                    MeterReadingDateTime = System.DateTime.ParseExact("2022-02-19 20:00:00", "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                    MeterReadValue = 12345
                };
                // If reading isn't already there, add it here so we can test it
                var exists = db.MeterReadings.FirstOrDefault(r => r == reading);
                if (exists == null)
                {
                    db.MeterReadings.Add(reading);
                    db.SaveChanges();
                }

                // Act
                try
                {
                    factory.AddMeterReading(db, reading);
                }
                catch (System.ArgumentOutOfRangeException e)
                {
                    // Assert
                    StringAssert.Contains(e.Message, MeterReadingsFactory.MeterReadingExistsMessage);
                    return;
                }

                Assert.Fail("The expected exception was not thrown.");
            }
        }

        [TestMethod]
        public void MeterReadings_LaterReadingExists_Test()
        {
            using (var db = new ENSEKTechTestDBContext())
            {
                // Arrange
                var factory = new MeterReadingsFactory();
                var reading = new MeterReading
                {
                    AccountId = 1234,
                    MeterReadingDateTime = System.DateTime.ParseExact("2022-02-19 20:00:00", "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                    MeterReadValue = 12345
                };
                // If reading isn't already there, add it here so we can test it
                var exists = db.MeterReadings.FirstOrDefault(r => r == reading);
                if (exists == null)
                {
                    db.MeterReadings.Add(reading);
                    db.SaveChanges();
                }

                // Act
                try
                {
                    reading.MeterReadingDateTime = System.DateTime.ParseExact("2022-02-19 19:00:00", "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    factory.AddMeterReading(db, reading);
                }
                catch (System.ArgumentOutOfRangeException e)
                {
                    // Assert
                    StringAssert.Contains(e.Message, MeterReadingsFactory.LaterReadingExistsMessage);
                    return;
                }

                Assert.Fail("The expected exception was not thrown.");
            }
        }

        [TestMethod]
        public void MeterReadings_InvalidMeterReadingWhenNegative_Test()
        {
            using (var db = new ENSEKTechTestDBContext())
            {
                // Arrange
                var factory = new MeterReadingsFactory();
                var reading = new MeterReading
                {
                    AccountId = 1234,
                    MeterReadingDateTime = System.DateTime.Now,
                    MeterReadValue = -1
                };

                // Act
                try
                {
                    factory.AddMeterReading(db, reading);
                }
                catch (System.ArgumentOutOfRangeException e)
                {
                    // Assert
                    StringAssert.Contains(e.Message, MeterReadingsFactory.InvalidMeterReadingMessage);
                    return;
                }

                Assert.Fail("The expected exception was not thrown.");
            }
        }

        [TestMethod]
        public void MeterReadings_InvalidMeterReadingWhenTooLarge_Test()
        {
            using (var db = new ENSEKTechTestDBContext())
            {
                // Arrange
                var factory = new MeterReadingsFactory();
                var reading = new MeterReading
                {
                    AccountId = 1234,
                    MeterReadingDateTime = System.DateTime.Now,
                    MeterReadValue = 123456
                };

                // Act
                try
                {
                    factory.AddMeterReading(db, reading);
                }
                catch (System.ArgumentOutOfRangeException e)
                {
                    // Assert
                    StringAssert.Contains(e.Message, MeterReadingsFactory.InvalidMeterReadingMessage);
                    return;
                }

                Assert.Fail("The expected exception was not thrown.");
            }
        }

        [TestMethod]
        public void MeterReadings_MeterReadingLowerThanPrevious_Test()
        {
            using (var db = new ENSEKTechTestDBContext())
            {
                // Arrange
                var factory = new MeterReadingsFactory();
                var highestReading = db.MeterReadings.Where(r => r.AccountId == 1234).OrderByDescending(r => r.MeterReadValue).FirstOrDefault();
                var highest = (highestReading == null ? 100 : highestReading.MeterReadValue);
                var reading = new MeterReading
                {
                    AccountId = 1234,
                    MeterReadingDateTime = System.DateTime.Now,
                    MeterReadValue = highest
                };
                // If no current readings, add one
                if (highestReading == null)
                {
                    db.MeterReadings.Add(reading);
                    db.SaveChanges();
                }
                reading.MeterReadValue -= 1;

                // Act
                try
                {
                    factory.AddMeterReading(db, reading);
                }
                catch (System.ArgumentOutOfRangeException e)
                {
                    // Assert
                    StringAssert.Contains(e.Message, MeterReadingsFactory.MeterReadingLowerThanPreviousMessage);
                    return;
                }

                Assert.Fail("The expected exception was not thrown.");
            }
        }

    }
}
