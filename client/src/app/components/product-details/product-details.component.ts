import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Product } from 'src/app/models/product';
import { ProductService } from 'src/app/services/product.service';

@Component({
  selector: 'app-product-details',
  templateUrl: './product-details.component.html',
  styleUrls: ['./product-details.component.scss']
})


export class ProductDetailsComponent implements OnInit {
  product: Product | null = null;

  constructor(private route: ActivatedRoute, private productService: ProductService) {}

  ngOnInit(): void {
    const id = +this.route.snapshot.paramMap.get('id')!;
    this.productService.getProduct(id).subscribe({
      next: p => this.product = p,
      error: err => console.error('Product not found', err)
    });
  }

  getImageUrl(pictureUrl: string): string {
    return 'assets/images/products/' + pictureUrl;
  }
}

