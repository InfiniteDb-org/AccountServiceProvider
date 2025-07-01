using AccountService.Contracts.Requests;

namespace Infrastructure.Messaging;

public static class EmailRequestFactory
{
    public static EmailSendRequest CreateVerificationEmail(string recipient, string code)
    {
        var subject = "InfiniteDb - Verification Code";
        var plainTextContent = $"""
Hi!

Your verification code is: {code}

Enter this code to verify your email address.

If you did not request this code, you can safely ignore this email.

Best regards,
The InfiniteDb Team
""";
        var htmlContent = $@"
<!DOCTYPE html>
<html lang='en'>
<head>
  <meta charset='UTF-8'>
  <title>Verify your email</title>
</head>
<body style='margin:0; padding:32px; font-family: Inter, sans-serif; background-color:#F7F7F7; color:#1E1E20;'>
  <div style='max-width:600px; margin:32px auto; background:#FFFFFF; border-radius:12px; padding:32px;'>
    <h1 style='font-size:32px; font-weight:600; color:#3747D0; text-align:center;'>Verify your email</h1>
    <p>Hi!</p>
    <p>Your verification code is:</p>
    <div style='font-size:2em; letter-spacing:2px; font-weight:bold; color:#234AA6; margin:24px 0 28px 0; text-align:center;'>{code}</div>
    <p>Enter this code to verify your email address.</p>
    <hr style='margin:32px 0; border:none; border-top:1px solid #eee;'/>
    <p style='font-size:0.95em; color:#888;'>If you did not request this code, you can safely ignore this email.</p>
    <div style='margin-top:32px; text-align:center; color:#777; font-size:12px;'>© infinitedb.com. All rights reserved.</div>
  </div>
</body>
</html>";
        return new EmailSendRequest
        {
            Recipients = [recipient],
            Subject = subject,
            PlainText = plainTextContent,
            Html = htmlContent
        };
    }

    public static EmailSendRequest CreateWelcomeEmail(string recipient)
    {
        var subject = "Welcome to InfiniteDb!";
        var plainTextContent =
            """
Hi and welcome to InfiniteDb!

Your account is now activated.

If you have any questions, contact our support at https://infinitedb.com/support

Best regards,
The InfiniteDb Team
""";
        var htmlContent = @"
<!DOCTYPE html>
<html lang='en'>
<head>
  <meta charset='UTF-8'>
  <title>Welcome to InfiniteDb!</title>
</head>
<body style='margin:0; padding:32px; font-family: Inter, sans-serif; background-color:#F7F7F7; color:#1E1E20;'>
  <div style='max-width:600px; margin:32px auto; background:#FFFFFF; border-radius:12px; padding:32px;'>
    <h1 style='font-size:32px; font-weight:600; color:#3747D0; text-align:center;'>Welcome to InfiniteDb!</h1>
    <p>Hi and welcome!</p>
    <p>Your account is now activated.</p>
    <div style='text-align:center; margin:32px 0;'>
      <a href='https://infinitedb.com/support' style='background-color:#234AA6; color:#FFFFFF; padding:12px 24px; border-radius:6px; text-decoration:none; font-weight:bold; font-size:1.1em;'>Contact support</a>
    </div>
    <div style='margin-top:32px; text-align:center; color:#777; font-size:12px;'>© infinitedb.com. All rights reserved.</div>
  </div>
</body>
</html>";
        return new EmailSendRequest
        {
            Recipients = [recipient],
            Subject = subject,
            PlainText = plainTextContent,
            Html = htmlContent
        };
    }

    public static EmailSendRequest CreatePasswordResetEmail(string recipient, string token)
    {
        var subject = "InfiniteDb - Reset Password";
        var plainTextContent =
            $"""
You requested to reset your password for InfiniteDb.

To reset your password, click the link below:
https://infinitedb.com/reset?token={token}

If you did not request a password reset, you can ignore this email.

Best regards,
The InfiniteDb Team
""";
        var htmlContent = $@"
<!DOCTYPE html>
<html lang='en'>
<head>
  <meta charset='UTF-8'>
  <title>Reset password</title>
</head>
<body style='margin:0; padding:32px; font-family: Inter, sans-serif; background-color:#F7F7F7; color:#1E1E20;'>
  <div style='max-width:600px; margin:32px auto; background:#FFFFFF; border-radius:12px; padding:32px;'>
    <h1 style='font-size:32px; font-weight:600; color:#234AA6; text-align:center;'>Reset your password</h1>
    <p>You requested to reset your password for InfiniteDb.</p>
    <div style='text-align:center; margin:32px 0;'>
      <a href='https://infinitedb.com/reset?token={token}' style='background-color:#234AA6; color:#FFFFFF; padding:12px 24px; border-radius:6px; text-decoration:none; font-weight:bold; font-size:1.1em;'>Reset Password</a>
    </div>
    <p>If you did not request a password reset, you can ignore this email.</p>
    <div style='margin-top:32px; text-align:center; color:#777; font-size:12px;'>© infinitedb.com. All rights reserved.</div>
  </div>
</body>
</html>";
        return new EmailSendRequest
        {
            Recipients = [recipient],
            Subject = subject,
            PlainText = plainTextContent,
            Html = htmlContent
        };
    }

    public static EmailSendRequest CreateAccountDeletedEmail(string recipient)
    {
        var subject = "InfiniteDb - Account Deleted";
        var plainTextContent =
            """
Your InfiniteDb account has been deleted.

If this was not you, please contact support immediately: https://infinitedb.com/support

Best regards,
The InfiniteDb Team
""";
        var htmlContent = @"
<!DOCTYPE html>
<html lang='en'>
<head>
  <meta charset='UTF-8'>
  <title>Account deleted</title>
</head>
<body style='margin:0; padding:32px; font-family: Inter, sans-serif; background-color:#F7F7F7; color:#1E1E20;'>
  <div style='max-width:600px; margin:32px auto; background:#FFFFFF; border-radius:12px; padding:32px;'>
    <h1 style='font-size:32px; font-weight:600; color:#D03434; text-align:center;'>Your InfiniteDb account has been deleted</h1>
    <p>If this was not you, <a href='https://infinitedb.com/support' style='color:#F26CF9; text-decoration:underline;'>contact support</a> immediately.</p>
    <div style='margin-top:32px; text-align:center; color:#777; font-size:12px;'>© infinitedb.com. All rights reserved.</div>
  </div>
</body>
</html>";
        return new EmailSendRequest
        {
            Recipients = [recipient],
            Subject = subject,
            PlainText = plainTextContent,
            Html = htmlContent
        };
    }
}
