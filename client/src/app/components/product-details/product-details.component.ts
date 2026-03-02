import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Product } from 'src/app/models/product';
import { ProductService } from 'src/app/services/product.service';
import { environment } from '../../../environments/environment';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-product-details',
  templateUrl: './product-details.component.html',
  styleUrls: ['./product-details.component.scss']
})
export class ProductDetailsComponent implements OnInit {
  product: Product | null = null;
  selectedFile: File | null = null;
  brands: string[] = [];
  types: string[] = [];

  constructor(
    private route: ActivatedRoute,
    private productService: ProductService,
    private router: Router,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.loadProduct(id);
    this.loadBrands();
    this.loadTypes();
  }

  loadProduct(id: number): void {
    this.productService.getProduct(id).subscribe({
      next: res => this.product = res,
      error: err => {
        console.error('Product not found', err);
        this.toastr.error('Product not found', 'Error');
        this.router.navigate(['/']);
      }
    });
  }

  loadBrands(): void {
    this.productService.getBrands().subscribe({
      next: res => this.brands = res,
      error: err => console.error('Failed to load brands', err)
    });
  }

  loadTypes(): void {
    this.productService.getTypes().subscribe({
      next: res => this.types = res,
      error: err => console.error('Failed to load types', err)
    });
  }

  getImageUrl(pictureUrl: string): string {
  if (!pictureUrl) return '';

  if (/^https?:\/\//i.test(pictureUrl)) {
    return pictureUrl;
  }

  const origin = environment.apiUrl.replace(/\/api\/?$/i, '');

  const path = pictureUrl.startsWith('/') ? pictureUrl : `/${pictureUrl}`;

  return `${origin}${path}`;
}

  onFileSelected(event: any): void {
    const file = event.target.files[0];
    if (file) {
      this.selectedFile = file;
    }
  }

  saveChanges(): void {
    if (!this.product) return;

    const formData = new FormData();
    Object.entries(this.product).forEach(([key, value]) => {
      formData.append(key, value.toString());
    });

    if (this.selectedFile) {
      formData.append('image', this.selectedFile);
    }

    this.productService.updateProduct(this.product.id, formData).subscribe({
  next: () => {
    this.toastr.success('The product has been successfully updated.');
    this.selectedFile = null;
    this.loadProduct(this.product!.id);
  },
  error: (error) => {
    if (error.status === 400 && error.error?.errors) {
      const errorMessages = this.extractValidationErrors(error.error.errors);
      errorMessages.forEach(msg => this.toastr.error(msg, 'Validation Error'));
    } else {
      this.toastr.error(error.error?.title || 'Error updating product.', 'Error');
    }
  }
});
  }

  deleteProduct(): void {
    if (!this.product) return;

    if (confirm('Are you sure you want to delete the product?')) {
  this.productService.deleteProduct(this.product.id).subscribe({
    next: () => {
      this.toastr.success('The product has been successfully deleted.');
      this.router.navigateByUrl('/');
    },
    error: () => {
      this.toastr.error('Error deleting product.');
    }
  });
  }
  }

  private extractValidationErrors(errors: any): string[] {
    const messages: string[] = [];
    for (const field in errors) {
      if (errors.hasOwnProperty(field)) {
        const fieldErrors = errors[field];
        if (Array.isArray(fieldErrors)) {
          fieldErrors.forEach(error => messages.push(`${field}: ${error}`));
        }
      }
    }
    return messages;
  }
}