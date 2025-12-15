# CI/CD Deployment Guide
## Solar System Application - Azure Deployment

---

## Architecture Overview

```
┌─────────────────────────────────────────────────────────────────┐
│                        GitHub Repository                         │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐              │
│  │  Frontend   │  │   Backend   │  │   Shared    │              │
│  │  (Blazor)   │  │  (Web API)  │  │  (Models)   │              │
│  └─────────────┘  └─────────────┘  └─────────────┘              │
└────────────────────────────┬────────────────────────────────────┘
                             │
                             ▼ Push to main
┌─────────────────────────────────────────────────────────────────┐
│                    GitHub Actions Pipeline                       │
│  ┌──────────┐    ┌──────────┐    ┌──────────────┐              │
│  │  Build   │ -> │  Test    │ -> │   Deploy     │              │
│  │  & Pack  │    │  (Unit)  │    │  to Azure    │              │
│  └──────────┘    └──────────┘    └──────────────┘              │
└────────────────────────────┬────────────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────────┐
│                     Microsoft Azure                              │
│                                                                  │
│  ┌────────────────────────────────────────────────────────────┐ │
│  │                    Resource Group                           │ │
│  │                                                              │ │
│  │  ┌─────────────────┐        ┌─────────────────┐            │ │
│  │  │  App Service    │        │  Azure SQL      │            │ │
│  │  │  (Web App)      │ -----> │  Database       │            │ │
│  │  │                 │        │                 │            │ │
│  │  │  - Blazor WASM  │        │  - CelestialBodies│          │ │
│  │  │  - Web API      │        │  - Contacts      │           │ │
│  │  │  - SignalR      │        │  - ApiSnapshots  │           │ │
│  │  └─────────────────┘        └─────────────────┘            │ │
│  │                                                              │ │
│  └────────────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────────┘
```

---

## Prerequisites

1. **Azure Account** with active subscription
2. **GitHub Repository** with code pushed
3. **Azure CLI** installed locally
4. **.NET 9 SDK** installed

---

## Step 1: Create Azure Resources

Run the Azure setup script:

```bash
# Login to Azure
az login

# Run the setup script
chmod +x azure/setup-azure-resources.sh
./azure/setup-azure-resources.sh
```

This creates:
- Resource Group
- App Service Plan (Linux, B1)
- Web App (.NET 9)
- Azure SQL Server
- Azure SQL Database

---

## Step 2: Configure GitHub Secrets

Go to your GitHub repository → Settings → Secrets and variables → Actions

Add these secrets:

| Secret Name | Description | Example |
|-------------|-------------|---------|
| `AZURE_CLIENT_ID` | Service Principal App ID | `xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx` |
| `AZURE_TENANT_ID` | Azure AD Tenant ID | `xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx` |
| `AZURE_SUBSCRIPTION_ID` | Azure Subscription ID | `xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx` |
| `DB_CONNECTION_STRING` | SQL Database connection | `Server=tcp:sql-solarsystem.database.windows.net...` |

---

## Step 3: Configure Federated Credentials (OIDC)

For secure passwordless authentication:

```bash
# Create federated credential for GitHub Actions
az ad app federated-credential create \
  --id <APP_ID> \
  --parameters '{
    "name": "github-actions-main",
    "issuer": "https://token.actions.githubusercontent.com",
    "subject": "repo:<OWNER>/<REPO>:ref:refs/heads/main",
    "audiences": ["api://AzureADTokenExchange"]
  }'
```

---

## Step 4: Update Workflow Configuration

Edit `.github/workflows/azure-deploy.yml`:

```yaml
env:
  AZURE_WEBAPP_NAME: 'your-actual-webapp-name'  # Update this
```

---

## Step 5: Update appsettings.Production.json

Create `appsettings.Production.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Will be set from Azure App Settings"
  },
  "NasaApi": {
    "ApiKey": "Your-NASA-API-Key",
    "BaseUrl": "https://api.nasa.gov/"
  },
  "SolarSystemApi": {
    "BaseUrl": "https://api.le-systeme-solaire.net/rest/",
    "EnableAutoRefresh": true,
    "RefreshIntervalHours": 24
  }
}
```

---

## Step 6: Deploy

Push to main branch to trigger deployment:

```bash
git add .
git commit -m "Configure CI/CD deployment"
git push origin main
```

---

## Pipeline Stages

### 1. Build Stage
- Checkout code
- Setup .NET 9 SDK
- Restore NuGet packages
- Build solution in Release mode
- Run unit tests
- Create deployment package

### 2. Deploy Stage
- Download build artifact
- Login to Azure (OIDC)
- Deploy to Azure Web App
- Configure app settings

### 3. Migration Stage
- Apply EF Core migrations
- Validate database schema

---

## Verification Checklist

After deployment, verify:

- [ ] Web App loads at `https://your-app.azurewebsites.net`
- [ ] 3D Solar System visualization works
- [ ] Database verification page shows connected
- [ ] Contact form submits successfully
- [ ] NASA API endpoints return data
- [ ] SignalR hub connects

---

## Troubleshooting

### Common Issues

1. **Deployment fails with 401**
   - Check AZURE_CLIENT_ID, TENANT_ID, SUBSCRIPTION_ID
   - Verify federated credentials are configured

2. **Database connection fails**
   - Check DB_CONNECTION_STRING secret
   - Verify SQL Server firewall allows Azure services

3. **Application crashes on startup**
   - Check App Service logs: `az webapp log tail --name <app> --resource-group <rg>`
   - Verify .NET version matches

### View Logs

```bash
# Stream live logs
az webapp log tail \
  --name solarsystem-app \
  --resource-group rg-solarsystem

# Download logs
az webapp log download \
  --name solarsystem-app \
  --resource-group rg-solarsystem
```

---

## Cost Optimization

| Resource | SKU | Monthly Cost (Est.) |
|----------|-----|---------------------|
| App Service | B1 | ~$13 |
| SQL Database | S0 | ~$15 |
| **Total** | | **~$28/month** |

For development, consider:
- Use F1 (Free) App Service tier
- Use serverless SQL (pay per use)

---

## Security Best Practices

1. ✅ Use OIDC authentication (no stored secrets)
2. ✅ Connection strings in App Settings, not code
3. ✅ Enable HTTPS only
4. ✅ Use Managed Identity where possible
5. ✅ Enable Azure Defender for SQL

---

## Resume Description

> "Implemented CI/CD pipeline using GitHub Actions to deploy a multi-tier ASP.NET Core 9 application (Blazor WebAssembly, Web API, SignalR) to Azure App Services with Azure SQL Database. Configured automated builds, EF Core migrations, and secure OIDC authentication for zero-secret deployments."

---

## Support

- [ASP.NET Core Docs](https://docs.microsoft.com/aspnet/core)
- [Azure App Service Docs](https://docs.microsoft.com/azure/app-service)
- [GitHub Actions Docs](https://docs.github.com/actions)
