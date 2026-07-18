import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class HealthService {
  private healthUrl = `${environment.apiUrl}health`;

  constructor(private http: HttpClient) {}

  checkHealth(): Observable<boolean> {
    return this.http.get(this.healthUrl).pipe(
      map(() => true)
    );
  }
}