using System;

namespace StopwatchApp
{
    /// <summary>
    /// Handles all stopwatch state and time tracking logic.
    /// Keeps the UI layer separate from the core timer behaviour.
    /// Now supports millisecond precision for better accuracy.
    /// </summary>
    public class StopwatchLogic
    {
        // Total elapsed milliseconds accumulated before the last pause
        private long _accumulatedMilliseconds;

        // The millisecond at which the current running session started
        private long _sessionStartMilliseconds;

        // Tracks whether the watch is actively running
        private bool _isRunning;

        /// <summary>Gets whether the stopwatch is currently running.</summary>
        public bool IsRunning => _isRunning;

        /// <summary>Gets whether the stopwatch has ever been started (i.e. time > 0 or running).</summary>
        public bool HasStarted => _isRunning || _accumulatedMilliseconds > 0;

        /// <summary>
        /// Returns the total elapsed milliseconds, including any time from the current session.
        /// </summary>
        /// <param name="currentMillisecond">The current millisecond from the UI timer tick.</param>
        public long GetElapsedMilliseconds(long currentMillisecond)
        {
            if (_isRunning)
                return _accumulatedMilliseconds + (currentMillisecond - _sessionStartMilliseconds);

            return _accumulatedMilliseconds;
        }

        /// <summary>
        /// Returns the total elapsed seconds for backward compatibility.
        /// </summary>
        /// <param name="currentSecond">The current second from the UI timer tick.</param>
        public int GetElapsedSeconds(int currentSecond)
        {
            return (int)(GetElapsedMilliseconds(currentSecond * 1000) / 1000);
        }

        /// <summary>
        /// Starts the stopwatch from zero.
        /// </summary>
        /// <param name="currentMillisecond">The current millisecond provided by the UI timer.</param>
        public void Start(long currentMillisecond)
        {
            _accumulatedMilliseconds = 0;
            _sessionStartMilliseconds = currentMillisecond;
            _isRunning = true;
        }

        /// <summary>
        /// Starts the stopwatch from zero (backward compatibility).
        /// </summary>
        /// <param name="currentSecond">The current second provided by the UI timer.</param>
        public void Start(int currentSecond)
        {
            Start(currentSecond * 1000L);
        }

        /// <summary>
        /// Pauses the stopwatch and saves elapsed time so far.
        /// </summary>
        /// <param name="currentMillisecond">The current millisecond at the moment of pausing.</param>
        public void Pause(long currentMillisecond)
        {
            if (!_isRunning) return;

            _accumulatedMilliseconds += currentMillisecond - _sessionStartMilliseconds;
            _isRunning = false;
        }

        /// <summary>
        /// Pauses the stopwatch and saves elapsed time so far (backward compatibility).
        /// </summary>
        /// <param name="currentSecond">The current second at the moment of pausing.</param>
        public void Pause(int currentSecond)
        {
            Pause(currentSecond * 1000L);
        }

        /// <summary>
        /// Resumes the stopwatch from where it was paused.
        /// </summary>
        /// <param name="currentMillisecond">The current millisecond at the moment of resuming.</param>
        public void Resume(long currentMillisecond)
        {
            if (_isRunning) return;

            _sessionStartMilliseconds = currentMillisecond;
            _isRunning = true;
        }

        /// <summary>
        /// Resumes the stopwatch from where it was paused (backward compatibility).
        /// </summary>
        /// <param name="currentSecond">The current second at the moment of resuming.</param>
        public void Resume(int currentSecond)
        {
            Resume(currentSecond * 1000L);
        }

        /// <summary>
        /// Resets all state back to zero without stopping the UI timer.
        /// </summary>
        public void Reset()
        {
            _accumulatedMilliseconds = 0;
            _sessionStartMilliseconds = 0;
            _isRunning = false;
        }

        /// <summary>
        /// Stops the stopwatch and saves the current elapsed time.
        /// </summary>
        /// <param name="currentMillisecond">The current millisecond at the moment of stopping.</param>
        public void Stop(long currentMillisecond)
        {
            if (_isRunning)
                _accumulatedMilliseconds += currentMillisecond - _sessionStartMilliseconds;

            _isRunning = false;
        }

        /// <summary>
        /// Stops the stopwatch and saves the current elapsed time (backward compatibility).
        /// </summary>
        /// <param name="currentSecond">The current second at the moment of stopping.</param>
        public void Stop(int currentSecond)
        {
            Stop(currentSecond * 1000L);
        }

        /// <summary>
        /// Formats a total number of milliseconds into hh:mm:ss.fff string format.
        /// </summary>
        /// <param name="totalMilliseconds">The total milliseconds to format.</param>
        /// <returns>A string in the format 00:00:00.000.</returns>
        public static string FormatTime(long totalMilliseconds)
        {
            if (totalMilliseconds < 0) totalMilliseconds = 0;

            long totalSeconds = totalMilliseconds / 1000;
            long milliseconds = totalMilliseconds % 1000;

            long hours = totalSeconds / 3600;
            long minutes = (totalSeconds % 3600) / 60;
            long seconds = totalSeconds % 60;

            return $"{hours:D2}:{minutes:D2}:{seconds:D2}.{milliseconds:D3}";
        }

        /// <summary>
        /// Formats a total number of seconds into hh:mm:ss string format (backward compatibility).
        /// </summary>
        /// <param name="totalSeconds">The total seconds to format.</param>
        /// <returns>A string in the format 00:00:00.000.</returns>
        public static string FormatTime(int totalSeconds)
        {
            return FormatTime(totalSeconds * 1000L);
        }
    }
}