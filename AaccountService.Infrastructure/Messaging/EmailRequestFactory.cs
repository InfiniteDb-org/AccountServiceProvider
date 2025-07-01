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
<body style='margin:0; padding:32px; font-family: -apple-system, BlinkMacSystemFont, ""Segoe UI"", Roboto, sans-serif; background-color:#FFFFFF; color:#000000; line-height:1.5;'>
  <div style='max-width:600px; margin:0 auto; background:#FFFFFF; padding:40px;'>
    <h1 style='font-size:32px; font-weight:600; color:#4F46E5; text-align:center; margin:0 0 40px 0;'>Verify your email</h1>
    <p style='margin:0 0 20px 0; font-size:16px;'>Hi!</p>
    <p style='margin:0 0 20px 0; font-size:16px;'>Your verification code is:</p>
    <div style='margin:32px 0; padding:24px; background:#F8FAFC; border:1px solid #E2E8F0; border-radius:8px; text-align:center;'>
      <div style='font-size:36px; font-weight:bold; color:#4F46E5; letter-spacing:4px;'>{code}</div>
    </div>
    <p style='margin:0 0 20px 0; font-size:16px;'>Enter this code to verify your email address.</p>
    <div style='margin:24px 0; padding:16px; background:#FEF3C7; border-left:4px solid #F59E0B; border-radius:4px;'>
      <p style='margin:0; font-size:14px; color:#92400E;'><strong>Important:</strong> This code is valid for 15 minutes and can only be used once.</p>
    </div>
    <p style='margin:24px 0 0 0; font-size:14px; color:#6B7280;'>If you did not request this code, you can safely ignore this email.</p>
    <div style='margin-top:60px; text-align:center; color:#9CA3AF; font-size:12px; border-top:1px solid #F3F4F6; padding-top:24px;'>© infinitedb.com. All rights reserved.</div>
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
<body style='margin:0; padding:32px; font-family: -apple-system, BlinkMacSystemFont, ""Segoe UI"", Roboto, sans-serif; background-color:#FFFFFF; color:#000000; line-height:1.5;'>
  <div style='max-width:600px; margin:0 auto; background:#FFFFFF; padding:40px;'>
    <h1 style='font-size:32px; font-weight:600; color:#4F46E5; text-align:center; margin:0 0 40px 0;'>Welcome to InfiniteDb!</h1>
    <p style='margin:0 0 20px 0; font-size:16px;'>Hi and welcome!</p>
    <p style='margin:0 0 32px 0; font-size:16px;'>Your account is now activated.</p>
    <div style='text-align:center; margin:32px 0;'>
      <a href='https://infinitedb.com/support' style='display:inline-block; background-color:#4F46E5; color:#FFFFFF; padding:12px 32px; border-radius:8px; text-decoration:none; font-weight:600; font-size:16px;'>Contact support</a>
    </div>
    <p style='margin:24px 0 0 0; font-size:14px; color:#6B7280;'>If you have any questions, we're here to help.</p>
    <div style='margin-top:60px; text-align:center; color:#9CA3AF; font-size:12px; border-top:1px solid #F3F4F6; padding-top:24px;'>© infinitedb.com. All rights reserved.</div>
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
<body style='margin:0; padding:32px; font-family: -apple-system, BlinkMacSystemFont, ""Segoe UI"", Roboto, sans-serif; background-color:#FFFFFF; color:#000000; line-height:1.5;'>
  <div style='max-width:600px; margin:0 auto; background:#FFFFFF; padding:40px;'>
    <h1 style='font-size:32px; font-weight:600; color:#4F46E5; text-align:center; margin:0 0 40px 0;'>Reset your password</h1>
    <p style='margin:0 0 32px 0; font-size:16px;'>You requested to reset your password for InfiniteDb.</p>
    <div style='text-align:center; margin:32px 0;'>
      <a href='https://infinitedb.com/reset?token={token}' style='display:inline-block; background-color:#4F46E5; color:#FFFFFF; padding:12px 32px; border-radius:8px; text-decoration:none; font-weight:600; font-size:16px;'>Reset Password</a>
    </div>
    <p style='margin:24px 0 0 0; font-size:14px; color:#6B7280;'>If you did not request a password reset, you can ignore this email.</p>
    <div style='margin-top:60px; text-align:center; color:#9CA3AF; font-size:12px; border-top:1px solid #F3F4F6; padding-top:24px;'>© infinitedb.com. All rights reserved.</div>
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
<body style='margin:0; padding:32px; font-family: -apple-system, BlinkMacSystemFont, ""Segoe UI"", Roboto, sans-serif; background-color:#FFFFFF; color:#000000; line-height:1.5;'>
  <div style='max-width:600px; margin:0 auto; background:#FFFFFF; padding:40px;'>
    <h1 style='font-size:32px; font-weight:600; color:#DC2626; text-align:center; margin:0 0 40px 0;'>Account deleted</h1>
    <p style='margin:0 0 20px 0; font-size:16px;'>Your InfiniteDb account has been deleted.</p>
    <p style='margin:0 0 20px 0; font-size:16px;'>If this was not you, <a href='https://infinitedb.com/support' style='color:#4F46E5; text-decoration:none; font-weight:600;'>contact support</a> immediately.</p>
    <div style='margin-top:60px; text-align:center; color:#9CA3AF; font-size:12px; border-top:1px solid #F3F4F6; padding-top:24px;'>© infinitedb.com. All rights reserved.</div>
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