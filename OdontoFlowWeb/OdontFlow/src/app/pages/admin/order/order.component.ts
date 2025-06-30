import {
  Component, OnInit, ViewChild, inject, signal, AfterViewInit
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Table, TableLazyLoadEvent } from 'primeng/table';
import { ConfirmationService, MessageService } from 'primeng/api';
import { PrimengModule } from '../../../shared/primeng.module';
import { of } from 'rxjs';
import { switchMap, filter, catchError } from 'rxjs/operators';
import { MenuItem } from 'primeng/api';
import { OrderModalComponent } from './modal/order-modal.component';
import { OrderViewModel } from '../../../core/model/order.model';
import { OrderService } from '../../../core/services/order.service';
import { ProductService } from '../../../core/services/product.service';
import { SpinnerService } from '../../../core/services/spinner.service';
import { ProductViewModel } from '../../../core/model/product-view.model';
import { Menu } from 'primeng/menu';
import { LazyLoadEvent } from 'primeng/api';

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
  templateUrl: './order.component.html',
  imports: [CommonModule, FormsModule, PrimengModule, OrderModalComponent]
})
export class OrderComponent implements OnInit, AfterViewInit {
  showDialog = false;
  submitted = false;

  entity: OrderViewModel = {};
  selectedEntities: OrderViewModel[] | null = null;
  entities = signal<OrderViewModel[]>([]);
  products: ProductViewModel[] = [];

  scrollHeight: string = '65vh';
  totalRecords = 0;
  loading = false;

  @ViewChild('menu') menu!: Menu;
  currentMenuItems: MenuItem[] = [];

  @ViewChild('dt') dt!: Table;
  cols: Column[] = [];
  exportColumns: ExportColumn[] = [];

  categories = [
    { label: 'GAMMA', value: 'GAMMA' },
    { label: 'ZIRCONIA', value: 'ZIRCONIA' }
  ];

  estados = [
    { label: 'CONFIRMADA', value: 'CONFIRMADA' },
    { label: 'EN_PROCESO', value: 'EN_PROCESO' },
    { label: 'TERMINADO', value: 'TERMINADO' },
    { label: 'ENTREGADO', value: 'ENTREGADO' },
    { label: 'PAGADO', value: 'PAGADO' },
  ];

  factura = [
    { label: 'Sí', value: true },
    { label: 'No', value: false }
  ];

  // Services
  private service = inject(OrderService);
  private messageService = inject(MessageService);
  private productServive = inject(ProductService);
  private confirmationService = inject(ConfirmationService);
  private spinnerService = inject(SpinnerService);

  ngOnInit() {
    this.initializeColumns();
  }

  ngAfterViewInit() {
    const offset = 310;
    const vh = window.innerHeight;
    this.scrollHeight = `${vh - offset}px`;
  }

