import { Component, OnInit } from '@angular/core';
import { Pagination } from 'src/app/models/pagination';
import { Product } from 'src/app/models/product';
import { ProductService } from 'src/app/services/product.service';

@Component({
  selector: 'app-product-list',
  templateUrl: './product-list.component.html'
})
export class ProductListComponent implements OnInit {
  products: Product[] = [];
  brands: string[] = [];
  types: string[] = [];
  sort: string = '';

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

  constructor(private productService: ProductService) {}

  ngOnInit(): void {
    this.loadProducts();
    this.loadBrands();
    this.loadTypes();
  }

  loadProducts(): void {
    const brand = this.selectedBrand === 'All' ? undefined : this.selectedBrand;
    const type = this.selectedType === 'All' ? undefined : this.selectedType;
  
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
        },
        error: err => console.error(err)
      });
  }

  loadAllProducts(): void {
    const brand = this.selectedBrand === 'All' ? undefined : this.selectedBrand;
    const type = this.selectedType === 'All' ? undefined : this.selectedType;

    this.productService.getProducts(brand, type, this.sort).subscribe({
      next: res => {this.products = res; console.log(res)},
      error: err => console.error(err)
    });
  }

  loadBrands(): void {
    this.productService.getBrands().subscribe({
      next: res => this.brands = ['All', ...res],
      error: err => console.error(err)
    });
  }

  loadTypes(): void {
    this.productService.getTypes().subscribe({
      next: res => this.types = ['All', ...res],
      error: err => console.error(err)
    });
  }

  onBrandSelected(brand: string): void {
    this.selectedBrand = brand;
    this.loadProducts();
  }

  onTypeSelected(type: string): void {
    this.selectedType = type;
    this.loadProducts();
  }

  getImageUrl(pictureUrl: string): string {
    return 'assets/images/products/' + pictureUrl;
  }

  onPageChanged(page: number): void {
    if (page !== this.pagination.pageIndex) {
      this.pageNumber = page;
      this.loadProducts();
    }
  }
}