import { Component, OnInit, ViewChild, inject, signal, AfterViewInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Table } from 'primeng/table';
import { ConfirmationService, MessageService } from 'primeng/api';

import { ProductService } from '../../../core/services/product.service';
import { ProductViewModel } from '../../../core/model/product-view.model';
import { WorkStationModalComponent } from './modal/workStation-modal.component';
import { PrimengModule } from '../../../shared/primeng.module';
import { of } from 'rxjs';
import { switchMap, filter, catchError } from 'rxjs/operators';
import { WorkStationViewModel } from '../../../core/model/workStation.model';
import { WorkStationService } from '../../../core/services/workStation.service';

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
  templateUrl: './workStation.component.html',
  imports: [CommonModule, FormsModule, PrimengModule, WorkStationModalComponent]
})
export class WorkStationComponent implements OnInit {
  // Estado de vista
  showDialog = false;
  submitted = false;
  loading: boolean = true;

  // Datos
  entity: WorkStationViewModel = {};
  emtitiesSelected: WorkStationViewModel[] | null = null;
  entities = signal<WorkStationViewModel[]>([]);
 
  // Tabla
  @ViewChild('dt') dt!: Table;
  cols: Column[] = [];
  exportColumns: ExportColumn[] = [];

  // Servicios (usando inject para Angular moderno)
  private service = inject(WorkStationService);
  private messageService = inject(MessageService);
  private confirmationService = inject(ConfirmationService);

  ngOnInit() {
    this.initializeColumns();
    this.loadEntities();
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
          this.entities.set(res.payload);
        } else {
          if (res.errors?.length > 0){
            this.showError(res.error?.errors?.Error[0] ?? 'No se pudieron cargar las estaciones de trabajo.');
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
    this.entity = {};
    this.submitted = false;
    this.showDialog = true;
  }

  editEntity(entity: WorkStationViewModel) {
    this.entity = { ...entity };
    this.showDialog = true;
  }

  deleteEntity(entity: WorkStationViewModel) {
    this.confirmationService.confirm({
      message: `¿Deseas eliminar la estacion de trabajo "${entity.name}"?`,
      header: 'Confirmación',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.loading = true;
        of(entity.id).pipe(
          switchMap(id => this.service.delete(id!)),
          filter((res: any) => {
            if (!res.success) {
              this.showError(res.error?.errors?.Error[0] ?? 'No se pudo eliminar la estación de trabajo');
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
            this.showSuccess('Producto eliminado correctamente.');
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
