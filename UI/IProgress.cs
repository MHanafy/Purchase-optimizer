namespace Gluh.TechnicalTest.UI
{
    interface IProgress
    {
        void Report(double value, string activity = "");
    }
}
