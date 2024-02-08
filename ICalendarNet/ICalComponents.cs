namespace ICalendarNet
{
    public enum ICalComponent
    {
        /// <summary>
        ///        icalbody   = calprops component
        ///
        ///calprops   = *(
        ///          ;
        ///          ; The following are REQUIRED,
        ///          ; but MUST NOT occur more than once.
        ///          ;
        ///prodid / version /
        ///          ;
        ///          ; The following are OPTIONAL,
        ///          ; but MUST NOT occur more than once.
        ///          ;
        ///calscale / method /
        ///          ;
        ///          ; The following are OPTIONAL,
        ///          ; and MAY occur more than once.
        ///          ;
        ///x-prop / iana-prop
        ///          ;
        ///          )
        ///
        ///component  = 1*(eventc / todoc / journalc / freebusyc /
        ///timezonec / iana-comp / x-comp)
        ///
        ///iana-comp  = "BEGIN" ":" iana-token CRLF
        ///            1*contentline
        ///            "END" ":" iana-token CRLF
        ///
        ///x-comp     = "BEGIN" ":" x-name CRLF
        ///            1*contentline
        ///            "END" ":" x-name CRLF
        /// </summary>
        VCALENDAR,

        /// <summary>
        /// eventc     = "BEGIN" ":" "VEVENT" CRLF
        /// eventprop* alarmc
        ///             "END" ":" "VEVENT" CRLF
        ///eventprop = *(
        ///           ;
        ///           ; The following are REQUIRED,
        ///           ; but MUST NOT occur more than once.
        ///           ;
        /// dtstamp / uid /
        ///           ;
        ///           ; The following is REQUIRED if the component
        ///           ; appears in an iCalendar object that doesn't
        ///           ; specify the "METHOD" property; otherwise, it
        ///           ; is OPTIONAL; in any case, it MUST NOT occur
        ///           ; more than once.
        ///           ;
        ///           dtstart /
        ///           ;
        ///           ; The following are OPTIONAL,
        ///           ; but MUST NOT occur more than once.
        ///           ;
        ///           class / created / description / geo /
        ///           last-mod / location / organizer / priority /
        ///           seq / status / summary / transp /
        ///           url / recurid /
        ///           ;
        ///           ; The following is OPTIONAL,
        ///           ; but SHOULD NOT occur more than once.
        ///           ;
        ///           rrule /
        ///           ;
        ///           ; Either 'dtend' or 'duration' MAY appear in
        ///           ; a 'eventprop', but 'dtend' and 'duration'
        ///           ; MUST NOT occur in the same 'eventprop'.
        ///           ;
        /// dtend / duration /
        ///           ;
        ///           ; The following are OPTIONAL,
        ///           ; and MAY occur more than once.
        ///           ;
        /// attach / attendee / categories / comment /
        ///           contact / exdate / rstatus / related /
        ///           resources / rdate / x-prop / iana-prop
        ///           ;
        ///         )
        /// </summary>
        VEVENT,

        /// <summary>
        /// todoc      = "BEGIN" ":" "VTODO" CRLF
        /// todoprop* alarmc
        ///             "END" ":" "VTODO" CRLF
        ///todoprop = *(
        ///           ;
        ///           ; The following are REQUIRED,
        ///           ; but MUST NOT occur more than once.
        ///           ;
        ///           dtstamp / uid /
        ///           ;
        ///           ; The following are OPTIONAL,
        ///           ; but MUST NOT occur more than once.
        ///           ;
        ///           class / completed / created / description /
        ///           dtstart / geo / last-mod / location / organizer /
        ///           percent / priority / recurid / seq / status /
        ///           summary / url /
        ///           ;
        ///           ; The following is OPTIONAL,
        ///           ; but SHOULD NOT occur more than once.
        ///           ;
        ///           rrule /
        ///           ;
        ///           ; Either 'due' or 'duration' MAY appear in
        ///           ; a 'todoprop', but 'due' and 'duration'
        ///           ; MUST NOT occur in the same 'todoprop'.
        ///           ; If 'duration' appear in a 'todoprop',
        ///           ; then 'dtstart' MUST also appear in
        ///           ; the same 'todoprop'.
        ///           ;
        /// due / duration /
        ///           ;
        ///           ; The following are OPTIONAL,
        ///           ; and MAY occur more than once.
        ///           ;
        /// attach / attendee / categories / comment / contact /
        ///           exdate / rstatus / related / resources /
        ///           rdate / x-prop / iana-prop
        ///           ;
        ///           )
        /// </summary>
        VTODO,

        /// <summary>
        /// journalc   = "BEGIN" ":" "VJOURNAL" CRLF
        ///     jourprop
        ///             "END" ":" "VJOURNAL" CRLF
        ///jourprop = *(
        ///           ;
        ///           ; The following are REQUIRED,
        ///           ; but MUST NOT occur more than once.
        ///           ;
        ///     dtstamp / uid /
        ///           ;
        ///           ; The following are OPTIONAL,
        ///           ; but MUST NOT occur more than once.
        ///           ;
        ///           class / created / dtstart /
        ///           last-mod / organizer / recurid / seq /
        ///           status / summary / url /
        ///           ;
        ///           ; The following is OPTIONAL,
        ///           ; but SHOULD NOT occur more than once.
        ///           ;
        ///           rrule /
        ///           ;
        ///           ; The following are OPTIONAL,
        ///           ; and MAY occur more than once.
        ///           ;
        /// attach / attendee / categories / comment /
        ///           contact / description / exdate / related / rdate /
        ///           rstatus / x-prop / iana-prop
        ///           ;
        ///           )
        /// </summary>
        VJOURNAL,

        /// <summary>
        /// freebusyc  = "BEGIN" ":" "VFREEBUSY" CRLF
        ///     fbprop
        ///             "END" ":" "VFREEBUSY" CRLF
        ///fbprop = *(
        ///           ;
        ///           ; The following are REQUIRED,
        ///           ; but MUST NOT occur more than once.
        ///           ;
        ///     dtstamp / uid /
        ///           ;
        ///           ; The following are OPTIONAL,
        ///           ; but MUST NOT occur more than once.
        ///           ;
        ///     contact / dtstart / dtend /
        ///           organizer / url /
        ///           ;
        ///           ; The following are OPTIONAL,
        ///           ; and MAY occur more than once.
        ///           ;
        ///     attendee / comment / freebusy / rstatus / x-prop /
        ///           iana-prop
        ///           ;
        ///           )
        /// </summary>
        VFREEBUSY,

        /// <summary>
        /// timezonec  = "BEGIN" ":" "VTIMEZONE" CRLF
        ///             *(
        ///             ;
        ///             ; 'tzid' is REQUIRED, but MUST NOT occur more
        ///             ; than once.
        ///             ;
        /// tzid /
        ///             ;
        ///             ; 'last-mod' and 'tzurl' are OPTIONAL,
        ///             ; but MUST NOT occur more than once.
        ///             ;
        /// last-mod / tzurl /
        ///             ;
        ///             ; One of 'standardc' or 'daylightc' MUST occur
        ///             ; and each MAY occur more than once.
        ///             ;
        /// standardc / daylightc /
        ///             ;
        ///             ; The following are OPTIONAL,
        ///             ; and MAY occur more than once.
        ///             ;
        /// x-prop / iana-prop
        ///             ;
        ///             )
        ///             "END" ":" "VTIMEZONE" CRLF
        ///standardc = "BEGIN" ":" "STANDARD" CRLF
        ///             tzprop
        ///             "END" ":" "STANDARD" CRLF
        ///daylightc = "BEGIN" ":" "DAYLIGHT" CRLF
        ///             tzprop
        ///             "END" ":" "DAYLIGHT" CRLF
        ///tzprop = *(
        ///             ;
        ///             ; The following are REQUIRED,
        ///             ; but MUST NOT occur more than once.
        ///             ;
        /// dtstart / tzoffsetto / tzoffsetfrom /
        ///             ;
        ///             ; The following is OPTIONAL,
        ///             ; but SHOULD NOT occur more than once.
        ///             ;
        /// rrule /
        ///             ;
        ///             ; The following are OPTIONAL,
        ///             ; and MAY occur more than once.
        ///             ;
        /// comment / rdate / tzname / x-prop / iana-prop
        ///             ;
        ///             )
        /// </summary>
        VTIMEZONE,


        STANDARD,

        DAYLIGHT,

        /// <summary>
        /// alarmc     = "BEGIN" ":" "VALARM" CRLF
        ///             (audioprop / dispprop / emailprop)
        ///             "END" ":" "VALARM" CRLF
        ///audioprop = *(
        ///           ;
        ///           ; 'action' and 'trigger' are both REQUIRED,
        ///           ; but MUST NOT occur more than once.
        ///           ;
        ///           action / trigger /
        ///           ;
        ///           ; 'duration' and 'repeat' are both OPTIONAL,
        ///           ; and MUST NOT occur more than once each;
        ///           ; but if one occurs, so MUST the other.
        ///           ;
        ///           duration / repeat /
        ///           ;
        ///           ; The following is OPTIONAL,
        ///           ; but MUST NOT occur more than once.
        ///           ;
        ///           attach /
        ///           ;
        ///           ; The following is OPTIONAL,
        ///           ; and MAY occur more than once.
        ///           ;
        /// x-prop / iana-prop
        ///           ;
        ///           )
        ///dispprop   = *(
        ///           ;
        ///           ; The following are REQUIRED,
        ///           ; but MUST NOT occur more than once.
        ///           ;
        ///           action / description / trigger /
        ///           ;
        ///           ; 'duration' and 'repeat' are both OPTIONAL,
        ///           ; and MUST NOT occur more than once each;
        ///           ; but if one occurs, so MUST the other.
        ///           ;
        ///           duration / repeat /
        ///           ;
        ///           ; The following is OPTIONAL,
        ///           ; and MAY occur more than once.
        ///           ;
        /// x-prop / iana-prop
        ///           ;
        ///           )
        ///emailprop  = *(
        ///           ;
        ///           ; The following are all REQUIRED,
        ///           ; but MUST NOT occur more than once.
        ///           ;
        ///           action / description / trigger / summary /
        ///           ;
        ///           ; The following is REQUIRED,
        ///           ; and MAY occur more than once.
        ///           ;
        /// attendee /
        ///           ;
        ///           ; 'duration' and 'repeat' are both OPTIONAL,
        ///           ; and MUST NOT occur more than once each;
        ///           ; but if one occurs, so MUST the other.
        ///           ;
        ///           duration / repeat /
        ///           ;
        ///           ; The following are OPTIONAL,
        ///           ; and MAY occur more than once.
        ///           ;
        /// attach / x-prop / iana-prop
        ///           ;
        ///           )
        /// </summary>
        VALARM
    };
}
