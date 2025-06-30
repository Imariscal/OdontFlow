import { Component, OnInit, ViewChild, inject, signal, AfterViewInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Table } from 'primeng/table';
import { ConfirmationService, MessageService } from 'primeng/api';

import { ProductService } from '../../../core/services/product.service';
import { ProductViewModel } from '../../../core/model/product-view.model'; 
import { PrimengModule } from '../../../shared/primeng.module';
import { of } from 'rxjs';
import { switchMap, filter, catchError } from 'rxjs/operators';
import { ProductModalComponent } from './modal/product-modal.component';

interface Column {
  field: string;
  header: string;
  customExportHeader?: string;
}
interface ExportColumn {
  title: string;
  dataKey: string;
}

@Component({
  selector: 'app-crud',
  standalone: true,
  templateUrl: './products.component.html',
  imports: [CommonModule, FormsModule, PrimengModule, ProductModalComponent]
})
export class ProductsComponents implements OnInit {
  // Estado de vista
  productDialog = false;
  submitted = false;
  loading: boolean = true;

  // Datos
  product: ProductViewModel = {};
  selectedProducts: ProductViewModel[] | null = null;
  products = signal<ProductViewModel[]>([]);
  categories = [
    { label: 'GAMMA', value: 'GAMMA' },
    { label: 'ZIRCONIA', value: 'ZIRCONIA' }
  ];


  activo = [ 
    { label: 'Sí', value: true },
    { label: 'No', value: false } 
  ];

  scrollHeight: string = '65vh'; 

  ngAfterViewInit() {
    const offset = 310; // píxeles a restar por header, toolbar, etc.
    const vh = window.innerHeight;
    this.scrollHeight = `${vh - offset}px`;
  }

 
  // Tabla
  @ViewChild('dt') dt!: Table;
  cols: Column[] = [];
  exportColumns: ExportColumn[] = [];

  // Servicios (usando inject para Angular moderno)
  private productService = inject(ProductService);
  private messageService = inject(MessageService);
  private confirmationService = inject(ConfirmationService);

  ngOnInit() {
    this.initializeColumns();
    this.loadProducts();
  } 

  private initializeColumns() {
    this.cols = [
      { field: 'name', header: 'Name' },
      { field: 'price', header: 'Price' },
      { field: 'category', header: 'Category' }
    ];
    this.exportColumns = this.cols.map(col => ({
      title: col.header,
      dataKey: col.field
    }));
  }

  private loadProducts() {
    this.productService.get().subscribe({
      next: (res: any) => {
        if (res.success && res.payload?.length > 0) {
          this.products.set(res.payload);
        } else {
          if (res.errors?.length > 0){
            this.showError(res.error?.errors?.Error[0] ?? 'No se pudieron cargar los productos.');
          }     
        }
        this.loading = false;
      },
      error: err => this.showError(err.message)
    });
  }

  exportCSV() {
    this.dt.exportCSV();
  }

  openNew() {
    this.product = {};
    this.submitted = false;
    this.productDialog = true;
  }

  editProduct(product: ProductViewModel) {
    this.product = { ...product };
    this.productDialog = true;
  }

  deleteProduct(product: ProductViewModel) {
    this.confirmationService.confirm({
      message: `¿Deseas eliminar el producto "${product.name}"?`,
      header: 'Confirmación',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.loading = true;
        of(product.id).pipe(
          switchMap(id => this.productService.delete(id!)),
          filter((res: any) => {
            if (!res.success) {
              this.showError(res.error?.errors?.Error[0] ?? 'No se pudo eliminar el producto.');
            }
            return res.success;
          }),
          switchMap(() => this.productService.get()),
          catchError(err => {
            this.showError(err.message);
            return of(null);
          })
        ).subscribe((res: any) => {
          if (res?.success) {
            this.products.set(res.payload);
            this.showSuccess('Producto eliminado correctamente.');
          }
          this.loading = false;
        });
      }
    });
  }

  onSave(product: ProductViewModel) {
    this.loadProducts();
    this.productDialog = false;
  }

  onClose() {
    this.productDialog = false;
  }

  // Utilidades de mensaje
  private showSuccess(detail: string) {
    this.messageService.add({ severity: 'success', summary: 'Éxito', detail, life: 3000 });
  }

  private showError(detail: string) {
    this.messageService.add({ severity: 'error', summary: 'Error', detail, life: 3000 });
  }

  findIndexById(id: string): number {
    return this.products().findIndex(p => p.id === id);
  }

  clear(table: Table) {
    table.clear();
  }

  onGlobalFilter(table: Table, event: Event) {
    table.filterGlobal((event.target as HTMLInputElement).value, 'contains');
  }
}
