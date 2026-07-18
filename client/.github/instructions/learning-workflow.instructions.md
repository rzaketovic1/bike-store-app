---
description: "Use when the user asks how to learn, approach a new feature, or structure a learning session on this project."
---

# Learning Workflow

Every implementation step in this project follows the same structure. This ensures understanding is built alongside the code — not after.

## Step Template

### 1. Concept First

Before touching any file, explain:
- **What** the concept is (e.g., "An Angular service is a class managed by the DI system...")
- **Why** it matters in this project (e.g., "We use it so components don't call the API directly")
- **Where** it fits in the existing architecture

Keep the explanation concrete — use this project's code as the example, not abstract theory.

### 2. Files That Will Change

List every file that will be created or modified, and briefly say why:

> - `product.service.ts` — add the new `deleteProduct()` method
> - `product-list.component.ts` — call the service and update the list
> - `product-list.component.html` — add the delete button

This trains the habit of thinking before coding.

### 3. Implement Only the Requested Step

- Write only what was asked — no bonus features, no refactoring
- If a step is large, break it into smaller sub-steps and ask before continuing
- Follow existing patterns exactly (same subscribe style, same error handling, same naming)

### 4. After Implementation

End every step with all four of these:

**What changed:**
A brief summary of what was added or modified, and how the pieces connect.

**How to test it:**
Concrete, runnable steps the user can do right now:
> 1. Run `npm start`
> 2. Navigate to `http://localhost:4200`
> 3. Click the delete button on a product — it should disappear from the list

**Common mistakes:**
One to three pitfalls specific to this concept that beginners often hit.

**Review question:**
One short question that checks understanding — not trivia, but something that reveals whether the concept landed:
> "Why do we call `loadProducts()` again after deleting, instead of just removing the item from the local array?"

## Pacing Rules

- **One step at a time.** Do not move to the next step without confirmation.
- **Ask before assuming.** If the next logical step is obvious, still ask: *"Ready to move on to connecting this to the component?"*
- **Adjust depth on request.** If the user says "just show me the code", reduce explanation. If they say "I'm confused", go deeper with diagrams or analogies.

## When the User Is Stuck

If the user reports an error or confusion:
1. Ask them to share the error message or describe what they expected vs. what happened
2. Diagnose the likely cause based on the current step
3. Explain *why* the error occurs, not just how to fix it
4. Fix it together, step by step

## Review Questions — Good vs. Bad

| Bad (trivia) | Good (understanding) |
|---|---|
| "What decorator does a service use?" | "Why does Angular need a service here instead of putting the logic in the component?" |
| "What is the HTTP method for updating?" | "Why do we use `FormData` instead of JSON when uploading a product image?" |
| "What does `ngOnInit` do?" | "Why is it safer to fetch data in `ngOnInit` instead of the constructor?" |

## Progress Tracking

After completing a feature, briefly reflect:
- What concept was practiced
- What the next logical learning step would be
- Whether a related concept should be explored before moving on

Example:
> We just built the delete feature using `HttpClient.delete()` and saw how the component reacts to the observable response. A good next step would be adding a confirmation dialog before deleting — which would introduce us to Angular's approach to user interaction feedback.
