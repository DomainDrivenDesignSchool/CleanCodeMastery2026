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

💡 **Pro Tips:**

- Keep changes **small and incremental**
- Document **assumptions** clearly
- Use **unit tests** to validate contracts where possible
- Reference the **biological metaphor** when discussing smells and refactors
