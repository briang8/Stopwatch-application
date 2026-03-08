using Microsoft.VisualStudio.TestTools.UnitTesting;
using StopwatchApp;

namespace StopwatchApp.Tests
{
    /// <summary>
    /// Unit tests for StopwatchLogic following the TDD approach.
    /// Each method tests one behaviour in isolation.
    /// </summary>
    [TestClass]
    public class StopwatchLogicTests
    {
        // -- FormatTime -------------------------------------------------------

        /// <summary>Zero milliseconds should produce the zeroed display string.</summary>
        [TestMethod]
        public void FormatTime_ZeroMilliseconds_ReturnsZeroString()
        {
            string result = StopwatchLogic.FormatTime(0L);
            Assert.AreEqual("00:00:00.000", result);
        }

        /// <summary>Zero seconds should produce the zeroed display string (backward compatibility).</summary>
        [TestMethod]
        public void FormatTime_Zero_ReturnsZeroString()
        {
            string result = StopwatchLogic.FormatTime(0);
            Assert.AreEqual("00:00:00.000", result);
        }

        /// <summary>59 seconds should show as 00:00:59.000.</summary>
        [TestMethod]
        public void FormatTime_59Seconds_ShowsSeconds()
        {
            Assert.AreEqual("00:00:59.000", StopwatchLogic.FormatTime(59));
        }

        /// <summary>59 seconds and 123 milliseconds should show as 00:00:59.123.</summary>
        [TestMethod]
        public void FormatTime_59123Milliseconds_ShowsSecondsWithMilliseconds()
        {
            Assert.AreEqual("00:00:59.123", StopwatchLogic.FormatTime(59123L));
        }

        /// <summary>60 seconds should roll over into one minute.</summary>
        [TestMethod]
        public void FormatTime_60Seconds_ShowsOneMinute()
        {
            Assert.AreEqual("00:01:00.000", StopwatchLogic.FormatTime(60));
        }

        /// <summary>60 seconds and 500 milliseconds should show as 00:01:00.500.</summary>
        [TestMethod]
        public void FormatTime_60500Milliseconds_ShowsOneMinuteWithMilliseconds()
        {
            Assert.AreEqual("00:01:00.500", StopwatchLogic.FormatTime(60500L));
        }

        /// <summary>3 600 seconds should show as exactly one hour.</summary>
        [TestMethod]
        public void FormatTime_3600Seconds_ShowsOneHour()
        {
            Assert.AreEqual("01:00:00.000", StopwatchLogic.FormatTime(3600));
        }

        /// <summary>3600 seconds and 750 milliseconds should show as 01:00:00.750.</summary>
        [TestMethod]
        public void FormatTime_3600750Milliseconds_ShowsOneHourWithMilliseconds()
        {
            Assert.AreEqual("01:00:00.750", StopwatchLogic.FormatTime(3600750L));
        }

        /// <summary>3 661 seconds should produce 01:01:01.000.</summary>
        [TestMethod]
        public void FormatTime_3661Seconds_ShowsMixed()
        {
            Assert.AreEqual("01:01:01.000", StopwatchLogic.FormatTime(3661));
        }

        /// <summary>3 661 seconds and 999 milliseconds should produce 01:01:01.999.</summary>
        [TestMethod]
        public void FormatTime_3661999Milliseconds_ShowsMixedWithMilliseconds()
        {
            Assert.AreEqual("01:01:01.999", StopwatchLogic.FormatTime(3661999L));
        }

        /// <summary>Negative input should be clamped to 00:00:00.000.</summary>
        [TestMethod]
        public void FormatTime_Negative_ReturnsZeroString()
        {
            Assert.AreEqual("00:00:00.000", StopwatchLogic.FormatTime(-5));
        }

        /// <summary>Negative milliseconds input should be clamped to 00:00:00.000.</summary>
        [TestMethod]
        public void FormatTime_NegativeMilliseconds_ReturnsZeroString()
        {
            Assert.AreEqual("00:00:00.000", StopwatchLogic.FormatTime(-5000L));
        }

        // -- Millisecond precision tests -------------------------------------

        /// <summary>GetElapsedMilliseconds should work with millisecond precision.</summary>
        [TestMethod]
        public void GetElapsedMilliseconds_WorksWithPrecision()
        {
            var sw = new StopwatchLogic();
            sw.Start(0L);
            Assert.AreEqual(1500L, sw.GetElapsedMilliseconds(1500L));
        }

        /// <summary>Pausing and resuming should preserve millisecond precision.</summary>
        [TestMethod]
        public void MillisecondPrecision_PauseResume_PreservesPrecision()
        {
            var sw = new StopwatchLogic();
            sw.Start(0L);
            sw.Pause(1234L);    // 1.234 seconds
            sw.Resume(2000L);   // resume 766ms later
            Assert.AreEqual(2734L, sw.GetElapsedMilliseconds(3500L)); // 1234 + (3500 - 2000)
        }

        // -- IsRunning / HasStarted -------------------------------------------

        /// <summary>A fresh instance should not be running.</summary>
        [TestMethod]
        public void NewInstance_IsNotRunning()
        {
            var sw = new StopwatchLogic();
            Assert.IsFalse(sw.IsRunning);
        }

        /// <summary>A fresh instance should not count as started.</summary>
        [TestMethod]
        public void NewInstance_HasNotStarted()
        {
            var sw = new StopwatchLogic();
            Assert.IsFalse(sw.HasStarted);
        }

        // -- Start ------------------------------------------------------------

        /// <summary>After Start, IsRunning should be true.</summary>
        [TestMethod]
        public void Start_SetsIsRunning()
        {
            var sw = new StopwatchLogic();
            sw.Start(0);
            Assert.IsTrue(sw.IsRunning);
        }

