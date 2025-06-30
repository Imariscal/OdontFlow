import { Component, OnInit, ViewChild, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Table } from 'primeng/table';
import { ConfirmationService, MenuItem, MessageService } from 'primeng/api';
import { PrimengModule } from '../../../shared/primeng.module';
import { StationWorkDetailViewModel } from '../../../core/model/station-work-details.model';
import { ActivatedRoute, Router } from '@angular/router';
import { LabService } from '../../../core/services/lab.service';
import { ChartModule } from 'primeng/chart';
import { OrderModalComponent } from '../../admin/order/modal/order-modal.component';
import { OrderViewModel } from '../../../core/model/order.model';
import { OrderService } from '../../../core/services/order.service';
import { Menu } from 'primeng/menu';

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
  templateUrl: './workDetail.component.html',
  imports: [CommonModule, FormsModule, ChartModule, PrimengModule, OrderModalComponent]
})
export class WorkDetailComponent implements OnInit {
  showDialog = false;
  submitted = false;
  loading = true;
  showRejectDialog = false;
  showBloqueoDialog = false;

  mensajeRechazo = '';
  mensajeBloqueo = '';
  entidadRechazo?: StationWorkDetailViewModel;

  entity: StationWorkDetailViewModel = {};
  emtitiesSelected: StationWorkDetailViewModel[] | null = null;
  entities = signal<StationWorkDetailViewModel[]>([]);
  originalEntities: StationWorkDetailViewModel[] = [];

  orderEntity : OrderViewModel = {};

  // Menu 
  @ViewChild('menu') menu!: Menu;
  currentMenuItems: MenuItem[] = [];

  @ViewChild('dt') dt!: Table;
  cols: Column[] = [];
  exportColumns: ExportColumn[] = [];

  private service = inject(LabService);
  private orderService = inject(OrderService);
  private messageService = inject(MessageService);
  private confirmationService = inject(ConfirmationService);
  private route: ActivatedRoute = inject(ActivatedRoute);
  private router: Router = inject(Router);

  stationId!: string | null;

  ngOnInit() {
    this.stationId = this.route.snapshot.paramMap.get('id');
    if (this.stationId) {
      this.initializeColumns();
      this.loadEntities(this.stationId);
    }
  }

  private initializeColumns() {
    this.cols = [
      { field: 'name', header: 'Name' },
      { field: 'price', header: 'Price' },
      { field: 'category', header: 'Category' }
    ];
    this.exportColumns = this.cols.map(col => ({ title: col.header, dataKey: col.field }));
  }

