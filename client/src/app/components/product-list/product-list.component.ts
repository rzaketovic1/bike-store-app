import { Component, OnInit } from '@angular/core';
import { Pagination } from 'src/app/models/pagination';
import { Product } from 'src/app/models/product';
import { ProductService } from 'src/app/services/product.service';
import { environment } from '../../../environments/environment';
import { ToastrService } from 'ngx-toastr';

declare var bootstrap: any;

@Component({
  selector: 'app-product-list',
  templateUrl: './product-list.component.html'
})
export class ProductListComponent implements OnInit {
  products: Product[] = [];
  brands: string[] = [];
  types: string[] = [];
  brandsForForm: string[] = [];  // brands without "All"
  typesForForm: string[] = [];   // types without "All"
  sort: string = '';
  isLoading: boolean = false;
  

  selectedFile: File | null = null;
  selectedBrand: string = 'All';
  selectedType: string = 'All';

  pagination!: Pagination;

  pageNumber = 1;
  pageSize = 6;

  sortOptions = [
    { name: 'Name (A-Z)', value: '' },
    { name: 'Price: Low to High', value: 'priceAsc' },
    { name: 'Price: High to Low', value: 'priceDesc' }
  ];

  newProduct = {
  name: '',
  description: '',
  price: 0,
  pictureUrl: '',
  brand: '',
  type: '',
  quantityInStock: 0
  };

  showForm = false;
  

  constructor(
    private productService: ProductService,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.loadProducts();
    this.loadBrands();
    this.loadTypes();
  }

  loadProducts(): void {
    const brand = this.selectedBrand === 'All' ? undefined : this.selectedBrand;
    const type = this.selectedType === 'All' ? undefined : this.selectedType;
  
    this.isLoading = true;
    this.productService
      .getPaginatedProducts(brand, type, this.sort, this.pageNumber, this.pageSize)
      .subscribe({
        next: res => {
          this.products = res.data;
          this.pagination = {
            pageIndex: res.pageIndex,
            pageSize: res.pageSize,
            totalCount: res.totalCount,
            totalPages: res.totalPages
          };
          this.isLoading = false;
        },
        error: err => {
          console.error(err);
          this.isLoading = false;
        }
      });
  }

  loadAllProducts(): void {
    const brand = this.selectedBrand === 'All' ? undefined : this.selectedBrand;
    const type = this.selectedType === 'All' ? undefined : this.selectedType;

    this.productService.getProducts(brand, type, this.sort).subscribe({
  next: res => {
    this.products = res;
    console.table(this.products); 
  },
  error: err => console.error(err)
});
  }

  loadBrands(): void {
    this.productService.getBrands().subscribe({
      next: res => {
        this.brands = ['All', ...res];
        this.brandsForForm = [...res]; // Exclude "All" for form
      },
      error: err => console.error(err)
    });
  }

loadTypes(): void {
  this.productService.getTypes().subscribe({
    next: res => {
      this.typesForForm = res;  // For form: no "All"
      this.types = ['All', ...res];  // For filters: with "All"
    },
    error: err => console.error(err)
  });
}

  onBrandSelected(brand: string): void {
    this.selectedBrand = brand;
    this.pageNumber = 1;
    this.loadProducts();
  }

  onTypeSelected(type: string): void {
    this.selectedType = type;
    this.pageNumber = 1;
    this.loadProducts();
  }

  onSortChanged(): void {
    this.pageNumber = 1;
    this.loadProducts();
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

  onPageChanged(page: number): void {
    if (page !== this.pageNumber) {
      this.pageNumber = page;
      this.loadProducts();
    }
  }

  onFileSelected(event: any) {
  const file: File = event.target.files[0];
  if (file) {
    this.selectedFile = file;
  }
}

  onSubmit() {
  if (!this.selectedFile) {
    alert("Please select an image.");
    return;
  }

  const formData = new FormData();
  formData.append('image', this.selectedFile);
  formData.append('name', this.newProduct.name);
  formData.append('description', this.newProduct.description);
  formData.append('price', this.newProduct.price.toString());
  formData.append('brand', this.newProduct.brand);
  formData.append('type', this.newProduct.type);
  formData.append('quantityInStock', this.newProduct.quantityInStock.toString());

  this.productService.uploadProductWithImage(formData).subscribe({
    next: (res) => {
      this.toastr.success('Product created successfully!', 'Success');
      this.resetForm();
      this.loadProducts();

      const modalEl = document.getElementById('createProductModal');
      if (modalEl) {
        const modal = bootstrap.Modal.getInstance(modalEl) || new bootstrap.Modal(modalEl);
        modal.hide();
      }
    },
    error: (error) => {
      if (error.status === 400 && error.error?.errors) {
        const errorMessages = this.extractValidationErrors(error.error.errors);
        errorMessages.forEach(msg => this.toastr.error(msg, 'Validation Error'));
      } else {
        this.toastr.error(error.error?.title || 'Failed to create product', 'Error');
      }
    }
  });
  }

  resetForm() {
  this.newProduct = {
    name: '',
    description: '',
    price: 0,
    pictureUrl: '',
    brand: '',
    type: '',
    quantityInStock: 0
  };
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