        /// <summary>Elapsed time at tick 0 should be 0 right after Start.</summary>
        [TestMethod]
        public void Start_ElapsedIsZeroAtTickZero()
        {
            var sw = new StopwatchLogic();
            sw.Start(0);
            Assert.AreEqual(0, sw.GetElapsedSeconds(0));
        }

        /// <summary>After 5 ticks, elapsed should be 5 seconds.</summary>
        [TestMethod]
        public void Start_ElapsedIncreasesWithTicks()
        {
            var sw = new StopwatchLogic();
            sw.Start(0);
            Assert.AreEqual(5, sw.GetElapsedSeconds(5));
        }

        // -- Pause ------------------------------------------------------------

        /// <summary>Pausing should stop IsRunning.</summary>
        [TestMethod]
        public void Pause_StopsRunning()
        {
            var sw = new StopwatchLogic();
            sw.Start(0);
            sw.Pause(10);
            Assert.IsFalse(sw.IsRunning);
        }

        /// <summary>Elapsed time should be frozen at the pause point.</summary>
        [TestMethod]
        public void Pause_FreezesElapsedTime()
        {
            var sw = new StopwatchLogic();
            sw.Start(0);
            sw.Pause(10);
            // Even if more ticks pass, elapsed stays at 10
            Assert.AreEqual(10, sw.GetElapsedSeconds(20));
        }

        /// <summary>Calling Pause when already paused should be a no-op.</summary>
        [TestMethod]
        public void Pause_WhenAlreadyPaused_DoesNothing()
        {
            var sw = new StopwatchLogic();
            sw.Start(0);
            sw.Pause(10);
            sw.Pause(20); // second call should not add more time
            Assert.AreEqual(10, sw.GetElapsedSeconds(30));
        }

        // -- Resume -----------------------------------------------------------

        /// <summary>Resume should set IsRunning back to true.</summary>
        [TestMethod]
        public void Resume_SetsIsRunning()
        {
            var sw = new StopwatchLogic();
            sw.Start(0);
            sw.Pause(5);
            sw.Resume(5);
            Assert.IsTrue(sw.IsRunning);
        }

        /// <summary>After pause then resume, elapsed should continue accumulating correctly.</summary>
        [TestMethod]
        public void Resume_ContinuesFromPausedTime()
        {
            var sw = new StopwatchLogic();
            sw.Start(0);
            sw.Pause(10);   // 10 s accumulated
            sw.Resume(10);  // resume at tick 10
            // 5 more ticks -> 15 total
            Assert.AreEqual(15, sw.GetElapsedSeconds(15));
        }

        /// <summary>Calling Resume while already running should be a no-op.</summary>
        [TestMethod]
        public void Resume_WhenAlreadyRunning_DoesNothing()
        {
            var sw = new StopwatchLogic();
            sw.Start(0);
            sw.Resume(5); // already running, should not shift session start
            Assert.AreEqual(10, sw.GetElapsedSeconds(10));
        }

        // -- Reset ------------------------------------------------------------

        /// <summary>Reset should zero out elapsed time.</summary>
        [TestMethod]
        public void Reset_ZerosElapsedTime()
        {
            var sw = new StopwatchLogic();
            sw.Start(0);
            sw.Pause(30);
            sw.Reset();
            Assert.AreEqual(0, sw.GetElapsedSeconds(100));
        }

        /// <summary>Reset should stop the watch.</summary>
        [TestMethod]
        public void Reset_StopsRunning()
        {
            var sw = new StopwatchLogic();
            sw.Start(0);
            sw.Reset();
            Assert.IsFalse(sw.IsRunning);
        }

        /// <summary>HasStarted should be false after a Reset.</summary>
        [TestMethod]
        public void Reset_ClearsHasStarted()
        {
            var sw = new StopwatchLogic();
            sw.Start(0);
            sw.Pause(5);
            sw.Reset();
            Assert.IsFalse(sw.HasStarted);
        }

        // -- Stop -------------------------------------------------------------

        /// <summary>Stop should set IsRunning to false.</summary>
        [TestMethod]
        public void Stop_StopsRunning()
        {
            var sw = new StopwatchLogic();
            sw.Start(0);
            sw.Stop(20);
            Assert.IsFalse(sw.IsRunning);
        }

        /// <summary>Elapsed time should be captured at the moment Stop is called.</summary>
        [TestMethod]
        public void Stop_RecordsCorrectElapsedTime()
        {
            var sw = new StopwatchLogic();
            sw.Start(0);
            sw.Stop(45);
            // Further ticks should not change the recorded value
            Assert.AreEqual(45, sw.GetElapsedSeconds(100));
        }

        /// <summary>Stopping an already-stopped watch should not alter elapsed time.</summary>
        [TestMethod]
        public void Stop_WhenAlreadyStopped_DoesNotAddTime()
        {
            var sw = new StopwatchLogic();
            sw.Start(0);
            sw.Stop(20);
            sw.Stop(50); // second call should not add 30 more seconds
            Assert.AreEqual(20, sw.GetElapsedSeconds(100));
        }

        // -- Pause + Resume multiple cycles -----------------------------------

        /// <summary>Multiple pause/resume cycles should keep the total time correct.</summary>
        [TestMethod]
        public void MultiCycle_PauseResume_AccumulatesCorrectly()
        {
            var sw = new StopwatchLogic();
            sw.Start(0);
            sw.Pause(10);    // 10 s
            sw.Resume(15);   // gap of 5 s ignored
            sw.Pause(25);    // 10 more s -> 20 s total
            sw.Resume(30);
            // 5 more ticks -> 25 s total
            Assert.AreEqual(25, sw.GetElapsedSeconds(35));
        }
    }
}