namespace ICalendarNet
{
    public static class Statics
    {
        public enum ICalParameter
        {
            ALTREP,
            CN,
            CUTYPE,
            DELEGATED_FROM,
            DELEGATED_TO,
            DIR,
            ENCODING,
            FMTTYPE,
            FBTYPE,
            LANGUAGE,
            MEMBER,
            PARTSTAT,
            RANGE,
            RELATED,
            RELTYPE,
            ROLE,
            RSVP,
            SENT_BY,
            TZID,
            VALUE,
            X_FILENAME,
            X_MS_OLK_RESPTIME,
            X_MICROSOFT_ISLEAPMONTH,
            DISPLAY,
            EMAIL,
            FEATURE,
            LABEL,
            ORDER,
            SCHEMA,
            DERIVED,
            GAP,
            LINKREL
        }

        public static readonly string[] ICalParameters = new string[] {
            "ALTREP",
            "CN",
            "CUTYPE",
            "DELEGATED-FROM",
            "DELEGATED-TO",
            "DIR",
            "ENCODING",
            "FMTTYPE",
            "FBTYPE",
            "LANGUAGE",
            "MEMBER",
            "PARTSTAT",
            "RANGE",
            "RELATED",
            "RELTYPE",
            "ROLE",
            "RSVP",
            "SENT-BY",
            "TZID",
            "VALUE",
            "X-FILENAME",
            "X-MS-OLK-RESPTIME",
            "X-MICROSOFT-ISLEAPMONTH",
            "DISPLAY",
            "EMAIL",
            "FEATURE",
            "LABEL",
            "ORDER",
            "SCHEMA",
            "DERIVED",
            "GAP",
            "LINKREL"
        };

        public enum ICalProperty
        {
            /// <summary>
            /// 3.7.1 Value Type: TEXT
            /// This property defines the calendar scale used for the calendar information specified in the iCalendar object.
            /// This property can be specified once in an iCalendar object.
            /// The default value is "GREGORIAN".
            /// </summary>
            CALSCALE,

            /// <summary>
            /// 3.7.2 Value Type: TEXT
            /// This property defines the iCalendar object method associated with the calendar object.
            /// This property can be specified once in an iCalendar object.
            /// </summary>
            METHOD,

            /// <summary>
            /// 3.7.3 Value Type: TEXT
            /// This property specifies the identifier for the product that created the iCalendar object.
            /// The property MUST be specified once in an iCalendar object.
            /// </summary>
            PRODID,

            /// <summary>
            /// 3.7.3 Value Type: TEXT
            /// This property specifies the identifier corresponding to the
            /// highest version number or the minimum and maximum range of the
            /// iCalendar specification that is required in order to interpret the iCalendar object.
            /// This property MUST be specified once in an iCalendar object.
            /// </summary>
            VERSION,

            X_CALEND,
            X_CALSTART,
            X_CLIPEND,
            X_CLIPSTART,
            X_MICROSOFT_CALSCALE,
            X_MS_OLK_FORCEINSPECTOROPEN,
            X_MS_WKHRDAYS,
            X_MS_WKHREND,
            X_MS_WKHRSTART,
            X_OWNER,
            X_PRIMARY_CALENDAR,
            X_PUBLISHED_TTL,
            X_WR_CALDESC,
            X_WR_CALNAME,
            X_WR_RELCALID,

            /// <summary>
            /// 3.8.1.1 Value Type: URI or BINARY
            /// This property provides the capability to associate a document object with a calendar component.
            /// This property can be specified multiple times in a "VEVENT", "VTODO", "VJOURNAL", or "VALARM" calendar component with
            /// the exception of AUDIO alarm that only allows this property to occur once.
            /// </summary>
            ATTACH,

            /// <summary>
            /// 3.8.1.2 Value Type: TEXT
            /// This property defines the categories for a calendar component.
            /// The property can be specified within "VEVENT", "VTODO", or "VJOURNAL" calendar components.
            /// </summary>
            CATEGORIES,

            /// <summary>
            /// 3.8.1.3 Value Type: TEXT
            /// This property defines the access classification for a calendar component.
            /// The property can be specified once in a "VEVENT", "VTODO", or "VJOURNAL" calendar components.
            /// </summary>
            CLASS,

