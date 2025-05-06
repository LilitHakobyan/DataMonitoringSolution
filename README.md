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
  - `MessageSenderService`  

  It uses a `Lazy<Task>` pattern (`dynamicDataClientServiceTask`) to guarantee **single, async-safe** initialization. This prevents race conditions where background services might trigger multiple initializations concurrently.

- **`DynamicDataService`**  
  A `BackgroundService` responsible for:
  - Waiting for external triggers to **start** or **stop** subscriptions.
  - Handling activity data received through callbacks during an active subscription.
  - Ensuring that only one subscription session is active at any time.

- **`DynamicDataSubscriptionManager`**  
  Manages the subscription lifecycle and coordinates with SignalR and the data service.

  #### 📌 `DynamicSubscriptionManager` Summary

  The `DynamicSubscriptionManager` is responsible for managing **subscribe** and **unsubscribe** triggers for dynamic data processing. It provides thread-safe mechanisms to initiate and await subscription state changes in response to external events, such as client connections or disconnections.

  **Key Features:**
  - **Thread-Safe Triggering**:  
    Uses a lock to safely reset and trigger `TaskCompletionSource` instances.
  - **Trigger Mechanisms**:
    - `RequestSubscribe()` to signal a subscribe request.
    - `RequestUnsubscribe()` to signal an unsubscribe request.
    - `WaitForSubscribeTriggerAsync()` and `WaitForUnsubscribeTriggerAsync()` to await signals.
  - **Async-Friendly**:  
    Uses `RunContinuationsAsynchronously` to avoid deadlocks in async contexts.
  - **Reusable Lifecycle**:  
    `ResetTriggers()` ensures proper reuse across subscription cycles.

  **Usage Flow:**
  1. Client connects → `RequestSubscribe()` is called.
  2. System awaits `WaitForSubscribeTriggerAsync()` to begin streaming.
  3. Client disconnects → `RequestUnsubscribe()` is called.
  4. System awaits `WaitForUnsubscribeTriggerAsync()` to stop streaming.

- **SignalR Integration**  
  Clients connect and disconnect through SignalR. The following logic is applied:
  - On connect → **Subscribe**
  - On disconnect → **Unsubscribe**
  - Updates from the external service are pushed to clients via the SignalR hub in real-time.

---

### 🔹 ExternalMessaging (Interface Definition)

Defines the **contract** for external data providers:

- Specifies interfaces and data structures expected from external messaging sources.
- Provides an abstraction layer, allowing different implementations to plug into the system seamlessly.

---

### 🔹 API Projects (Integration Layer)

These projects host the application and manage integration:

- Register all services from `ExternalMessageHandling` and `ExternalMessaging` using **Dependency Injection**.
- Host the **SignalR Hub** for client communication.
- Act as the entry point for clients to connect and receive updates.

---

## 🔄 Data Flow

1. **App Startup**  
   `DynamicDataClientInitializer` runs and sets up `DynamicDataClientManager`.

2. **Client Connects via SignalR**  
   → Triggers a **subscribe** event  
   → `DynamicDataService` begins data stream

3. **External Data Received**  
   → Passed to `MessageSenderService`  
   → Relayed to the specific client via SignalR

4. **Client Disconnects**  
   → Triggers **unsubscribe**  
   → Data stream is stopped

---

## 🛠 Technologies Used

- .NET Core / ASP.NET Core
- SignalR
- Hosted Services / Background Services
- Dependency Injection
- Lazy Task Initialization
