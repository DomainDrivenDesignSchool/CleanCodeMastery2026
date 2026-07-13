# Rambo Refactorers – Session 02

Welcome to **Session 02**!  
In this session, you’ll practice **detecting code smells**, **scoring them**, **mapping refactors**, and **applying Design by Contract (DbC)** principles.

---

## TASK 1 – Code Smell Hunting

**Goal:** Enhance the OrderService to handle a new loyalty discount feature while preserving correct calculation of discounts and tax.

### Feature Description:

- Loyalty discount: 5% of the base price for returning customers.
- Constraint: Total price cannot go below 0.
- Existing behavior: The service still calculates the total price including base discount and tax.
- Tax cap: Tax cannot exceed a maximum of 25,000.

> The service should now be able to calculate the **total price including discount and tax**, with the constraint that **tax cannot exceed a maximum value of 25,000**.

**Instructions:**

1. Review the current `OrderService` or `TaxService`.
2. Identify any **code smells** (duplicate logic, primitive obsession, long methods, unclear naming, etc.).
3. Make small modifications to implement the feature **without refactoring yet**.
4. Keep the code readable and document any assumptions.

**Output:**

- Updated service code
- A list of **all code smells you found**
- Short notes on why they are smells

---

## TASK 2 – Scoring Code Smells

**Goal:** Discuss each code smell in your group, and put them in the **Impact/Effort matrix**.

**Instructions:**

1. Take the list of smells you identified in TASK 1.
2. Discuss as a group:
   - How **urgent** is it to fix this smell?
   - How **hard** would it be to fix?
3. Place each smell in the matrix:

| Impact | Effort | Example Smells |
|--------|--------|----------------|
| High   | Low    | …              |
| High   | High   | …              |
| Low    | Low    | …              |
| Low    | High   | …              |

4. Prepare to **justify your decisions** in a 5-minute group discussion.

---

## TASK 3 – Scoring Refactoring Techniques

**Goal:** Discuss each refactoring technique in your group, and put them in the **Impact/Effort matrix**.

**Instructions:**

1. Review the **common refactoring techniques**:
   - Extract Method
   - Rename Variable / Method
   - Introduce Parameter Object
   - Move Method / Class
   - Replace Conditional with Polymorphism
2. Discuss:
   - Which techniques give **high impact** for minimal effort?
   - Which techniques are **complex but high-value**?
3. Fill out the matrix:

| Impact | Effort | Refactoring Techniques |
|--------|--------|----------------------|
| High   | Low    | …                    |
| High   | High   | …                    |
| Low    | Low    | …                    |
| Low    | High   | …                    |

---

## TASK 4 – Code Smell => Refactoring Mapping

**Goal:** Based on the **biological metaphor**, group together code smells and their corresponding refactorings in each layer of the architecture.

**Instructions:**

1. Map **code smells** and **refactorings** to the biological levels:

| Biological Layer | Example Code Smells | Suggested Refactoring |
|-----------------|------------------|---------------------|
| Atom (Field/Property) | … | … |
| Molecule (Method/Function) | … | … |
| Cell (Class/Service) | … | … |
| Tissue (Module/Layer) | … | … |
| Organism (System) | … | … |

2. Discuss **why this mapping makes sense**.
3. Be ready to present **1–2 examples** to the full group.

---

## TASK 5 – Design by Contract (DbC)

**Goal:** Make implicit assumptions explicit by adding **contracts** to your service.

**Instructions:**

1. Identify **preconditions** (what must be true before calling a method), e.g.:
   - `discount >= 0`
   - `basePrice >= 0`
2. Identify **postconditions** (what is guaranteed after method execution), e.g.:
   - `totalTax <= 25`
   - `totalPrice >= 0`
3. Identify **invariants** for your class or module:
   - `Order.TotalAmount >= 0`
   - `User.Id != null`
4. Implement **DbC checks** using validation in code.
5. Discuss:
   - How these contracts help **reduce code smells**
   - How contracts make **refactoring safer**

**Output:**

- Updated code with explicit preconditions, postconditions, and invariants
- Notes on which **code smells are mitigated** by these contracts

> Tip: Think of DbC as rules that protect your code from invalid usage.
---

## ✅ Submission

- Commit your code and notes to your **team branch**.
- Open a **PR to your team branch** with:
  - TASK 1–5 outputs
  - Filled **Impact/Effort matrices**
  - DbC notes
- Be ready to **present your findings** to the other teams.

---



---

## TASK 6 – Code Smell Hunting

