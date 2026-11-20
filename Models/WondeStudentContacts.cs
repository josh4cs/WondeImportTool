namespace WondeModels
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public partial class WondeStudent_Contacts
    {
        [JsonProperty("data")]
        public List<WondeStudent_Contact> data { get; set; }
    }

    public partial class WondeStudent_Contact
    {
        [JsonProperty("id")]
        public string id { get; set; }

        [JsonProperty("mis_id")]
        public string mis_id { get; set; }

        [JsonProperty("upi")]
        public string upi { get; set; }

        [JsonProperty("title")]
        public string title { get; set; }

        [JsonProperty("initials")]
        public string initials { get; set; }

        [JsonProperty("surname")]
        public string surname { get; set; }

        [JsonProperty("forename")]
        public string forename { get; set; }

        [JsonProperty("middle_names")]
        public string middle_names { get; set; }

        [JsonProperty("legal_surname")]
        public string legal_surname { get; set; }

        [JsonProperty("legal_forename")]
        public string legal_forename { get; set; }

        [JsonProperty("gender")]
        public string gender { get; set; }

        [JsonProperty("date_of_birth")]
        public WondeStudent_CreatedAt date_of_birth { get; set; }

        [JsonProperty("created_at")]
        public WondeStudent_CreatedAt created_at { get; set; }

        [JsonProperty("updated_at")]
        public WondeStudent_CreatedAt updated_at { get; set; }

        [JsonProperty("relationship")]
        public WondeStudent_ContactRelationship relationship { get; set; }

        [JsonProperty("contact_details")]
        public WondeStudent_ContactDetails contact_details { get; set; }
    }

    public partial class WondeStudent_ContactRelationship
    {
        [JsonProperty("bill_payer")]
        public bool? bill_payer { get; set; }
        [JsonProperty("lives_with_pupil")]
        public bool? lives_with_pupil { get; set; }
        [JsonProperty("email_bills")]
        public bool? email_bills { get; set; }
        [JsonProperty("copy_bills")]
        public bool? copy_bills { get; set; }
        [JsonProperty("court_order")]
        public bool? court_order { get; set; }
        [JsonProperty("pupil_report")]
        public bool? pupil_report { get; set; }
        [JsonProperty("parental_responsibility")]
        public bool? parental_responsibility { get; set; }
        [JsonProperty("correspondence")]
        public bool? correspondence { get; set; }
        [JsonProperty("email_notification")]
        public bool? email_notification { get; set; }
        [JsonProperty("sms_notification")]
        public bool? sms_notification { get; set; }
        [JsonProperty("mail_notification")]
        public bool? mail_notification { get; set; }
        [JsonProperty("push_notification")]
        public bool? push_notification { get; set; }
        [JsonProperty("in_touch_communication")]
        public bool? in_touch_communication { get; set; }
        [JsonProperty("priority")]
        public int? priority { get; set; }
        [JsonProperty("relationship")]
        public string relationship { get; set; }
        [JsonProperty("emergency_contact")]
        public bool? emergency_contact { get; set; }
        [JsonProperty("portal_access")]
        public bool? portal_access { get; set; }
        [JsonProperty("attendance_notification")]
        public bool? attendance_notification { get; set; }
        [JsonProperty("mail_merge")]
        public bool? mail_merge { get; set; }
    }

    public partial class WondeStudent_ContactDetails
    {
        [JsonProperty("data")]
        public WondeStudent_ContactDetailsData data { get; set; }
    }

    public partial class WondeStudent_ContactDetailsData
    {
        [JsonProperty("phones")]
        public WondeStudent_ContactPhones phones { get; set; }
        [JsonProperty("emails")]
        public WondeStudent_ContactEmails emails { get; set; }
        [JsonProperty("addresses")]
        public WondeStudent_ContactAddresses addresses { get; set; }
        [JsonProperty("salutation")]
        public string salutation { get; set; }
    }

    public partial class WondeStudent_ContactPhones
    {
        [JsonProperty("phone")]
        public string phone { get; set; }
        [JsonProperty("primary")]
        public string primary { get; set; }
        [JsonProperty("home")]
        public string home { get; set; }
        [JsonProperty("work")]
        public string work { get; set; }
        [JsonProperty("mobile")]
        public string mobile { get; set; }
    }

    public partial class WondeStudent_ContactEmails
    {
        [JsonProperty("email")]
        public string email { get; set; }
        [JsonProperty("primary")]
        public string primary { get; set; }
        [JsonProperty("home")]
        public string home { get; set; }
        [JsonProperty("work")]
        public string work { get; set; }
    }

    public partial class WondeStudent_ContactAddresses
    {
        [JsonProperty("home")]
        public WondeStudent_ContactAddress home { get; set; }
        [JsonProperty("work")]
        public WondeStudent_ContactAddress work { get; set; }
        [JsonProperty("postal")]
        public WondeStudent_ContactAddress postal { get; set; }
    }

    public partial class WondeStudent_ContactAddress
    {
        [JsonProperty("house_number")]
        public string house_number { get; set; }
        [JsonProperty("house_name")]
        public string house_name { get; set; }
        [JsonProperty("apartment")]
        public string apartment { get; set; }
        [JsonProperty("street")]
        public string street { get; set; }
        [JsonProperty("district")]
        public string district { get; set; }
        [JsonProperty("town")]
        public string town { get; set; }
        [JsonProperty("county")]
        public string county { get; set; }
        [JsonProperty("country")]
        public string country { get; set; }
        [JsonProperty("postcode")]
        public string postcode { get; set; }
    }
}
