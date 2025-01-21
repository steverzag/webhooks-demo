# Webhook System

A simple and efficient webhook system built on ASP.NET Core 9 and leveraging .NET Aspire for streamlined development. This system supports webhook creation, management, and processing with extensibility for various use cases.

## Features

- **Webhook Management**: Create, update, and delete webhooks.
- **Event Triggering**: Automatically trigger webhooks based on predefined events.
- **Secure Delivery**: Ensure data integrity with HMAC-based signatures.

## Technologies Used

- **ASP.NET Core 9**: For building a robust and scalable API.
- **.NET Aspire**: For simplifying project setup and development workflows.

## Prerequisites

Ensure you have the following installed:

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker](https://www.docker.com/) (for containerized database and admin tools)
- Any HTTP client or tool for testing webhooks (e.g., Postman or curl)

## Getting Started

1. **Clone the Repository**:
   ```bash
   git clone https://github.com/steverzag/webhooks-demo.git
   cd webhooks-demo

2. **Set the Aspire Project as the Startup Project**:
   - If you're using an IDE like Visual Studio:
     - Right-click the Aspire project (WebhookSystem.AppHost) in the solution explorer.
     - Select **Set as Startup Project**.
   - Alternatively, if running from the command line, ensure you specify the Aspire project directory:
     ```bash
     dotnet run --project ./WebhookSystem.AppHost
     ```

3. **Run Docker Containers with Aspire**:
   - Aspire automatically manages PostgreSQL and pgAdmin containers along with volumes.
   - Ensure Docker is running on your system.

4. **Build and Run the Application**:
   ```bash
   dotnet run --project ./WebhookSystem.AppHost