**Goal:** Enhance the `OrderService` to handle a new loyalty discount feature while preserving correct calculation of discounts and tax.

### Feature Description:
- **Loyalty discount:** 5% of the base price for returning customers.
- **Constraint:** Total price cannot go below 0.
- **Existing behavior:** The service still calculates the total price including base discount and tax.
- **Tax cap:** Tax cannot exceed a maximum of 25,000.

The service should now be able to calculate the total price including discount and tax, with the constraint that tax cannot exceed a maximum value of 25,000.

### Instructions:
1. Review the current `OrderService` or `TaxService`.
2. Identify any code smells (duplicate logic, primitive obsession, long methods, unclear naming, etc.).
3. Make small modifications to implement the feature without refactoring yet.
4. Keep the code readable and document any assumptions.

### Output:
- Updated service code
- A list of all code smells you found
- Short notes on why they are smells

---

## TASK 7 – Scoring Code Smells

**Goal:** Discuss each code smell in your group, and put them in the Impact/Effort matrix.

### Instructions:
1. Take the list of smells you identified in TASK 1.
2. Discuss as a group:
   - How urgent is it to fix this smell?
   - How hard would it be to fix?
3. Place each smell in the matrix:

| Impact | Effort | Example Smells |
|--------|--------|----------------|
| High   | Low    | …              |
| High   | High   | …              |
| Low    | Low    | …              |
| Low    | High   | …              |

4. Prepare to justify your decisions in a 5-minute group discussion.

---

## TASK 8 – Scoring Refactoring Techniques

**Goal:** Discuss each refactoring technique in your group, and put them in the Impact/Effort matrix.

### Instructions:
1. Review the common refactoring techniques:
   - Extract Method
   - Rename Variable / Method
   - Introduce Parameter Object
   - Move Method / Class
   - Replace Conditional with Polymorphism
2. Discuss:
   - Which techniques give high impact for minimal effort?
   - Which techniques are complex but high-value?
3. Fill out the matrix:

| Impact | Effort | Refactoring Techniques |
|--------|--------|-------------------------|
| High   | Low    | …                      |
| High   | High   | …                      |
| Low    | Low    | …                      |
| Low    | High   | …                      |

---

## TASK 9 – Code Smell => Refactoring Mapping

**Goal:** Based on the biological metaphor, group together code smells and their corresponding refactorings in each layer of the architecture.

### Instructions:
1. Map code smells and refactorings to the biological levels:

| Biological Layer     | Example Code Smells | Suggested Refactoring |
|----------------------|----------------------|------------------------|
| Atom (Field/Property)| …                    | …                      |
| Molecule (Method/Function) | …             | …                      |
| Cell (Class/Service) | …                    | …                      |
| Tissue (Module/Layer)| …                    | …                      |
| Organism (System)    | …                    | …                      |

2. Discuss why this mapping makes sense.
3. Be ready to present 1–2 examples to the full group.

---

## TASK 10 – Design by Contract (DbC)

**Goal:** Make implicit assumptions explicit by adding contracts to your service.

### Instructions:
1. Identify **preconditions** (what must be true before calling a method), e.g.:
   - `discount >= 0`
   - `basePrice >= 0`
2. Identify **postconditions** (what is guaranteed after method execution), e.g.:
   - `totalTax <= 25,000`
   - `totalPrice >= 0`
3. Identify **invariants** for your class or module:
   - `Order.TotalAmount >= 0`
   - `User.Id != null`
4. Implement DbC checks using validation in code.
5. Discuss:
   - How these contracts help reduce code smells
   - How contracts make refactoring safer

### Output:
- Updated code with explicit preconditions, postconditions, and invariants
- Notes on which code smells are mitigated by these contracts

**Tip:** Think of DbC as rules that protect your code from invalid usage.

---

## 💡 Pro Tips:
- Keep changes small and incremental
- Document assumptions clearly
- Use unit tests to validate contracts where possible
- Reference the biological metaphor when discussing smells and refactors

---

## Completed Findings

### TASK 6 – Code Smell Hunting Results

The loyalty discount feature was added with the smallest practical change:

- Returning customers receive a 5% loyalty discount from the base price.
- Standard customers still receive the entered discount amount.
- Discounted price and final total are clamped so they cannot go below 0.
- Tax is calculated from the discounted price.
- Tax is capped at 25,000.
- Basic Design by Contract checks were added for negative base price, discount, and tax ratio.

### Code Smell Cards

