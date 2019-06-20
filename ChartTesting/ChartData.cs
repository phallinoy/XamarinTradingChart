using System;
using System.Collections.Generic;
using System.Text;

namespace ChartTesting
{
    class ChartData
    {
        private string date;
        private double open, high, low, close, volume;

        public ChartData(String date, double open, double high, double low, double close, double volume)
        {
            this.date = date;
            this.open = open;
            this.high = high;
            this.low = low;
            this.close = close;
            this.volume = volume;
        }

        public string Date
        {
            get
            {
                return date;
            }
            set
            {
                date = value;
            }
        }
        public double Open
        {
            get
            {
                return open;
            }
            set
            {
                open = value;
            }
        }
        public double High
        {
            get
            {
                return high;
            }
            set
            {
                high = value;
            }
        }
        public double Low
        {
            get
            {
                return low;
            }
            set
            {
                low = value;
            }
        }
        public double Close
        {
            get
            {
                return close;
            }
            set
            {
                close = value;
            }
        }
        public double Volume
        {
            get
            {
                return volume;
            }
            set
            {
                volume = value;
            }
        }

    }
}