            /// <summary>
            /// 3.8.1.4 Value Type: TEXT
            /// This property specifies non-processing information intended to provide a comment to the calendar user.
            /// This property can be specified multiple times in "VEVENT", "VTODO", "VJOURNAL", and "VFREEBUSY" calendar components
            /// as well as in the "STANDARD" and "DAYLIGHT" sub-components.
            /// </summary>
            COMMENT,

            /// <summary>
            /// 3.8.1.5 Value Type: TEXT
            /// This property provides a more complete description of the calendar component than that provided by the "SUMMARY" property.
            /// The property can be specified in the "VEVENT", "VTODO", "VJOURNAL", or "VALARM" calendar components.
            /// The property can be specified multiple times only within a "VJOURNAL" calendar component.
            /// </summary>
            DESCRIPTION,

            /// <summary>
            /// 3.8.1.6 Value Type: FLOAT
            /// This property specifies information related to the global position for the activity specified by a calendar component.
            /// This property can be specified in "VEVENT" or "VTODO" calendar components.
            /// </summary>
            GEO,

            /// <summary>
            /// 3.8.1.7 Value Type: TEXT
            /// This property defines the intended venue for the activity defined by a calendar component.
            /// This property can be specified in "VEVENT" or "VTODO" calendar component.
            /// </summary>
            LOCATION,

            /// <summary>
            /// 3.8.1.8 Value Type: INTEGER
            /// This property is used by an assignee or delegatee of a to-do to convey the percent completion of a to-do to the "Organizer".
            /// This property can be specified once in a "VTODO" calendar component.
            /// </summary>
            PERCENT_COMPLETE,

            /// <summary>
            /// 3.8.1.9 Value Type: INTEGER
            /// This property defines the relative priority for a calendar component.
            /// This property can be specified in "VEVENT" and "VTODO" calendar components.
            /// </summary>
            PRIORITY,

            /// <summary>
            /// 3.8.1.10 Value Type: TEXT
            /// This property defines the equipment or resources anticipated for an activity specified by a calendar component.
            /// This property can be specified once in "VEVENT" or "VTODO" calendar component.
            /// </summary>
            RESOURCES,

            /// <summary>
            /// 3.8.1.11 Value Type: TEXT
            /// This property defines the overall status or confirmation for the calendar component.
            /// This property can be specified once in "VEVENT", "VTODO", or "VJOURNAL" calendar components.
            /// </summary>
            STATUS,

            /// <summary>
            /// 3.8.1.12 Value Type: TEXT
            /// This property defines a short summary or subject for the calendar component.
            /// The property can be specified in "VEVENT", "VTODO", "VJOURNAL", or "VALARM" calendar components.
            /// </summary>
            SUMMARY,

            /// <summary>
            /// 3.8.2.1 Value Type: DATE-TIME
            /// This property defines the date and time that a to-do was actually completed.
            /// The property can be specified in a "VTODO" calendar component.  The value MUST be specified as a date with UTC time.
            /// </summary>
            COMPLETED,

            /// <summary>
            /// 3.8.2.2 Value Type: DATE-TIME
            /// This property specifies the date and time that a calendar component ends.
            /// This property can be specified in "VEVENT" or "VFREEBUSY" calendar components.
            /// </summary>
            DTEND,

            /// <summary>
            /// 3.8.2.3 Value Type: DATE-TIME
            /// This property defines the date and time that a to-do is expected to be completed.
            /// The property can be specified once in a "VTODO" calendar component.
            /// </summary>
            DUE,

            /// <summary>
            /// 3.8.2.4 Value Type: DATE-TIME
            /// This property specifies when the calendar component begins.
            /// This property can be specified once in the "VEVENT", "VTODO", or "VFREEBUSY" calendar components as well as in the
            /// </summary>
            DTSTART,

            /// <summary>
            /// 3.8.2.5 Value Type: DURATION
            /// This property specifies a positive duration of time.
            /// This property can be specified in "VEVENT", "VTODO", or "VALARM" calendar components.
            /// </summary>
            DURATION,

