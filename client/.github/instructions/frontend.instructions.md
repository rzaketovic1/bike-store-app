---
applyTo: "client/src/**/*.ts,client/src/**/*.html,client/src/**/*.scss"
description: "Use when working on the Angular frontend — components, services, routing, forms, or styling."
---

# Frontend Instructions (Angular 16)

## Project Context

Angular 16 SPA in `client/src/`. Bootstrap 5 for layout and UI components. ngx-toastr for notifications. Communicates with the .NET API at `http://localhost:5000/api/`.

## Mentor Approach

Before writing any Angular code:
1. Explain the Angular concept involved (e.g., reactive forms vs template-driven, services vs component state)
2. Identify which files will change and why
3. Connect the concept to how Angular's change detection or DI system is involved

After writing code:
- Describe what changed and why it works
- Explain how to test it in the browser (`npm start`, navigate to the route)
- Point out common Angular pitfalls for that concept
- Ask one review question

## Architecture Conventions

| Layer | Location | Responsibility |
|-------|----------|----------------|
| Components | `src/app/components/<name>/` | UI and user interaction |
| Services | `src/app/services/` | API calls, shared state |
| Models | `src/app/models/` | TypeScript interfaces (no classes) |
| Interceptors | `src/app/interceptors/` | Cross-cutting HTTP concerns |
| Routing | `src/app/app-routing.module.ts` | Route definitions |

Single `AppModule` — no lazy loading, no standalone components. All components must be declared in `AppModule`.

## Component Conventions

- Generate with: `ng generate component components/<name>`
- Always implement `OnInit` for data fetching; put API calls in `ngOnInit()`, not the constructor
- Use constructor **only** for dependency injection
- Track loading state with a `isLoading: boolean` flag
- Track errors with an `error: string` field; display inline in the template
- Use `subscribe()` with `{ next, error }` callbacks — do **not** use `.pipe(catchError(...))`

```typescript
// Preferred subscribe pattern
this.productService.getProducts().subscribe({
  next: (products) => {
    this.products = products;
    this.isLoading = false;
  },
  error: (err) => {
    this.error = err.error?.message || 'Something went wrong.';
    this.isLoading = false;
  }
});
```

## Service Conventions

- All HTTP calls belong in services, never in components
- Inject `HttpClient` via constructor
- Return `Observable<T>` — do not subscribe inside services
- Use `environment.apiUrl` for the base URL, never hardcode it

```typescript
// Preferred service pattern
constructor(private http: HttpClient) {}

getProducts(): Observable<Product[]> {
  return this.http.get<Product[]>(`${environment.apiUrl}Products/all`);
}
```

## Forms

- Use **template-driven forms** (`FormsModule`, `ngModel`) — already imported in `AppModule`
- Reference the form with `#form="ngForm"`
- Validate with built-in directives: `required`, `email`, `minlength`
- Access validity with `form.valid` and `form.submitted`
- Do not introduce Reactive Forms (`ReactiveFormsModule`) unless explicitly requested

## Authentication

- `AuthService` manages login, register, logout, and user state
- Token and user object stored in `localStorage` under key `'user'`
- `AuthInterceptor` automatically attaches `Authorization: Bearer <token>` to every HTTP request
- Check login state with `authService.isLoggedIn()`

## Routing

Current routes (`app-routing.module.ts`):

| Path | Component |
|------|-----------|
| `/` | `ProductListComponent` |
| `/product/:id` | `ProductDetailsComponent` |
| `/login` | `LoginComponent` |
| `/register` | `RegisterComponent` |

- Navigate programmatically with `Router.navigate(['/path'])`
- Read route params with `ActivatedRoute.snapshot.paramMap.get('id')`

## Styling

- Use Bootstrap 5 utility classes and grid system — avoid writing custom CSS unless necessary
- Component-scoped styles go in the `.component.scss` file
- Global styles go in `src/styles.scss`
- Use `ngx-toastr` for success/error notifications: inject `ToastrService`, call `this.toastr.success(...)` or `this.toastr.error(...)`

## Models

Define as plain TypeScript `interface`, not `class`. Keep in `src/app/models/`.

```typescript
// Existing interfaces
interface Product { id, name, description, price, pictureUrl, type, brand, quantityInStock }
interface Pagination { pageIndex, pageSize, totalCount, totalPages }
interface PaginatedResult<T> { data: T, pageIndex, pageSize, totalCount, totalPages }
```

## Common Mistakes to Watch For

- Forgetting to declare a new component in `AppModule`
- Subscribing inside a service instead of returning the `Observable`
- Putting API calls in the constructor instead of `ngOnInit`
- Using `*ngIf` and `*ngFor` on the same element (use `<ng-container>` to wrap)
- Not unsubscribing from long-lived observables (use `takeUntilDestroyed` or `async` pipe for those cases)
- Hardcoding the API URL instead of using `environment.apiUrl`
