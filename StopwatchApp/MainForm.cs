using System;
using System.Drawing;
using System.Windows.Forms;

namespace StopwatchApp
{
    /// <summary>
    /// Main form for the Stopwatch application.
    /// Wires up the UI controls to the StopwatchLogic class.
    /// </summary>
    public class MainForm : Form
    {
        // -- Controls --
        private Label _lblTime = null!;
        private Label _lblStatus = null!;
        private Button _btnStart = null!;
        private Button _btnPause = null!;
        private Button _btnResume = null!;
        private Button _btnReset = null!;
        private Button _btnStop = null!;
        private Timer _timer = null!;

        // Ticks every 10 milliseconds; used as our time source for precision
        private long _tickCount;

        // The core logic object; keeps state separate from the UI
        private readonly StopwatchLogic _stopwatch = new StopwatchLogic();

        /// <summary>Initializes the form and all its child controls.</summary>
        public MainForm()
        {
            InitialiseComponents();
        }

        // -- UI setup ---------------------------------------------------------

        /// <summary>Builds and lays out every control on the form.</summary>
        private void InitialiseComponents()
        {
            // Form properties
            Text = "Stopwatch";
            Size = new Size(400, 340);
            MinimumSize = new Size(400, 340);
            MaximumSize = new Size(400, 340);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(30, 30, 30);
            Font = new Font("Segoe UI", 10f);

            // Time display label
            _lblTime = new Label
            {
                Text = "00:00:00.000",
                Font = new Font("Courier New", 36f, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(380, 80),
                Location = new Point(10, 20)
            };

            // Status line shown below the clock
            _lblStatus = new Label
            {
                Text = "Ready",
                Font = new Font("Segoe UI", 10f),
                ForeColor = Color.Silver,
                BackColor = Color.Transparent,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(380, 24),
                Location = new Point(10, 105)
            };

            // Button layout helpers
            int btnW = 68, gap = 8;
            int totalW = (btnW * 5) + (gap * 4);
            int startX = (380 - totalW) / 2 + 10;
            int btnY = 150;

            _btnStart = MakeButton("Start", Color.FromArgb(40, 167, 69), new Point(startX, btnY));
            _btnPause = MakeButton("Pause", Color.FromArgb(255, 193, 7), new Point(startX + (btnW + gap), btnY));
            _btnResume = MakeButton("Resume", Color.FromArgb(23, 162, 184), new Point(startX + (btnW + gap) * 2, btnY));
            _btnReset = MakeButton("Reset", Color.FromArgb(108, 117, 125), new Point(startX + (btnW + gap) * 3, btnY));
            _btnStop = MakeButton("Stop", Color.FromArgb(220, 53, 69), new Point(startX + (btnW + gap) * 4, btnY));

            // Disable buttons that make no sense before the watch is started
            _btnPause.Enabled = false;
            _btnResume.Enabled = false;
            _btnReset.Enabled = false;
            _btnStop.Enabled = false;

            // Wire up click handlers
            _btnStart.Click += OnStart;
            _btnPause.Click += OnPause;
            _btnResume.Click += OnResume;
            _btnReset.Click += OnReset;
            _btnStop.Click += OnStop;

            // Timer fires every 10 ms for millisecond precision
            _timer = new Timer { Interval = 10 };
            _timer.Tick += OnTick;

            // Add everything to the form
            Controls.Add(_lblTime);
            Controls.Add(_lblStatus);
            Controls.Add(_btnStart);
            Controls.Add(_btnPause);
            Controls.Add(_btnResume);
            Controls.Add(_btnReset);
            Controls.Add(_btnStop);
        }

        /// <summary>Helper that creates a styled button at the given position.</summary>
        /// <param name="text">Button label.</param>
        /// <param name="color">Background colour.</param>
        /// <param name="location">Position on the form.</param>
        private static Button MakeButton(string text, Color color, Point location)
        {
            return new Button
            {
                Text = text,
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(68, 36),
                Location = location,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
        }

        // -- Button handlers --------------------------------------------------

        /// <summary>Starts the stopwatch from zero.</summary>
        private void OnStart(object? sender, EventArgs e)
        {
            _tickCount = 0;
            _timer.Start();
            _stopwatch.Start(0);

            UpdateDisplay();
            SetStatus("Running...");

            _btnStart.Enabled = false;
            _btnPause.Enabled = true;
            _btnResume.Enabled = false;
            _btnReset.Enabled = true;
            _btnStop.Enabled = true;
        }

        /// <summary>Pauses the stopwatch and shows the current time in the status line.</summary>
        private void OnPause(object? sender, EventArgs e)
        {
            _stopwatch.Pause(_tickCount * 10); // Convert ticks to milliseconds
            _timer.Stop();

            string time = StopwatchLogic.FormatTime(_stopwatch.GetElapsedMilliseconds(_tickCount * 10));
            SetStatus($"Paused at {time}");

            _btnPause.Enabled = false;
            _btnResume.Enabled = true;
        }

        /// <summary>Resumes the stopwatch from the paused time.</summary>
        private void OnResume(object? sender, EventArgs e)
        {
            _stopwatch.Resume(_tickCount * 10); // Convert ticks to milliseconds
            _timer.Start();

            SetStatus("Running...");

            _btnResume.Enabled = false;
            _btnPause.Enabled = true;
        }

        /// <summary>Resets the stopwatch back to 00:00:00.000.</summary>
        private void OnReset(object? sender, EventArgs e)
        {
            _timer.Stop();
            _tickCount = 0;
            _stopwatch.Reset();

            UpdateDisplay();
            SetStatus("Reset. Press Start to begin again.");

            _btnStart.Enabled = true;
            _btnPause.Enabled = false;
            _btnResume.Enabled = false;
            _btnReset.Enabled = false;
            _btnStop.Enabled = false;
        }

        /// <summary>Stops the stopwatch and shows the final time.</summary>
        private void OnStop(object? sender, EventArgs e)
        {
            _stopwatch.Stop(_tickCount * 10); // Convert ticks to milliseconds
            _timer.Stop();

            string time = StopwatchLogic.FormatTime(_stopwatch.GetElapsedMilliseconds(_tickCount * 10));
            SetStatus($"Stopped. Last time: {time}");

            _btnStart.Enabled = true;
            _btnPause.Enabled = false;
            _btnResume.Enabled = false;
            _btnReset.Enabled = true;
            _btnStop.Enabled = false;
        }

        // -- Timer tick -------------------------------------------------------

        /// <summary>Called every 10 milliseconds while the stopwatch is running.</summary>
        private void OnTick(object? sender, EventArgs e)
        {
            _tickCount++;
            UpdateDisplay();
        }

        // -- Helpers ----------------------------------------------------------

        /// <summary>Refreshes the time label with the latest elapsed time.</summary>
        private void UpdateDisplay()
        {
            long elapsed = _stopwatch.GetElapsedMilliseconds(_tickCount * 10); // Convert ticks to milliseconds
            _lblTime.Text = StopwatchLogic.FormatTime(elapsed);
        }

        /// <summary>Updates the status label text.</summary>
        /// <param name="message">The message to display.</param>
        private void SetStatus(string message)
        {
            _lblStatus.Text = message;
        }
    }
}