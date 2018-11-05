using System;
using System.IO;

namespace Omtime
{
    public class TimestampStreamWriter : StreamWriter
    {
        public TimestampStreamWriter(Stream stream)
            : base(stream)
        {
        }

        public override void WriteLine(string value)
        {
            value = String.Format("{0:D2}:{1:D2}:{2:D2}.{3:D3} {4}",
                DateTime.Now.Hour,
                DateTime.Now.Minute,
                DateTime.Now.Second,
                DateTime.Now.Millisecond,
                value);
            base.WriteLine(value);
        }
    }
}
