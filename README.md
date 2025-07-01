# AccountServiceProvider

## Overview
AccountServiceProvider is a modular .NET microservice for user account management, registration, email verification, and secure password handling.

## Features
- User registration & email verification
- Secure password reset & credential validation
- Azure Communication Services integration for email delivery
- Event-driven architecture with clear separation of concerns
- xUnit tests with edge case validation

## Project Structure
```
AccountServiceProvider/
├── AccountService.Infrastructure/    # Data access, messaging, email templates
├── AccountService.Application/       # Business logic, service layer
├── AccountService.Contracts/         # Request/response models
├── EmailServiceProvider/             # Email Azure Function
├── AccountService_Tests/             # xUnit test project (mocked repo, event publisher)
```