import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ChartModule } from 'primeng/chart';
import { TableModule } from 'primeng/table';
import { DropdownModule } from 'primeng/dropdown';
import { TagModule } from 'primeng/tag';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { PrimengModule } from '../../../shared/primeng.module';
import { ReportService } from '../../../core/services/report.service'; 
import { StationWorkDetailViewModel } from '../../../core/model/station-work-details.model';
import { ProductivityReportViewModel } from '../../../core/model/productivity-report.model';
import { StationWorkByEmployeeViewModel } from '../../../core/model/station-work-summary.model';
import { OrderModalComponent } from '../../admin/order/modal/order-modal.component';
import { OrderViewModel } from '../../../core/model/order.model';
import { OrderService } from '../../../core/services/order.service';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-productivity',
  standalone: true,
  templateUrl: './productivity.component.html',
  imports: [
    CommonModule,
    FormsModule,
    ChartModule,
    TableModule,
    DropdownModule,
    TagModule,
    ProgressSpinnerModule,
    PrimengModule,
    OrderModalComponent
  ]
})
export class ProductivityComponent implements OnInit {
  loading = true;
  selectedFilter = 'currentWeek';
  summary: ProductivityReportViewModel | null = null;
  detail: StationWorkDetailViewModel[] = [];
  detailByEmployee: StationWorkByEmployeeViewModel[] = [];
  donutData: any;
  donutOptions: any;
  showDialog = false;
  orderEntity: OrderViewModel = {};
  
  private reportService = inject(ReportService);
  private orderService = inject(OrderService);
  private messageService = inject(MessageService);

  ngOnInit(): void {
    this.loadData();
  }

  onFilterChange() {
    this.loadData();
  }

  private loadData() {
    const { startDate, endDate } = this.getDateRange(this.selectedFilter);
  
    this.loading = true;
  
    this.reportService.getResume(startDate, endDate).subscribe({
      next: (res: any) => {
        if (res.success) {
          this.summary = res.payload;
          this.setupDonutChart();
        }
        this.loading = false;
      },
      error: () => this.loading = false
    });
  
    this.reportService.getStationDetail(startDate, endDate).subscribe({
      next: (res: any) => {
        if (res.success) {
          this.detail = res.payload?.detallePorEstacion ?? [];
        }
      },
      error: () => {}
    });
  
    this.reportService.getEmploeeDetail(startDate, endDate).subscribe({
      next: (res: any) => {
        if (res.success) {
          this.detailByEmployee = res.payload?.detallePorEmpleado ?? [];
        }
      },
      error: () => {}
    });
  }

  private setupDonutChart() {
    if (!this.summary) return;
    this.donutData = {
      labels: ['A Tiempo', 'Con Alarma', 'Con Retraso', 'Bloqueados'],
      datasets: [
        {
          data: [
            this.summary.trabajosEnTiempo ?? 0,
            this.summary.trabajosConAlarma ?? 0,
            this.summary.trabajosConRetraso ?? 0,
            this.summary.trabajosBloqueados ?? 0
          ]
        }
      ]
    };

    this.donutOptions = {
      responsive: true,
      plugins: {
        legend: {
          position: 'bottom'
        },
        tooltip: {
          callbacks: {
            label: function (context: any) {
              return `${context.label}: ${context.raw}`;
            }
          }
        }
      }
    };
  }

  private getDateRange(option: string): { startDate: Date, endDate: Date } {
    const today = new Date();
    let start: Date;
    let end: Date = new Date(today);

    switch (option) {
      case 'currentWeek': {
        const dow = today.getDay();
        const daysSinceThursday = (dow + 7 - 4) % 7;
        start = new Date(today);
        start.setDate(today.getDate() - daysSinceThursday);
        break;
      }
      case 'lastWeek': {
        const lastFriday = new Date(today);
        lastFriday.setDate(today.getDate() - (((today.getDay() + 7 - 5) % 7) + 7));
        start = new Date(lastFriday);
        start.setDate(lastFriday.getDate() - 6);
        end = lastFriday;
        break;
      }
      case 'currentMonth': {
        start = new Date(today.getFullYear(), today.getMonth(), 1);
        break;
      }
      case 'lastMonth': {
        start = new Date(today.getFullYear(), today.getMonth() - 1, 1);
        end = new Date(today.getFullYear(), today.getMonth(), 0);
        break;
      }
      default:
        start = new Date(today);
    }

    return { startDate: start, endDate: end };
  }

  getProductivityClass(percent: string): string {
    if (!percent || percent === 'N/A') return 'text-gray-500 font-medium';
  
    const value = parseFloat(percent.replace('%', ''));
  
    if (value >= 120) return 'text-green-600 font-bold';
    if (value >= 80) return 'text-yellow-600 font-semibold';
    return 'text-red-600 font-semibold';
  }

  openOrderModal(orderId: any) {
    if (orderId) {
      this.orderService.getById(orderId).subscribe({
        next: (res: any) => {
          if (res.success) {
            this.orderEntity = res.payload;
            this.showDialog = true;
          } else if (res.errors?.length > 0) {
            this.showError(res.errors[0] ?? 'No se pudo cargar la orden.');
          }
          this.loading = false;
        },
        error: err => this.showError(err.message)
      });
    }
  }
  
  private showError(detail: string) {
    this.messageService.add({ severity: 'error', summary: 'Error', detail, life: 3000 });
  }
}
