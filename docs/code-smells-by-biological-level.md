# Code Smells by Biological Level
← [Home](../README.md)

**Related Docs**
- [Clean Code, Smells & Refactoring (The Trilogy)](./clean-code-trilogy.md)
- [Biological Metaphor (Mental Model)](./biological-metaphor.md)
- [Impact / Effort Matrix for Code Smells](./impact-effort-matrix.md)
- [Design by Contract](./design-by-contract.md)

---

### A Systematic Catalog Using the Biological Metaphor

---

## What Is a Code Smell?

A **Code Smell** is not a bug.  
It is a **signal** — an indicator that something *might* be wrong with the design.

- Smells do not mean the code is broken
- Smells suggest increased risk
- Smells indicate harder change in the future

> Smells are symptoms, not diagnoses.

Not every smell requires refactoring.  
But **every refactoring should start from a smell or a clear intent**.

---

## Why Classifying Smells by Level Matters

Most teams fail at refactoring because:
- They detect a smell
- But fix it at the wrong level

Example:
- Trying to “optimize architecture” to fix a long method
- Or renaming variables to solve a domain boundary issue

Using the biological metaphor helps us:
- Detect *where* the problem lives
- Choose the *right* refactoring scope
- Avoid over-engineering

---

## Atom Level Smells (Field / Property / Variable)

Atoms are the smallest semantic units.  
Smells here are **linguistic and semantic**.

### Common Atom-Level Smells

#### 1. Mysterious Name
- Variables like `x`, `data`, `temp`, `value`
- No domain meaning

**Impact**: Confusion propagates upward  
**Fix**: Rename using domain language (LDD)

---

#### 2. Primitive Obsession
- Using primitives instead of domain concepts
- `string email`, `decimal money`, `int status`

**Impact**: Loss of invariants  
**Fix**: Introduce Value Objects

---

#### 3. Boolean Blindness
- Multiple boolean flags with unclear meaning

```csharp
process(true, false, true);
```
**Impact**: Impossible to reason about behavior
**Fix**:Replace with explicit types or enums

---

#### 4. Mutable Shared State
- Fields modified from multiple places

**Impact**: Hidden coupling
**Fix**:Reduce scope, enforce immutability

---

### Molecule Level Smells (Method / Function)
This is where most smells appear first.

Common Molecule-Level Smells

#### 5. Long Method
- Too many responsibilities

- Multiple abstraction levels

**Impact**: Low readability, high risk
**Fix**:Extract Method, clarify intent

---

#### 6. Deep Nesting
- if inside if inside if

**Impact**: Cognitive overload
**Fix**:Guard clauses, early returns

---

#### 7. Hidden Side Effects
- Method name suggests query, but mutates state
```
getUserAndUpdateCache()
```

**Impact**: Violates trust
**Fix**:Command–Query Separation

---

#### 8. Long Parameter List
- Too many arguments

**Impact**: Fragile method signatures
**Fix**:Introduce Parameter Object

### Cell Level Smells (Class / Service)
- Cells represent responsibility boundaries.

Common Cell-Level Smells

---

#### 9. God Class
- Knows too much
- Does too much

**Impact**: Change ripple everywhere
**Fix**:Split by responsibility

---

#### 10. Feature Envy
- Class uses another class’s data excessively

**Impact**: Wrong ownership
**Fix**:Move behavior closer to data

---

#### 11. Anemic Domain Model
- Data-only classes
- Logic pushed to services

**Impact**: Loss of domain expressiveness
**Fix**:Move behavior into domain objects

---

#### 12. Inappropriate Intimacy
- Classes depend on internals of each other


**Impact**: Tight coupling
**Fix**:Redesign interfaces

---

### Tissue Level Smells (Module / Layer)
- Tissues define system structure and flow.

Common Tissue-Level Smells
#### 13. Cyclic Dependencies
- Modules depend on each other directly or indirectly

**Impact**: Impossible to change safely
**Fix**:Dependency inversion, boundary redesign

---

#### 14. Leaky Abstractions
- Lower-level details exposed upward

**Impact**: Fragile layers
**Fix**:Stronger contracts, better encapsulation

---

#### 15. Cross-Layer Violations
- UI accessing persistence directly
- Domain calling infrastructure

**Impact**: Architecture erosion
**Fix**:Enforce layer boundaries

---

### Organism Level Smells (System)
- System-level smells are structural and cultural.

Common Organism-Level Smells

#### 16. Big Ball of Mud
- No clear architecture
- Everything connected to everything

**Impact**: Fear-driven development
**Fix**:Incremental architectural refactoring

---

#### 17. Inconsistent Domain Language
- Same concept named differently across system

**Impact**: Communication breakdown
**Fix**:Ubiquitous Language (DDD / LDD)

---

#### 18. No Clear Ownership
- Nobody knows who owns what

**Impact**: Stagnation
**Fix**:Define bounded contexts and responsibility

---

### Smells Propagate Upward
A critical principle:

> Smells ignored at lower levels amplify at higher levels

Ignored At	Becomes
Atom	Confusing logic
Molecule	God classes
Cell	Coupled modules
Tissue	Fragile system
Key Takeaways
Smells are signals, not enemies

Each smell belongs to a level. Refactoring must target the correct biological layer

Clean Code is achieved by consistency across all levels. You don’t refactor blindly.
You refactor with biological awareness.

