import { Component, OnInit, ViewChild, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Table } from 'primeng/table';
import { ConfirmationService, MessageService } from 'primeng/api'; 
import { PrimengModule } from '../../../shared/primeng.module';
import { of } from 'rxjs';
import { switchMap, filter, catchError } from 'rxjs/operators'; 
import { SupplierViewModel } from '../../../core/model/supplier-view.model'; 
import { ClientViewModel } from '../../../core/model/client-vew.model';
import { ClientService } from '../../../core/services/client.service';
import { ClientModalComponent } from './modal/client-modal.component';
import { PriceListService } from '../../../core/services/pricelist.service';
import { PriceListViewModel } from '../../../core/model/pricelist-view.model';
import { EmployeeService } from '../../../core/services/employee.service';
import { EmployeeViewModel } from '../../../core/model/employee-view.model';

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
  templateUrl: './client.component.html',
  imports: [CommonModule, FormsModule, PrimengModule, ClientModalComponent] 
})
export class ClientComponent implements OnInit {

  // Estado de vista
  showDialog = false;
  submitted = false;

  // Datos
  entity: ClientViewModel = {};
  selectedEntities: ClientViewModel[] | null = null;
  entities = signal<ClientViewModel[]>([]);
  priceList! : PriceListViewModel[];
  employees! : EmployeeViewModel[];

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

  //Services:
  private service = inject(ClientService);
  private employeeService = inject(EmployeeService);
  private priceListService = inject(PriceListService);
  private messageService  = inject(MessageService);
  private confirmationService = inject(ConfirmationService);

  representantes : any[] = [    
  ];

  activo = [ 
    { label: 'Sí', value: true },
    { label: 'No', value: false } 
  ];
 
  ngOnInit() {
    this.initializeColumns();
    this.loadEnities();
    this.loadPriceList();
    this.loadEmployees();
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
      next: (res : any) => {
        if (res.success && res.payload?.length > 0) {
          this.entities.set(res.payload);
        } else {
          if (res.errors?.length > 0){
            this.showError(res.error?.errors?.Error[0] ?? 'No se pudieron cargar la lista de clientes.');
          }     
        }
      },
      error: err => this.showError(err.message)
    });
  }

  private loadPriceList() {
    this.priceListService.getOnlyActive().subscribe({
      next: (res : any) => {
        if (res.success && res.payload?.length > 0) {
          this.priceList  = res.payload;
        } else {
          if (res.errors?.length > 0){
            this.showError(res.error?.errors?.Error[0] ?? 'No se pudieron cargar los precios de lista.');
          }     
        }
      },
      error: err => this.showError(err.message)
    });
  }

  private loadEmployees() {
    this.employeeService.getActive().subscribe({
      next: (res : any) => {
        if (res.success && res.payload?.length > 0) {
          this.employees  = res.payload;

          this.representantes = Array.from(
            new Map(this.employees.map(item => [item.name, { value: item.name, text: item.name }])).values()
          );

          console.log( this.representantes);
        } else {
          if (res.errors?.length > 0){
            this.showError(res.error?.errors?.Error[0] ?? 'No se pudieron cargar los empleados.');
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

  editEntity(entity: ClientViewModel) {
    this.entity = { ...entity };
    console.log(this.entity);
    this.showDialog = true;
  }
 

  deleteEntity(entity: ClientViewModel) {
    this.confirmationService.confirm({
      message: `¿Deseas eliminar el cliente "${entity.name}"?`,
      header: 'Confirmación',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        of(entity.id).pipe(
          switchMap(id => this.service.delete(id!)),
          filter( (res : any) => {
            if (!res.success) { 
                this.showError(res.error?.errors?.Error[0] ?? 'No se pudo eliminar el cliente.');                 
            }
            return res.success;
          }),
          switchMap(() => this.service.get()),
          catchError(err => {
            this.showError(err.message);
            return of(null);
          })
        ).subscribe( (res : any) => {
          if (res?.success) {
            this.entities.set(res.payload);
            this.showSuccess('Cliente eliminado correctamente.');
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
 
  private showSuccess(detail: string) {
    this.messageService.add({ severity: 'success', summary: 'Éxito', detail, life: 3000 });
  }

  private showError(detail: string) {
    this.messageService.add({ severity: 'error', summary: 'Error', detail, life: 3000 });
  }
 
  findIndexById(id: string): number {
    return this.entities().findIndex(p => p.id === id);
  }

  clear(table: Table) {
    table.clear();
  }
}
