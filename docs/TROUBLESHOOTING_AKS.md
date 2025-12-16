# 🚑 Troubleshooting: "No Such Host" Error in GitHub Actions

## The Problem
Your GitHub Actions workflow failed with:
```
dial tcp: lookup solar-aks-solarrg-ce6542-ofzzoqh4.hcp.centralindia.azmk8s.io ... no such host
```

## The Cause
The **Kubernetes Cluster** that your GitHub Secret `KUBE_CONFIG` points to **does not exist anymore**.
This happens when:
1. You deleted and recreated the AKS cluster.
2. The GitHub Secret contains credentials for an old/mismatched cluster.

## The Solution
You need to **update the `KUBE_CONFIG` secret** in GitHub with the credentials for your **current** running cluster.

### Step 1: Get the New Kubeconfig
Go to [Azure Cloud Shell](https://shell.azure.com) (or your local terminal with Azure CLI) and run:

```bash
# 1. Get credentials for your CURRENT cluster
# Replace 'myResourceGroup' and 'myAKSCluster' with your actual names
az aks get-credentials --resource-group solarrg --name solar-aks --file kubeconfig.new

# 2. Verify it works
kubectl --kubeconfig=kubeconfig.new get nodes

# 3. Convert to Base64 (Required for GitHub Secrets)
cat kubeconfig.new | base64 -w 0
```
> **Note:** If `base64 -w 0` fails (on Mac), use `base64 -i kubeconfig.new`. The output should be a very long string.

### Step 2: Update GitHub Secret
1. Go to your GitHub Repository.
2. Click **Settings** → **Secrets and variables** → **Actions**.
3. Find `KUBE_CONFIG`.
4. Click the **Pencil Icon** (Edit).
5. **Delete** the old value and **Paste** the new Base64 string from Step 1.
6. Click **Update Secret**.

### Step 3: Re-run Workflow
1. Go to the **Actions** tab in GitHub.
2. Select the failed workflow run.
3. Click **Re-run jobs**.

---
**Tip:** If you are unsure of your cluster name, run `az aks list -o table` in Azure Cloud Shell to see all your clusters.
