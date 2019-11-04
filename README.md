# microservice-sample
Microservice and container based sample application powerd by .NET Core, Angular, gRPC, RabbitMQ, Docker, Kubernetes.

## Overview
This sample application is based on microservice oriented architecture.
Services are developed with C#/ASP.NET Core 3.0
and web(client) is developed with Angular 8.2.10 (Angular CLI 8.3.9).
Both server and client side are cross-platform and can be run on Linux and Windows.

"services/api-service" is desined as API gateway service / BFF(Backends for Frontends). It provides frontend-friendly RESTful API to frontends
while it connects to backend services with gRPC + Protocol Buffers.

"services/omikuji-service" is a sample of microservice providing a function to draw a paper fortune. ("omikuji" is "a paper fortune" in Japanese.)
It has a gRPC endpoint only.

"services/log-service" is a logging microservice. It subscribes RabbitMQ and logs target events.
It also provides a gRPC endpoint.

"web" is a very small UI to draw a paper fortune. It sends a request to "service/api-service" with REST + JSON.

## Features
* ASP.NET Core 3.x / C# (backend services)
* Angular 8.x (frontend)
* gRPC + Protocol Buffers (backend internal connections)
* REST + JSON (backend endpoint for frontend)
* RabbitMQ (for backends event-based communications)
* Docker/Kubernetes

## Installation
1. **Running locally (no Docker):**
    - .NET Core 3.x and Angular CLI 8.x required. 
    - Cannot run on MacOS at the moment. (Kestrel does not support HTTP/2 on MacOS, and will do in the future release)
    - RabbitMQ should be installed and running in default port (5672).
    - Run all services and frontend.
        ``` sh
        $ cd ./services/omikuji-service
        $ dotnet run
        ```
        ``` sh
        $ cd ./services/api-service
        $ dotnet run
        ```        
        ``` sh
        $ cd ./services/log-service
        $ dotnet run
        ```
        ``` sh
        $ cd ./web
        $ npm install   # required only for the first time.
        $ npm start
        ```
2. **Running with Kubernetes:**
    - Docker and Kubernetes required.  

        *Note: Developed with "Docker for Windows" and "Docker for Mac". Some components (e.g. Nginx Ingress Controller https://github.com/kubernetes/ingress-nginx/blob/master/docs/deploy/index.md ) may not work properly on the other environments.*  

        *Note: RabbitMQ is NOT required to be installed on your machine.*

    - Run the following commands.
        ``` sh
        $ cd ./deploy
        $ ./deploy_k8s_all.sh
        ```
    - Access `http://localhost`

## Changelog
### Nov 4, 2019
* add Dockerfiles, manifests(yaml) and scripts for deploying on kubernetes.
### Oct 22, 2019
* fix log-service startup. (should start without RabbitMQ)
### Oct 20, 2019
* add functionality of publishing "OmikujiDrawn" event for RabbitMQ to omikuji-service.
* add log-service. (gRPC endpoint & RabbitMQ subscription)