  private loadEntities(id: string | null) {
    this.loading = true;
    this.service.getStationDetails(id).subscribe({
      next: (res: any) => {
        if (res.success && res.payload?.length > 0) {
          this.originalEntities = res.payload;
          this.entities.set(res.payload);
        } else if (res.errors?.length > 0) {
          this.showError(res.error?.errors?.Error[0] ?? 'No se pudieron cargar las estaciones de trabajo.');
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

  starProcess(entity: StationWorkDetailViewModel) {
    this.entities.set([]);
    this.service.postProcessOrder(entity.stationWorkId).subscribe({
      next: (res: any) => {
        if (res.success) {
          this.loadEntities(this.stationId);
        } else if (res.errors?.length > 0) {
          this.showError(res.error?.errors?.Error[0] ?? 'No se pudieron cargar las estaciones de trabajo.');
        }
        this.loading = false;
      },
      error: err => this.showError(err.message)
    });
  }

  completeProcess(entity: StationWorkDetailViewModel) {
    this.entities.set([]);
    this.service.postCompleteOrder(entity.stationWorkId).subscribe({
      next: (res: any) => {
        if (res.success) {
          this.loadEntities(this.stationId);
        } else if (res.errors?.length > 0) {
          this.showError(res.error?.errors?.Error[0] ?? 'No se pudieron cargar las estaciones de trabajo.');
        }
        this.loading = false;
      },
      error: err => this.showError(err.message)
    });
  }

  rejectProcess(entity: StationWorkDetailViewModel) {
    this.entidadRechazo = entity;
    this.mensajeRechazo = '';
    this.showRejectDialog = true;
  }

  blockProcess(entity: StationWorkDetailViewModel) {
    this.entidadRechazo = entity;
    this.mensajeBloqueo = '';
    this.showBloqueoDialog = true;
  }

  cancelarRechazo() {
    this.showRejectDialog = false;
    this.entidadRechazo = undefined;
  }

  cancelarBloque() {
    this.showBloqueoDialog = false;
    this.entidadRechazo = undefined;
  }

  confirmarRechazo() {
    if (this.entidadRechazo && this.mensajeRechazo.trim()) {
      this.service.rejectOrder(this.entidadRechazo.stationWorkId, this.mensajeRechazo).subscribe({
        next: (res: any) => {
          if (res.success) {
            this.entities.set([]);
            this.loadEntities(this.stationId);
          } else if (res.errors?.length > 0) {
            this.showError(res.error?.errors?.Error[0] ?? 'No se pudieron cargar las estaciones de trabajo.');
          }
          this.loading = false;
        },
        error: err => this.showError(err.message)
      });
      this.showRejectDialog = false;
      this.entidadRechazo = undefined;
    }
  }

  confirmarBloqueo() {
    if (this.entidadRechazo && this.mensajeBloqueo.trim()) {
      this.service.blockOrder(this.entidadRechazo.stationWorkId, this.mensajeBloqueo).subscribe({
        next: (res: any) => {
          if (res.success) {
            this.entities.set([]);
            this.loadEntities(this.stationId);
          } else if (res.errors?.length > 0) {
            this.showError(res.error?.errors?.Error[0] ?? 'No se pudieron cargar las estaciones de trabajo.');
          }
          this.loading = false;
        },
        error: err => this.showError(err.message)
      });
      this.showBloqueoDialog = false;
      this.entidadRechazo = undefined;
    }
  }

  unBlockProcess(entity: StationWorkDetailViewModel) {
    this.service.unBlockOrder(entity.stationWorkId).subscribe({
      next: (res: any) => {
        if (res.success) {
          this.entities.set([]);
          this.loadEntities(this.stationId);
        } else if (res.errors?.length > 0) {
          this.showError(res.error?.errors?.Error[0] ?? 'No se pudieron cargar las estaciones de trabajo.');
        }
        this.loading = false;
      },
      error: err => this.showError(err.message)
    });
  }

  clear(table: Table) {
    table.clear();
  }

  onGlobalFilter(table: Table, event: Event) {
    table.filterGlobal((event.target as HTMLInputElement).value, 'contains');
  }
  
  onGlobalBarcodeFilter(table: Table, event: KeyboardEvent) {
    if (event.key === 'Enter') {
      const input = event.target as HTMLInputElement;
      const code = input.value.trim();
  
      // Buscamos si el input es un barcode directo o un orderNumber
      const matchedByBarcode = this.originalEntities.find(x => x.barcode === code);
      const matchedByOrderNumber = this.originalEntities.filter(x => x.orderNumber === code);
  
      if (matchedByBarcode) {
        // Caso 1: Se encontró por barcode (producto individual)
        this.processOrder(matchedByBarcode);
        this.highlightedOrderId = matchedByBarcode.barcode;
      } else if (matchedByOrderNumber.length > 0) {
        // Caso 2: Se encontró por orderNumber (varios productos)
        matchedByOrderNumber.forEach(order => {
          this.processOrder(order);
        });
        this.highlightedOrderId = code; // puedes resaltar el número de orden
      } else {
        this.showError(`La orden o producto: ${code} no está disponible en esta estación.`);
      }
  
      setTimeout(() => {
        this.highlightedOrderId = null;
      }, 3000);
  
      input.value = '';
    }
  }
  
  /**
   * Aplica el switch de estado para una orden individual
   */
  private processOrder(order: any) {
    switch (order.workStatus) {
      case 1: // ESPERA
        this.starProcess(order);
        this.showInfo(`#${order.barcode} puesta en proceso (ESPERA → PROCESO).`);
        break;
      case 2: // PROCESO
        this.completeProcess(order);
        this.showInfo(`#${order.barcode} marcada como terminada (PROCESO → TERMINADO).`);
        break;
      case 4: // BLOQUEADO
        this.unBlockProcess(order);
        this.showInfo(`#${order.barcode} desbloqueada (BLOQUEADO → ESPERA).`);
        break;
      default:
        this.showInfo(`#${order.barcode} no puede ser procesada automáticamente desde el estado actual.`);
        break;
    }
  }
  
  filterByStatus(status: number) {
    const filtered = this.originalEntities.filter(x => x.workStatusIndicator === status);
    this.entities.set(filtered);
  }

  filterByRechazado() {
    const filtered = this.originalEntities.filter(x =>  x.workStatus === 5);
    this.entities.set(filtered);
  }

  filterByWorking() {
    const filtered = this.originalEntities.filter(x => x.workStatus === 2);
    this.entities.set(filtered);
  }

  get trabajosEnTiempo(): number {
    return this.entities().filter(x => x.workStatusIndicator === 1 &&  x.workStatus !== 4).length;
  }

  get trabajosConAlarma(): number {
    return this.entities().filter(x => x.workStatusIndicator === 2 &&  x.workStatus !== 4).length;
  }

  get trabajosConRetraso(): number {
    return this.entities().filter(x => x.workStatusIndicator === 3 &&  x.workStatus !== 4).length;
  }

  get trabajosRechazados(): number {
    return this.entities().filter(x => x.workStatus === 5).length;
  }

  get trabajosBloqueados(): number {
    return this.entities().filter(x => x.workStatus === 4).length;
  }

  get totalTrabajos(): number {
    return this.entities().length;
  }

  get totalTrabajosEnProceso(): number {
    return this.entities().filter(x => x.workStatus === 2).length;
  }

  get donutData() {
    return {
      labels: ['A tiempo', 'Con alarma', 'Con retraso', 'Bloqueados'],
      datasets: [
        {
          data: [
            this.trabajosEnTiempo,
            this.trabajosConAlarma,
            this.trabajosConRetraso,
            this.trabajosBloqueados
          ],
          backgroundColor: ['#4CAF50', '#FFEB3B', '#F44336','#9E9E9E'],
          hoverBackgroundColor: ['#66BB6A', '#FFF176', '#EF5350','#9E9E9E']
        }
      ]
    };
  }

  get donutOptions() {
    return {
      plugins: {
        legend: {
          position: 'bottom'
        }
      },
      responsive: true,
      maintainAspectRatio: false
    };
  }

  get trabajosEnProgresoList() {
    return this.entities().filter(e => e.workStatus === 2);
  }

  get trabajosATiempoList() {
    return this.entities().filter(e => e.workStatusIndicator === 1);
  }

  get trabajosConAlarmaList() {
    return this.entities().filter(e => e.workStatusIndicator === 2);
  }

  get trabajosConRetrasoList() {
    return this.entities().filter(e => e.workStatusIndicator === 3);
  }

  get trabajosConRehazoList() {
    return this.entities().filter(e => e.workStatus === 5);
  }

  private showError(detail: string) {
    this.messageService.add({ severity: 'error', summary: 'Error', detail, life: 3000 });
  }

  openOrderModal(orderDetail : StationWorkDetailViewModel) {
    if (orderDetail.orderId) {
      this.orderService.getById(orderDetail.orderId).subscribe({
        next: (res: any) => {
          if (res.success) { 
            this.orderEntity = res.payload;
            this.showDialog = true;
          } else if (res.errors?.length > 0) {
            this.showError(res.error?.errors?.Error[0] ?? 'No se pudo cargar la orden.');
          }
          this.loading = false;
        },
        error: err => this.showError(err.message)
      });
    }
  } 

  openMenu(event: MouseEvent, row: any): void {
    this.currentMenuItems = this.getActions(row);
    this.menu.toggle(event);
  }

  getActions(entity: any): MenuItem[] {
    return [
      {
        label: 'Procesar',
        icon: 'pi pi-cog', 
        disabled: entity.workStatus !== 1,
        command: () => this.starProcess(entity)
      },
      {
        label: 'Terminar',
        icon: 'pi pi-check', 
        disabled: entity.workStatus !== 2,
        command: () => this.completeProcess(entity)
      },
      {
        label: 'Bloquear',
        icon: 'pi pi-lock', 
        disabled: entity.workStatus === 4 || entity.workStatus >= 3,
        command: () => this.blockProcess(entity)
      },
      {
        label: 'Desbloquear',
        icon: 'pi pi-unlock', 
        disabled: entity.workStatus !== 4,
        command: () => this.unBlockProcess(entity)
      },
      {
        label: 'Rechazar',
        icon: 'pi pi-times', 
        disabled: entity.workStatus !== 2,
        command: () => this.rejectProcess(entity)
      }
    ];
  }

  getColorClass(t: any): string {
    switch (t.workStatusIndicator) {
      case 1: return 'text-green-500'; // A tiempo
      case 2: return 'text-yellow-500'; // Con alarma
      case 3: return 'text-red-500'; // Con retraso
      default:
        return t.workStatus === 5 ? 'text-gray-500' : 'text-blue-500'; // Bloqueado o por defecto
    }
  }

  
getIconClass(t: any): string {
  switch (t.workStatusIndicator) {
    case 1: return 'pi pi-check-circle text-green-600';
    case 2: return 'pi pi-exclamation-triangle text-yellow-600';
    case 3: return 'pi pi-times-circle text-red-600';
    default:
      return t.workStatus === 5 ? 'pi pi-ban text-gray-500' : 'pi pi-info-circle text-blue-500';
  }
}
showInfo(message: string) {
  this.messageService.add({ severity: 'info', summary: 'Información', detail: message });
}

getStatusLabel(status: number): string {
  switch (status) {
    case 1: return 'Espera';
    case 2: return 'Proceso';
    case 3: return 'Terminado';
    case 4: return 'Bloqueado';
    case 5: return 'Rechazado';
    default: return 'Desconocido';
  }
}

getStatusIcon(status: number): string {
  switch (status) {
    case 1: return 'pi pi-clock';
    case 2: return 'pi pi-spinner';
    case 3: return 'pi pi-check';
    case 4: return 'pi pi-lock';
    case 5: return 'pi pi-times-circle';
    default: return 'pi pi-question';
  }
}

getStatusSeverity(status: number): 'info' | 'success' | 'danger' | 'warn' | undefined {
  switch (status) {
    case 1: return 'info';     // Espera
    case 2: return 'warn';  // Proceso
    case 3: return 'success';  // Terminado
    case 4: return 'danger';   // Bloqueado
    case 5: return 'danger';   // Rechazado
    default: return undefined;
  }
}

statusOptions = [
  { label: 'Espera', value: 1 },
  { label: 'Proceso', value: 2 },
  { label: 'Terminado', value: 3 },
  { label: 'Bloqueado', value: 4 },
  { label: 'Rechazado', value: 5 }
];

highlightedOrderId: string | undefined | null = undefined;

getDelayMinutes(stationEndDate: Date): number {
  const now = new Date();
  const end = new Date(stationEndDate);
  const diffMs = now.getTime() - end.getTime();
  const diffMinutes = Math.floor(diffMs / (1000 * 60));
  return diffMinutes > 0 ? diffMinutes : 0;
}
}