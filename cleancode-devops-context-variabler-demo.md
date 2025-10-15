# 🧪 Demo: Context & Variabler i GitHub Workflows
## 🎯 Syfte
Visa hur man använder olika typer av context (github, env, matrix, etc.) och hur man skapar och använder variabler i olika scope (workflow, job, step). Vi bygger upp ett workflow steg för steg.

## 🧩 Steg 1: Grundläggande workflow_dispatch med context-inspektion
Vi börjar med ett enkelt workflow som körs manuellt och skriver ut några grundläggande context-värden.
### `.github/workflows/context-demo.yml`
```yaml
name: Context Demo

on:
  workflow_dispatch:

jobs:
  show-context:
    runs-on: ubuntu-latest
    steps:
      - name: Print GitHub context
        run: echo "${{ toJSON(github) }}"

      - name: Print Job context
        run: echo "${{ toJSON(job) }}"

      - name: Print Runner context
        run: echo "${{ toJSON(runner) }}"
```

🔍 Här använder vi toJSON() för att skriva ut hela objektet för varje context. Det ger oss en bra överblick.

## 🧩 Steg 2: Skapa och använda env-variabler i olika scope
Vi utökar workflowet med miljövariabler på tre nivåer: workflow, job och step.
### `.github/workflows/env-demo.yml`
```yaml
name: Env Context Demo

on:
  workflow_dispatch:

env:
  WORKFLOW_LEVEL: "Workflow scope"

jobs:
  show-env:
    runs-on: ubuntu-latest
    env:
      JOB_LEVEL: "Job scope"
    steps:
      - name: Step with its own env
        env:
          STEP_LEVEL: "Step scope"
        run: |
          echo "Workflow level: $WORKFLOW_LEVEL"
          echo "Job level: $JOB_LEVEL"
          echo "Step level: $STEP_LEVEL"

      #implementera efter att visat att workflow fungerar
      - name: Try to access STEP_LEVEL1 in another step
            env:
              STEP_LEVEL2: "Step scope variable 2"
            run: |
              if [ -z "$STEP_LEVEL1" ]; then
                echo "STEP_LEVEL1 is not available in this step — fallback message shown."
              else
                echo "STEP_LEVEL1 is: $STEP_LEVEL1"
              fi
            # -z tests if a variable is eiher unset or empty
```

📌 Notera att varje env: block är isolerat — step-variabler är inte tillgängliga utanför sitt step.

## 🧩 Steg 3: Evaluera uttryck med operatorer och statusfunktioner
Vi lägger till ett exempel på if:-block med uttryck som använder operatorer och statusfunktioner.
### `.github/workflows/condition-demo.yml`
```yaml
name: Condition Demo

on:
  workflow_dispatch:

jobs:
  conditional-job:
    runs-on: ubuntu-latest
    steps:
      - name: Always runs
        run: echo "This always runs"

      - name: Runs only if previous step succeeded
        if: success()
        run: echo "Previous step succeeded"

      - name: Runs only if cancelled (will not run here)
        if: cancelled()
        run: echo "This runs only if cancelled"

      - name: Simulating a failing step
        run: |
          echo "Step fails intentionally"
          exit 1

      - name: Runs only if previous fails
        if: failure()
        run: echo "Previous fails, therefore I run"
```

🧠 Här visar vi hur success(), cancelled(), failure() och always() fungerar i if:-block.

## 🧩 Steg 4: Matrix-strategi med flera OS och .NET-versioner
Vi bygger ett jobb som körs i flera kombinationer av OS och .NET-versioner.
### .github/workflows/matrix-demo.yml
```yaml
name: Matrix Demo

on:
  workflow_dispatch

jobs:
  build:
    runs-on: ${{ matrix.os }}
    strategy:
      matrix:
        os: [ubuntu-latest, windows-latest]
        version: [9.0, 8.0]

    steps:
      - name: Setup .NET
        uses: actions/setup-dotnet@v5.0.0
        with:
          dotnet-version: ${{ matrix.version }}

      - name: Print maxtrix values
        run: |
          echo "Running on OS: ${{ matrix.os }}"
          echo "Using .NET version: ${{ matrix.version }}"

  post-build:
    needs: build
    runs-on: ubuntu-latest
    steps:
      - name: Post-build summary
        run: echo "All matrix builds are complete — running post-build job"
```

🧮 Detta skapar 6 parallella körningar — en för varje kombination av OS och version.
