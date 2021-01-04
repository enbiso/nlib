# Enbiso NLib

.Net Standard Libraries to build simple microservices

![Nugets CI/CD](https://github.com/enbiso/Enbiso.NLib/workflows/Nugets%20CI/CD/badge.svg)

## Domain

Simple domain seedwork libraries to use in DDD designs

- Enbiso.NLib.Domain
- Enbiso.NLib.Domain.Events

## CQRS
- Enbiso.NLib.Cqrs
- Enbiso.NLib.Cqrs.Idempotent

## IOC Extensions

IOC extensions based off microsoft dependency injection abstractions to provide attribute based injection.

- Enbiso.NLib.DependencyInjection

## Event Bus

Async message publishing and subscription library set

- Enbiso.NLib.EventBus
- Enbiso.NLib.EventBus.Abstractions

Implementations available for NATS and RabbitMQ

- Enbiso.NLib.EventBus.Nats
- Enbiso.NLib.EventBus.RabbitMq

## Event Logger
- Enbiso.NLib.EventLogger
- Enbiso.NLib.EventLogger.EntityFramework
- Enbiso.NLib.EventLogger.Mongo

## API Exception Handlers
- Enbiso.NLib.GlobalExceptions

## Idempotency
- Enbiso.NLib.Idempotency
- Enbiso.NLib.Idempotency.EntityFramework

## OpenAPI Extensions

Simple wrapper on top of Swashbuckle to maintain uniformity amoung the APIs

- Enbiso.NLib.OpenApi

## REST Client implementation

REST client built using httpclient with Open ID Connect

- Enbiso.NLib.RestClient
