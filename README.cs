using Field_Service_Ticketing___Maintenance_Scheduling_Backend.Models;

namespace Field_Service_Ticketing___Maintenance_Scheduling_Backend
{
    public class README
    {
        /*
            # FieldOps Backend Service

            ## Project Overview

                    FieldOps is the backend system for Nova IoT Systems managing field devices, service tickets, and technician assignments.It implements business rules like ticket escalation after SLA breach, technician capacity limits, and integrates external weather APIs to flag tickets with weather-related risks.

            ---
             ### Prerequisites

            - [.NET 7 SDK] (https://dotnet.microsoft.com/en-us/download)
            - PostgreSQL or SQL Server installed and running
            - Optional: Docker(if using docker-compose setup)

            ### Environment Variables

            Copy `.env.example` to `.env` and fill in required keys:

            - `Jwt__Key` - JWT secret key  
            - `ConnectionStrings__DefaultConnection` - database connection string  
            - `WeatherApi__BaseUrl` - base URL of weather API
            - `WeatherApi__ApiKey` - (if needed) API key for weather service

            ### Database Setup

            Run migrations to setup database schema:

            ```bash
            dotnet ef database update

            ## Setup and Running Instructions
                Start the backend: dotnet run
                By default it will listen on http://localhost:5000.
                Run all unit and integration tests with: dotnet test
                
            ## Architecture Overview
                Controllers: HTTP API endpoints with role-based authorization
                Services: Business logic (escalation, capacity, weather integration) isolated from controllers
                Data: Entity Framework Core DbContext for PostgreSQL/SQL Server
                External API Client: WeatherService integrates with Open-Meteo API with fallback

            ## Database Schema
                Main entities:

                Users: Store email, password hash, role (Admin or Technician)
                Devices: Name, type, site location (latitude,longitude), status
                Tickets: Linked to devices, track description, priority, status, assigned technician, SLA due date, escalation timestamp, weather risk flag
                Relationships:

                One user (technician) can have many assigned tickets
                One device can have many tickets
                Indexes:
                Ticket status and assigned technician indexed for efficient filtering

            ## Business Rules
                Ticket Escalation: Tickets past SLA due date escalate automatically or via explicit API call. Priority increases one level up to CRITICAL. Status changes to ESCALATED.
                Technician Capacity: Technicians can only have 5 active (ASSIGNED/IN_PROGRESS) tickets concurrently. Assignment attempts beyond this reject with 409 Conflict.

            ## External API Integration
                Weather data is fetched from Open-Meteo using device site coordinates.
                Weather risk flags:
                SEVERE: Wind speed > 20 km/h
                CAUTION: Temp < 0°C or > 35°C
                NONE: Otherwise
                UNKNOWN: Weather API failure fallback
                Calls time out and fail gracefully without blocking ticket creation.

            ## Assumptions
                Customer treated as device attribute; no separate Customer user role.
                Self-registration allowed only for Technician role to prevent admin creation abuse. Admins created manually or via DB seeding.
                Allowed status transitions enforced strictly (e.g., OPEN → ASSIGNED → IN_PROGRESS → RESOLVED). No skipping steps.

            ## Known Limitations
                Ticket escalation defaults to manual /tickets/escalate endpoint; scheduled automation is a bonus.
                No notifications besides logging on escalations.
                No frontend — tested with Swagger UI and API clients.

            ## AI Usage Disclosure
                AI-assisted code suggestions were used primarily for boilerplate and external API integration patterns. All logic and secure handling reviewed and customized for this assignment.


        */

    }
}
