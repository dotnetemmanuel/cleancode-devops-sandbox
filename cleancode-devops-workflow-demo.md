# Demo Script
- **Clone the empty repo from github**

> !Add link

- **Create a .NET 9 Console App via Rider/Visual Studio** - `CleanCodeDevOps.App`
> Test the Hello World
> Add to .gitignore
```
#Jetbrains
.idea/
```

- **Push changes to remote**

- **In the remote repo on GitHub, create**
`.github/workflows/dotnet-build.yml`

> Explain that this can also be done by simply adding a file to the project and then push it

- **Go through the different phases and versions of the workflow.**

## Simple workflow using expression in string
```yaml
name: Manual .NET Build

on:
  workflow_dispatch:

jobs:
  build:
    runs-on: ubuntu-latest

    steps:
      - name: 🔔 Start build process
        run: echo "Build process has started..."

      - name: 🧾 Show runtime context
        run: |
          echo "🔧 Runner OS: ${{ runner.os }}"
          echo "📦 Repository: ${{ github.repository }}"
          echo "📂 Workflow: ${{ github.workflow }}"
          echo "🚀 Triggered by: ${{ github.event_name }}"
          echo "👤 Actor: ${{ github.actor }}"
          echo "🔢 Run number: ${{ github.run_number }}"
          echo "🌿 Branch: ${{ github.ref }}"
```

---

## Include anchors for demo purposes
```yaml
name: Manual .NET Build

on:
  workflow_dispatch:

jobs:
  build:
    runs-on: ubuntu-latest

    steps:
      - name: 🔔 Start build process
        run: echo "Build process has started..."

      - &echo-template
        name: 🔔 Echo template
        run: echo "This is a reusable echo step."
        shell: bash # will persist unless explicitly overridden

      - name: 📝 Show runtime context
        run: |
          echo "🔧 Runner OS: ${{ runner.os}}"
          echo "📦 Repository: ${{ github.repository }}"
          echo "📂 Workflow: ${{ github.workflow }}"
          echo "🚀 Triggered by: ${{ github.event_name }}"
          echo "👤 Actor: ${{ github.actor }}"
          echo "🔢 Run number: ${{ github.run_number }}"
          echo "🌿 Branch: ${{ github.ref }}"

      - *echo-template
```

---

## Build project

> Show marketplace for e.g actions (to get the right version)

```yaml
name: Manual .NET Build

on:
  workflow_dispatch:

jobs:
  build:
    runs-on: ubuntu-latest

    steps:
      - &echo-template
        name: 🔔 Echo template
        run: echo "This is a reusable echo step."

      - name: 🧾 Show runtime context
        run: |
          echo "🔧 Runner OS: ${{ runner.os }}"
          echo "📦 Repository: ${{ github.repository }}"
          echo "📂 Workflow: ${{ github.workflow }}"
          echo "🚀 Triggered by: ${{ github.event_name }}"
          echo "👤 Actor: ${{ github.actor }}"
          echo "🔢 Run number: ${{ github.run_number }}"
          echo "🌿 Branch: ${{ github.ref }}"
          echo "🔗 Commit SHA: ${{ github.sha }}"

      - *echo-template

      - name: 📥 Checkout code
        uses: actions/checkout@v5

      - name: 🛠️ Install .NET 9
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '9.0.x'

      - name: 🏗️ Build project
        run: dotnet build

      - name: 🏃🏼‍♀️‍➡️ Run code
              run: dotnet run --project CleanCodeDevops.App/CleanCodeDevops.App.csproj
```

---
## Add "push-to-main" trigger
> Pull changes from remote

> Edit Hello World, push and see workflow run

```yaml
name: Manual and Push .NET Build

on:
  workflow_dispatch:
  push:
    branches:
      - main

jobs:
  build:
    runs-on: ubuntu-latest

    steps:
      - &echo-template
        name: 🔔 Echo template
        run: echo "This is a reusable echo step."

      - name: 🧾 Show runtime context
        run: |
          echo "🔧 Runner OS: ${{ runner.os }}"
          echo "📦 Repository: ${{ github.repository }}"
          echo "📂 Workflow: ${{ github.workflow }}"
          echo "🚀 Triggered by: ${{ github.event_name }}"
          echo "👤 Actor: ${{ github.actor }}"
          echo "🔢 Run number: ${{ github.run_number }}"
          echo "🌿 Branch: ${{ github.ref }}"
          echo "🔗 Commit SHA: ${{ github.sha }}"

      - *echo-template

      - name: 📥 Checkout code
        uses: actions/checkout@v3

      - name: 🛠️ Install .NET 9
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '9.0.x'

      - name: 🏗️ Build project
        run: dotnet build

      - name: 🏃🏼‍♀️‍➡️ Run code
              run: dotnet run --project CleanCodeDevops.App/CleanCodeDevops.App.csproj
```