  private initializeColumns() {
    this.cols = [
      { field: 'barcode', header: 'Código' },
      { field: 'clientName', header: 'Cliente' },
      { field: 'orderStatus', header: 'Estado' },
      { field: 'creationDate', header: 'Fecha Alta' }
    ];
    this.exportColumns = this.cols.map(col => ({
      title: col.header,
      dataKey: col.field
    }));
  }

loadEntitiesLazy(event: TableLazyLoadEvent) {
 

  const rows = event.rows ?? 25;
  const page = (event.first ?? 0) / rows + 1;

  const filters = event.filters || {};
  const global = (event.globalFilter as string) || '';

  const filterObject = Object.keys(filters).reduce((acc: any, key) => {
    const filterMeta : any  = filters[key];
    if (filterMeta?.value !== undefined && filterMeta?.value !== null && filterMeta?.value !== '') {
      acc[key] = {
        value: filterMeta.value,
        matchMode: filterMeta.matchMode || 'contains'
      };
    }
    return acc;
  }, {});

  this.service.getPaged({
    page,
    pageSize: rows,
    sortField:  event.sortField,
    sortOrder: event.sortOrder,
    global,
    filters: filterObject
  }).subscribe({
    next: (res: any) => {
      if (res.success && res.payload) {
        this.entities.set(res.payload.items);
        this.totalRecords = res.payload.totalCount;
      }
 
    },
    error: err => {
      this.showError(err.message);
 
    }
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

  editEntity(entity: OrderViewModel) {
    this.entity = { ...entity };
    this.showDialog = true;
  }

  onSave() {
    this.dt.reset();  
    this.showDialog = false;
  }

  onClose() {
    this.showDialog = false;
  }

  deleteEntity(entity: OrderViewModel) {
    this.confirmationService.confirm({
      message: `¿Deseas eliminar la orden?`,
      header: 'Confirmación',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        of(entity.id).pipe(
          switchMap(id => this.service.delete(id!)),
          filter((res: any) => {
            if (!res.success) {
              this.showError(res.error?.errors?.Error[0] ?? 'No se pudo eliminar la orden');
            }
            return res.success;
          }),
          catchError(err => {
            this.showError(err.message);
            return of(null);
          })
        ).subscribe(() => {
          this.dt.reset(); // recargar paginación desde página 1
          this.showSuccess('Orden eliminada correctamente.');
        });
      }
    });
  }

  deleveryEntity(entity: OrderViewModel) {
    this.confirmationService.confirm({
      message: `¿Deseas entregar la orden?`,
      header: 'Confirmación',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        of(entity.id).pipe(
          switchMap(id => this.service.deliver(id!)),
          filter((res: any) => {
            if (!res.success) {
              this.showError(res.error?.errors?.Error[0] ?? 'No se pudo entregar la orden');
            }
            return res.success;
          }),
          catchError(err => {
            this.showError(err.message);
            return of(null);
          })
        ).subscribe(() => {
          this.dt.reset();
          if (entity.client?.groupId === 1) {
            this.service.printNoteGamma(entity.id);
          } else {
            this.service.printNoteZirconia(entity.id);
          }
          this.showSuccess('Orden entregada correctamente.');
        });
      }
    });
  }

  confirmOrder(entity: OrderViewModel) {
    this.confirmationService.confirm({
      message: `¿Deseas confirmar la orden seleccionada?`,
      header: 'Confirmación',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        of(entity.id).pipe(
          switchMap(id => this.service.confirm(id!)),
          filter((res: any) => {
            if (!res.success) {
              this.showError(res.error?.errors?.Error[0] ?? 'No se pudo confirmar la orden');
            }
            return res.success;
          }),
          catchError(err => {
            this.showError(err.message);
            return of(null);
          })
        ).subscribe(() => {
          this.dt.reset();
          this.showSuccess('Orden confirmada correctamente.');
        });
      }
    });
  }

  printOrder(entity: OrderViewModel) {
    this.service.printOrderPdf(entity.id);
  }

  printNote(entity: OrderViewModel) {
    if (entity.client?.groupId === 1) {
      this.service.printNoteGamma(entity.id);
    } else {
      this.service.printNoteZirconia(entity.id);
    }
  }

  findIndexById(id: string): number {
    return this.entities().findIndex(p => p.id === id);
  }

  clear(table: Table) {
    table.clear();
  }

  openMenu(event: MouseEvent, row: any): void {
    this.currentMenuItems = this.getActions(row);
    this.menu.toggle(event);
  }

  getActions(entity: any): MenuItem[] {
    return [
      {
        label: 'Editar',
        icon: 'pi pi-pencil',
        command: () => this.editEntity(entity)
      },
      {
        label: 'Cancelar',
        icon: 'pi pi-trash',
        disabled: entity.orderStatusId !== 1,
        command: () => this.deleteEntity(entity)
      },
      {
        label: 'Entregar',
        icon: 'pi pi-car',
        disabled: entity.orderStatusId !== 4,
        command: () => this.deleveryEntity(entity)
      },
      {
        label: 'Imprimir',
        icon: 'pi pi-barcode',
        command: () => this.printOrder(entity)
      },
      {
        label: 'Nota Pago',
        icon: 'pi pi-receipt',
        disabled: entity.orderStatusId !== 5,
        command: () => this.printNote(entity)
      }
    ];
  }

  getStatusSeverity(status: string): 'success' | 'secondary' | 'info' | 'warn' | 'danger' | 'contrast' | undefined {
    switch (status) {
      case 'CONFIRMADA':
        return 'info';
      case 'EN_PROCESO':
        return 'warn';
      case 'TERMINADO':
        return 'success';
      case 'ENTREGADO':
        return 'contrast';
      case 'PAGADO':
        return 'secondary';
      default:
        return 'info';
    }
  }

  private showSuccess(detail: string) {
    this.messageService.add({ severity: 'success', summary: 'Éxito', detail, life: 3000 });
  }

  private showError(detail: string) {
    this.messageService.add({ severity: 'error', summary: 'Error', detail, life: 3000 });
  }
}
