# CertiBlock
CertiBlock is a research-oriented system designed to evaluate the efficiency of blockchain technologies (specifically Ethereum and Polygon) in the registration and verification of digital certificates.
The project is part of a master's thesis focused on comparing blockchain performance and assessing their suitability for decentralized certificate management.

## 🚀 Overview
CertiBlock enables users to:
- Register digital certificates on blockchain networks (Ethereum and Polygon)
- Verify certificates through blockchain transaction data
- Collect, store, and compare blockchain performance metrics
- Visualize metrics in Grafana dashboards using InfluxDB and RabbitMQ for asynchronous data flow
The project consists of several services and components connected via Docker.

## Technologies
- C#
- .NET
- PostgreSQL
- MongoDB
- Docker
- RabbitMQ
- Seq
- TypeScript
- React

## 📈 Grafana Dashboards
Metrics from **InfluxDB** are visualized in Grafana to compare Ethereum and Polygon in real-time:
- Average transaction time  
- Average gas used  
- Cost per operation  
- Success rate trends  

### 🗂️ Dashboard Files
Predefined Grafana dashboards are available in the **`/Grafana`** folder of this repository.  
You can import them directly into your Grafana instance by following these steps:

1. Open Grafana (`http://localhost:3000`)
2. Go to **Dashboards → Import**
3. Upload a `.json` file from the `/Grafana` folder
4. Assign the **InfluxDB** data source when prompted
5. Save and open the dashboard to start visualizing blockchain metrics