            /// <summary>
            /// 3.8.2.6 Value Type: PERIOD
            /// This property defines one or more free or busy time intervals.
            /// The property can be specified in a "VFREEBUSY" calendar component.
            /// </summary>
            FREEBUSY,

            /// <summary>
            /// 3.8.2.7 Value Type: TEXT
            /// This property defines whether or not an event is transparent to busy time searches.
            /// This property can be specified once in a "VEVENT" calendar component.
            /// </summary>
            TRANSP,

            /// <summary>
            /// 3.8.3.1 Value Type: TEXT
            /// This property specifies the text value that uniquely identifies the "VTIMEZONE"
            /// calendar component in the scope of an iCalendar object.
            /// This property MUST be specified in a "VTIMEZONE" calendar component.
            /// </summary>
            TZID,

            /// <summary>
            /// 3.8.3.2 Value Type: TEXT
            /// This property specifies the customary designation for a time zone description.
            /// This property can be specified in "STANDARD" and "DAYLIGHT" sub-components.
            /// </summary>
            TZNAME,

            /// <summary>
            /// 3.8.3.3 Value Type: UTC-OFFSET
            /// This property specifies the offset that is in use prior to this time zone observance.
            /// This property MUST be specified in "STANDARD" and "DAYLIGHT" sub-components.
            /// </summary>
            TZOFFSETFROM,

            /// <summary>
            /// 3.8.3.4 Value Type: UTC-OFFSET
            /// This property specifies the offset that is in use in this time zone observance.
            /// This property MUST be specified in "STANDARD" and "DAYLIGHT" sub-components.
            /// </summary>
            TZOFFSETTO,

            /// <summary>
            /// 3.8.3.5 Time Zone URL Value Type: UTC-OFFSET
            /// This property provides a means for a "VTIMEZONE" component to point to a network
            /// location that can be used to retrieve an up-to-date version of itself.
            /// This property can be specified in a "VTIMEZONE" calendar component.
            /// </summary>
            TZURL,

            /// <summary>
            /// 3.8.4.1 Value Type: CAL-ADDRESS
            /// This property defines an "Attendee" within a calendar component.
            /// This property MUST be specified in an iCalendar object that specifies a group-scheduled calendar entity.
            /// This property MUST NOT be specified in an iCalendar object when publishing the calendar information.
            /// This property is not specified in an iCalendar object that specifies only a time zone definition
            /// or that defines calendar components that are not group-scheduled components,
            /// but are components only on a single user's calendar.
            /// </summary>
            ATTENDEE,

            /// <summary>
            /// 3.8.4.2 Value Type: TEXT
            /// This property is used to represent contact information or alternately a reference to contact information
            /// associated with the calendar component.
            /// This property can be specified in a "VEVENT", "VTODO", "VJOURNAL", or "VFREEBUSY" calendar component.
            /// </summary>
            CONTACT,

            /// <summary>
            /// 3.8.4.3 Value Type: CAL-ADDRESS
            /// This property defines the organizer for a calendar component.
            /// This property MUST be specified in an iCalendar object that specifies a group-scheduled calendar entity.
            /// This property MUST be specified in an iCalendar object that specifies the publication of a calendar user's busy time.
            /// This property MUST NOT be specified in an iCalendar object that specifies only a time zone definition or that defines
            /// calendar components that are not group-scheduled components, but are components only on a single user's calendar.
            /// </summary>
            ORGANIZER,

            /// <summary>
            /// 3.8.4.4 Value Type: DATE-TIME
            /// This property is used in conjunction with the "UID" and "SEQUENCE" properties to identify a specific instance of a
            /// recurring "VEVENT", "VTODO", or "VJOURNAL" calendar component.
            /// The property value is the original value of the "DTSTART" property of the recurrence instance.
            /// This property can be specified in an iCalendar object containing a recurring calendar component.
            /// </summary>
            RECURRENCE_ID,

            /// <summary>
            /// 3.8.4.5 Value Type: DATE-TIME
            /// This property is used to represent a relationship or reference between one calendar component and another.
            /// This property can be specified in the "VEVENT", "VTODO", and "VJOURNAL" calendar components.
            /// </summary>
            RELATED_TO,

