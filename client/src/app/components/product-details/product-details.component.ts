import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Product } from 'src/app/models/product';
import { ProductService } from 'src/app/services/product.service';
import { environment } from '../../../environments/environment';

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
    private router: Router
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
      error: err => console.error('Product not found', err)
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

  // Ako je već pun URL (http/https), vrati ga
  if (/^https?:\/\//i.test(pictureUrl)) {
    return pictureUrl;
  }

  // Ukloni "/api" sa kraja apiUrl (ako postoji)
  const origin = environment.apiUrl.replace(/\/api\/?$/i, '');

  // Dodaj / ispred pictureUrl ako ga nema
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
        alert('Product updated successfully!');
        this.selectedFile = null;

        // 🔄 Ponovno učitaj proizvod da bi se prikazala nova slika
        this.loadProduct(this.product!.id);
      },
      error: () => {
        alert('Failed to update product');
      }
    });
  }

  deleteProduct(): void {
    if (!this.product) return;

    if (confirm('Are you sure you want to delete this product?')) {
      this.productService.deleteProduct(this.product.id).subscribe({
        next: () => {
          alert('Product deleted successfully!');
          this.router.navigateByUrl('/'); // redirect na početnu
        },
        error: () => {
          alert('Failed to delete product');
        }
      });
    }
  }
}