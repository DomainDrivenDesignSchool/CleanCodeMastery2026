# Yoda Architects – Session 01

## TASK 1 – Responsibility & Modeling

**Goal:** Analyze and improve architecture, naming, and responsibility assignment in popular libraries.

### Target files

- [`DefaultListableBeanFactory.java` – Spring](https://github.com/spring-projects/spring-framework/blob/main/spring-beans/src/main/java/org/springframework/beans/factory/support/DefaultListableBeanFactory.java#L1025)
- [`BeanDefinitionParserDelegate.java` – Spring](https://github.com/spring-projects/spring-framework/blob/main/spring-beans/src/main/java/org/springframework/beans/factory/xml/BeanDefinitionParserDelegate.java#L550)
- [`UsernamePasswordAuthenticationFilter.java` – Spring Security](https://github.com/spring-projects/spring-security/blob/main/web/src/main/java/org/springframework/security/web/authentication/UsernamePasswordAuthenticationFilter.java#L150)
- [`DefaultAuthenticationEventPublisher.java` – Spring Security]()

### Instructions

Identify naming, modularity, responsibility issues.  
Propose improvements in `code_smells.md`:  
- Meaningful names  
- Responsibility assignment  
- Modular decomposition  
- Single Responsibility violations  

### Output

- `code_smells.md`  
- Refactored snippet examples (optional, if applicable)

---

## TASK 2 – Technical Debt Auction

**Goal:** Prioritize and discuss technical debt items using a **2x2 matrix**:

| Impact / Effort | **Low Impact** | **High Impact** |
|-----------------|----------------|-----------------|
| **Easy**        | 3rd priority   | **1st priority** |
| **Hard**        | 4th priority   | **2nd priority** |

### Example debts

- Global variables scattered  
- 2000-line god class  
- Critical paths without tests  
- Copy-paste code ×15  

### Instructions

1. Each team ranks the debts using the matrix.  
2. Document reasoning in `technical_debt_ranking.md`.  
3. Discuss trade-offs and strategies.

---

## TASK 3 – Clean Code Manifesto (30 min)

**Goal:** Each team defines **5 concrete Clean Code principles** from their perspective.

### Instructions

1. Discuss in team  
2. Write manifesto in `clean_code_manifesto.md`  
3. Each principle: **1–2 sentences + rationale**

### Output

- `clean_code_manifesto.md`
