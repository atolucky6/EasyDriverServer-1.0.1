using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyScada.Winforms.Controls
{
    public class DateTimePoint
    {
        static DateTimePoint _empty = new DateTimePoint();
        public static DateTimePoint Empty 
        {   get
            {
                _empty.Time = DateTime.Now;
                return _empty;
            }
        }

        public DateTime Time { get; set; }
        public double Value { get; set; }

        public DateTimePoint(double value)
        {
            Value = value;
            Time = DateTime.Now;
        }

        public DateTimePoint()
        {
            Time = DateTime.Now;
        }

        public DateTimePoint(DateTime time, double value)
        {
            Time = time;
            Value = value;
        }
    }
}
