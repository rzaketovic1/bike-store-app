import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Product } from '../models/product';
import { PaginatedResult } from '../models/paginatedResults';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  private baseUrl = `${environment.apiUrl}Products/`;

  constructor(private http: HttpClient) {}

  // GET: /api/Products (sa filtrima i sortiranjem)
  getProducts(brand?: string, type?: string, sort?: string): Observable<Product[]> {
    let params = new HttpParams();
    if (brand) params = params.set('brand', brand);
    if (type) params = params.set('type', type);
    if (sort) params = params.set('sort', sort);

    return this.http.get<Product[]>(`${this.baseUrl}all`, { params });
  }

  createProduct(product: any): Observable<any> {
    return this.http.post<any>(this.baseUrl, product);
  }

  updateProduct(id: number, formData: FormData): Observable<Product> {
    return this.http.put<Product>(`${this.baseUrl}${id}/with-image`, formData);
  }

  deleteProduct(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}${id}`);
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

    return this.http.get<PaginatedResult<Product[]>>(this.baseUrl, { params });
  }

  getBrands(): Observable<string[]> {
    return this.http.get<string[]>(`${this.baseUrl}brands`);
  }

  getTypes(): Observable<string[]> {
    return this.http.get<string[]>(`${this.baseUrl}types`);
  }

  getProduct(id: number): Observable<Product> {
    return this.http.get<Product>(`${this.baseUrl}${id}`);
  }

  uploadProductWithImage(data: FormData): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}with-image`, data);
  }
}