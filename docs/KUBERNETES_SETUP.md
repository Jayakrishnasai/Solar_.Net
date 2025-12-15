# Kubernetes Cluster Setup Guide

## Option 1: Azure Kubernetes Service (AKS)

### 1. Create AKS Cluster

```bash
# Variables
RESOURCE_GROUP="rg-solarsystem"
CLUSTER_NAME="aks-solarsystem"
LOCATION="eastus"

# Login to Azure
az login

# Create resource group
az group create --name $RESOURCE_GROUP --location $LOCATION

# Create AKS cluster
az aks create \
  --resource-group $RESOURCE_GROUP \
  --name $CLUSTER_NAME \
  --node-count 2 \
  --node-vm-size Standard_B2s \
  --enable-managed-identity \
  --attach-acr solarsyatem \
  --generate-ssh-keys

# Get credentials
az aks get-credentials --resource-group $RESOURCE_GROUP --name $CLUSTER_NAME

# Verify connection
kubectl get nodes
```

### 2. Attach ACR to AKS

```bash
# Attach existing ACR
az aks update \
  --resource-group $RESOURCE_GROUP \
  --name $CLUSTER_NAME \
  --attach-acr solarsyatem
```

### 3. Install NGINX Ingress Controller

```bash
# Add ingress-nginx repo
helm repo add ingress-nginx https://kubernetes.github.io/ingress-nginx
helm repo update

# Install ingress controller
helm install nginx-ingress ingress-nginx/ingress-nginx \
  --namespace ingress-basic \
  --create-namespace \
  --set controller.replicaCount=2 \
  --set controller.nodeSelector."kubernetes\.io/os"=linux
```

---

## Option 2: Local Kubernetes (Minikube)

### 1. Install Minikube

```bash
# Windows (PowerShell as Admin)
choco install minikube kubernetes-cli

# Or download from https://minikube.sigs.k8s.io/
```

### 2. Start Cluster

```bash
# Start with Docker driver
minikube start --driver=docker --memory=4096 --cpus=2

# Enable ingress
minikube addons enable ingress

# Verify
kubectl get nodes
```

### 3. Configure Docker for Minikube

```bash
# Use Minikube's Docker daemon
minikube docker-env | Invoke-Expression

# Build image directly
docker build -t solarsystem:latest .

# Update deployment to use local image
# Change imagePullPolicy to Never
```

---

## Deploy Application

### 1. Create Secrets

```bash
# ACR Secret
kubectl create secret docker-registry acr-secret \
  --docker-server=solarsyatem.azurecr.io \
  --docker-username=solarsyatem \
  --docker-password="YOUR_ACR_PASSWORD"

# App Secrets
kubectl create secret generic solarsystem-secrets \
  --from-literal=supabase-connection-string="Host=db.xxx.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=YOUR_PASSWORD;SSL Mode=Require;Trust Server Certificate=true" \
  --from-literal=nasa-api-key="YOUR_NASA_API_KEY"
```

### 2. Deploy Application

```bash
# Apply deployment
kubectl apply -f k8s/deployment.yaml

# Check status
kubectl get pods
kubectl get services
kubectl get ingress

# Watch deployment
kubectl rollout status deployment/solarsystem-app
```

### 3. Get External IP

```bash
# For LoadBalancer service
kubectl get service solarsystem-service

# For Minikube
minikube service solarsystem-service --url
```

---

## GitHub Secrets Configuration

Go to GitHub → Repository → Settings → Secrets → Actions

### Required Secrets:

| Secret | How to Get |
|--------|------------|
| `ACR_USERNAME` | `solarsyatem` |
| `ACR_PASSWORD` | From Azure ACR access keys |
| `KUBE_CONFIG` | See below |
| `SUPABASE_CONNECTION_STRING` | From Supabase dashboard |
| `NASA_API_KEY` | https://api.nasa.gov |

### Get KUBE_CONFIG:

```bash
# For AKS
az aks get-credentials --resource-group rg-solarsystem --name aks-solarsystem --file ./kubeconfig

# Base64 encode for GitHub secret (PowerShell)
[Convert]::ToBase64String([IO.File]::ReadAllBytes("./kubeconfig"))

# Or (Linux/Mac)
base64 -w 0 ./kubeconfig
```

Copy the base64 output and paste as `KUBE_CONFIG` secret.

---

## Monitoring & Logs

```bash
# View pod logs
kubectl logs -l app=solarsystem -f

# Describe pod
kubectl describe pod -l app=solarsystem

# Get events
kubectl get events --sort-by='.lastTimestamp'

# Port forward for local access
kubectl port-forward service/solarsystem-service 8080:80
```

---

## Scaling

```bash
# Manual scaling
kubectl scale deployment solarsystem-app --replicas=3

# Horizontal Pod Autoscaler
kubectl autoscale deployment solarsystem-app \
  --cpu-percent=70 \
  --min=2 \
  --max=10
```

---

## Troubleshooting

### Pod in CrashLoopBackOff
```bash
kubectl logs <pod-name> --previous
kubectl describe pod <pod-name>
```

### ImagePullBackOff
```bash
# Check ACR secret
kubectl get secret acr-secret -o yaml

# Verify ACR login
docker login solarsyatem.azurecr.io
```

### Service not accessible
```bash
# Check endpoints
kubectl get endpoints solarsystem-service

# Test from inside cluster
kubectl run test --rm -it --image=busybox -- wget -qO- http://solarsystem-service/api/database/health
```

---

## Cost Optimization

### AKS
- Use B-series VMs for dev/test
- Enable cluster autoscaler
- Use spot instances for non-critical workloads

### Estimated Monthly Cost (AKS):
- 2x Standard_B2s nodes: ~$30
- LoadBalancer: ~$15
- **Total: ~$45/month**

---

## Architecture Diagram

```
                    Internet
                        │
                        ▼
              ┌─────────────────┐
              │   LoadBalancer  │
              │     (Public)    │
              └────────┬────────┘
                       │
              ┌────────┴────────┐
              │  NGINX Ingress  │
              │   Controller    │
              └────────┬────────┘
                       │
         ┌─────────────┼─────────────┐
         ▼             ▼             ▼
    ┌─────────┐   ┌─────────┐   ┌─────────┐
    │  Pod 1  │   │  Pod 2  │   │  Pod 3  │
    │ Solar   │   │ Solar   │   │ Solar   │
    │ System  │   │ System  │   │ System  │
    └────┬────┘   └────┬────┘   └────┬────┘
         │             │             │
         └─────────────┼─────────────┘
                       │
                       ▼
              ┌─────────────────┐
              │    Supabase     │
              │   PostgreSQL    │
              │   (External)    │
              └─────────────────┘
```
