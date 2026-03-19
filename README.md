# CertiBlock

> **Research system for evaluating the efficiency of blockchain technologies (Ethereum & Polygon) in the registration and verification of digital certificates.**

CertiBlock is a microservice-based application developed as part of a master's thesis. It compares the performance of Ethereum and Polygon networks in the context of decentralized certificate management, collecting and visualizing detailed on-chain metrics.

![CI Pipeline](https://github.com/Kamil-Matusz/CertiBlock/actions/workflows/ci.yml/badge.svg)

---

## 🎯 Key Features

- 📜 **Certificate Registration** — issue and register digital certificates on the Ethereum and Polygon blockchains
- ✅ **Certificate Verification** — verify certificate authenticity via on-chain transaction data
- 📊 **Metrics Collection** — measure and store blockchain performance data (transaction time, gas usage, cost, success rate)
- 📈 **Grafana Dashboards** — real-time comparison of Ethereum vs. Polygon metrics via InfluxDB
- 👤 **User Management** — authentication and user account handling
- 🌐 **API Gateway** — single entry point routing traffic to all backend services

---

## 🏗️ Architecture

CertiBlock follows a **microservices architecture** managed via Docker Compose. All services communicate asynchronously through **RabbitMQ**.

```
┌─────────────────────────────────────────────────────────────┐
│                        Frontend (React)                     │
│               Vite + TypeScript + MUI + react-admin         │
└─────────────────────────┬───────────────────────────────────┘
                          │ HTTP
                ┌─────────▼──────────┐
                │    API Gateway     │
                │  (YARP / Ocelot)   │
                └──┬──┬──┬──┬──┬────┘
                   │  │  │  │  │
       ┌───────────┘  │  │  │  └───────────┐
       │              │  │  │              │
  ┌────▼────┐  ┌──────▼┐ │ ┌▼────────┐ ┌─▼──────┐
  │  Users  │  │ Certs │ │ │Ethereum │ │Polygon │
  │ Service │  │Service│ │ │Service  │ │Service │
  └─────────┘  └───────┘ │ └─────────┘ └────────┘
                          │
                   ┌──────▼──────┐
                   │   Metrics   │
                   │   Service   │
                   └─────────────┘

  Infrastructure: PostgreSQL · MongoDB · RabbitMQ · InfluxDB · Seq · Grafana
```

---

## 🧩 Services

| Service | Description | Stack |
|---|---|---|
| **Gateway** | API Gateway — single entry point for all client requests | .NET 9 |
| **Certificate Service** | Issues and manages digital certificates | .NET 9, PostgreSQL |
| **Ethereum Service** | Registers/verifies certificates on the Ethereum network | .NET 9, MongoDB |
| **Polygon Service** | Registers/verifies certificates on the Polygon network | .NET 9, MongoDB |
| **Metrics Service** | Collects and stores blockchain performance metrics | .NET 9, InfluxDB |
| **Users Service** | User registration, authentication and account management | .NET 9, PostgreSQL |
| **Frontend** | Admin panel and user interface | React 19, Vite, MUI, react-admin |

---

## 🛠️ Technology Stack

### Backend
- **.NET 9** / C# — all microservices
- **PostgreSQL** — relational data (users, certificates)
- **MongoDB** — document storage (blockchain transaction data)
- **RabbitMQ** — asynchronous messaging between services
- **InfluxDB 2** — time-series storage for blockchain metrics
- **Seq** — structured logging and log aggregation

### Frontend
- **React 19** + **TypeScript**
- **Vite** — build tool and dev server
- **Material UI (MUI 7)**
- **react-admin** — admin panel framework
- **React Router v7**

### Infrastructure & Observability
- **Docker** / **Docker Compose** — containerised deployment
- **Grafana** — metrics dashboards
- **GitHub Actions** — CI pipeline (build + test on every push/PR)

---

## 🚀 Getting Started

### Prerequisites

- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9)
- [Node.js 20+](https://nodejs.org/) (for frontend development)

### 1. Start Infrastructure Services

Launch all infrastructure containers (databases, message broker, monitoring):

```bash
docker compose up -d
```

This starts the following services:

| Service | URL |
|---|---|
| PostgreSQL | `localhost:5432` |
| MongoDB | `localhost:27017` |
| RabbitMQ Management | `http://localhost:15672` (guest/guest) |
| Seq (Logs) | `http://localhost:8081` |
| InfluxDB | `http://localhost:8086` |
| Grafana | `http://localhost:3000` (admin/admin) |

### 2. Run Backend Services

Open the solution in Visual Studio or Rider and run individual service projects, or use the .NET CLI:

```bash
dotnet restore CertiBlock.sln
dotnet build CertiBlock.sln
dotnet run --project src/Gateway/CertiBlock.Gateway/CertiBlock.Gateway.csproj
```

### 3. Run the Frontend

```bash
cd src_frontend
npm install
npm run dev
```

The frontend will be available at `http://localhost:5173`.

---

## 📈 Grafana Dashboards

Blockchain metrics collected in **InfluxDB** are visualised in Grafana and allow real-time comparison between Ethereum and Polygon:

- ⏱️ Average transaction time
- ⛽ Average gas used
- 💰 Cost per operation
- ✅ Success rate trends

### Importing Dashboards

Predefined dashboard definitions are available in the **`/grafana`** folder.

1. Open Grafana at `http://localhost:3000` (admin / admin)
2. Go to **Dashboards → Import**
3. Upload a `.json` file from the `/grafana` folder
4. Assign the **InfluxDB** data source when prompted
5. Save and open the dashboard

---

## 🧪 Tests

```bash
dotnet test CertiBlock.sln --verbosity normal
```

Test projects are located in the **`/tests`** directory.

---

## ⚙️ CI/CD

GitHub Actions pipeline (`.github/workflows/ci.yml`) runs on every push and pull request:

1. Checkout code
2. Set up .NET 9
3. Restore dependencies
4. Build solution
5. Run all tests

---

## 📁 Project Structure

```
CertiBlock/
├── src/
│   ├── Certificate/        # Certificate microservice
│   ├── Ethereum/           # Ethereum blockchain service
│   ├── Polygon/            # Polygon blockchain service
│   ├── Metrics/            # Metrics collection service
│   ├── Users/              # User management service
│   ├── Gateway/            # API Gateway
│   └── Shared/             # Shared libraries & contracts
├── src_frontend/           # React frontend application
├── grafana/                # Grafana dashboard JSON files
├── tests/                  # Integration & unit tests
├── scripts/                # Utility scripts
├── docker-compose.yaml     # Infrastructure services
└── CertiBlock.sln          # .NET solution file
```

---

## 📄 License

This project was created for academic research purposes as part of a master's thesis.
