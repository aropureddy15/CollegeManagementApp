# College Management App — .NET 8 MVC + AWS RDS MySQL

## Features
- ASP.NET Core MVC, Identity authentication, Admin/User roles
- Students and courses: everyone authenticated can view; only Admin can create, edit, delete
- Admin-only course cart, scoped to the signed-in admin
- Entity Framework Core with MySQL and migration support
- Student enrollment date, course start date, and UTC audit timestamps

## Requirements
- .NET 8 SDK
- AWS RDS for MySQL (or local MySQL for development)
- Network access from the application host to the database on TCP 3306

## Configure connection string
For local development, use User Secrets (recommended) instead of committing credentials:
```bash
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:CollegeDb" "Server=YOUR-RDS-ENDPOINT;Port=3306;Database=college_management;User=APP_DB_USER;Password=YOUR_PASSWORD;"
```
On Linux, environment variables can be used:
```bash
export ConnectionStrings__CollegeDb='Server=YOUR-RDS-ENDPOINT;Port=3306;Database=college_management;User=APP_DB_USER;Password=YOUR_PASSWORD;'
export ADMIN_EMAIL='admin@example.com'
export ADMIN_PASSWORD='Use-A-Long-Unique-Password!123'
```
Use AWS Secrets Manager or Systems Manager Parameter Store for production secrets. Do not put real passwords in source control or appsettings.json.

## AWS RDS setup
1. Create an RDS MySQL instance and database named `college_management`.
2. Prefer private subnets and keep public access disabled for production.
3. Add an RDS security-group inbound rule for MySQL TCP 3306 from the application EC2 security group only. Do not open 3306 to `0.0.0.0/0`.
4. Create a least-privilege database user for this app. Use a secure password.
5. Ensure the EC2 instance and RDS instance can route to each other. For local development, only allow your trusted public IP temporarily, then remove it.
6. Enable backups and encryption at rest. Use TLS for database connections where configured and required by your RDS setup.

## Create database and run
Install EF tools:
```bash
dotnet tool install --global dotnet-ef --version 8.0.11
```
Then, from the project directory:
```bash
dotnet restore
dotnet ef migrations add InitialCreate
dotnet ef database update
dotnet run
```
The app also runs `Database.Migrate()` at startup, so create and test migrations before deployment. For production, a controlled migration step during deployment is safer than allowing multiple app instances to migrate simultaneously.

Open the local URL shown by `dotnet run`. Use Register to create a normal user. The admin account is created on first startup only when `ADMIN_EMAIL` and `ADMIN_PASSWORD` are set. The password must satisfy the configured policy. After creating the admin, remove or rotate the bootstrap environment variable according to your operational process.

## Deploy to EC2
1. Install the .NET 8 ASP.NET Core Runtime on an Ubuntu EC2 instance.
2. Copy the project or publish artifact to EC2. For production, build/publish in a build pipeline:
```bash
dotnet publish -c Release -o publish
```
3. Set `ConnectionStrings__CollegeDb`, `ADMIN_EMAIL`, and `ADMIN_PASSWORD` as protected service environment variables or retrieve them from AWS Secrets Manager.
4. Run the app with `dotnet CollegeManagementApp.dll` from the publish directory.
5. Configure a `systemd` service to keep the process running and Nginx as a reverse proxy with HTTPS. Open only ports 80/443 publicly; do not expose the database port.
6. Check logs with `journalctl -u college-management -f` if using a systemd service.

## Security notes
- Authorization is enforced in controllers, not just by hiding buttons.
- Anti-forgery validation is applied to POST actions.
- Identity hashes passwords; no plaintext passwords are stored.
- Public self-registration creates ordinary accounts, not admins.
- The sample stores audit timestamps in UTC. Enrollment and course dates are calendar dates.
- Before production, add tests, structured logging, rate limiting, email verification if needed, secure cookie settings, backups, and a deployment-managed migration process.
