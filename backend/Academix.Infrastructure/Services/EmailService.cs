using Academix.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        private readonly string _fromEmail;
        private readonly string _fromName;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
            _smtpHost = configuration["Email:SmtpHost"] ?? "smtp.gmail.com";
            _smtpPort = int.Parse(configuration["Email:SmtpPort"] ?? "587");
            _smtpUsername = configuration["Email:SmtpUsername"] ?? "";
            _smtpPassword = configuration["Email:SmtpPassword"] ?? "";
            _fromEmail = configuration["Email:FromEmail"] ?? "noreply@academix.com";
            _fromName = configuration["Email:FromName"] ?? "Academix";
        }

        public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = true)
        {
            try
            {
                using var message = new MailMessage
                {
                    From = new MailAddress(_fromEmail, _fromName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = isHtml
                };

                message.To.Add(to);

                using var client = new SmtpClient(_smtpHost, _smtpPort)
                {
                    Credentials = new NetworkCredential(_smtpUsername, _smtpPassword),
                    EnableSsl = true
                };

                await client.SendMailAsync(message);
                Console.WriteLine($"[EMAIL] Sent to {to}: {subject}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EMAIL ERROR] Failed to send to {to}: {ex.Message}");
                throw;
            }
        }

        public async Task SendConfirmationEmailAsync(string email, string confirmationLink)
        {
            var subject = "Confirm your email - Academix";
            var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                    <h2 style='color: #2563eb;'>Welcome to Academix!</h2>
                    <p>Thank you for registering. Please confirm your email address by clicking the button below:</p>
                    <div style='text-align: center; margin: 30px 0;'>
                        <a href='{confirmationLink}' 
                           style='background-color: #2563eb; color: white; padding: 12px 30px; 
                                  text-decoration: none; border-radius: 5px; display: inline-block;'>
                            Confirm Email
                        </a>
                    </div>
                    <p>Or copy and paste this link into your browser:</p>
                    <p style='color: #666; word-break: break-all;'>{confirmationLink}</p>
                    <p style='color: #999; font-size: 12px; margin-top: 30px;'>
                        This link will expire in 24 hours. If you didn't create an account, please ignore this email.
                    </p>
                </div>
            </body>
            </html>
        ";

            await SendEmailAsync(email, subject, body);
        }

        public async Task SendPasswordResetEmailAsync(string email, string resetLink)
        {
            var subject = "Reset your password - Academix";
            var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                    <h2 style='color: #2563eb;'>Password Reset Request</h2>
                    <p>We received a request to reset your password. Click the button below to reset it:</p>
                    <div style='text-align: center; margin: 30px 0;'>
                        <a href='{resetLink}' 
                           style='background-color: #dc2626; color: white; padding: 12px 30px; 
                                  text-decoration: none; border-radius: 5px; display: inline-block;'>
                            Reset Password
                        </a>
                    </div>
                    <p>Or copy and paste this link into your browser:</p>
                    <p style='color: #666; word-break: break-all;'>{resetLink}</p>
                    <p style='color: #999; font-size: 12px; margin-top: 30px;'>
                        This link will expire in 1 hour. If you didn't request a password reset, please ignore this email.
                    </p>
                </div>
            </body>
            </html>
        ";

            await SendEmailAsync(email, subject, body);
        }

        public async Task SendWelcomeEmailAsync(string email, string displayName)
        {
            var subject = "Welcome to Academix!";
            var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                    <h2 style='color: #2563eb;'>Welcome to Academix, {displayName}!</h2>
                    <p>Your email has been confirmed and your account is now active.</p>
                    <p>You can now:</p>
                    <ul>
                        <li>Browse and enroll in courses</li>
                        <li>Track your learning progress</li>
                        <li>Earn certificates</li>
                    </ul>
                    <div style='text-align: center; margin: 30px 0;'>
                        <a href='https://academix.com/dashboard' 
                           style='background-color: #2563eb; color: white; padding: 12px 30px; 
                                  text-decoration: none; border-radius: 5px; display: inline-block;'>
                            Go to Dashboard
                        </a>
                    </div>
                    <p style='color: #999; font-size: 12px; margin-top: 30px;'>
                        If you have any questions, feel free to contact our support team.
                    </p>
                </div>
            </body>
            </html>
        ";

            await SendEmailAsync(email, subject, body);
        }
    }
}