            /// <summary>
            /// 3.8.4.6 Uniform Resource Locator Value Type: URI
            /// This property defines a Uniform Resource Locator (URL) associated with the iCalendar object.
            /// This property can be specified once in the "VEVENT", "VTODO", "VJOURNAL", or "VFREEBUSY" calendar components.
            /// </summary>
            URL,

            /// <summary>
            /// 3.8.4.7 Unique Identifier Value Type: TEXT
            /// This property defines the persistent, globally unique identifier for the calendar component.
            /// The property MUST be specified in the "VEVENT", "VTODO", "VJOURNAL", or "VFREEBUSY" calendar components.
            /// </summary>
            UID,

            /// <summary>
            /// 3.8.5.1 Exception Date-Times Value Type: DATE-TIME
            /// This property defines the list of DATE-TIME exceptions for recurring events, to-dos, journal entries, or time zone definitions.
            /// This property can be specified in recurring "VEVENT", "VTODO", and "VJOURNAL" calendar components
            /// as well as in the "STANDARD" and "DAYLIGHT" sub-components of the "VTIMEZONE" calendar component.
            /// </summary>
            EXDATE,

            /// <summary>
            /// 3.8.5.2 Recurrence Date-Times Value Type: DATE-TIME
            /// This property defines the list of DATE-TIME values for recurring events, to-dos, journal entries, or time zone definitions.
            /// This property can be specified in recurring "VEVENT", "VTODO", and "VJOURNAL" calendar components
            /// as well as in the "STANDARD" and "DAYLIGHT" sub-components of the "VTIMEZONE" calendar component.
            /// </summary>
            RDATE,

            /// <summary>
            /// 3.8.5.2 Recurrence Rule Value Type: RECUR
            /// This property defines a rule or repeating pattern for recurring events, to-dos, journal entries, or time zone definitions.
            /// This property can be specified in recurring "VEVENT", "VTODO", and "VJOURNAL" calendar components
            /// as well as in the "STANDARD" and "DAYLIGHT" sub-components of the "VTIMEZONE" calendar component,
            /// but it SHOULD NOT be specified more than once. The recurrence set generated with multiple "RRULE" properties is undefined.
            /// </summary>
            RRULE,

            /// <summary>
            /// 3.8.6.1 Value Type: TEXT
            /// This property defines the action to be invoked when an alarm is triggered.
            /// Can be AUDIO, DISPLAY, EMAIL
            /// AUDIO: Raises sound found in the Attach property
            /// DISPLAY: displays text from Description property
            /// EMAIL: sends email to one or more Attendee properties, Summary as subject, Description as body, Attach as attachments
            /// This property MUST be specified once in a "VALARM" calendar component.
            /// </summary>
            ACTION,

            /// <summary>
            /// 3.8.6.2 Value Type: INTEGER
            /// This property defines the number of times the alarm should be repeated, after the initial trigger.
            /// This property can be specified in a "VALARM" calendar component.
            /// </summary>
            REPEAT,

            /// <summary>
            /// 3.8.6.3 Value Type: DURATION or DATE-TIME
            /// This property specifies when an alarm will trigger.
            /// This property MUST be specified in a "VALARM" calendar component.
            /// </summary>
            TRIGGER,

            /// <summary>
            /// 3.8.7.1 Value Type: DATE-TIME
            /// This property specifies the date and time that the calendar information was created
            /// by the calendar user agent in the calendar store.
            /// The property can be specified once in "VEVENT", "VTODO", or "VJOURNAL" calendar components.
            /// The value MUST be specified as a date with UTC time.
            /// </summary>
            CREATED,

            /// <summary>
            /// 3.8.7.2 Value Type: DATE-TIME
            /// In the case of an iCalendar object that specifies a "METHOD" property,
            /// this property specifies the date and time that the instance of the iCalendar object was created.
            /// In the case of an iCalendar object that doesn't specify a "METHOD" property,
            /// this property specifies the date and time that the information associated with the calendar component was last revised in the calendar store.
            /// This property MUST be included in the "VEVENT", "VTODO", "VJOURNAL", or "VFREEBUSY" calendar components.
            /// </summary>
            DTSTAMP,

