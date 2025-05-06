# External Message Handling System

## Overview

This solution provides real-time data delivery from an external service to connected clients using SignalR. It manages dynamic subscriptions and ensures clients receive updates as long as they are connected. The system is divided into three main components:

---

## 🧠 Project Structure

### 🔹 ExternalMessageHandling (Core Logic)

This project contains the main logic for managing dynamic data subscriptions and client communication.

#### Key Components

- **`DynamicDataClientInitializer`**  
  A `HostedService` that bootstraps the `IDynamicDataClientManager` on application start.

- **`DynamicDataClientManager`**  
  A singleton service that coordinates the initialization of the `IDynamicDataClientService` and exposes access to:
  - `DynamicDataService`
