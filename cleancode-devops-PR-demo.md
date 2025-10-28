Tack för förtydligandet, Emmanuel! Här kommer ett uppdaterat och pedagogiskt **demo-manus** som visar hur du använder Pull Requests i Git — från kloning till commit — med en **kort textfil** som innehåller namnen på medlemmarna i *The Fellowship of the Ring*.

---

## 🧪 **Demo: Skapa en PR med en enkel textfil**

### 🧱 **1. Klona repot**
```bash
git clone https://github.com/<ditt-användarnamn>/<ditt-repo>.git
cd <ditt-repo>
```
> 🔍 *Hämtar repot och går in i mappen.*

---

### 🌿 **2. Skapa en ny branch från `main`**
```bash
git checkout -b feature/fellowship-list main
```
> 🌱 *Skapar en ny branch för din ändring.*

---

### 📄 **3. Skapa en textfil med medlemmarna i Fellowship**
```bash
echo -e "The Fellowship of the Ring:\n- Frodo\n- Sam\n- Merry\n- Pippin\n- Aragorn\n- Legolas\n- Gimli\n- Gandalf\n- Boromir" > fellowship.txt
```
> ✍️ *Skapar en fil `fellowship.txt` med en enkel lista på engelska.*

---

### ✅ **4. Lägg till och committa ändringen**
```bash
git add fellowship.txt
git commit -m "Add list of Fellowship of the Ring members"
```
> 💾 *Stage:ar och committar filen med ett tydligt meddelande.*

---

### 🚀 **5. Push till GitHub**
```bash
git push -u origin feature/fellowship-list
```
> 🔗 *Pushar din branch till GitHub.*

---

### 🧷 **6. Skapa Pull Request**
> Gå till GitHub → repot → klicka på "Compare & pull request" → skriv titel och beskrivning → klicka "Create pull request".

> Visa runt i PR

> mergea in i main
---

### 🧷 **7. Skapa en PR-template**
 > hämta senaste main

 > brancha ut till feature/pr-template

 > skapa .github mapp i roten av repot

 > skapa `pull-request-template.md`

 > populera med mallen fråm Powertoys

 > Pusha och skapa PR => se mallen

---
