import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../environments/environment';

export interface ProductStock { name: string; stockQuantity: number; stockStatus: string; }
export interface ProductResponse { items: ProductStock[]; }
export interface SalesDay { date: string; total: number; }

@Injectable({ providedIn: 'root' })
export class ErpApiService {
  constructor(private readonly http: HttpClient) {}

  getProductsStock(tenantId: string): Observable<ProductResponse> {
    return this.http.get<ProductResponse>(`${environment.apiUrl}/api/dashboard/products-stock?tenantId=${tenantId}`);
  }

  getSalesHistory(tenantId: string): Observable<SalesDay[]> {
    const startDate = new Date();
    startDate.setDate(1);
    const endDate = new Date();
    return this.http.get<SalesDay[]>(`${environment.apiUrl}/api/dashboard/sales-history?tenantId=${tenantId}&startDate=${startDate.toISOString()}&endDate=${endDate.toISOString()}`);
  }
}
