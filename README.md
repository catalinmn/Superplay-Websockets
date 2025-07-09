## Solution Structure
The solution follows clean architecture principles with clear separation of concerns across four projects:

### Core Project (Domain Layer)
Core/
├── Connection/       # Connection management abstractions
├── Domain/           # Business entities and logic
├── Services/         # Application service interfaces
├── Messages/         # Request/response contracts
└── Abstractions/     # Infrastructure contracts

### Infrastructure Project (Persistence Layer)
Infrastructure/
├── Data/             # Database context and migrations
├── Repositories/     # Persistence implementations
└── Services/         # Concrete service implementations

### Server Project (Presentation Layer)
Server/
├── Handlers/         # Message processors
├── Middleware/       # Custom pipeline components
└── Program.cs        # Entry point with DI


### Client Project (Testing)
Client/
├── Services/         # WebSocket client
└── Dockerfile        # Docker configuration for client

### Solution Items 
Solution/
└── Dockerfile        # Docker configuration for server
---

## Key Architecture Decisions

### 1. Clean Architecture Implementation
**Why**:
- Strict separation of concerns
- Domain layer completely independent
- Infrastructure implements Core contracts
- Presentation depends on Core and Infrastructure
- Test client depends only on Core

**Benefit**: Maintainable, testable, and flexible to change

### 2. WebSocket-Centric Design
**Why**:
- Requirement to use native WebSockets
- Real-time communication for game events
- Persistent connections for game state

**Implementation**:
Client->>+Server: WebSocket Upgrade
Server->>Middleware: HTTP → WS
loop Game Session
    Client->>Server: JSON Message
    Server->>HandlerFactory: Route by MessageType
    HandlerFactory->>Handler: Execute
    Handler->>Service: Business Logic
    Service->>Repository: DB Operations
    Handler->>Client: JSON Response
end

### 3. Handler Pattern for Messages
**Why**:
- Clean extension point for new features
- Single Responsibility Principle
- Decoupled from connection management
- Easy unit testing

**Core Components**:
- `IMessageHandler` (Core): Handler contract
- `MessageHandlerFactory` (Server): Routing logic
- `LoginHandler`, `ResourceHandler`, `GiftHandler` (Server): Concrete implementations

### 4. Domain-Driven Design
**Why**:
- Business logic belongs in domain entities
- Prevents anemic domain model
- Encapsulates validation rules

**Key Entities**:
- `Player` (Core/Domain):
  - Manages identity and resources
  - Contains business logic for resource updates
- `GiftTransaction` (Core/Domain):
  - Value object for gift history
  - Ensures auditability

### 5. Repository Pattern
**Why**:
- Abstracts persistence details
- Allows switching database providers
- Centralizes data access patterns

**Implementation**:
- `IPlayerRepository` (Core): Persistence contract
- `PlayerRepository` (Infrastructure): SQLite implementation
- Transaction management for critical operations

---

## Critical Components

### Core Project
1. **Connection Management**
   - `IConnectionManager`: Tracks active WebSocket connections
   - `ConnectionData`: Per-connection context (PlayerId, Socket)

2. **Domain Entities**
   - `Player`: Manages resource balances with business rules
   - `GiftTransaction`: Records gift transfers

3. **Service Interfaces**
   - `IPlayerService`: Player authentication and retrieval
   - `IResourceService`: Resource management operations
   - `IGiftService`: Gift sending with validation

4. **Message Contracts**
   - Request/response DTOs for all operations
   - Standardized error response format

### Infrastructure Project
1. **Persistence Layer**
   - `AppDbContext`: EF Core context for SQLite
   - `PlayerRepository`: Implements repository interface
   - Transaction management for gift operations

2. **Service Implementations**
   - `PlayerService`: Creates/retrieves players
   - `ResourceService`: Updates player resources
   - `GiftService`: Handles gift transfers atomically

3. **Connection Manager**
   - `ConnectionManager`: Thread-safe active connection tracking

### Server Project
1. **WebSocket Middleware**
   - Handles HTTP to WebSocket upgrade
   - Manages connection lifecycle
   - Message processing loop

2. **Handler Implementations**
   - `LoginHandler`: Authenticates players
   - `UpdateResourcesHandler`: Processes resource updates
   - `SendGiftHandler`: Manages gift sending and notifications

3. **Exception Handling**
   - Global exception middleware
   - Domain exception to error response mapping
   - Consistent error formatting

### Client Project
1. **WebSocket Client**
   - Reusable connection manager
   - Message serialization/deserialization
   - Connection lifecycle management

2. **Test Scenarios**
   - `LoginScenario`: Tests authentication
   - `ResourceScenario`: Tests balance updates
   - `GiftScenario`: Tests gift transfers

---

## Docker Compose and Application Startup

### Docker Compose
The solution includes a `docker-compose.yml` file to orchestrate both the server and client containers. The configuration ensures communication between the server and client.

#### Docker Compose File Structuredocker-compose.yml
services:
  client:
    container_name: client
    build:
      context: ./src/Client
      dockerfile: Dockerfile
    ports:
      - "4200:4200"
    depends_on:
      - server

  server:
    container_name: server
    build:
      context: ./src/Server
      dockerfile: Dockerfile
    ports:
      - "5000:5000"
    environment:
      - ASPNETCORE_URLS=http://+:5000
### How to Start the Application

#### Option 1: Start Client and Server Separately
1. **Start the Server**:
   - Navigate to the `src/Server` directory.
   - Run the following command: 
 ```bash
 dotnet run
 ```  
 - Or set Server as startup project in Visual Studio and run it.
 - The server will start and listen on `http://localhost:5000`.

2. **Start the Client**:
   - Navigate to the `src/Client` directory.
   - Run the following commands: 
 ```bash
 npm install
 npm run start
 ```  
 - The client will start and listen on `http://localhost:4200`.

#### Option 2: Use Docker Compose
1. **Build and Start Containers**:
   - Navigate to the solution directory.
   - Run the following command: 
 ```bash
 docker-compose up --build
 ```   
 - Or set the docker-compose as startup project in Visual Studio and run it.
 - This will build and start both the server and client containers.

2. **Access the Applications**:
   - Server: `http://localhost:5000`
   - Client: `http://localhost:4200`

---

## Why This Architecture Works

### 1. Requirement Alignment
- Pure WebSocket implementation
- SQLite persistence
- Clean separation of concerns
- Serilog logging throughout

### 2. Professional Standards
- SOLID principles applied
- Domain-driven design
- Repository pattern
- Transactional operations
- Comprehensive error handling

### 3. Extensibility
- New handlers: Add class + register in DI
- New services: Implement Core interfaces
- Database changes: Modify Infrastructure only
- Protocol changes: Update message DTOs

---

## Key Benefits
- **Maintainability**: Clear separation of concerns
- **Scalability**: Handler-based processing allows horizontal scaling
- **Reliability**: Transactional operations ensure data consistency
- **Extensibility**: New features can be added cleanly
- **Professionalism**: Industry-standard patterns and practices
