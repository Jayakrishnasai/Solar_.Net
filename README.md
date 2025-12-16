# Solar System Explorer 🪐

**Solar System Explorer** is a full-stack, 3D interactive Solar System visualization built with .NET 8, Blazor WebAssembly, Three.js, and Supabase. This project provides a real-time, educational experience, allowing users to explore planets, moons, and celestial bodies in our solar system.

![Solar System Demo](docs/solar-system-demo.webp)

## 📚 Table of Contents

- [Features](#-features)
- [Technologies Used](#️-technologies-used)
- [Architectural Overview](#-architectural-overview)
- [Getting Started](#-getting-started)
- [Project Structure](#-project-structure)
- [File Structure](#-file-structure)
- [Documentation](#-documentation)
- [Contributing](#-contributing)
- [License](#-license)

## ✨ Features

- **Real-time 3D Visualization:** Explore a dynamic and interactive 3D model of the solar system.
- **Accurate Orbital Data:** View precise orbital paths and positions of celestial bodies based on real-world data.
- **Educational Information:** Access detailed information about planets, moons, and other celestial objects.
- **Interactive Controls:** Zoom, pan, and rotate the camera to get a closer look at your favorite celestial bodies.
- **Real-time Updates:** Experience a live simulation with real-time updates for planetary positions.

## 🛠️ Technologies Used

- **Frontend:** Blazor WebAssembly, Three.js, SignalR
- **Backend:** .NET 8, ASP.NET Core Web API, Entity Framework Core
- **Database:** PostgreSQL (Supabase)
- **Infrastructure:** Docker, Kubernetes, Azure

## 🏗️ Architectural Overview

This project follows a modern, full-stack architecture with a clear separation of concerns between the frontend and backend.

-   **Frontend:** The client-side is a Blazor WebAssembly application responsible for rendering the 3D solar system visualization. It uses **Three.js** for rendering and **SignalR** to receive real-time updates from the backend.
-   **Backend:** The server-side is an ASP.NET Core Web API that serves the Blazor application and provides data to the client. It is responsible for fetching celestial body data, calculating orbital positions, and broadcasting real-time updates via SignalR.
-   **Database:** A PostgreSQL database, hosted on **Supabase**, stores all the information about celestial bodies, their properties, and orbital parameters.
-   **Real-time Communication:** **SignalR** establishes a persistent, real-time connection between the client and server, enabling the backend to push live simulation data to the frontend.

## 🚀 Getting Started

### Prerequisites

- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0/)
- [Git](https://git-scm.com/downloads/)

### Configuration

Before running the application, you need to configure the following environment variables:

- `ConnectionStrings__DefaultConnection`: Your Supabase PostgreSQL connection string.
- `NasaApi__ApiKey`: Your API key for NASA's open APIs.

You can obtain a NASA API key from [api.nasa.gov](https://api.nasa.gov/). For Supabase, you can create a free account at [supabase.com](https://supabase.com/).

### Running with Docker (Recommended)

1.  **Clone the repository:**
    ```bash
    git clone https://github.com/your-username/solar-system-explorer.git
    cd solar-system-explorer
    ```
2.  **Build the Docker image:**
    ```bash
    docker build -t solarsystem:local .
    ```
3.  **Run the Docker container:**
    ```bash
    docker run -p 8080:8080 \
      -e ConnectionStrings__DefaultConnection="YOUR_SUPABASE_CONNECTION_STRING" \
      -e NasaApi__ApiKey="YOUR_NASA_API_KEY" \
      solarsystem:local
    ```
    > **Note:** Replace `YOUR_SUPABASE_CONNECTION_STRING` and `YOUR_NASA_API_KEY` with your credentials.
4.  Open your browser and navigate to `http://localhost:8080`.

### Running Locally for Development

1.  **Clone the repository:**
    ```bash
    git clone https://github.com/your-username/solar-system-explorer.git
    cd solar-system-explorer
    ```
2.  **Set up User Secrets:**
    - Navigate to the API project directory:
      ```bash
      cd src/SolarSystem.Api/SolarSystem.Api
      ```
    - Initialize user secrets:
      ```bash
      dotnet user-secrets init
      ```
    - Set your credentials:
      ```bash
      dotnet user-secrets set "ConnectionStrings:DefaultConnection" "YOUR_SUPABASE_CONNECTION_STRING"
      dotnet user-secrets set "NasaApi:ApiKey" "YOUR_NASA_API_KEY"
      ```
3.  **Run the application:**
    ```bash
    dotnet run
    ```
    The API will be available at `https://localhost:7045`.

## 🛠 Project Structure

- `src/SolarSystem.Api` - Backend API & Server
- `src/SolarSystem.Api.Client` - Blazor WebAssembly Client
- `src/SolarSystem.Shared` - Shared models & DTOs
- `k8s/` - Kubernetes manifests
- `docs/` - Technical documentation

## 📂 File Structure

```
.
├── docs
├── k8s
└── src
    ├── SolarSystem.Api
    │   ├── SolarSystem.Api
    │   │   ├── Components
    │   │   │   └── Pages
    │   │   ├── Controllers
    │   │   ├── Data
    │   │   ├── Hubs
    │   │   ├── Properties
    │   │   ├── Services
    │   │   └── wwwroot
    │   │       ├── css
    │   │       ├── geo
    │   │       ├── js
    │   │       └── lib
    │   │           └── bootstrap
    │   │               └── dist
    │   │                   ├── css
    │   │                   └── js
    │   └── SolarSystem.Api.Client
    │       ├── Layout
    │       ├── Pages
    │       └── wwwroot
    └── SolarSystem.Shared
        ├── DTOs
        └── Models
```

## 📚 Documentation

- [Deployment Guide (Kubernetes & Azure)](docs/DEPLOYMENT.md)
- [Supabase Setup](docs/SUPABASE_SETUP.md)
- [Kubernetes Setup](docs/KUBERNETES_SETUP.md)

## 🤝 Contributing

Contributions are welcome! If you'd like to contribute, please follow these steps:

1.  Fork the repository.
2.  Create a new branch for your feature or bug fix.
3.  Make your changes and commit them with a descriptive message.
4.  Push your changes to your fork.
5.  Create a pull request to the main repository.

## 📜 License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
