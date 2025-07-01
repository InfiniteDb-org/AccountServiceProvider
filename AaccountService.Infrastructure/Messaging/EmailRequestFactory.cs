using AccountService.Contracts.Requests;

namespace Infrastructure.Messaging;

public static class EmailRequestFactory
{
    public static EmailSendRequest CreateVerificationEmail(string recipient, string code)
    {
        var subject = $"Your code is {code}";
        var plainTextContent = 
            $"""
                Verify Your Email Address\n\nHello,\n\nTo 
                complete your verification, please enter the following code:\n\n{code}\n\nAlternatively, 
                you can open the verification page using the following link:\nhttps://infinitedb.com/verify?email={recipient}&token={code}\n\n
                If you did not initiate this request, you can safely disregard this email.\n
                We take your privacy seriously. No further action is required if you did not initiate this request.\n\n
                For more information about how we process personal data, please see our Privacy Policy: https://infinitedb.com/privacy-policy\n\n© infinitedb.com. 
                All rights reserved.
             """;

        var htmlContent =
            $"""
                <!DOCTYPE html>
                <html lang='en'>
                <head>
                    <meta charset='UTF-8'>
                    <title>Your verification code</title>
                </head>
                <body style='margin:0; padding:32px; font-family: Inter, sans-serif; background-color:#F7F7F7; color:#1E1E20;'>
                    <div style='max-width:600px; margin:32px auto; background:#FFFFFF; border-radius:12px; padding:32px;'>
                        <h1 style='font-size:32px; font-weight:600; color:#3747D0; margin-bottom:16px; text-align:center;'>Verify Your Email Address</h1>
                        <p style='font-size:16px; color:#1E1E20; margin-bottom:16px;'>Hello,</p>
                        <p style='font-size:16px; color:#1E1E20; margin-bottom:24px;'>To complete your verification, please enter the code below or click the button to open a new page.</p>
                        <div style='display:flex; justify-content:center; align-items:center; padding:16px; background-color:#FCD3FE; color:#1C2346; font-size:2em; letter-spacing:2px; font-weight:bold; margin-bottom:28px;'>
                            {code}
                        </div>
                        <div style='text-align:center; margin-bottom:32px;'>
                            <a href='https://infinitedb.com/verify?email={recipient}&token={code}' style='background-color:#F26CF9; color:#FFFFFF; padding:12px 24px; border-radius:6px; text-decoration:none; font-weight:bold; font-size:1.1em;'>Open Verification Page</a>
                        </div>
                        <p style='color:#222; font-size:1.05em;'>If you did not initiate this request, you can safely disregard this email.</p>
                        <p style='color:#222; font-size:1.05em;'>We take your privacy seriously. No further action is required if you did not initiate this request.</p>
                        <a href='https://infinitedb.com/privacy-policy' style='color:#F26CF9; text-decoration:none;'>Privacy Policy</a>
                        <div style='color:#777; font-size:12px; margin-top:24px; text-align:center;'>© infinitedb.com. All rights reserved.</div>
                    </div>
                </body>
                </html>
             """;

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
                Welcome to InfiniteDb!
                Your account is now activated.

                If you have any questions, visit our support: https://infinitedb.com/support

                We take your privacy seriously. For more information, see our Privacy Policy: https://infinitedb.com/privacy-policy

                © infinitedb.com. All rights reserved.
            """;
        var htmlContent = 
            """
                <!DOCTYPE html>
                <html lang='en'>
                <head>
                    <meta charset='UTF-8'>
                    <title>Welcome to InfiniteDb!</title>
                </head>
                <body style='margin:0; padding:32px; font-family: Inter, sans-serif; background-color:#F7F7F7; color:#1E1E20;'>
                    <div style='max-width:600px; margin:32px auto; background:#FFFFFF; border-radius:12px; padding:32px;'>
                        <h1 style='font-size:32px; font-weight:600; color:#3747D0; margin-bottom:16px; text-align:center;'>Welcome to InfiniteDb!</h1>
                        <p style='font-size:16px; color:#1E1E20; margin-bottom:24px;'>Your account is now activated.</p>
                        <div style='text-align:center; margin-bottom:32px;'>
                            <a href='https://infinitedb.com/support' style='background-color:#234AA6; color:#FFFFFF; padding:12px 24px; border-radius:6px; text-decoration:none; font-weight:bold; font-size:1.1em;'>Visit Support</a>
                        </div>
                        <p style='color:#222; font-size:1.05em;'>We take your privacy seriously. For more information, see our <a href='https://infinitedb.com/privacy-policy' style='color:#F26CF9; text-decoration:none;'>Privacy Policy</a>.</p>
                        <div style='color:#777; font-size:12px; margin-top:24px; text-align:center;'>© infinitedb.com. All rights reserved.</div>
                    </div>
                </body>
                </html>
            """;
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
        var subject = "Reset your password";
        var plainTextContent = 
            $"""
                You requested a password reset for your InfiniteDb account.

                To reset your password, use this link:
                https://infinitedb.com/reset?token={token}

                If you did not request a password reset, you can ignore this email.

                We take your privacy seriously. For more information, see our Privacy Policy: https://infinitedb.com/privacy-policy

                © infinitedb.com. All rights reserved.
            """;
        var htmlContent = 
            $"""
                <!DOCTYPE html>
                <html lang='en'>
                <head>
                    <meta charset='UTF-8'>
                    <title>Reset your password</title>
                </head>
                <body style='margin:0; padding:32px; font-family: Inter, sans-serif; background-color:#F7F7F7; color:#1E1E20;'>
                    <div style='max-width:600px; margin:32px auto; background:#FFFFFF; border-radius:12px; padding:32px;'>
                        <h1 style='font-size:32px; font-weight:600; color:#234AA6; margin-bottom:16px; text-align:center;'>Reset your password</h1>
                        <p style='font-size:16px; color:#1E1E20; margin-bottom:24px;'>Click the button below to reset your password:</p>
                        <div style='text-align:center; margin-bottom:32px;'>
                            <a href='https://infinitedb.com/reset?token={token}' style='background-color:#234AA6; color:#FFFFFF; padding:12px 24px; border-radius:6px; text-decoration:none; font-weight:bold; font-size:1.1em;'>Reset Password</a>
                        </div>
                        <p style='color:#222; font-size:1.05em;'>If you did not request a password reset, you can ignore this email.</p>
                        <p style='color:#222; font-size:1.05em;'>We take your privacy seriously. For more information, see our <a href='https://infinitedb.com/privacy-policy' style='color:#F26CF9; text-decoration:none;'>Privacy Policy</a>.</p>
                        <div style='color:#777; font-size:12px; margin-top:24px; text-align:center;'>© infinitedb.com. All rights reserved.</div>
                    </div>
                </body>
                </html>
            """;
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
        var subject = "Your InfiniteDb account has been deleted";
        var plainTextContent = 
            """
                Your InfiniteDb account has been deleted.

                If this was not you, please contact our support immediately: https://infinitedb.com/support

                We take your privacy seriously. For more information, see our Privacy Policy: https://infinitedb.com/privacy-policy

                © infinitedb.com. All rights reserved.
            """;
        var htmlContent = 
            """
                <!DOCTYPE html>
                <html lang='en'>
                <head>
                    <meta charset='UTF-8'>
                    <title>Account deleted</title>
                </head>
                <body style='margin:0; padding:32px; font-family: Inter, sans-serif; background-color:#F7F7F7; color:#1E1E20;'>
                    <div style='max-width:600px; margin:32px auto; background:#FFFFFF; border-radius:12px; padding:32px;'>
                        <h1 style='font-size:32px; font-weight:600; color:#D03434; margin-bottom:16px; text-align:center;'>Your InfiniteDb account has been deleted</h1>
                        <p style='font-size:16px; color:#1E1E20; margin-bottom:24px;'>If this was not you, please <a href='https://infinitedb.com/support' style='color:#F26CF9; text-decoration:underline;'>contact our support</a> immediately.</p>
                        <p style='color:#222; font-size:1.05em;'>We take your privacy seriously. For more information, see our <a href='https://infinitedb.com/privacy-policy' style='color:#F26CF9; text-decoration:none;'>Privacy Policy</a>.</p>
                        <div style='color:#777; font-size:12px; margin-top:24px; text-align:center;'>© infinitedb.com. All rights reserved.</div>
                    </div>
                </body>
                </html>
            """;
        return new EmailSendRequest
        {
            Recipients = [recipient],
            Subject = subject,
            PlainText = plainTextContent,
            Html = htmlContent
        };
    }
}