            /// <summary>
            /// 3.8.7.3 Value Type: DATE-TIME
            /// This property specifies the date and time that the information associated with the calendar component was last revised in the calendar store.
            /// This property can be specified in the "VEVENT", "VTODO", "VJOURNAL", or "VTIMEZONE" calendar components.
            /// </summary>
            LAST_MODIFIED,

            /// <summary>
            /// 3.8.7.4 Sequence Number Value Type: INTEGER
            /// This property defines the revision sequence number of the calendar component within a sequence of revisions.
            /// The property can be specified in "VEVENT", "VTODO", or "VJOURNAL" calendar component.
            /// </summary>
            SEQUENCE,

            /// <summary>
            /// 3.8.8.3 Value Type: TEXT
            /// This property defines the status code returned for a scheduling request.
            /// The property can be specified in the "VEVENT", "VTODO", "VJOURNAL", or "VFREEBUSY" calendar component.
            /// </summary>
            REQUEST_STATUS,

            X_ALT_DESC,
            X_MICROSOFT_CDO_ALLDAYEVENT,
            X_MICROSOFT_CDO_APPT_SEQUENCE,
            X_MICROSOFT_CDO_ATTENDEE_CRITICAL_CHANGE,
            X_MICROSOFT_CDO_BUSYSTATUS,
            X_MICROSOFT_CDO_IMPORTANCE,
            X_MICROSOFT_CDO_INSTTYPE,
            X_MICROSOFT_CDO_INTENDEDSTATUS,
            X_MICROSOFT_CDO_OWNERAPPTID,
            X_MICROSOFT_CDO_OWNER_CRITICAL_CHANGE,
            X_MICROSOFT_CDO_REPLYTIME,
            X_MICROSOFT_DISALLOW_COUNTER,
            X_MICROSOFT_EXDATE,
            X_MICROSOFT_ISDRAFT,
            X_MICROSOFT_MSNCALENDAR_ALLDAYEVENT,
            X_MICROSOFT_MSNCALENDAR_BUSYSTATUS,
            X_MICROSOFT_MSNCALENDAR_IMPORTANCE,
            X_MICROSOFT_MSNCALENDAR_INTENDEDSTATUS,
            X_MICROSOFT_RRULE,
            X_MS_OLK_ALLOWEXTERNCHECK,
            X_MS_OLK_APPTLASTSEQUENCE,
            X_MS_OLK_APPTSEQTIME,
            X_MS_OLK_AUTOFILLLOCATION,
            X_MS_OLK_AUTOSTARTCHECK,
            X_MS_OLK_COLLABORATEDOC,
            X_MS_OLK_CONFCHECK,
            X_MS_OLK_CONFTYPE,
            X_MS_OLK_DIRECTORY,
            X_MS_OLK_MWSURL,
            X_MS_OLK_NETSHOWURL,
            X_MS_OLK_ONLINEPASSWORD,
            X_MS_OLK_ORGALIAS,
            X_MS_OLK_SENDER,
            BUSYTYPE,
            NAME,
            REFRESH_INTERVAL,
            SOURCE,
            COLOR,
            IMAGE,
            CONFERENCE,
            CALENDAR_ADDRESS,
            LOCATION_TYPE,
            PARTICIPANT_TYPE,
            RESOURCE_TYPE,
            STRUCTURED_DATA,
            STYLED_DESCRIPTION,
            ACKNOWLEDGED,
            PROXIMITY,
            CONCEPT,
            LINK,
            REFID,
            SYNCTOKEN,
            ETAG,
            CATEGORY,
            X_APPLE_STRUCTURED_LOCATION
        };

