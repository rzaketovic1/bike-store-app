import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Product } from '../models/product';
import { PaginatedResult } from '../models/paginatedResults';


@Injectable({
  providedIn: 'root'
})
export class ProductService {
  baseUrl = 'https://localhost:5001/api/';

  constructor(private http: HttpClient) {}

  getProducts(brand?: string, type?: string, sort?: string): Observable<Product[]> {
    let params = new HttpParams();
    if (brand) params = params.set('brand', brand);
    if (type) params = params.set('type', type);
    if (sort) params = params.set('sort', sort);

    return this.http.get<Product[]>(this.baseUrl + 'products/all', { params });
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

}
