import { Component, OnInit, ViewChild, inject, signal, effect  } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Table } from 'primeng/table';
import { PrimengModule } from '../../../../shared/primeng.module';
import { OrderViewModel } from '../../../../core/model/order.model';
import { GetOrdersByAdvancedFilterQuery } from '../../../../core/model/get-orders-by-advanced-filter.model';
import { ReportService } from '../../../../core/services/report.service';
import { OrderModalComponent } from '../../order/modal/order-modal.component';
 
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
  templateUrl: './orders-report.component.html',
  imports: [CommonModule, FormsModule, PrimengModule, OrderModalComponent]
})
export class OrdersReportComponent implements OnInit {
    advancedFiltersVisible = false;
  // Datos
  entities = [];
  selectedEntities: OrderViewModel[] | null = null;
 
  // Tabla
  @ViewChild('dt') dt!: Table;
  cols: Column[] = [];
  exportColumns: ExportColumn[] = [];
  
  expandedRows = {};
  showDialog = false;
  orderEntity! : OrderViewModel;

  // Filtros de búsqueda
  filters: GetOrdersByAdvancedFilterQuery = {
  page: 1,
  pageSize: 50,
  creationDateStart: (() => {
    const d = new Date();
    d.setMonth(d.getMonth() - 1);
    return d;
  }) () as any,
  creationDateEnd: new Date() as any
  };
  
  scrollHeight: string = '65vh'; 

  ngAfterViewInit() {
    const offset = 310; // píxeles a restar por header, toolbar, etc.
    const vh = window.innerHeight;
    this.scrollHeight = `${vh - offset}px`;
  }


  // Catalogos (dummy para ejemplo)
  statusOptions = [
    { label: 'Creado', value: 1 },
    { label: 'Confirmada', value: 2 },
    { label: 'En Proceso', value: 3 },
    { label: 'Terminado', value: 4 },
    { label: 'Entregado', value: 5 },
    { label: 'Pagado', value: 6 },
  ];
  
  groupOptions = [
    { label: 'GAMMA', value: 1 },
    { label: 'ZIRCONIA', value: 2}
  ];

  /* indicadores */ 

  indicators = {
    totalOrders: 0,
    totalProducts: 0,
    totalPagadas: 0,
    totalPendientes: 0,
    totalFacturadas: 0,
    totalSinFactura: 0,
    totalBalancePendiente: 0,
    totalPagado: 0,
    subtotalGeneral: 0,
    ivaGeneral: 0,
    totalGeneral: 0,
  };
  // Servicios
  private reportService = inject(ReportService);

  ngOnInit() {
    this.initializeColumns();
    this.loadEntities();
 
    
  }

  private initializeColumns() {
    this.cols = [
      { field: 'barcode', header: 'Orden' },
      { field: 'clientName', header: 'Cliente' },
      { field: 'patientName', header: 'Paciente' },
      { field: 'workGroup', header: 'Grupo' },
      { field: 'orderStatus', header: 'Estatus' },
      { field: 'creationDate', header: 'Fecha Creación' },
      { field: 'deliveryDate', header: 'Fecha Entrega' },
      { field: 'balance', header: 'Saldo' },
    ];
    this.exportColumns = this.cols.map(col => ({
      title: col.header,
      dataKey: col.field
    }));
  }

  private loadEntities() {
    this.reportService.getOrdersByAdvancedFilter(this.filters).subscribe({
      next: (res: any) => {
        if (res.success && res.payload?.items?.length > 0) {
          this.entities = res.payload.items || [];
        } else {
          this.entities = [];
        }

        this.calculateIndicators(this.entities);
      }
    });
  }   
  
  exportCSV() {
    this.dt.exportCSV();
  }

  onGlobalFilter(table: Table, event: Event) {
    table.filterGlobal((event.target as HTMLInputElement).value, 'contains');
  }

  search() {
    this.filters.page = 1; // Reiniciar la paginación al buscar
    this.loadEntities();

  }

  clearFilters() {
    this.filters = {
      page: 1,
      pageSize: 50
    };
    this.loadEntities();
  }

  clear(table: Table) {
    table.clear();
  }


  getStatusSeverity(status: string): 'success' | 'secondary' | 'info' | 'warn' | 'danger' | 'contrast' | undefined {
    switch (status) {
      case 'CREADO':
        return 'info';
      case 'CONFIRMADA':
        return 'info';
      case 'EN_PROCESO':
        return 'warn';
      case 'TERMINADO':
        return 'success';
      case 'ENTREGADO':
        return 'success';
      case 'PAGADO':
        return 'success';
      default:
        return 'info';
    }
  }


  
    openOrderModal(orderDetail : OrderViewModel) {
        this.orderEntity = orderDetail;
        this.showDialog = true;
    } 

    private calculateIndicators(data: OrderViewModel[]): void {
      this.indicators = {
        totalOrders: data.length,
        totalProducts: data.reduce((sum, o) => sum + (o.items?.length ?? 0), 0),
        totalPagadas: data.filter(o => o.paymentComplete).length,
        totalPendientes: data.filter(o => !o.paymentComplete).length,
        totalFacturadas: data.filter(o => o.applyInvoice).length,
        totalSinFactura: data.filter(o => !o.applyInvoice).length,
        totalBalancePendiente: data.reduce((sum, o) => sum + (o.balance ?? 0), 0),
        totalPagado: data.reduce((sum, o) => sum + (o.payment ?? 0), 0),
        subtotalGeneral: data.reduce((sum, o) => sum + (o.subtotal ?? 0), 0),
        ivaGeneral: data.reduce((sum, o) => sum + (o.tax ?? 0), 0),
        totalGeneral: data.reduce((sum, o) => sum + (o.total ?? 0), 0)
      };
    } 
 
}
