import { Component, OnInit, ViewChild, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Table } from 'primeng/table';
import { ConfirmationService, MessageService } from 'primeng/api';
import { ProductService } from '../../../core/services/product.service';
import { PrimengModule } from '../../../shared/primeng.module';
import { of } from 'rxjs';
import { switchMap, filter, catchError } from 'rxjs/operators';
import { PriceListViewModel } from '../../../core/model/pricelist-view.model';
import { PriceListService } from '../../../core/services/pricelist.service';
import { PriceListItemViewModel } from '../../../core/model/pricelistItem-view.model';
import { ProductListDetailModalComponent } from './modal/price-list-detail-modal.component';
import { PriceListItemService } from '../../../core/services/pricelistItem.service';
import { ProductViewModel } from '../../../core/model/product-view.model';

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
  templateUrl: './price-list-details.component.html',
  imports: [CommonModule, FormsModule, PrimengModule, ProductListDetailModalComponent] 
})
export class PriceListDetailsComponent implements OnInit {

  // Estado de vista
  showDialog = false;
  submitted = false;

  // Datos
  entity: PriceListItemViewModel = {};
  selectedEntities: PriceListItemViewModel[] | null = null;
  plans = signal<PriceListViewModel[]>([]);
  selectedPlan!: PriceListViewModel;
  products!: ProductViewModel[];
  entities : PriceListItemViewModel[] = [];
  // Tabla
  @ViewChild('dt') dt!: Table;
  cols: Column[] = [];
  exportColumns: ExportColumn[] = [];
  
  scrollHeight: string = '65vh'; 

  ngAfterViewInit() {
    const offset = 310; // píxeles a restar por header, toolbar, etc.
    const vh = window.innerHeight;
    this.scrollHeight = `${vh - offset}px`;
  }


  /// Services
  service = inject(PriceListService);
  productService = inject(ProductService);
  priceListItemService = inject(PriceListItemService);
  messageService = inject(MessageService);
  confirmationService = inject(ConfirmationService);

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
    this.service.getOnlyActive().subscribe({
      next: (res: any) => {
        if (res.success && res.payload?.length > 0) {
          this.plans.set(res.payload);
        } else {
          if (!res.success)
          this.showError(res.error?.errors?.Error[0] ?? 'No se pudo obtener la lista el planes.');
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

  editEntity(entity: PriceListItemViewModel) {
    this.entity = { ...entity }; 
    this.showDialog = true;
  }


  deleteEntity(entity: PriceListItemViewModel) {
    this.confirmationService.confirm({
      message: `¿Deseas eliminar producto "${entity.product?.name}"?`,
      header: 'Confirmación',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        of(entity.id).pipe(
          switchMap(id => this.priceListItemService.delete(id!)),
          filter((res: any) => {
            if (!res.success) {
              this.showError(res.error?.errors?.Error[0] ?? 'No se pudo eliminar el producto.');
            }
            return res.success;
          }),
          switchMap(() => this.priceListItemService.get(this.selectedPlan.id)),
          catchError(err => {
            this.showError(err.message);
            return of(null);
          })
        ).subscribe((res: any) => {
          if (res?.success) {
            this.entities = res.payload;
            this.showSuccess('Producto eliminado de la lista correctamente.');
          }
        });
      }
    });
  }


  onSave(product: any) {
    this.loadProductsPlan(); 
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

 

  clear(table: Table) {
    table.clear();
  }

  changePlanEvent(plan: PriceListViewModel) {
    if (plan) {
      this.selectedPlan = plan;
      this.products = [];
      this.productService.getByCategory(this.selectedPlan.categoryId).subscribe({
        next: (res: any) => {
          if (res.success && res.payload?.length > 0) {
            this.products = res.payload;
          } else { 
            if (!res.success)
            this.showError(res.error?.errors?.Error[0] ?? 'No se pudieron los productos por categoria.');        
          }
        },
        error: err => this.showError(err.message)
      });
      this.loadProductsPlan();
    }
  }

  private loadProductsPlan() {
    if (this.selectedPlan) {
      this.priceListItemService.get(this.selectedPlan.id).subscribe({
        next: (res: any) => {
          this.entities = res.payload ?? [];
          if (res.success && res.payload?.length > 0) {
            this.entities = res.payload;
          } else {
            if (!res.success)
            this.showError(res.error?.errors?.Error[0] ?? 'No se pudo cargar los productos del plan selecionado.');
          }
        },
        error: err => this.showError(err.message)
      });
    }
  }
 
}