| # | Code Smell | Where It Appears | Why It Is a Smell | Refactoring Path | Impact | Effort |
|---|------------|------------------|-------------------|------------------|--------|--------|
| 1 | Magic numbers | `0.05M`, `50M`, tax cap values | Business rules are hidden in raw numeric values, which makes policy changes risky and easy to miss. | Replace Magic Number with Named Constant or configuration. | High | Low |
| 2 | Guard logic in the wrong place | Input validation originally lived in `Main` | Domain invariants should protect the domain even when code is called from somewhere other than the console. | Move Guard Clauses into `Order`, `TaxService`, and request validation. | High | Low |
| 3 | Conditional complexity | `if/else` for returning customer vs standard customer | Loyalty and standard discount behavior are separate pricing policies but are represented as a boolean branch. | Replace Conditional with Polymorphism, e.g. `IDiscountPolicy`. | High | High |
| 4 | Unclear naming | `CalculateDiscount` returns price after discount; `customerReturn` was unclear | Names describe the implementation poorly, so callers can misunderstand what value is returned or what the flag means. | Rename Method / Variable, e.g. `CalculatePriceAfterDiscount`, `isReturningCustomer`. | Low | Low |
| 5 | Primitive obsession | `decimal` values and `bool` flags represent money, tax ratio, discount, and customer type | Important business concepts have no type boundaries, so invalid combinations are easy to pass around. | Introduce Value Objects and Parameter Objects. | High | High |
| 6 | Long procedural composition | `Main` reads input, validates, builds domain objects, calculates discount, calculates tax, and prints output | One method coordinates too many responsibilities, making changes harder to isolate. | Extract Method or introduce an application service. | Low | Low |
| 7 | Anemic domain object | `Order` only stores `BasePrice` and `Discount` | The object holds business data but originally did not protect its own invariants or behavior. | Move domain rules closer to `Order`; add invariants in constructor. | High | Low |
| 8 | Misplaced responsibility | Discount and tax rules are split across ad hoc services and console flow | Pricing policy is scattered, so total price rules are not represented as one coherent calculation. | Move Method / Class, introduce `PricingService` or strategy-based policies. | High | High |

### TASK 7 – Code Smell Impact/Effort Matrix

| Impact | Effort | Example Smells |
|--------|--------|----------------|
| High | Low | Magic numbers; Guard logic in the wrong place; Anemic domain object |
| High | High | Conditional complexity; Primitive obsession; Misplaced responsibility |
| Low | Low | Unclear naming; Long procedural composition |
| Low | High | None identified for this exercise |

### TASK 8 – Refactoring Technique Impact/Effort Matrix

| Impact | Effort | Refactoring Techniques |
|--------|--------|-------------------------|
| High | Low | Replace Magic Number with Named Constant; Move Guard Clauses; Rename Method / Variable where names block understanding |
| High | High | Replace Conditional with Polymorphism; Introduce Value Objects; Introduce Parameter Object |
| Low | Low | Extract small helper methods from `Main`; formatting and local cleanup |
| Low | High | Broad service reshaping without changing the pricing model |

### TASK 9 – Biological Mapping

| Biological Layer | Example Code Smells | Suggested Refactoring |
|------------------|---------------------|-----------------------|
| Atom (Field/Property) | Magic numbers; primitive money/rate values | Named constants; value objects |
| Molecule (Method/Function) | Unclear method names; long procedural calculation flow | Rename Method; Extract Method |
| Cell (Class/Service) | Anemic `Order`; misplaced discount logic | Move Method; add class invariants |
| Tissue (Module/Layer) | Pricing rules split across console flow, discount service, and tax service | Introduce application/domain service boundary |
| Organism (System) | Customer pricing policy modeled as boolean branching | Replace Conditional with Polymorphism; strategy-based pricing policies |

This mapping works because smaller smells usually affect local comprehension first, while larger smells affect where business behavior lives and how safely the system can grow.

### TASK 10 – Design by Contract Notes

Preconditions added:

- `basePrice >= 0`
- `discount >= 0`
- `taxRatio >= 0`
- Tax calculation input price must be non-negative.

Postconditions enforced:

- Discounted price cannot go below 0.
- Final total cannot go below 0.
- Tax cannot exceed 25,000.

Invariants protected:

- `Order.BasePrice` is always non-negative.
- `Order.Discount` is always non-negative.
- `TaxService` always applies the configured tax cap.

Contracts mitigate the guard-location, anemic-domain, primitive-value, and magic-number smells by making the pricing assumptions explicit. They also make later refactoring safer because extracted services or polymorphic discount policies must preserve the same observable rules.
