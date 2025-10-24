# 🚀 Code Along: Blazor Server App with GitHub Actions + Render (Staging → Blue-Green)

## 🟢 Del 1: Grundflöde (Staging → Manuell Promotion)
---

### 🔹 Step 0: Create a Render Account
1. Go to [https://render.com](https://render.com)
2. Click **Sign Up**
3. Choose **GitHub** as your login method (recommended)
4. Authorize Render to access your GitHub repositories

---

### 🔹 Step 1: Prepare Your GitHub Repo
Make sure your Blazor Server app is in a GitHub repo with this structure:

```
/MySolution
  /BlazorApp
    BlazorApp.csproj
    Program.cs
    ...
```

Push this to GitHub if it’s not already there.

---

### 🔹 Step 2: Create Two Render Services

#### 🟡 A. Staging Environment
1. In Render, click **“New Web Service”**
2. Select your GitHub repo
3. Set:
   - **Name**: `blazor-staging`
   - **Environment**: .NET Core
   - **Branch**: `main`
   - **Build Command**:
     ```bash
     dotnet publish MySolution/BlazorApp/BlazorApp.csproj -c Release -o out
     ```
   - **Start Command**:
     ```bash
     dotnet out/BlazorApp.dll
     ```
   - **Plan**: Free

#### 🟢 B. Production Environment (Blue-Green)
Repeat the steps above, but:
- **Name**: `blazor-production`
- **Auto-deploy**: **Disable**
- You’ll promote manually from staging

---

### 🔹 Step 3: Create GitHub Action for Staging

Create `.github/workflows/deploy-staging.yml` in your repo:

```yaml
name: Deploy to Render Staging

on:
  push:
    branches:
      - main

jobs:
  build:
    runs-on: ubuntu-latest

    steps:
      - name: Checkout code
        uses: actions/checkout@v3

      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '8.0.x'

      - name: Restore dependencies
        run: dotnet restore MySolution/BlazorApp/BlazorApp.csproj

      - name: Build
        run: dotnet build MySolution/BlazorApp/BlazorApp.csproj --configuration Release

      - name: Publish
        run: dotnet publish MySolution/BlazorApp/BlazorApp.csproj -c Release -o out

      - name: Notify Render
        run: echo "Render auto-deploys staging from main"
```

✅ This satisfies: **“Skapa en GitHub Action från GitHub”** and **“Deploy till Staging”**

---

### 🔹 Step 4: Promote to Production (Blue-Green)

Once staging is verified:

#### Option A: Manual Promotion
1. Go to `blazor-production` in Render
2. Click **Manual Deploy**
3. Choose the latest commit from `main`

#### Option B: GitHub Action Trigger (Optional)
Create `.github/workflows/promote-to-prod.yml`:

```yaml
name: Promote to Production

on:
  workflow_dispatch:

jobs:
  notify:
    runs-on: ubuntu-latest
    steps:
      - name: Manual deploy reminder
        run: echo "Go to Render and manually deploy blazor-production"
```

✅ This satisfies: **“Byt målmiljö, blue-green”**


## 🧠 Del 2: Automatisering med Deploy Hook
---

### 🔹 Steg 0: Förutsättningar (samma som Del 1)
- GitHub-repo med Blazor Server-app i t.ex. `MySolution/BlazorApp`
- Två Render-tjänster:
  - `blazor-staging` (auto-deploy från `main`)
  - `blazor-production` (manuell deploy, blue-green)

---

### 🔹 Steg 1: Skapa Deploy Hook i Render

1. Gå till `blazor-production` i Render
2. Klicka på **Manual Deploy** → **Create Deploy Hook**
3. Kopiera den genererade URL:n (t.ex. `https://api.render.com/deploy/srv-xxxxxx?key=yyyyyy`)

---

### 🔹 Steg 2: Lägg till GitHub Secret

1. Gå till GitHub → Repo → **Settings** → **Secrets and variables**
2. Skapa en ny secret:
   - **Name**: `RENDER_DEPLOY_HOOK_PROD`
   - **Value**: din deploy hook URL

---

### 🔹 Steg 3: Uppdatera Staging Workflow

Skapa eller uppdatera `.github/workflows/deploy-staging.yml`:

```yaml
name: Deploy to Staging and Promote to Production

on:
  push:
    branches:
      - main

jobs:
  deploy:
    runs-on: ubuntu-latest

    steps:
      - name: Checkout code
        uses: actions/checkout@v3

      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '8.0.x'

      - name: Restore dependencies
        run: dotnet restore MySolution/BlazorApp/BlazorApp.csproj

      - name: Build
        run: dotnet build MySolution/BlazorApp/BlazorApp.csproj --configuration Release

      - name: Publish
        run: dotnet publish MySolution/BlazorApp/BlazorApp.csproj -c Release -o out

      - name: Notify Render (Staging)
        run: echo "Render auto-deploys staging from main"

      - name: Promote to Production (Blue-Green)
        run: curl -X POST ${{ secrets.RENDER_DEPLOY_HOOK_PROD }}
```

✅ Detta gör att varje push till `main`:
- Bygger och publicerar Blazor-appen
- Deployar till `blazor-staging`
- Triggar deploy till `blazor-production` automatiskt


### 🔹 Steg 4: (Valfritt) Automatisk Promotion efter Staging Deploy
Vill du att produktion ska uppdateras direkt efter staging, lägg till detta i slutet av din staging workflow:
```yaml
- name: Promote to Production
  run: curl -X POST ${{ secrets.RENDER_DEPLOY_HOOK_PROD }}
```
---

### 🔹 Steg 5: (Valfritt) `.render.yaml` för IaC
```yaml
services:
  - type: web
    name: blazor-staging
    env: dotnet
    branch: main
    buildCommand: dotnet publish MySolution/BlazorApp/BlazorApp.csproj -c Release -o out
    startCommand: dotnet out/BlazorApp.dll
    plan: free

  - type: web
    name: blazor-production
    env: dotnet
    branch: main
    autoDeploy: false
    buildCommand: dotnet publish MySolution/BlazorApp/BlazorApp.csproj -c Release -o out
    startCommand: dotnet out/BlazorApp.dll
    plan: free
```

---

### ✅ Sammanfattning: Automatiserad CD

| Moment i föreläsningen             | Teknik i code along                                     |
|------------------------------------|----------------------------------------------------------|
| **Skapa GitHub Action**            | `deploy-staging.yml` med build + publish                |
| **Deploy till Staging**            | Render auto-deployar `blazor-staging` från `main`       |
| **Byt målmiljö, blue-green**       | Deploy Hook triggar `blazor-production` automatiskt     |

---
