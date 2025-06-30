import { Component, OnInit, ViewChild, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Table } from 'primeng/table';
import { ConfirmationService, MessageService } from 'primeng/api';
import { ProductService } from '../../../core/services/product.service';
import { PrimengModule } from '../../../shared/primeng.module';
import { of } from 'rxjs';
import { switchMap, filter, catchError } from 'rxjs/operators';
import { SupplierViewModel } from '../../../core/model/supplier-view.model';
import { PriceListViewModel } from '../../../core/model/pricelist-view.model';
import { PriceListModalComponent } from './modal/pricelist-modal.component';
import { PriceListService } from '../../../core/services/pricelist.service';

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
  standalone: true,
  templateUrl: './price-list.component.html',
  imports: [CommonModule, FormsModule, PrimengModule, PriceListModalComponent]
})
export class PriceListComponent implements OnInit {

  // Estado de vista
  showDialog = false;
  submitted = false;

  // Datos
  entity: PriceListViewModel = {};
  selectedEntities: PriceListViewModel[] | null = null;
  entities = signal<PriceListViewModel[]>([]);

  // Tabla
  @ViewChild('dt') dt!: Table;
  cols: Column[] = [];
  exportColumns: ExportColumn[] = [];

  //Services:
  private service = inject(PriceListService);
  private messageService = inject(MessageService);
  private confirmationService = inject(ConfirmationService);

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


  ngOnInit() {
    this.initializeColumns();
    this.loadEnities();
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

  private loadEnities() {
    this.service.get().subscribe({
      next: (res: any) => {
        if (res.success && res.payload?.length > 0) {
          this.entities.set(res.payload);
        } else {
          if (res.errors?.length > 0){
            this.showError(res.error?.errors?.Error[0] ?? 'No se pudieron cargar las listas.');
          }   
        }
      },
      error: err => this.showError(err.message)
    });
  }

  exportCSV() {
    this.dt.exportCSV();
  }

  onGlobalFilter(table: Table, event: Event) {
    table.filterGlobal((event.target as HTMLInputElement).value, 'contains');
  }

  openNew() {
    this.entity = {};
    this.submitted = false;
    this.showDialog = true;
  }

  editEntity(entity: SupplierViewModel) {
    this.entity = { ...entity };
    console.log(this.entity);
    this.showDialog = true;
  }


  deleteEntity(entity: SupplierViewModel) {
    this.confirmationService.confirm({
      message: `¿Deseas eliminar la lista "${entity.name}"?`,
      header: 'Confirmación',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        of(entity.id).pipe(
          switchMap(id => this.service.delete(id!)),
          filter((res: any) => {
            if (!res.success) {
              this.showError(res.error?.errors?.Error[0] ?? 'No se pudo eliminar la lista.');
            }
            return res.success;
          }),
          switchMap(() => this.service.get()),
          catchError(err => {
            this.showError(err.message);
            return of(null);
          })
        ).subscribe((res: any) => {
          if (res?.success) {
            this.entities.set(res.payload);
            this.showSuccess('Lista eliminada correctamente.');
          }
        });
      }
    });
  }


  onSave(product: any) {
    this.loadEnities();
    this.showDialog = false;
  }

  onClose() {
    this.showDialog = false;
  }

  // Utilidades
  private showSuccess(detail: string) {
    this.messageService.add({ severity: 'success', summary: 'Éxito', detail, life: 3000 });
  }

  private showError(detail: string) {
    this.messageService.add({ severity: 'error', summary: 'Error', detail, life: 3000 });
  }

  // Extra: solo si sigues usando este método en algún lugar
  findIndexById(id: string): number {
    return this.entities().findIndex(p => p.id === id);
  }

  clear(table: Table) {
    table.clear();
  }


}
