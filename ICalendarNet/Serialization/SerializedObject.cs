using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICalendarNet.Serialization
{
    public class SerializedObject(IEnumerable<string> lines, ICalComponent calComponent)
    {
        public IEnumerable<string> Lines { get; set; } = lines;
        public ICalComponent CalComponent { get; set; } = calComponent;
    }
}
