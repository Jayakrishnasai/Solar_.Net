# Solar System Explorer - Deployment Guide

## Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                        GitHub Repository                         │
└────────────────────────────┬────────────────────────────────────┘
                             │ Push to main
                             ▼
┌─────────────────────────────────────────────────────────────────┐
│                    GitHub Actions CI/CD                          │
│            Build Docker → Push to ACR → Deploy to K8s           │
└────────────────────────────┬────────────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────────┐
│                     Kubernetes Cluster                           │
│  ┌─────────────────────────────────────────────────────────────┐│
│  │  LoadBalancer → Pods (Solar System App) → Supabase (DB)    ││
│  └─────────────────────────────────────────────────────────────┘│
└─────────────────────────────────────────────────────────────────┘
```

---

## Prerequisites

- Docker Desktop
- kubectl CLI
- Azure CLI (for AKS)
- GitHub account
- Supabase account

---

## Quick Start

### 1. Create Supabase Database

See [SUPABASE_SETUP.md](./SUPABASE_SETUP.md)

### 2. Create Kubernetes Cluster

See [KUBERNETES_SETUP.md](./KUBERNETES_SETUP.md)

### 3. Configure GitHub Secrets

Go to: GitHub → Repository → Settings → Secrets → Actions

| Secret | Description |
|--------|-------------|
| `ACR_USERNAME` | Azure Container Registry username |
| `ACR_PASSWORD` | Azure Container Registry password |
| `KUBE_CONFIG` | Base64 encoded kubeconfig |
| `SUPABASE_CONNECTION_STRING` | PostgreSQL connection string |
| `NASA_API_KEY` | NASA API key |

### 4. Deploy

Push to `main` branch to trigger automatic deployment:

```bash
git push origin main
```

---

## Manual Deployment

### Build Docker Image

```bash
docker build -t solarsyatem.azurecr.io/solarsystem:latest .
```

### Push to ACR

```bash
az acr login --name solarsyatem
docker push solarsyatem.azurecr.io/solarsystem:latest
```

### Deploy to Kubernetes

```bash
kubectl apply -f k8s/deployment.yaml
kubectl rollout status deployment/solarsystem-app
```

---

## Verify Deployment

```bash
# Check pods
kubectl get pods -l app=solarsystem

# Check service
kubectl get service solarsystem-service

# View logs
kubectl logs -l app=solarsystem -f

# Test health endpoint
curl http://<EXTERNAL-IP>/api/database/health
```

---

## Troubleshooting

### Pod CrashLoopBackOff
```bash
kubectl logs <pod-name> --previous
kubectl describe pod <pod-name>
```

### ImagePullBackOff
```bash
kubectl delete secret acr-secret
kubectl create secret docker-registry acr-secret \
  --docker-server=solarsyatem.azurecr.io \
  --docker-username=solarsyatem \
  --docker-password=YOUR_PASSWORD
```

### Database Connection Failed
- Verify Supabase connection string
- Check SSL Mode is set to Require
- Ensure Supabase project is active

---

## Files

| File | Purpose |
|------|---------|
| `Dockerfile` | Multi-stage Docker build |
| `k8s/deployment.yaml` | K8s Deployment + Service |
| `k8s/secrets.yaml` | Secrets template |
| `.github/workflows/k8s-deploy.yml` | CI/CD pipeline |

---

## Resume Description

> "Implemented GitHub Actions CI/CD to deploy a full-stack ASP.NET Core application on Azure VM with Supabase PostgreSQL, including secure secret management and zero-downtime deployment."
