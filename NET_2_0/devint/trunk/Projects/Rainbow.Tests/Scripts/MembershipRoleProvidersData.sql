
-- another user, for Role tests
INSERT INTO dbo.aspnet_Users 
VALUES (N'{CA8DEE6B-0B81-4462-AF3B-6A22B36A0304}', N'{34ADB714-92B0-47ff-B5AF-5DB2E0D124A9}', N'user@user.com', N'user@user.com', NULL, 0, '2006-04-05T18:12:37.140')
go
INSERT INTO dbo.aspnet_Membership(ApplicationId, UserId, Password, PasswordFormat, PasswordSalt, MobilePIN, Email, LoweredEmail, PasswordQuestion, PasswordAnswer, IsApproved, IsLockedOut, CreateDate, LastLoginDate, LastPasswordChangedDate, LastLockoutDate, FailedPasswordAttemptCount, FailedPasswordAttemptWindowStart, FailedPasswordAnswerAttemptCount, FailedPasswordAnswerAttemptWindowStart) 
VALUES (N'{CA8DEE6B-0B81-4462-AF3B-6A22B36A0304}', N'{34ADB714-92B0-47ff-B5AF-5DB2E0D124A9}', N'admin', 0, N'', NULL, N'user@user.com', N'user@user.com', N'question', N'answer', 1, 0, '2006-03-23T13:46:25.000', '2006-03-23T21:46:58.203', '2006-03-23T13:46:25.000', '1754-01-01T00:00:00.000', 1, '2006-04-05T18:12:37.110', 0, '1754-01-01T00:00:00.000')
go
