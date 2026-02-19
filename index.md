# Clean Code Mastery Workshop - 2026

Welcome to the **Clean Code Mastery Workshop** repository!  
This repo contains all practical exercises, refactors, and examples used throughout the workshop.

---

## 📚 Core Concepts & Reading Path

This workshop is built on a simple but powerful trilogy:

> **Clean Code  =>  Code Smells  =>  Refactoring**

To support this, we provide three foundational documents.  
Participants are **strongly encouraged** to read them progressively.

### 1️⃣ Clean Code, Smells & Refactoring (The Trilogy)
📄 [`docs/clean-code-trilogy.md`](./docs/clean-code-trilogy.md)

- What *Clean Code* really means (beyond formatting)
- Code Smells as **signals**, not bugs
- Refactoring as a **disciplined, intentional activity**
- The relationship between decisions, smells, and change

---

### 2️⃣ The Biological Metaphor (Mental Model)
📄 [`docs/biological-metaphor-for-clean-code.md`](./docs/biological-metaphor-for-clean-code.md)

We use a biological metaphor to reason about software structure and decay:

| Software Concept | Biological Metaphor |
|------------------|---------------------|
| Field / Property | Atom |
| Method / Function | Molecule |
| Class / Service | Cell |
| Module / Layer | Tissue |
| System | Organism |

This metaphor helps:
- Detect smells at the *right level*
- Avoid over-refactoring the wrong abstraction
- Reason about system health, not just code style

---

### 3️⃣ Impact / Effort Matrix for Code Smells
📄 [`docs/impact-effort-matrix.md`](./docs/impact-effort-matrix.md)

Not every smell should be refactored.

This document introduces:
- Prioritization of refactoring work
- Quick wins vs strategic refactors
- When **not** to refactor
- How to balance risk, value, and effort

---

### 4️⃣ Design by Contract
📄 [`docs/design-by-contract.md`](./docs/design-by-contract.md)

Design by Contract makes **implicit assumptions explicit** and turns
clean code intentions into **enforceable guarantees**.

This document covers:

- **Preconditions** – what must be true before a method is called  
- **Postconditions** – what the method guarantees after execution  
- **Invariants** – rules that must *always* hold for a domain concept  

It also explores:

- **Trust boundaries in code** (where to validate, where to trust)
- How missing or broken contracts manifest as **code smells**
- Why **safe refactoring depends on stable contracts**
- How contracts strengthen **DDD models and ubiquitous language**
- The role of contracts alongside **tests (not instead of them)**

> Clean Code builds trust through readability.  
> Design by Contract protects that trust through rules.


---

### 📌 How to Use These Documents

- **Before coding:** Read to align mental models
- **During reviews:** Reference concepts explicitly
- **During refactoring:** Justify *why* this change, *why now*

> Refactoring without intent is just rewriting.

---
## 🎭 Epic Teams

Three legendary teams - each with their unique superpower:

### 🐱 Garfield Grumblers (3 members)
> **Motto:** “This code stinks worse than Mondays.”  
> **Focus:** Code smell detection, readability, checklists

### 🟢 Yoda Architects (4 members)
> **Motto:** “Do. Or do not. There is no sloppy code.”  
> **Focus:** Meaningful naming, responsibility, modeling

### 💥 Rambo Refactors (4 members)
> **Motto:** “Painless refactor - guaranteed.”  
> **Focus:** Safe refactoring, debt elimination, bold changes

---

## 🛠 Branch Structure

```
main
├── session-01/
│ ├── garfield-grumblers/
│ ├── yoda-architects/
│ └── rambo-refactors/
├── session-02/
└── examples/
└── smells/
```

**Rule:** Never push directly to `main`.  
Work only within your **team branch**.

---

##   Pull Request Rules

PRs should be **constructive**, **human-friendly**, and **discussion-driven**.

---

## ✅ How to Submit a PR

1. From your team branch, create a new branch:  
   `feature/session-XX-taskname`
2. Keep commits **small** and **meaningful**.
3. Open the PR **to your team branch** (not `main`).
4. Fill out the **PR template** completely.

---

### 📝 PR Template

**What Changed?**  
_1–2 sentences summarizing the refactor or fix._

**Why? (Your Team Focus)**  
_Which smell/responsibility/debt are you tackling? How does your team’s superpower apply?_

**Risks / Trade-offs**  
_Potential issues, side effects, or tough decisions._

**Questions for Reviewers**  
_Prompt discussions like: “Extract or inline?” “Better naming?”_

---

## 🔄 Cross-Team Review (Mandatory)

No team reviews their own PR!  
Review rotation is as follows:

| PR From | Reviewed By |
|----------|--------------|
| Rambo | Yoda |
| Yoda | Garfield |
| Garfield | Rambo |

**Review Style:** Be *curious*, not *commanding*.

❌ “This is wrong.”  
✅ “What smell does this target? Why this approach?”

Each reviewer must leave **at least 3 comments**, checking for:

- ✅ **Why:** Do you understand the reasoning?
- ✅ **Risk:** Any side effects?
- ✅ **Alternative:** Is there a simpler solution?
- ✅ **Team Focus:** Did they apply their superpower?

---

## 📅 Session Workflow

### 🔴 BEFORE (30–60 min prep)
- Read the sample code.  
- Answer prep questions in `notes/prep-session-XX.md`.  
- Each team lists **one** smell or responsibility issue.

---

## 🧭 Team Roles

| Role | Responsibility |
|------|----------------|
| **Driver** | Types the code |
| **Navigator** | Guides design direction |
| **Spotter** | Hunts for code smells |
| **Challenger** | Constantly asks “Why this way?” |

---

## 💡 Pro Tips

- Small commits = easier reviews
- Write for **humans first**, machines second
- Use **VS Code Live Share** for pair programming

---

🚀 **Let’s Write Legendary Code!**  
Garfield Grumblers, Yoda Architects, Rambo Refactors,

Code so clean, **Uncle Bob would approve.**!
