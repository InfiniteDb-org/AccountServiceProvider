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

Important: This code is valid for 15 minutes and can only be used once.

If you did not request this code, you can safely ignore this email.

Best regards,
The InfiniteDb Team
""";

        var htmlContent = $@"
<!DOCTYPE html>
<html lang='en'>
<head>
  <meta charset='UTF-8'>
  <meta name='viewport' content='width=device-width, initial-scale=1.0'>
  <title>Verify your email</title>
</head>
<body style='margin:0; padding:32px; font-family: -apple-system, BlinkMacSystemFont, Inter, sans-serif; background-color:#F7F7F7; color:#1E1E20; line-height:1.5;'>
  <div style='max-width:600px; margin:32px auto; background:#FFFFFF; border-radius:12px; padding:40px; box-shadow:0 2px 8px rgba(0,0,0,0.1);'>
    <h1 style='font-size:28px; font-weight:600; color:#3747D0; text-align:center; margin:0 0 32px 0;'>Verify your email</h1>
    <p style='margin:16px 0;'>Hi!</p>
    <p style='margin:16px 0;'>Your verification code is:</p>
    <div style='font-size:32px; letter-spacing:3px; font-weight:bold; color:#234AA6; margin:32px 0; text-align:center; background:#F8FAFC; padding:20px; border-radius:8px; border:2px solid #E2E8F0;'>{code}</div>
    <p style='margin:16px 0;'>Enter this code to verify your email address.</p>
    <p style='margin:16px 0; padding:12px; background:#FEF3C7; border-left:4px solid #F59E0B; border-radius:4px; font-size:14px; color:#92400E;'><strong>Important:</strong> This code is valid for 15 minutes and can only be used once.</p>
    <p style='font-size:14px; color:#64748B; margin:32px 0 0 0;'>If you did not request this code, you can safely ignore this email.</p>
    <div style='margin-top:40px; text-align:center; color:#94A3B8; font-size:12px; border-top:1px solid #E2E8F0; padding-top:24px;'>© infinitedb.com. All rights reserved.</div>
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
        
        var plainTextContent = """
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
  <meta name='viewport' content='width=device-width, initial-scale=1.0'>
  <title>Welcome to InfiniteDb!</title>
</head>
<body style='margin:0; padding:32px; font-family: -apple-system, BlinkMacSystemFont, Inter, sans-serif; background-color:#F7F7F7; color:#1E1E20; line-height:1.5;'>
  <div style='max-width:600px; margin:32px auto; background:#FFFFFF; border-radius:12px; padding:40px; box-shadow:0 2px 8px rgba(0,0,0,0.1);'>
    <h1 style='font-size:28px; font-weight:600; color:#3747D0; text-align:center; margin:0 0 32px 0;'>Welcome to InfiniteDb!</h1>
    <p style='margin:16px 0;'>Hi and welcome!</p>
    <p style='margin:16px 0;'>Your account is now activated.</p>
    <div style='text-align:center; margin:32px 0;'>
      <a href='https://infinitedb.com/support' style='display:inline-block; background-color:#234AA6; color:#FFFFFF; padding:14px 28px; border-radius:8px; text-decoration:none; font-weight:600; font-size:16px;'>Contact support</a>
    </div>
    <p style='font-size:14px; color:#64748B; margin:32px 0 0 0;'>If you have any questions, we're here to help.</p>
    <div style='margin-top:40px; text-align:center; color:#94A3B8; font-size:12px; border-top:1px solid #E2E8F0; padding-top:24px;'>© infinitedb.com. All rights reserved.</div>
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
        
        var plainTextContent = $"""
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
  <meta name='viewport' content='width=device-width, initial-scale=1.0'>
  <title>Reset password</title>
</head>
<body style='margin:0; padding:32px; font-family: -apple-system, BlinkMacSystemFont, Inter, sans-serif; background-color:#F7F7F7; color:#1E1E20; line-height:1.5;'>
  <div style='max-width:600px; margin:32px auto; background:#FFFFFF; border-radius:12px; padding:40px; box-shadow:0 2px 8px rgba(0,0,0,0.1);'>
    <h1 style='font-size:28px; font-weight:600; color:#234AA6; text-align:center; margin:0 0 32px 0;'>Reset your password</h1>
    <p style='margin:16px 0;'>You requested to reset your password for InfiniteDb.</p>
    <div style='text-align:center; margin:32px 0;'>
      <a href='https://infinitedb.com/reset?token={token}' style='display:inline-block; background-color:#234AA6; color:#FFFFFF; padding:14px 28px; border-radius:8px; text-decoration:none; font-weight:600; font-size:16px;'>Reset Password</a>
    </div>
    <p style='font-size:14px; color:#64748B; margin:32px 0 0 0;'>If you did not request a password reset, you can ignore this email.</p>
    <div style='margin-top:40px; text-align:center; color:#94A3B8; font-size:12px; border-top:1px solid #E2E8F0; padding-top:24px;'>© infinitedb.com. All rights reserved.</div>
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
        
        var plainTextContent = """
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
  <meta name='viewport' content='width=device-width, initial-scale=1.0'>
  <title>Account deleted</title>
</head>
<body style='margin:0; padding:32px; font-family: -apple-system, BlinkMacSystemFont, Inter, sans-serif; background-color:#F7F7F7; color:#1E1E20; line-height:1.5;'>
  <div style='max-width:600px; margin:32px auto; background:#FFFFFF; border-radius:12px; padding:40px; box-shadow:0 2px 8px rgba(0,0,0,0.1);'>
    <h1 style='font-size:28px; font-weight:600; color:#DC2626; text-align:center; margin:0 0 32px 0;'>Account deleted</h1>
    <p style='margin:16px 0;'>Your InfiniteDb account has been deleted.</p>
    <p style='margin:16px 0;'>If this was not you, <a href='https://infinitedb.com/support' style='color:#234AA6; text-decoration:none; font-weight:600;'>contact support</a> immediately.</p>
    <div style='margin-top:40px; text-align:center; color:#94A3B8; font-size:12px; border-top:1px solid #E2E8F0; padding-top:24px;'>© infinitedb.com. All rights reserved.</div>
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