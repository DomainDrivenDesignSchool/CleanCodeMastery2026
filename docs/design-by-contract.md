# Design by Contract (DbC)

> **Clear expectations  =>  explicit guarantees  =>  safer change**

Design by Contract is about making **implicit assumptions explicit**.

Code should not *hope* to be used correctly.  
It should **define the rules of engagement**.

---

← [Home](../README.md)

**Related Docs**
- [Clean Code, Smells & Refactoring (The Trilogy)](./clean-code-trilogy.md)
- [Biological Metaphor (Mental Model)](./biological-metaphor.md)
- [Impact / Effort Matrix for Code Smells](./impact-effort-matrix.md)
- [Design by Contract](./design-by-contract.md)

---

## 🧠 What Is Design by Contract?

Design by Contract (DbC) is a design approach where software elements
clearly state:

- **What they expect**
- **What they guarantee**
- **What must always remain true**

Every interaction becomes a **contract**, not a guess.

---

## 📜 The Three Pillars of a Contract

### 1️⃣ Preconditions (What I Expect)
Conditions that **must be true before** a method is called.

Examples:
- Input is not null
- Value is within a valid range
- Required state already exists

> ❌ If preconditions are violated  =>  the caller is wrong

---

### 2️⃣ Postconditions (What I Guarantee)
Conditions that **will be true after** the method finishes successfully.

Examples:
- Returned value is non-negative
- State has changed in a specific way
- Side effects are clearly defined

> ❌ If postconditions are violated  =>  the method is wrong

---

### 3️⃣ Invariants (What Always Holds)
Conditions that must **always be true** for an object or module.

Examples:
- An order total is never negative
- A user always has an ID
- A collection size matches its internal index

> Invariants define **identity and consistency**

---

## 🧼 Design by Contract & Clean Code

Clean Code is not about beauty - it is about **trust**.

Design by Contract makes that trust explicit.

| Clean Code Principle | Contract Connection |
|----------------------|---------------------|
| Readability | Rules are visible, not hidden |
| Predictability | Behavior is guaranteed |
| Simplicity | Fewer defensive checks everywhere |
| Intentionality | Code says *why*, not just *how* |

> Clean code reads like a promise - contracts make it enforceable.

---

## 👃 Design by Contract & Code Smells

Many code smells are actually **broken or missing contracts**.

### Common Smells Related to Contracts

- **Primitive Obsession**  =>  unclear expectations
- **Shotgun Null Checks**  =>  missing preconditions
- **Defensive Programming Everywhere**  =>  no trust boundaries
- **Long Parameter Lists**  =>  implicit assumptions
- **Temporal Coupling**  =>  hidden ordering contracts

> A smell often says: *“I don’t know how this is supposed to be used.”*

---

## 🔧 Design by Contract & Refactoring

Refactoring without contracts is risky.  
Contracts turn refactoring into a **safe operation**.

### Before Refactoring
- Identify current (implicit) contracts
- Make assumptions explicit

### During Refactoring
- Preserve contracts
- Strengthen invariants
- Simplify preconditions

### After Refactoring
- Contracts should be clearer, not more complex

> Refactor behavior **inside** the contract, not the contract itself.

---

## 🧬 Design by Contract in the Biological Metaphor

| Software Level | Biological Metaphor | Contract Role |
|---------------|--------------------|---------------|
| Field / Property | Atom | Value constraints |
| Method | Molecule | Preconditions & postconditions |
| Class / Service | Cell | Invariants |
| Module / Layer | Tissue | Interaction contracts |
| System | Organism | Global consistency rules |

Contracts prevent **local mutations** from killing the organism.

---

## 🧪 Contracts ≠ Tests (But They Work Together)

| Contracts | Tests |
|----------|-------|
| Define expectations | Verify behavior |
| Executed at runtime | Executed in test phase |
| Fail fast | Detect regressions |
| Guard boundaries | Validate scenarios |

> Contracts define *what must never happen*.  
> Tests explore *what should happen*.

---

## ⚠️ Common Myths

❌ “Design by Contract is just asserts”  
✅ Asserts are tools - contracts are design decisions

❌ “Contracts slow development”  
✅ They reduce debugging, rework, and fear of change

❌ “We have tests, we don’t need contracts”  
✅ Tests don’t protect runtime misuse

---

## 🧭 Practical Guideline

