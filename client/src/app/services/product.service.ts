import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Product } from '../models/product';
import { PaginatedResult } from '../models/paginatedResults';


@Injectable({
  providedIn: 'root'
})
export class ProductService {
  baseUrl = 'http://localhost:5000/api/';

  constructor(private http: HttpClient) {}

  getProducts(brand?: string, type?: string, sort?: string): Observable<Product[]> {
    let params = new HttpParams();
    if (brand) params = params.set('brand', brand);
    if (type) params = params.set('type', type);
    if (sort) params = params.set('sort', sort);

    return this.http.get<Product[]>(this.baseUrl + 'products/all', { params });
  }

  createProduct(product: any): Observable<any> {
    console.log(product);
    return this.http.post<any>(this.baseUrl + 'products', product);
  }

updateProduct(id: number, formData: FormData): Observable<Product> {
  return this.http.put<Product>(`http://localhost:5000/api/products/${id}/with-image`, formData);
}

deleteProduct(id: number): Observable<void> {
  return this.http.delete<void>(`http://localhost:5000/api/products/${id}`);
}

  getPaginatedProducts(
    brand?: string,
    type?: string,
    sort?: string,
    pageIndex = 1,
    pageSize = 6
  ): Observable<PaginatedResult<Product[]>> {
    let params = new HttpParams()
      .set('pageIndex', pageIndex)
      .set('pageSize', pageSize);
  
    if (brand) params = params.set('brand', brand);
    if (type) params = params.set('type', type);
    if (sort) params = params.set('sort', sort);
  
    return this.http.get<PaginatedResult<Product[]>>(this.baseUrl + 'products', { params });
  }

  getBrands(): Observable<string[]> {
    return this.http.get<string[]>(this.baseUrl + 'products/brands');
  }

  getTypes(): Observable<string[]> {
    return this.http.get<string[]>(this.baseUrl + 'products/types');
  }

  getProduct(id: number): Observable<Product> {
    return this.http.get<Product>(`${this.baseUrl}products/${id}`);
  }

  uploadProductWithImage(data: FormData): Observable<any> {
  return this.http.post('http://localhost:5000/api/products/with-image', data);
  }

}
