SET XACT_ABORT ON;
BEGIN TRANSACTION;

DELETE target
FROM dbo.contacts AS target
INNER JOIN
(
	SELECT DISTINCT school_id, ID
	FROM dbo.contacts_insert
	WHERE ID IS NOT NULL
) AS source
	ON target.school_id = source.school_id
	AND target.ID = source.ID;

INSERT INTO dbo.contacts
(
	school_id, ID, mis_id, upi, title, initials, surname, forename, middle_name,
	gender, dob, created_at, data_updated_on, data_imported_on
)
SELECT
	school_id, ID, mis_id, upi, title, initials, surname, forename, middle_name,
	gender, dob, created_at, data_updated_on, data_imported_on
FROM dbo.contacts_insert;

DELETE target
FROM dbo.contact_address AS target
INNER JOIN
(
	SELECT DISTINCT school_id, ID
	FROM dbo.contact_address_insert
	WHERE ID IS NOT NULL
) AS source
	ON target.school_id = source.school_id
	AND target.ID = source.ID;

INSERT INTO dbo.contact_address
(
	school_id, ID, address_type, house_number, house_name, apartment, street,
	district, town, county, country, postcode, data_imported_on
)
SELECT
	school_id, ID, address_type, house_number, house_name, apartment, street,
	district, town, county, country, postcode, data_imported_on
FROM dbo.contact_address_insert;

DELETE target
FROM dbo.contact_email AS target
INNER JOIN
(
	SELECT DISTINCT school_id, ID
	FROM dbo.contact_email_insert
	WHERE ID IS NOT NULL
) AS source
	ON target.school_id = source.school_id
	AND target.ID = source.ID;

INSERT INTO dbo.contact_email
(
	school_id, ID, Email, [primary], home, work, data_imported_on
)
SELECT
	school_id, ID, Email, [primary], home, work, data_imported_on
FROM dbo.contact_email_insert;

DELETE target
FROM dbo.contact_phone AS target
INNER JOIN
(
	SELECT DISTINCT school_id, ID
	FROM dbo.contact_phone_insert
	WHERE ID IS NOT NULL
) AS source
	ON target.school_id = source.school_id
	AND target.ID = source.ID;

INSERT INTO dbo.contact_phone
(
	school_id, ID, Phone, [primary], home, work, mobile, data_imported_on
)
SELECT
	school_id, ID, Phone, [primary], home, work, mobile, data_imported_on
FROM dbo.contact_phone_insert;

DELETE target
FROM dbo.contact_relationship AS target
INNER JOIN
(
	SELECT DISTINCT school_id, Contact_ID, Student_ID
	FROM dbo.contact_relationship_insert
) AS source
	ON target.school_id = source.school_id
	AND target.Contact_ID = source.Contact_ID
	AND target.Student_ID = source.Student_ID;

INSERT INTO dbo.contact_relationship
(
	school_id, Contact_ID, Student_ID, bill_payer, lives_with_pupil, email_bills,
	copy_bills, court_order, pupil_report, parental_responsibility, correspondence,
	email_notification, sms_notification, mail_notification, push_notification,
	in_touch_communication, priority, relationship, emergency_contact, portal_access,
	attendance_notification, mail_merge, created_at, data_updated_on, data_imported_on
)
SELECT
	school_id, Contact_ID, Student_ID, bill_payer, lives_with_pupil, email_bills,
	copy_bills, court_order, pupil_report, parental_responsibility, correspondence,
	email_notification, sms_notification, mail_notification, push_notification,
	in_touch_communication, priority, relationship, emergency_contact, portal_access,
	attendance_notification, mail_merge, created_at, data_updated_on, data_imported_on
FROM dbo.contact_relationship_insert;

COMMIT TRANSACTION;
