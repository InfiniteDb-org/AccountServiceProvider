# AccountServiceProvider

## Overview
AccountServiceProvider is a modular .NET microservice for user account management, registration, email verification, and secure password handling.

## Features
- User registration & email verification via event-driven flow
- Secure password reset & credential validation (configurable policy)
- Azure Communication Services integration for email delivery
- **Event-driven architecture:**
  - AccountService publishes strongly typed semantic events (e.g. `AccountCreatedEvent`, `PasswordResetRequestedEvent`, `VerificationCodeRequestedEvent`)
  - VerificationService listens, generates codes, and triggers downstream flows
  - EmailServiceProvider listens and sends emails
- Centralized email templates in EmailServiceProvider
- xUnit tests with edge case validation

## Project Structure
```
AccountServiceProvider/
├── AccountService.Api/              # HTTP API layer (Azure Functions endpoints)
├── AccountService.Infrastructure/   # Data access, messaging, event publishing
├── AccountService.Application/      # Business logic, service layer
├── AccountService.Contracts/        # Request/response & strongly typed event models (shared contracts)
├── AccountService_Tests/            # xUnit test project (mocked repo, event publisher)
```

## API Endpoints

| Method | Route                                         | Description                         |
|--------|-----------------------------------------------|-------------------------------------|
| POST   | /accounts/start-registration                  | Start user registration             |
| POST   | /accounts/confirm-email-code                  | Confirm email with code             |
| POST   | /accounts/complete-registration               | Complete registration (set password)|
| POST   | /accounts/validate                            | Validate credentials (login)        |
| GET    | /accounts/{userId}                            | Get account by user ID              |
| GET    | /accounts/by-email/{email}                    | Get account by email                |
| PUT    | /accounts/{userId}                            | Update user account                 |
| DELETE | /accounts/{userId}                            | Delete user account                 |
| POST   | /accounts/forgot-password                     | Request password reset              |
| POST   | /accounts/reset-password                      | Reset password                      |
| POST   | /accounts/{userId}/email-confirmation-token   | Generate new email confirmation code|

