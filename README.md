# MicroservicesProject

🚧 Work in progress – a practical project focused on exploring modern .NET backend architecture, distributed systems concepts, and software engineering practices.

## Overview

This repository contains a backend project created to explore and implement modern approaches used in enterprise .NET applications.

The main focus of the project:

- Microservice architecture patterns
- Clean architecture principles
- API design
- Application reliability
- Observability
- Scalability and maintainability

The project serves as a technical playground for evaluating architectural approaches and applying backend development practices in a realistic environment.

---

# Technology Stack

## Backend

- .NET 10
- ASP.NET Core Minimal APIs

Middleware pipeline:

- Correlation ID handling
- Global exception handling

Application features:

- Request rate limiting
  - Global rate limits
  - Endpoint-specific limits
- Memory caching
- Distributed caching with Redis
- Pagination support

---

## Logging & Observability

Implemented:

- Structured logging using Serilog
- Log analysis using Seq

Planned:

- OpenTelemetry integration
- Prometheus metrics
- Grafana dashboards

---

## Data Access

Planned:

- Entity Framework Core
- PostgreSQL

---

## Communication & Messaging

Planned:

- Asynchronous communication patterns
- Message broker integration:
  - RabbitMQ
  - Kafka

---

## Security & Validation

Planned:

- Authentication
- Authorization
- Fluent Validation

---

# Architecture Goals

The project focuses on:

- Clear separation of responsibilities
- Maintainable application structure
- Scalable service design
- Centralized error handling
- Reliable API communication
- Production-oriented development practices

---

# Implemented Features

- ✅ ASP.NET Core application structure
- ✅ Custom middleware pipeline
- ✅ Global exception handling
- ✅ Correlation ID propagation
- ✅ Request rate limiting
- ✅ Memory and distributed caching
- ✅ Structured application logging

---

# Planned Features

- Microservice decomposition
- API Gateway
- Authentication and authorization
- Validation layer
- Database integration
- Asynchronous communication
- Docker containerization
- CI/CD automation
- Automated testing

---

# Testing Strategy

Planned testing approach:

- Unit tests
- Integration tests

The goal is to demonstrate approaches for improving reliability and maintainability of backend services.

---

# Purpose

This project is a personal exploration of modern .NET backend development and architectural patterns commonly used in enterprise applications.

The focus is not only on individual technologies, but also on understanding architectural decisions and trade-offs involved in designing maintainable backend systems.
