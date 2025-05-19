// Project: zenon_Application
// Author: LinoValdi (Linandro Valderrama)
// Last Modified Date: 19th May 2025

using System.Globalization;

namespace zenon_Application
{
    /// <summary>
    /// Represents statistics for a variable, including its minimum and maximum values and their corresponding timestamps.
    /// </summary>
    /// <param name="variableId">The unique identifier of the variable whose statistics are being tracked.</param>
    /// <remarks>
    /// This class is used to track the min and max values of a variable over time.
    /// It is initialized with a variable ID and updates its statistics as new values are processed.
    /// The statistics can be output in a specific format for further analysis.
    /// </remarks>
    public class VariableStats(int variableId)
    {
        public int VariableId { get; } = variableId;
        public double MinValue { get; private set; } = double.MaxValue;
        public DateTime MinTimestamp { get; private set; }
        public double MaxValue { get; private set; } = double.MinValue;
        public DateTime MaxTimestamp { get; private set; }
        private bool _isInitialized;

        public void Update(double value, DateTime timestamp)
        {
            if (!_isInitialized)
            {
                MinValue = value;
                MinTimestamp = timestamp;
                MaxValue = value;
                MaxTimestamp = timestamp;
                _isInitialized = true;
            }
            else
            {
                if (value < MinValue)
                {
                    MinValue = value;
                    MinTimestamp = timestamp;
                }
                if (value > MaxValue)
                {
                    MaxValue = value;
                    MaxTimestamp = timestamp;
                }
            }
        }

        public override string ToString()
        {
            // Using InvariantCulture for consistent formatting of double and DateTime
            return $"{VariableId};{MinValue.ToString(CultureInfo.InvariantCulture)};{MinTimestamp.ToString("dd.MM.yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture)};{MaxValue.ToString(CultureInfo.InvariantCulture)};{MaxTimestamp.ToString("dd.MM.yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture)}";
        }
    }
}