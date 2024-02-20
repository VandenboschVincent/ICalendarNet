using ICalendarNet.Base;
using ICalendarNet.Extensions;
using System.Text;

namespace ICalendarNet.DataTypes
{
    /// <summary>
    /// Attachments represent the ATTACH element that can be associated with Alarms, Journals, Todos, and Events. There are two kinds of attachments:
    /// 1) A string representing a URI which is typically human-readable, OR
    /// 2) A base64-encoded string that can represent anything
    /// </summary>
    public class CalendarAttachment : ContentLine
    {
        private static readonly Encoding dataEncoding = Encoding.UTF8;

        /// <summary>
        /// Gets the location of the attachment if any.
        /// </summary>
        /// <returns></returns>
        public Uri? GetUri()
        {
            if (Uri.TryCreate(Value, UriKind.RelativeOrAbsolute, out Uri? uri))
                return uri;
            return null;
        }

        /// <summary>
        /// Gets the binary value of the attachment if any.
        /// </summary>
        /// <returns></returns>
        public byte[]? GetData()
        {
            if ((Value.Length % 4 == 0) && Value.IsBase64())
                return dataEncoding.GetBytes(Value);
            return null;
        }

        /// <summary>
        /// set the type of stored e.g. application/json
        /// </summary>
        public string? FMTTYPE
        {
            get => Parameters.GetValue("FMTTYPE");
            set => Parameters.SetOrAddValue("FMTTYPE", value!);
        }

        /// <summary>
        /// Encoding of the data in stored locally. usually set to BASE64.
        /// </summary>
        public string? ENCODING
        {
            get => Parameters.GetValue("ENCODING");
            set => Parameters.SetOrAddValue("ENCODING", value!);
        }

        /// <summary>
        /// the type of data when stored locally. usually BINARY
        /// </summary>
        public string? ValueType
        {
            get => Parameters.GetValue("VALUE");
            set => Parameters.SetOrAddValue("VALUE", value!);
        }

        public CalendarAttachment(string key, string value, ContentLineParameters? param) : base(key, value, param)
        { }
        public CalendarAttachment(Uri uri, string? fmttype)
            : base("ATTACH", uri.ToString(), null)
        {
            if (!string.IsNullOrEmpty(fmttype))
                Parameters.Add("FMTTYPE", new ContentLineParameter("FMTTYPE", fmttype));
        }
        public CalendarAttachment(byte[] data, string? fmttype, string valueType = "BINARY", string encoding = "BASE64")
            : base("ATTACH", dataEncoding.GetString(data), null)
        {
            if (!string.IsNullOrEmpty(fmttype))
                Parameters.Add("FMTTYPE", new ContentLineParameter("FMTTYPE", fmttype));
            if (!string.IsNullOrEmpty(valueType))
                Parameters.Add("VALUE", new ContentLineParameter("VALUE", valueType));
            if (!string.IsNullOrEmpty(encoding))
                Parameters.Add("ENCODING", new ContentLineParameter("ENCODING", encoding));
        }
    }
}
