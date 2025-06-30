using AccountService.Contracts.Requests;

namespace Infrastructure.Messaging;

public static class EmailRequestFactory
{
    public static EmailSendRequest CreateVerificationEmail(string recipient, string code)
    {
        return new EmailSendRequest
        {
            Recipients = [recipient],
            Subject = "Your verification code",
            PlainText =
              $"""
               Your InfiniteDb verification code:

               {code}

               Enter this code to verify your email address.

               If you did not request this code, you can ignore this email.

               ---
               This is an automated message. Please do not reply.
               InfiniteDb Team
               """,
            Html = $"""
                   <html>
                     <body style='font-family:sans-serif; background:#f6f8fa; padding:32px;'>
                       <div style='max-width:480px; margin:auto; background:#fff; border-radius:8px; box-shadow:0 2px 8px #e0e0e0; padding:32px 24px;'>
                         <h2 style='color:#234AA6; margin-top:0'>Your InfiniteDb verification code</h2>
                         <div style='font-size:2em; letter-spacing:2px; font-weight:bold; color:#234AA6; margin:24px 0 28px 0; text-align:center;'>{code}</div>
                         <p style='color:#222; font-size:1.05em;'>Enter this code to verify your email address.</p>
                         <div style='margin-top:32px; text-align:center;'>
                           <span style='color:#234AA6; font-weight:bold; font-size:1.2em;'>InfiniteDb Team</span>
                         </div>
                       </div>
                     </body>
                   </html>
                   """
        };
    }

    public static EmailSendRequest CreateWelcomeEmail(string recipient)
    {
        return new EmailSendRequest
        {
            Recipients = [recipient],
            Subject = "Welcome to InfiniteDb!",
            PlainText =
              $"""
               Welcome to InfiniteDb!
               Your account is now activated.

               If you have any questions, visit our support.

               ---
               This is an automated message. Please do not reply.
               InfiniteDb Team
               """,
            Html = """
                   <html>
                     <body style='font-family:sans-serif; background:#f6f8fa; padding:32px;'>
                       <div style='max-width:480px; margin:auto; background:#fff; border-radius:8px; box-shadow:0 2px 8px #e0e0e0; padding:32px 24px;'>
                         <h2 style='color:#234AA6; margin-top:0'>Welcome to InfiniteDb!</h2>
                         <p style='font-size:1.1em; color:#222;'>Your account is now activated.</p>
                         <div style='margin-top:32px; text-align:center;'>
                           <span style='color:#234AA6; font-weight:bold; font-size:1.2em;'>InfiniteDb Team</span>
                         </div>
                       </div>
                     </body>
                   </html>
                   """
        };
    }

    public static EmailSendRequest CreatePasswordResetEmail(string recipient, string token)
    {
        return new EmailSendRequest
        {
            Recipients = [recipient],
            Subject = "Reset your password",
            PlainText =
              $"""
               You requested a password reset for your InfiniteDb account.

               To reset your password, use this link:
               https://infinitedb/reset?token={token}

               If you did not request a password reset, you can ignore this email.

               ---
               This is an automated message. Please do not reply.
               InfiniteDb Team
               """,
            Html = $"""
                   <html>
                     <body style='font-family:sans-serif; background:#f6f8fa; padding:32px;'>
                       <div style='max-width:480px; margin:auto; background:#fff; border-radius:8px; box-shadow:0 2px 8px #e0e0e0; padding:32px 24px;'>
                         <h2 style='color:#234AA6; margin-top:0'>Reset your password</h2>
                         <p style='color:#222; font-size:1.05em;'>Click the link below to reset your password:</p>
                         <div style='margin:24px 0 28px 0; text-align:center;'>
                           <a href='https://infinitedb/reset?token={token}' style='background:#234AA6; color:#fff; padding:12px 32px; border-radius:6px; text-decoration:none; font-weight:bold;'>Reset Password</a>
                         </div>
                         <p style='color:#888; font-size:0.95em;'>If you did not request a password reset, you can ignore this email.</p>
                         <div style='margin-top:32px; text-align:center;'>
                           <span style='color:#234AA6; font-weight:bold; font-size:1.2em;'>InfiniteDb Team</span>
                         </div>
                       </div>
                     </body>
                   </html>
                   """
        };
    }

    public static EmailSendRequest CreateAccountDeletedEmail(string recipient)
    {
        return new EmailSendRequest
        {
            Recipients = [recipient],
            Subject = "Your InfiniteDb account has been deleted",
            PlainText =
              $"""
               Your InfiniteDb account has been deleted.

               If this was not you, please contact our support immediately.

               ---
               This is an automated message. Please do not reply.
               InfiniteDb Team
               """,
            Html = """
                   <html>
                     <body style='font-family:sans-serif; background:#f6f8fa; padding:32px;'>
                       <div style='max-width:480px; margin:auto; background:#fff; border-radius:8px; box-shadow:0 2px 8px #e0e0e0; padding:32px 24px;'>
                         <h2 style='color:#234AA6; margin-top:0'>Your InfiniteDb account has been deleted</h2>
                         <p style='color:#222; font-size:1.05em;'>If this was not you, please contact our support immediately.</p>
                         <div style='margin-top:32px; text-align:center;'>
                           <span style='color:#234AA6; font-weight:bold; font-size:1.2em;'>InfiniteDb Team</span>
                         </div>
                       </div>
                     </body>
                   </html>
                   """
        };
    }
}