- Public APIs  =>  **strong contracts**
- Internal methods  =>  **lighter contracts**
- Core domain  =>  **strict invariants**
- Boundaries  =>  **fail fast**

> The more critical the code, the stronger the contract.

---

Origin & Philosophy of Design by Contract
-----------------------------------------

Design by Contract was introduced by **Bertrand Meyer**,the creator of the Eiffel programming language, in his seminal book:

> **Object-Oriented Software Construction**

Meyer’s core idea was simple but radical:

> _Correct software is not accidental - it is designed through explicit agreements._

In Eiffel, contracts were **language-level features**, not comments or conventions:

*   Preconditions
    
*   Postconditions
    
*   Class invariants
    

This matters because Design by Contract was never meant to be a testing trick.It was designed as a **language-driven design discipline**.

🧠 Design by Contract in Domain-Driven Design (DDD)
---------------------------------------------------

Design by Contract fits **naturally** into Domain-Driven Design.

In DDD, contracts are not technical checks - they are **domain rules**.

### How DbC Maps to DDD Concepts

DDD ConceptContract RoleValue ObjectConstructor enforces validityEntityInvariants preserve identityAggregate RootGatekeeper of all contractsDomain ServiceExplicit behavioral expectationsBounded ContextContract boundaryUbiquitous LanguageContracts are executable language

> In DDD, **invariants are business rules**, not implementation details.

Example:

> “An order total can never be negative”This is **not a test**.This is a **domain invariant**.

🗣 Design by Contract & Language-Driven Design (LDD)
----------------------------------------------------

Design by Contract is a **natural extension of LDD**.

*   Contracts turn language into **executable meaning**
    
*   Preconditions define _when_ language applies
    
*   Postconditions define _what_ language guarantees
    
*   Invariants define _what must always be true_
    

> Contracts are where **language becomes enforceable behavior**.

Without contracts:

*   Language drifts
    
*   Assumptions leak
    
*   Models decay
    

⚙️ Clean Code vs Design by Contract (Not Opposites)
---------------------------------------------------

Clean Code (popularized by **Robert C. Martin**) focuses on:

*   Readability
    
*   Simplicity
    
*   Expressive intent
    
*   Tests as safety nets
    

Design by Contract focuses on:

*   Explicit guarantees
    
*   Fail-fast behavior
    
*   Clear responsibility between caller and callee
    

They are **complementary**:

> Clean Code makes intent readableDesign by Contract makes intent enforceable

⚠️ Real-World Challenges of Design by Contract
----------------------------------------------

Let’s be honest - DbC is powerful, but not free.

### 1️⃣ Lack of Native Language Support

Most mainstream languages **do not support DbC natively**.

LanguageContract SupportEiffelNativeJava / C#Convention-basedJavaScriptRuntime-onlyGoIdiomatic resistanceRustStrong types, weak contractsPythonDynamic, fragile

Result:

> Contracts become _culture_, not _compiler-enforced rules_.

### 2️⃣ Performance & Runtime Cost

*   Runtime checks add overhead
    
*   Often disabled in production
    
*   Teams fear latency impact
    

This leads to:

> “We’ll add contracts later.”

Later rarely comes.

### 3️⃣ Where Should Contracts Live?

Common mistakes:

*   ❌ Hidden in comments
    
*   ❌ Scattered asserts
    
*   ❌ Mixed with business logic
    

Better approach:

*   Contracts at **boundaries**
    
*   Invariants in **core domain**
    
*   Minimal contracts internally
    

> Trust inside - verify at the edges.

🧭 Practical, Modern DbC Guideline
----------------------------------

A realistic DbC strategy today:

*   **Value Objects** → validate in constructor
    
*   **Aggregate Roots** → enforce invariants
    
*   **Public APIs** → guard clauses
    
*   **Internal logic** → trust contracts
    
*   **Tests** → verify contract behavior
    

> Contracts should be boring, predictable, and loud when broken.

🔑 Why DbC Belongs in This Repository
-------------------------------------

Including Design by Contract here signals that:

*   Clean Code is about **decisions**
    
*   Code Smells are **signals**
    
*   Refactoring is **change with confidence**
    
*   Contracts are **the safety rails**
    

This repository is not just about prettier code —it’s about **designing trust into software**.

---

## 🧠 Final Thought

Code is a conversation.  
Contracts make sure everyone speaks the **same language**.
