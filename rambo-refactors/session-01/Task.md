# Rambo Refactors – Session 01

## TASK 1 – Refactor Practice

**Goal:** Identify and refactor bad code patterns in the FastAPI API.

### Target files

- [`routing.py` – FastAPI](https://github.com/fastapi/fastapi/blob/f2687dc1bb01f4cb62eee90e656b61c38c2aaf6a/fastapi/routing.py#L4)
- [`utils.py` – FastAPI](https://github.com/fastapi/fastapi/blob/f2687dc1bb01f4cb62eee90e656b61c38c2aaf6a/fastapi/dependencies/utils.py#L4)

### Instructions

1. Identify code smells:  
   - Long functions  
   - Duplication  
   - Unclear variable names  
   - Implicit dependencies  
2. Document your findings in `code_smells.md`.  
3. Apply safe refactorings using small commits.  
4. Each PR should be reviewable by team mates, focused on **one concrete refactor at a time**.

### Output

- `code_smells.md` with smell + refactor plan  
- PRs with small, safe refactors

---

## TASK 2 – Refactoring Checklist

**Goal:** Create a checklist specifically for safe refactoring and technical debt reduction.

### Instructions

1. Review your findings from Task 1.  
2. Create concrete rules for:  
   - Refactoring safely  
   - Maintaining tests  
   - Avoiding regressions  
   - Incremental improvements  
3. Write in `refactor_checklist.md`.

### Output

- `refactor_checklist.md`  
- Concrete examples of **before/after** code snippets
