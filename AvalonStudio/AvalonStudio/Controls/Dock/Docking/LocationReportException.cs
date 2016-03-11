using System;

namespace AvalonStudio.Controls.Dock.Docking
{
    public class LocationReportException : Exception
    {
        public LocationReportException()
        {
            
        }

        public LocationReportException(string message) : base(message)
        {
            
        }

        public LocationReportException(string message, Exception exception) : base(message, exception)
        {
            
        }
    }
}
