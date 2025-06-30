import { Component, OnInit, ViewChild, inject, signal, AfterViewInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Table } from 'primeng/table';
import { ConfirmationService, MessageService } from 'primeng/api'; 
import { PrimengModule } from '../../../shared/primeng.module';
import { of } from 'rxjs';
import { switchMap, filter, catchError } from 'rxjs/operators';
import { WorkStationViewModel } from '../../../core/model/workStation.model'; 
import { WorkPlanModelComponent } from './modal/workplan-modal.component';
import { WorkPlanViewModel } from '../../../core/model/workplan-view.mode';
import { WorkPlanService } from '../../../core/services/workplan.service';
import { WorkStationService } from '../../../core/services/workStation.service';
import { ProductService } from '../../../core/services/product.service';
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
  selector: 'app-crud',
  standalone: true,
  templateUrl: './workPlan.component.html',
  imports: [CommonModule, FormsModule, PrimengModule, WorkPlanModelComponent]
})
export class WorkPlanComponent implements OnInit {
  // Estado de vista
  showDialog = false;
  submitted = false;
  loading: boolean = true;

  // Datos
  entity: WorkPlanViewModel = {};
  emtitiesSelected: WorkPlanViewModel[] | null = null;
  entities = signal<WorkPlanViewModel[]>([]);
  workStations!: WorkStationViewModel[] ;
  products!: ProductViewModel[] ;
 
  // Tabla
  @ViewChild('dt') dt!: Table;
  cols: Column[] = [];
  exportColumns: ExportColumn[] = [];

  // Servicios (usando inject para Angular moderno)
  private service = inject(WorkPlanService);
  private workStationService = inject(WorkStationService);
  private productService = inject(ProductService);
  private messageService = inject(MessageService);
  private confirmationService = inject(ConfirmationService);

  ngOnInit() {
    this.initializeColumns();
    this.loadEntities();
    this.loadWorkStations();
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

  private loadEntities() {
    this.service.get().subscribe({
      next: (res: any) => {
        if (res.success && res.payload?.length > 0) {
          const transformedEntities = res.payload.map((entity: any) => ({
            ...entity,
            searchText: [
              entity.name,
              entity.status,
              ...(entity.stations ?? []).map((s: any) => s.name ?? ''),
              ...(entity.products ?? []).map((p: any) => p.product?.name ?? p.name ?? '')
            ]
              .filter(x => !!x) // <-- quita vacíos, nulls, undefined
              .join(' ')
              .toLowerCase()
          }));
  
          this.entities.set(transformedEntities);
        } else {
          if (res.errors?.length > 0) {
            this.showError(res.error?.errors?.Error[0] ?? 'No se pudieron cargar las estaciones de trabajo.');
          }
        }
        this.loading = false;
      },
      error: err => this.showError(err.message)
    });
  }
  

  private loadWorkStations () {
    this.workStationService.getActive().subscribe({
      next: (response : any) => {
        this.workStations = response.payload;
      },
      error: (err) => {
        console.error('Error al obtener estaciones activas', err);
        this.messageService.add({
          severity: 'error',
          summary: 'Error',
          detail: 'No se pudieron cargar las estaciones activas',
        });
      }
    })
  }

  
  private loadProducts () {
    this.productService.getWithoutPlan().subscribe({
      next: (response : any) => {
        this.products = response.payload;
      },
      error: (err) => {
        console.error('Error al obtener estaciones activas', err);
        this.messageService.add({
          severity: 'error',
          summary: 'Error',
          detail: 'No se pudieron cargar los planes de trabajos.',
        });
      }
    })

  }

  exportCSV() {
    this.dt.exportCSV();
  }

  openNew() {
    this.entity = {};
    this.submitted = false;
    this.showDialog = true;
  }

  editEntity(entity: WorkPlanViewModel) {
    this.entity = { ...entity };
    this.showDialog = true;
  }

  deleteEntity(entity: WorkPlanViewModel) {
    this.confirmationService.confirm({
      message: `¿Deseas eliminar el plan de trabajo "${entity.name}"?`,
      header: 'Confirmación',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.loading = true;
        of(entity.id).pipe(
          switchMap(id => this.service.delete(id!)),
          filter((res: any) => {
            if (!res.success) {
              this.showError(res.error?.errors?.Error[0] ?? 'No se pudo eliminar el plan de trabajo');
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
            this.showSuccess('Plan de trabajo eliminado correctamente.');
          }
          this.loading = false;
        });
      }
    });
  }

  onSave(entity: WorkStationViewModel) {
    this.loadEntities();
    this.showDialog = false;
  }

  onClose() {
    this.showDialog = false;
  }

  // Utilidades de mensaje
  private showSuccess(detail: string) {
    this.messageService.add({ severity: 'success', summary: 'Éxito', detail, life: 3000 });
  }

  private showError(detail: string) {
    this.messageService.add({ severity: 'error', summary: 'Error', detail, life: 3000 });
  }

 

  clear(table: Table) {
    table.clear();
  }

  onGlobalFilter(table: Table, event: Event) {
    table.filterGlobal((event.target as HTMLInputElement).value, 'contains');
  }
}
