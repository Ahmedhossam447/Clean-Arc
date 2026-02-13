namespace CleanArc.Core
{
    public static class EmailTemplates
    {
        public static string GetEmailConfirmationBody(string confirmationLink, string username)
        {
            return $@"
                <h2>Welcome to HappyPaws Haven, {username}!</h2>
                <p>Thank you for registering. To complete your registration, please confirm your email address by clicking the button below:</p>
                <p style=""text-align: center;"">
                    <a href=""{confirmationLink}"" class=""button"">Confirm Email Address</a>
                </p>
                <p>Or copy and paste this link into your browser:</p>
                <p style=""word-break: break-all; color: #666; font-size: 12px;"">{confirmationLink}</p>
                <p><strong>This link will expire in 24 hours.</strong></p>
                <p>If you did not create an account, please ignore this email.</p>";
        }

        public static string GetEmailConfirmationSubject()
        {
            return "Confirm Your Email Address - HappyPaws Haven";
        }

        public static string GetAdoptionConfirmationBody(string adopterName, string animalName, string animalType, DateTime adoptedAt)
        {
            return $@"
                <h2>Dear {adopterName},</h2>
                <p>We're thrilled to let you know that your adoption request for <strong>{animalName}</strong> has been approved!</p>
                <p><strong>Adoption Details:</strong></p>
                <ul>
                    <li><strong>Pet Name:</strong> {animalName}</li>
                    <li><strong>Pet Type:</strong> {animalType}</li>
                    <li><strong>Adoption Date:</strong> {adoptedAt:MMMM dd, yyyy}</li>
                </ul>
                <p>Thank you for choosing to adopt and giving {animalName} a loving home!</p>
                <p>If you have any questions, feel free to reach out to us.</p>
                <p>With love,<br/>The HappyPaws Team 🐾</p>";
        }

        public static string GetAdoptionConfirmationSubject(string animalName)
        {
            return $"Congratulations! You've adopted {animalName}! 🎉";
        }

        public static string GetAdoptionOwnerNotificationBody(string animalName, string animalType, DateTime adoptedAt)
        {
            return $@"
                <h2>Dear Pet Owner,</h2>
                <p>We are excited to inform you that your beloved pet <strong>{animalName}</strong> has found a new loving home!</p>
                <p><strong>Adoption Details:</strong></p>
                <ul>
                    <li><strong>Pet Name:</strong> {animalName}</li>
                    <li><strong>Pet Type:</strong> {animalType}</li>
                    <li><strong>Adoption Date:</strong> {adoptedAt:MMMM dd, yyyy}</li>
                </ul>
                <p>Thank you for entrusting us with the care of {animalName}. We wish you all the best!</p>
                <p>If you have any questions, please don't hesitate to contact us.</p>
                <p>Sincerely,<br/>The HappyPaws Team 🐾</p>";
        }

        public static string GetAdoptionOwnerNotificationSubject(string animalName)
        {
            return $"Your pet {animalName} has been adopted! 🐾";
        }

        public static string GetPasswordResetBody(string resetLink)
        {
            return $@"
                <h2>Password Reset Request</h2>
                <p>You have requested to reset your password. Click the button below to reset it:</p>
                <p style=""text-align: center;"">
                    <a href=""{resetLink}"" class=""button"">Reset Password</a>
                </p>
                <p>Or copy and paste this link into your browser:</p>
                <p style=""word-break: break-all; color: #666; font-size: 12px;"">{resetLink}</p>
                <p><strong>This link will expire in 1 hour.</strong></p>
                <p>If you did not request a password reset, please ignore this email. Your password will remain unchanged.</p>
                <p>For security reasons, if you did not make this request, please contact us immediately.</p>";
        }

        public static string GetPasswordResetSubject()
        {
            return "Reset Your Password - HappyPaws Haven";
        }
    }
}
