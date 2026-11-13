
CREATE TABLE [dbo].[attendance_codes](
	[w_id] [nvarchar](50) NULL,
	[code] [nvarchar](50) NULL,
	[parent_code] [nvarchar](50) NULL,
	[gov_code] [nvarchar](50) NULL,
	[description] [nvarchar](250) NULL,
	[type] [nvarchar](250) NULL,
	[sub_type] [nvarchar](50) NULL
)
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A708867432', N'7', N'', N'', N'Illness due to Covid 19', N'AUTHORISED', N'Authorised Absence')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A973492091', N'7', N'', N'7', N'Illness due to Covid 19', N'AUTHORISED', N'Authorised Absence')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A541526275', N'8', N'', N'', N'Self-isolating due to Covid 19', N'VOID', N'Attendance not required')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A913933485', N'8', N'', N'8', N'Self-isolating due to Covid 19', N'VOID', N'Attendance not required')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A1114701279', N'9', N'', N'9', N'Shielding due to Covid 19', N'VOID', N'Attendance not required')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A649833114', N'9', N'', N'', N'Shielding due to Covid 19', N'VOID', N'Attendance not required')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A1634008453', N'-', N'', N'-', N'All should attend / No mark recorded', N'VOID', N'Attendance not required')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A1985777634', N'-', N'', N'', N'All should attend / No mark recorded', N'VOID', N'Attendance not required')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A39307028', N'#', N'', N'', N'Planned whole or partial school closure', N'VOID', N'Attendance not required')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A930902628', N'#', N'', N'#', N'Planned whole or partial school closure', N'VOID', N'Attendance not required')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A1329183376', N'/', N'', N'/', N'Present (AM)', N'PRESENT', N'Present')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A508116038', N'/', N'', N'', N'Present (AM)', N'PRESENT', N'Present')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A1297912824', N';', N'', N';', N'Illness due to Covid 19', N'AUTHORISED', N'Authorised Absence')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A1283217476', N'[', N'', N'[', N'Remote learning due to Covid 19', N'VOID', N'Attendance not required')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A1161584171', N'\', N'', N'\', N'Present (PM)', N'PRESENT', N'Present')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A340644305', N'\', N'', N'', N'Present (PM)', N'PRESENT', N'Present')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A1484231030', N']', N'', N']', N'Shielding due to Covid 19', N'VOID', N'Attendance not required')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A813359890', N'=', N'', N'=', N'Illness due to Covid 19', N'AUTHORISED', N'Authorised Absence')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A1019104821', N'B', N'', N'', N'Educated off site', N'PRESENT', N'Approved Educational Activity')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A1529934786', N'B', N'', N'B', N'Educated off site', N'PRESENT', N'Approved Educational Activity')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A1362597725', N'C', N'', N'C', N'Other authorised circumstances', N'AUTHORISED', N'Authorised Absence')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A850584524', N'C', N'', N'', N'Other authorised circumstances', N'AUTHORISED', N'Authorised Absence')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A742433741', N'C1', N'C', N'C1', N'Absent with leave for the purpose of participating in a regulated performance', N'AUTHORISED', N'Authorised Absence')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A951720191', N'C2', N'C', N'C2', N'Absent with leave, of compulsory school age and temporary reduced timetable does not require them to attend', N'AUTHORISED', N'Authorised Absence')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A1219594087', N'D', N'', N'', N'Dual registration', N'VOID', N'Attendance not required')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A1462380788', N'D', N'', N'D', N'Dual registration', N'VOID', N'Attendance not required')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A1320442110', N'E', N'', N'', N'Excluded', N'AUTHORISED', N'Authorised Absence')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A1831783567', N'E', N'', N'E', N'Excluded', N'AUTHORISED', N'Authorised Absence')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A1151925385', N'F', N'', N'', N'Extended family holiday (agreed)', N'AUTHORISED', N'Authorised Absence')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A1663918118', N'F', N'', N'F', N'Extended family holiday (agreed)', N'AUTHORISED', N'Authorised Absence')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A1521455136', N'G', N'', N'', N'Family Holiday (not agreed)', N'UNAUTHORISED', N'Unauthorised Absence')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A2032141745', N'G', N'', N'G', N'Unauthorised holiday', N'UNAUTHORISED', N'Unauthorised Absence')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A1352807867', N'H', N'', N'', N'Annual family holiday (agreed)', N'AUTHORISED', N'Authorised Absence')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A2133121352', N'H', N'', N'H', N'Annual family holiday (agreed)', N'AUTHORISED', N'Authorised Absence')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A2133121352', N'H', N'', N'', N'Annual family holiday (agreed)', N'AUTHORISED', N'Authorised Absence')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A1453771090', N'I', N'', N'', N'Illness', N'AUTHORISED', N'Authorised Absence')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A1964735203', N'I', N'', N'I', N'Illness Not Medical or dental appointment', N'AUTHORISED', N'Authorised Absence')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A828725974', N'I01', N'I', N'I01', N'Illness', N'AUTHORISED', N'Authorised Absence')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A928644705', N'I02', N'I', N'I02', N'Illness- Confirmed case of coronavirus (COVID-19)', N'AUTHORISED', N'Authorised Absence')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A1500139297', N'J', N'', N'J', N'Approved education activity as pupil is attending interview', N'PRESENT', N'Approved Educational Activity')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A1822780141', N'J', N'', N'', N'Interview', N'AUTHORISED', N'Authorised Absence')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A575096676', N'J1', N'', N'J1', N'Absent with leave for the purpose of attending an interview for employment or for admission to another educational institution', N'AUTHORISED', N'Authorised Absence')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A374083122', N'K', N'', N'K', N'Attending a place, other than the school or another school at which they are a registered pupil, for educational provision arranged by LA', N'PRESENT', N'Approved Educational Activity')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A1654132356', N'L', N'', N'', N'Late (before reg closed)', N'PRESENT', N'Late')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A186523258', N'L', N'', N'L', N'Late (before reg closed)', N'PRESENT', N'Late')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A18002453', N'M', N'', N'M', N'Medical / dental appointments', N'AUTHORISED', N'Authorised Absence')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A2023531039', N'M', N'', N'', N'Medical/Dental', N'AUTHORISED', N'Authorised Absence')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A118577068', N'N', N'', N'N', N'No reason yet provided for absence', N'UNAUTHORISED', N'Unauthorised Absence')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A2123581366', N'N', N'', N'', N'No reason yet provided for absence', N'UNAUTHORISED', N'Unauthorised Absence')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A1955994433', N'O', N'', N'', N'Unauthorised Absence', N'UNAUTHORISED', N'Unauthorised Absence')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A487975751', N'O', N'', N'O', N'Unauthorised absence', N'UNAUTHORISED', N'Unauthorised Absence')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A176864472', N'P', N'', N'', N'Approved sporting activity', N'PRESENT', N'Approved Educational Activity')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A319327454', N'P', N'', N'P', N'Approved sporting activity', N'PRESENT', N'Approved Educational Activity')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A1052687510', N'Q', N'', N'Q', N'Unable to attend because of lack of access arrangements by LA to facilitate their attendance', N'VOID', N'Attendance not required')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A688857193', N'R', N'', N'R', N'Religious observance', N'AUTHORISED', N'Authorised Absence')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A8999027', N'R', N'', N'', N'Religious observance', N'AUTHORISED', N'Authorised Absence')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A109966346', N'S', N'', N'', N'Study leave', N'AUTHORISED', N'Authorised Absence')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A788775936', N'S', N'', N'S', N'Study leave', N'AUTHORISED', N'Authorised Absence')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A478185893', N'T', N'', N'', N'Traveller absence', N'AUTHORISED', N'Authorised Absence')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A621189531', N'T', N'', N'T', N'Traveller absence', N'AUTHORISED', N'Authorised Absence')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A310848828', N'U', N'', N'', N'Late (after registers closed)', N'UNAUTHORISED', N'Unauthorised Absence')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A990199090', N'U', N'', N'U', N'Late (after registers closed)', N'UNAUTHORISED', N'Unauthorised Absence')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A687591127', N'V', N'', N'', N'Educational visit', N'PRESENT', N'Approved Educational Activity')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A830070477', N'V', N'', N'V', N'Educational visit or trip', N'PRESENT', N'Approved Educational Activity')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A1601102008', N'W', N'', N'W', N'Approved education activity as pupil is attending work experience', N'PRESENT', N'Approved Educational Activity')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A788423278', N'W', N'', N'', N'Work Experience', N'PRESENT', N'Approved Educational Activity')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A1131916182', N'X', N'X', N'X', N'Non-compulsory school age absence - not counted in possible attendances', N'VOID', N'Attendance not required')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A206775165', N'X', N'X', N'', N'Non-compulsory school age or COVID-19 related', N'VOID', N'Attendance not required')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A124965801', N'X01', N'X', N'X01', N'Non compulsory school age pupil not required to be in school', N'VOID', N'Attendance not required')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A493975360', N'X02', N'X', N'X02', N'Pupil self-isolating with coronavirus (COVID-19) symptoms', N'VOID', N'Attendance not required')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A326371547', N'X03', N'X', N'', N'Pupil self-isolating due to potential contact with a confirmed case of coronavirus (COVID-19) inside the school', N'VOID', N'Attendance not required')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A694725746', N'X04', N'X', N'', N'Pupil self-isolating due to potential contact with a confirmed case of coronavirus (COVID-19) outside the school', N'VOID', N'Attendance not required')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A795820045', N'X05', N'X', N'X05', N'Pupils required to self-isolate as part of quarantine requirement (after arriving in the UK from a non-exempt country or territory)', N'VOID', N'Attendance not required')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A627303844', N'X06', N'X', N'X06', N'Pupil not in school because they have been advised specifically by their Doctor or public health authority that they are clinically extremely vulnerable and should not attend', N'VOID', N'Attendance not required')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A996587839', N'X07', N'X', N'X07', N'Pupil advised specifically not to attend school as part of restrictions to education set out in Government advice', N'VOID', N'Attendance not required')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A323305093', N'X08', N'X', N'X08', N'Pupil not attending school in line with advice from Directors of Public Health as part of outbreak management', N'VOID', N'Attendance not required')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A700043804', N'X09', N'X', N'X09', N'Pupil or student in a specific group required, by NHS test and trace, to self-isolate as a close contact of a confirmed case of COVID-19 (e.g. those over 18 years 6 months and not fully vaccinated)', N'VOID', N'Attendance not required')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A1433240659', N'Y', N'', N'', N'Enforced closure - not counted in possible attendances', N'VOID', N'Attendance not required')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A1801476586', N'Y', N'', N'Y', N'Enforced closure - not counted in possible attendances', N'VOID', N'Attendance not required')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A620562425', N'Y', N'', N'', N'Unable to attend due to exceptional circumstances', N'VOID', N'Attendance not required')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A884822049', N'Y1', N'', N'Y1', N'Unable to attend because school is not within walking distance of pupil''s home and transport to and from school normally provided is not available', N'VOID', N'Attendance not required')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A1253176760', N'Y2', N'', N'Y2', N'Unable to attend due to widespread disruption to travel caused by a local, national, or international emergency', N'VOID', N'Attendance not required')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A1085573459', N'Y3', N'', N'Y3', N'Part of the school premises is unavoidably out of use and pupil cannot be accommodated in parts of the premises that remain in use', N'VOID', N'Attendance not required')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A1185622762', N'Y4', N'', N'Y4', N'Whole school closed when school was due to meet for a session, but session has been cancelled', N'VOID', N'Attendance not required')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A1555025541', N'Y5', N'', N'Y5', N'Unable to attend because pupil is subject to a sentence of detention', N'VOID', N'Attendance not required')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A1386373660', N'Y6', N'', N'Y6', N'Travel to or attendance at school would be contrary/prohibited by guidance/law relating to incidence or transmission of infection or disease', N'VOID', N'Attendance not required')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A1755383735', N'Y7', N'', N'Y7', N'Unable to attend because of any other unavoidable cause', N'VOID', N'Attendance not required')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A1299388415', N'Z', N'', N'Z', N'Pupil not on roll', N'VOID', N'Attendance not required')
GO
INSERT [dbo].[attendance_codes] ([w_id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A407541935', N'Z', N'', N'', N'DfES Z: Pupil not on roll', N'VOID', N'Attendance not required')
GO
