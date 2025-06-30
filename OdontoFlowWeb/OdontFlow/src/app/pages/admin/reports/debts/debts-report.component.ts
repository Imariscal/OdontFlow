import { Component, OnInit, ViewChild, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Table } from 'primeng/table';
import { firstValueFrom } from 'rxjs';

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
  templateUrl: './debts-report.component.html',
  imports: [CommonModule, FormsModule, PrimengModule, OrderModalComponent]
})
export class DebtsReportComponent implements OnInit {
  // 📌 UI states
  advancedFiltersVisible = false;
  showDialog = false;

  // 📌 Table & pagination
  @ViewChild('dt') dt!: Table;
  entities: OrderViewModel[] = [];
  selectedEntities: OrderViewModel[] | null = null;
  orderEntity!: OrderViewModel;
  cols: Column[] = [];
  exportColumns: ExportColumn[] = [];
  expandedRows: { [key: string]: boolean } = {};
  totalRecords = 0;
  scrollHeight: string = '65vh';

  // 📌 Filters
  filters: GetOrdersByAdvancedFilterQuery = {
    page: 1,
    pageSize: 50,
    creationDateStart: (() => {
      const d = new Date();
      d.setMonth(d.getMonth() - 1);
      return d;
    })() as any,
    creationDateEnd: new Date() as any
  };

  groupOptions = [
    { label: 'GAMMA', value: 1 },
    { label: 'ZIRCONIA', value: 2 }
  ];

  // 📌 Summary indicators
  indicators = {
    totalOrders: 0,
    totalBalance: 0,
    averageDaysInDebt: 0,
    maxDaysInDebt: 0,
    maxBalance: 0
  };

  // 📌 Services
  private reportService = inject(ReportService);

  // 📌 Lifecycle
  ngOnInit() {
    this.initializeColumns();
    this.loadEntities();
  }

  ngAfterViewInit() {
    const offset = 310;
    this.scrollHeight = `${window.innerHeight - offset}px`;
  }

  // 📌 Column setup
  private initializeColumns() {
    this.cols = [
      { field: 'barcode', header: 'Orden' },
      { field: 'clientName', header: 'Cliente' },
      { field: 'patientName', header: 'Paciente' },
      { field: 'workGroup', header: 'Grupo' },
      { field: 'orderStatus', header: 'Estatus' },
      { field: 'creationDate', header: 'Fecha Creación' },
      { field: 'deliveryDate', header: 'Fecha Entrega' },
      { field: 'balance', header: 'Saldo' }
    ];

    this.exportColumns = this.cols.map(col => ({
      title: col.header,
      dataKey: col.field
    }));
  }

  // 📌 Data loading with lazy pagination
  async loadEntities() {
    try {
      const res = await firstValueFrom(
        this.reportService.getDebtsByAdvancedFilter(this.filters)
      );

      if (res.success) {
        const payload = res.payload;
        this.entities = payload.items ?? [];
        this.totalRecords = payload.totalCount ?? 0;

        this.indicators = payload.summary
          ? {
              totalOrders: payload.summary.totalOrders,
              totalBalance: payload.summary.totalAmount,
              averageDaysInDebt: payload.summary.avgDaysInDebt,
              maxDaysInDebt: payload.summary.maxDaysInDebt,
              maxBalance: payload.summary.maxSingleDebt
            }
          : {
              totalOrders: 0,
              totalBalance: 0,
              averageDaysInDebt: 0,
              maxDaysInDebt: 0,
              maxBalance: 0
            };
      } else {
        this.entities = [];
        this.totalRecords = 0;
      }
    } catch (error) {
      console.error('Error loading debt data:', error);
      this.entities = [];
      this.totalRecords = 0;
    }
  }

  // 📌 Lazy loading event
  onLazyLoad(event: any) {
    this.filters.page = Math.floor(event.first / event.rows) + 1;
    this.filters.pageSize = event.rows;
    this.loadEntities();
  }

  // 📌 Filters
  search() {
    this.filters.page = 1;
    this.loadEntities();
  }

  clearFilters() {
    this.filters = {
      page: 1,
      pageSize: 50
    };
    this.loadEntities();
  }

  // 📌 UI helpers
  clear(table: Table) {
    table.clear();
  }

 

  openOrderModal(orderDetail: OrderViewModel) {
    this.orderEntity = orderDetail;
    this.showDialog = true;
  }

  getStatusSeverity(status: string): 'success' | 'secondary' | 'info' | 'warn' | 'danger' | 'contrast' | undefined {
    switch (status) {
      case 'CREADO': return 'info';
      case 'CONFIRMADA': return 'info';
      case 'EN_PROCESO': return 'warn';
      case 'TERMINADO': return 'success';
      case 'ENTREGADO': return 'success';
      case 'PAGADO': return 'success';
      default: return 'info';
    }
  }

  getDebtSeverity(days: number): 'success' | 'warn' | 'danger' {
    if (days <= 15) return 'success';
    else if (days <= 30) return 'warn';
    else return 'danger';
  }

  // Export excel 
  async exportAllRecords() {
  try {
    const res = await firstValueFrom(this.reportService.exportDebtOrders(this.filters));
    if (res.success && res.payload?.length) {
      const exportData = res.payload.map(order => ({
        Orden: order.barcode,
        Cliente: order.clientName,
        Paciente: order.patientName,
        Grupo: order.workGroup,
        FechaAlta: new Date(order.creationDate!).toLocaleDateString(),
        Subtotal: order.subtotal,
        IVA: order.tax,
        Total: order.total,
        Pagos: order.payment,
        Adeudo: order.balance,
        Días: order.daysInDebt
      }));

      // Exportación simple a CSV
      const csv = this.convertToCSV(exportData);
      const blob = new Blob([csv], { type: 'text/csv;charset=utf-8;' });
      const link = document.createElement('a');
      link.href = URL.createObjectURL(blob);
      link.setAttribute('download', 'reporte_de_adeudos.csv');
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
    }
  } catch (error) {
    console.error('Error exporting all records:', error);
  }
}

private convertToCSV(objArray: any[]): string {
  const header = Object.keys(objArray[0]).join(',');
  const rows = objArray.map(row => Object.values(row).join(','));
  return [header, ...rows].join('\r\n');
}

}
