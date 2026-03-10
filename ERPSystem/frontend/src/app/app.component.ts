import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { MatCalendarCellClassFunction, MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { ErpApiService, ProductStock, SalesDay } from './erp-api.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, MatDatepickerModule, MatNativeDateModule],
  template: `
    <header>
      <h1>ERP Minimalista</h1>
      <small>Productos, Ventas por calendario, Facturación SRI y Contabilidad</small>
    </header>

    <main style="padding:16px" class="grid">
      <section class="card">
        <h2>Productos</h2>
        <p>Total: {{ products.length }}</p>
        <div *ngFor="let p of products">
          <strong>{{ p.name }}</strong> - Stock: 
          <span [class]="stockClass(p.stockStatus)">{{ p.stockQuantity }}</span>
        </div>
      </section>

      <section class="card">
        <h2>Ventas (Calendario)</h2>
        <mat-calendar [dateClass]="dateClass"></mat-calendar>
        <div *ngFor="let s of salesHistory">
          {{ s.date | date:'yyyy-MM-dd' }} → {{ s.total | currency:'USD' }}
        </div>
      </section>

      <section class="card">
        <h2>Facturación electrónica</h2>
        <p>API: <code>POST /api/electronic-billing/receive-invoice</code></p>
        <p>Envía al SRI + cola para PDF y WhatsApp.</p>
      </section>

      <section class="card">
        <h2>Contabilidad</h2>
        <ul>
          <li>ATS: <code>/api/accounting/ats</code></li>
          <li>IVA mensual: <code>/api/accounting/iva-monthly</code></li>
          <li>ATS simplificado: <code>/api/accounting/ats-simplified</code></li>
        </ul>
      </section>
    </main>
  `
})
export class AppComponent implements OnInit {
  products: ProductStock[] = [];
  salesHistory: SalesDay[] = [];
  highlightedDates = new Set<string>();

  constructor(private readonly api: ErpApiService) {}

  ngOnInit(): void {
    const tenantId = '11111111-1111-1111-1111-111111111111';
    this.api.getProductsStock(tenantId).subscribe(r => this.products = r.items);
    this.api.getSalesHistory(tenantId).subscribe(r => {
      this.salesHistory = r;
      this.highlightedDates = new Set(r.map(x => x.date.slice(0, 10)));
    });
  }

  stockClass(status: string): string {
    if (status === 'OK') return 'stock-ok';
    if (status === 'BAJO') return 'stock-low';
    return 'stock-out';
  }

  dateClass: MatCalendarCellClassFunction<Date> = (cellDate) => {
    const value = cellDate.toISOString().slice(0, 10);
    return this.highlightedDates.has(value) ? 'mat-calendar-body-today' : '';
  };
}
