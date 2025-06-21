# VerticalModularEshop

A real-world **modular monolith** (Modulith) project built with **.NET 8**, following modern architectural and design principles such as **Vertical Slice Architecture (VSA)**, **Domain-Driven Design (DDD)**, and **CQRS**. This project focuses on implementing clear module boundaries, reliable messaging, and robust API design in an e-commerce context.

## Overview

The application is composed of three main business modules:

- **Catalog**: Manages product data and pricing
- **Basket**: Provides shopping cart functionality with Redis caching
- **Ordering**: Handles order placement and processing

All modules are implemented as **vertical slices** with full encapsulation of their features (handlers, validators, DTOs, and endpoints). The system exposes a **Web API** only (no UI) and communicates asynchronously using **RabbitMQ** and reliably using the **Outbox pattern**.

Authentication and authorization are handled via **Keycloak**, integrated through **OAuth2/OpenID Connect** with JWT Bearer tokens.

## Architecture & Patterns

This project incorporates the following architectural and design patterns:

- **Modular Monolith (Modulith)**  
- **Vertical Slice Architecture (VSA)**  
- **Domain-Driven Design (DDD)**  
- **Command Query Responsibility Segregation (CQRS)**  
- **Outbox Pattern for Reliable Messaging**  
- **Event-Driven Communication with RabbitMQ and MassTransit**  
- **Authentication/Authorization with Keycloak**  
- **Infrastructure as Code with Docker and Docker Compose**

## Technology Stack

- **.NET 8**
- **ASP.NET Core Minimal APIs**
- **C# 12**
- **Entity Framework Core with PostgreSQL**
- **Redis** for caching (Basket module)
- **RabbitMQ** and **MassTransit** for messaging
- **MediatR** for request handling
- **FluentValidation** for pipeline-based validation
- **Keycloak** for Identity and Access Management
- **Docker** for local development

## Modules Breakdown

### Catalog Module

- Built using Minimal APIs and Carter
- Implements CQRS with MediatR
- Uses EF Core with PostgreSQL (code-first and migrations)
- Public API exposed for sync inter-module communication
- Handles product listing, pricing, and updates
- Includes validation, logging, exception handling, and health checks

### Basket Module

- Uses Redis for caching (Proxy and Cache-aside patterns)
- Publishes `BasketCheckoutEvent` to RabbitMQ
- Implements Outbox pattern for reliable message delivery
- Syncs product data from Catalog via method calls
- Handles shopping cart operations and checkout workflow

### Ordering Module

- Implements DDD and CQRS patterns
- Processes orders using basket checkout events
- Includes domain models, value objects, and aggregates
- Uses Outbox messaging to ensure consistency
- Clean separation of layers (Domain, Application, Infrastructure)

### Identity

- Authenticates via **Keycloak** (OIDC/OAuth2)
- Dockerized identity provider with user and role management
- Secures Web API using JWT Bearer tokens
- Centralized configuration for authentication flows

## Module Communication

- **Synchronous**: In-process public method calls for simple, tightly coupled interactions  
- **Asynchronous**: Event-based communication using RabbitMQ for decoupled, scalable integration

## Getting Started

### Prerequisites

- Visual Studio 2022 or Visual Studio Code
- .NET 8 SDK
- Docker Desktop (with at least 4 GB RAM and 2 CPUs allocated)

### Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/AliMohamadiDev/VerticalModularEshop.git
   ```
2. Start Docker Desktop and make sure it is running.

3. Run the project using Docker Compose:
    ```
    docker-compose -f docker-compose.yml -f docker-compose.override.yml up -d
    ```
4. Wait until all services are initialized.
5. Access the Web API via:
    ```
    https://localhost:6060
    ```
Use Postman to interact with the internal module APIs.

## Credits
This project was implemented while following the excellent educational material and structure provided by mehmetozkaya/EshopModularMonoliths, with adjustments, custom implementations, and enhancements for learning purposes.
