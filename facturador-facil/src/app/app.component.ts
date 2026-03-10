import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';

interface InvoiceItem {
  descripcion: string;
  cantidad: number;
  precioUnitario: number;
  total: number;
}

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  nombreCliente = '';
  identificacion = '';
  itemDescripcion = '';
  itemCantidad = 1;
  itemPrecio = 0;
  ivaPorcentaje = 15;
  items: InvoiceItem[] = [];

  agregarItem(): void {
    if (!this.itemDescripcion.trim() || this.itemCantidad <= 0 || this.itemPrecio <= 0) {
      return;
    }

    const total = this.itemCantidad * this.itemPrecio;
    this.items.push({
      descripcion: this.itemDescripcion.trim(),
      cantidad: this.itemCantidad,
      precioUnitario: this.itemPrecio,
      total
    });

    this.itemDescripcion = '';
    this.itemCantidad = 1;
    this.itemPrecio = 0;
  }

  eliminarItem(index: number): void {
    this.items.splice(index, 1);
  }

  get subtotal(): number {
    return this.items.reduce((suma, item) => suma + item.total, 0);
  }

  get iva(): number {
    return this.subtotal * (this.ivaPorcentaje / 100);
  }

  get totalFactura(): number {
    return this.subtotal + this.iva;
  }
}
