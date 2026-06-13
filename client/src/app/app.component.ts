import { Component, OnDestroy, OnInit } from '@angular/core';
import { AuthService } from './services/auth.service';
import { CartService } from './services/cart.service';
import { Router } from '@angular/router';
import { Observable, Subject, of, timer } from 'rxjs';
import { catchError, switchMap, takeUntil } from 'rxjs/operators';
import { HealthService } from './services/health.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit, OnDestroy {
  title = 'client';
  userDisplayName: string | null = null;
  cartCount$: Observable<number>;
  isServerReady = false;
  isServerDelayed = false;

  private readonly maxWaitMs = 60000;
  private readonly retryIntervalMs = 3000;
  private healthCheckStartedAt = 0;
  private stopHealthCheck$ = new Subject<void>();
  private destroy$ = new Subject<void>();

  constructor(
    public authService: AuthService,
    private router: Router,
    private cartService: CartService,
    private healthService: HealthService
  ) {
    this.cartCount$ = this.cartService.getCartCount();
  }

  ngOnInit() {
    this.startHealthCheck();
  }

  ngOnDestroy() {
    this.stopHealthCheck$.next();
    this.destroy$.next();
    this.stopHealthCheck$.complete();
    this.destroy$.complete();
  }

  retryHealthCheck() {
    this.startHealthCheck();
  }

  private startHealthCheck() {
    this.stopHealthCheck$.next();
    this.isServerReady = false;
    this.isServerDelayed = false;
    this.healthCheckStartedAt = Date.now();

    timer(0, this.retryIntervalMs)
      .pipe(
        takeUntil(this.stopHealthCheck$),
        takeUntil(this.destroy$),
        switchMap(() =>
          this.healthService.checkHealth().pipe(
            catchError(() => of(false))
          )
        )
      )
      .subscribe((isHealthy) => {
        if (isHealthy) {
          this.isServerReady = true;
          this.refreshUserDisplayName();
          this.stopHealthCheck$.next();
          return;
        }

        const elapsedMs = Date.now() - this.healthCheckStartedAt;
        if (elapsedMs >= this.maxWaitMs) {
          this.isServerDelayed = true;
          this.stopHealthCheck$.next();
        }
      });
  }

  refreshUserDisplayName() {
    this.userDisplayName = this.authService.getDisplayName();
  }

  logout() {
    this.authService.logout();
    this.refreshUserDisplayName();
    this.router.navigateByUrl('/');
  }
}