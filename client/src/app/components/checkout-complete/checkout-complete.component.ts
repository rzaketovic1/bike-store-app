import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-checkout-complete',
  templateUrl: './checkout-complete.component.html',
  styleUrls: ['./checkout-complete.component.scss']
})
export class CheckoutCompleteComponent {
  constructor(private router: Router) {}

  continueShopping(): void {
    this.router.navigateByUrl('/');
  }
}
