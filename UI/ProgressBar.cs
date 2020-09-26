using Gluh.TechnicalTest.UI;
using System;
using System.Text;
using System.Threading;

namespace Gluh.TechnicalTest
{
    /// <summary>
    /// An ASCII progress bar
    /// </summary>
    public class ProgressBar : IDisposable, IProgress
    {
        private const int blockCount = 40;
        private readonly TimeSpan animationInterval = TimeSpan.FromSeconds(1.0 / 8);
        private const string animation = @"|/-\";
        private bool showProgressBar = true;

        private readonly Timer timer;

        private double currentProgress = 0;
        private string _activity;
        private string currentText = string.Empty;
        private bool disabled = false;
        private int animationIndex = 0;


        public ProgressBar(bool ShowProgressBar = true)
        {
            showProgressBar = ShowProgressBar;
            timer = new Timer(TimerHandler);

            // A progress bar is only for temporary display in a console window.
            // If the console output is redirected to a file, draw nothing.
            // Otherwise, we'll end up with a lot of garbage in the target file.
            if (!Console.IsOutputRedirected)
            {
                ResetTimer();
            }
        }

        public void Show()
        {
            disabled = false; 
            showProgressBar = true;
        }
        public void Hide()
        {
            UpdateText(string.Empty);
            disabled = true;
            showProgressBar = false;
        }
        public void Report(double value, string activity = null)
        {
            // Make sure value is in [0..1] range
            value = Math.Max(0, Math.Min(1, value));
            Interlocked.Exchange(ref currentProgress, value);
            Interlocked.Exchange(ref _activity, activity);
        }

        private void TimerHandler(object state)
        {
            lock (timer)
            {
                if (disabled) return;


                string text = "";
                if (showProgressBar)
                {
                    int progressBlockCount = (int)(currentProgress * blockCount);
                    int percent = (int)(currentProgress * 100);
                    string barCurrent = new string('#', progressBlockCount);
                    string barRemaining = new string('-', blockCount - progressBlockCount);
                    char anim = animation[animationIndex++ % animation.Length];
                    text = string.Format($"[{barCurrent}{barRemaining}] {percent,3}% {anim}{(_activity==null?null:$" - {_activity}")}");
                }
                else
                {
                    text = animation[animationIndex++ % animation.Length].ToString();
                }
                UpdateText(text);


                ResetTimer();
            }
        }


        private void UpdateText(string text)
        {
            // Get length of common portion
            int commonPrefixLength = 0;
            int commonLength = Math.Min(currentText.Length, text.Length);
            while (commonPrefixLength < commonLength && text[commonPrefixLength] == currentText[commonPrefixLength])
            {
                commonPrefixLength++;
            }


            // Backtrack to the first differing character
            StringBuilder outputBuilder = new StringBuilder();
            outputBuilder.Append('\b', currentText.Length - commonPrefixLength);


            // Output new suffix
            outputBuilder.Append(text.Substring(commonPrefixLength));


            // If the new text is shorter than the old one: delete overlapping characters
            int overlapCount = currentText.Length - text.Length;
            if (overlapCount > 0)
            {
                outputBuilder.Append(' ', overlapCount);
                outputBuilder.Append('\b', overlapCount);
            }


            Console.Write(outputBuilder);
            currentText = text;
        }


        private void ResetTimer()
        {
            timer.Change(animationInterval, TimeSpan.FromMilliseconds(-1));
        }


        public void Dispose()
        {
            lock (timer)
            {
                disabled = true;
                UpdateText(string.Empty);
            }
        }
    }
}