        public static readonly string[] ICalProperties = new string[] {
            "CALSCALE",
            "METHOD",
            "PRODID",
            "VERSION",
            "X-CALEND",
            "X-CALSTART",
            "X-CLIPEND",
            "X-CLIPSTART",
            "X-MICROSOFT-CALSCALE",
            "X-MS-OLK-FORCEINSPECTOROPEN",
            "X-MS-WKHRDAYS",
            "X-MS-WKHREND",
            "X-MS-WKHRSTART",
            "X-OWNER",
            "X-PRIMARY-CALENDAR",
            "X-PUBLISHED-TTL",
            "X-WR-CALDESC",
            "X-WR-CALNAME",
            "X-WR-RELCALID",
            "ATTACH",
            "CATEGORIES",
            "CLASS",
            "COMMENT",
            "DESCRIPTION",
            "GEO",
            "LOCATION",
            "PERCENT-COMPLETE",
            "PRIORITY",
            "RESOURCES",
            "STATUS",
            "SUMMARY",
            "COMPLETED",
            "DTEND",
            "DUE",
            "DTSTART",
            "DURATION",
            "FREEBUSY",
            "TRANSP",
            "TZID",
            "TZNAME",
            "TZOFFSETFROM",
            "TZOFFSETTO",
            "TZURL",
            "ATTENDEE",
            "CONTACT",
            "ORGANIZER",
            "RECURRENCE-ID",
            "RELATED-TO",
            "URL",
            "UID",
            "EXDATE",
            "RDATE",
            "RRULE",
            "ACTION",
            "REPEAT",
            "TRIGGER",
            "CREATED",
            "DTSTAMP",
            "LAST-MODIFIED",
            "SEQUENCE",
            "REQUEST-STATUS",
            "X-ALT-DESC",
            "X-MICROSOFT-CDO-ALLDAYEVENT",
            "X-MICROSOFT-CDO-APPT-SEQUENCE",
            "X-MICROSOFT-CDO-ATTENDEE-CRITICAL-CHANGE",
            "X-MICROSOFT-CDO-BUSYSTATUS",
            "X-MICROSOFT-CDO-IMPORTANCE",
            "X-MICROSOFT-CDO-INSTTYPE",
            "X-MICROSOFT-CDO-INTENDEDSTATUS",
            "X-MICROSOFT-CDO-OWNERAPPTID",
            "X-MICROSOFT-CDO-OWNER-CRITICAL-CHANGE",
            "X-MICROSOFT-CDO-REPLYTIME",
            "X-MICROSOFT-DISALLOW-COUNTER",
            "X-MICROSOFT-EXDATE",
            "X-MICROSOFT-ISDRAFT",
            "X-MICROSOFT-MSNCALENDAR-ALLDAYEVENT",
            "X-MICROSOFT-MSNCALENDAR-BUSYSTATUS",
            "X-MICROSOFT-MSNCALENDAR-IMPORTANCE",
            "X-MICROSOFT-MSNCALENDAR-INTENDEDSTATUS",
            "X-MICROSOFT-RRULE",
            "X-MS-OLK-ALLOWEXTERNCHECK",
            "X-MS-OLK-APPTLASTSEQUENCE",
            "X-MS-OLK-APPTSEQTIME",
            "X-MS-OLK-AUTOFILLLOCATION",
            "X-MS-OLK-AUTOSTARTCHECK",
            "X-MS-OLK-COLLABORATEDOC",
            "X-MS-OLK-CONFCHECK",
            "X-MS-OLK-CONFTYPE",
            "X-MS-OLK-DIRECTORY",
            "X-MS-OLK-MWSURL",
            "X-MS-OLK-NETSHOWURL",
            "X-MS-OLK-ONLINEPASSWORD",
            "X-MS-OLK-ORGALIAS",
            "X-MS-OLK-SENDER",
            "BUSYTYPE",
            "NAME",
            "REFRESH-INTERVAL",
            "SOURCE",
            "COLOR",
            "IMAGE",
            "CONFERENCE",
            "CALENDAR-ADDRESS",
            "LOCATION-TYPE",
            "PARTICIPANT-TYPE",
            "RESOURCE-TYPE",
            "STRUCTURED-DATA",
            "STYLED-DESCRIPTION",
            "ACKNOWLEDGED",
            "PROXIMITY",
            "CONCEPT",
            "LINK",
            "REFID",
            "SYNCTOKEN",
            "ETAG",
            "CATEGORY"
        };
    }
}