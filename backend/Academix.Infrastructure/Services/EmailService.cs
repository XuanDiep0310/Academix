using Academix.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<EmailService> _logger;
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        private readonly string _fromEmail;
        private readonly string _fromName;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;

            _smtpHost = _configuration["Email:SmtpHost"] ?? "smtp.gmail.com";
            _smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
            _smtpUsername = _configuration["Email:Username"] ?? "";
            _smtpPassword = _configuration["Email:Password"] ?? "";
            _fromEmail = _configuration["Email:FromEmail"] ?? "noreply@academix.com";
            _fromName = _configuration["Email:FromName"] ?? "Academix Platform";
        }

        public async Task SendPasswordResetEmailAsync(string toEmail, string resetToken, string userName)
        {
            var resetLink = $"{_configuration["AppUrl"]}/reset-password?token={resetToken}";

            var subject = "Reset Your Password - Academix";
            var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                    <h2 style='color: #2563eb;'>Password Reset Request</h2>
                    <p>Hello {userName},</p>
                    <p>We received a request to reset your password for your Academix account.</p>
                    <p>Click the button below to reset your password:</p>
                    <div style='margin: 30px 0;'>
                        <a href='{resetLink}' 
                           style='background-color: #2563eb; 
                                  color: white; 
                                  padding: 12px 30px; 
                                  text-decoration: none; 
                                  border-radius: 5px;
                                  display: inline-block;'>
                            Reset Password
                        </a>
                    </div>
                    <p>Or copy and paste this link into your browser:</p>
                    <p style='color: #666; word-break: break-all;'>{resetLink}</p>
                    <p style='color: #999; font-size: 12px; margin-top: 30px;'>
                        This link will expire in 1 hour. If you didn't request this, please ignore this email.
                    </p>
                    <hr style='margin: 30px 0; border: none; border-top: 1px solid #eee;'>
                    <p style='color: #999; font-size: 12px;'>
                        Best regards,<br>
                        The Academix Team
                    </p>
                </div>
            </body>
            </html>
        ";

            await SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendWelcomeEmailAsync(string toEmail, string userName, string temporaryPassword)
        {
            var loginUrl = $"{_configuration["AppUrl"]}/login";

            var subject = "Welcome to Academix - Your Account Details";
            var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                    <h2 style='color: #2563eb;'>Welcome to Academix!</h2>
                    <p>Hello {userName},</p>
                    <p>Your account has been created successfully. Here are your login credentials:</p>
                    <div style='background-color: #f3f4f6; padding: 20px; border-radius: 5px; margin: 20px 0;'>
                        <p style='margin: 5px 0;'><strong>Email:</strong> {toEmail}</p>
                        <p style='margin: 5px 0;'><strong>Temporary Password:</strong> {temporaryPassword}</p>
                    </div>
                    <p style='color: #dc2626;'>
                        <strong>Important:</strong> Please change your password after your first login for security reasons.
                    </p>
                    <div style='margin: 30px 0;'>
                        <a href='{loginUrl}' 
                           style='background-color: #2563eb; 
                                  color: white; 
                                  padding: 12px 30px; 
                                  text-decoration: none; 
                                  border-radius: 5px;
                                  display: inline-block;'>
                            Login Now
                        </a>
                    </div>
                    <hr style='margin: 30px 0; border: none; border-top: 1px solid #eee;'>
                    <p style='color: #999; font-size: 12px;'>
                        Best regards,<br>
                        The Academix Team
                    </p>
                </div>
            </body>
            </html>
        ";

            await SendEmailAsync(toEmail, subject, body);
        }

        private async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                using var client = new SmtpClient(_smtpHost, _smtpPort)
                {
                    Credentials = new NetworkCredential(_smtpUsername, _smtpPassword),
                    EnableSsl = true
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_fromEmail, _fromName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);

                await client.SendMailAsync(mailMessage);
                _logger.LogInformation($"Email sent successfully to {toEmail}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to send email to {toEmail}: {ex.Message}");
                throw;
            }
        }
    }
}
