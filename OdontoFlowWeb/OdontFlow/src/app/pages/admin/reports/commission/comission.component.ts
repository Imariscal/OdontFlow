import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PrimengModule } from '../../../../shared/primeng.module';
import { ReportService } from '../../../../core/services/report.service';
import { GetOrdersByAdvancedFilterQuery } from '../../../../core/model/get-orders-by-advanced-filter.model';
import { CommissionOrdersReportViewModel } from '../../../../core/model/comission-view.model';
import { MessageService } from 'primeng/api';
import { OrderViewModel } from '../../../../core/model/order.model';
import { OrderModalComponent } from '../../order/modal/order-modal.component';
import { ChartModule } from 'primeng/chart';
 
@Component({
  selector: 'app-commission-report',
  standalone: true,
  templateUrl: './comission.component.html',
  imports: [CommonModule, FormsModule, PrimengModule, OrderModalComponent, ChartModule]
})
export class CommissionReportComponent implements OnInit {
  showDialog = false;
  orderEntity! : OrderViewModel;
  donutChartData: any;
  barChartData: any;
  donutOptions: any;
  barOptions: any;
  showFilters = false;
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
  private messageService = inject(MessageService);
  reportOrders: CommissionOrdersReportViewModel = {
    items: [],
    summary: [],
    totalRecords: 0
  };

  scrollHeight: string = '65vh'; 

  ngAfterViewInit() {
    const offset = 310; // píxeles a restar por header, toolbar, etc.
    const vh = window.innerHeight;
    this.scrollHeight = `${vh - offset}px`;
  }

  private reportService = inject(ReportService);

  ngOnInit() {
    this.loadReport();

    this.donutOptions = {
      plugins: {
        legend: {
          position: 'bottom'
        },
        tooltip: {
          callbacks: {
            label: function (context: any) {
              const total = context.dataset.data.reduce((a: number, b: number) => a + b, 0);
              const value = context.raw;
              const percentage = ((value / total) * 100).toFixed(1);
              return `$${value} (${percentage}%)`;
            }
          }
        }
      },
      responsive: true,
      maintainAspectRatio: false
    };

    
    this.barOptions = {
 
      plugins: {
        legend: {
          position: 'top'
        },
        tooltip: {
          callbacks: {
            label: function (context: any) {
              return `${context.dataset.label}: $${context.raw}`;
            }
          }
        }
      },
      scales: {
        y: {
          beginAtZero: true,
          ticks: {
            stepSize: 1
          }
        }
      }
    };
  }

  loadReport() {

        try {
          this.reportService.getComissionByAdvancedFilter(this.filters).subscribe({
            next: (res : any) => {
              if (res.success) {
                debugger;
                this.reportOrders = res.payload;  
         
                this.donutChartData = {
                  labels: this.reportOrders.summary.map(s => s.employeeName),
                  datasets: [
                    {
                      data: this.reportOrders.summary.map(s => s.totalCommission),
                      backgroundColor: ['#42A5F5', '#66BB6A', '#FFA726', '#AB47BC', '#EC407A']
                    }
                  ]
                };
                
                this.barChartData = {
                  labels: this.reportOrders.summary.map(s => s.employeeName),
                  datasets: [
                    {
                      label: 'Comisión Total',
                      backgroundColor: '#42A5F5',
                      data: this.reportOrders.summary.map(s => s.totalCommission)
                    }
                  ]
                };
              }
              else {
                this.messageService.add({
                  severity: 'error',
                  summary: 'Error',
                  detail: res?.error?.errors?.Error?.[0] ?? 'Error desconocido',
                  life: 3000
                });
              }
            },
            error: (err) => {
              this.messageService.add({
                severity: 'error',
                summary: 'Error del servidor',
                detail: err?.error?.errors?.Error?.[0] ?? 'Error inesperado',
                life: 3000
              });
            }
          });
        } catch (err: any) {
          this.messageService.add({
            severity: 'error',
            summary: 'Error del servidor',
            detail: err?.error?.errors?.Error?.[0] ?? 'Error inesperado',
            life: 3000
          });
        }
   
  }

  search() {
    this.filters.page = 1;
    this.loadReport();
  }

  clearFilters() {
    this.filters = { page: 1, pageSize: 50 };
    this.loadReport();
  }

  exportCSV() {
 
  }
     openOrderModal(orderDetail : OrderViewModel) {
          this.orderEntity = orderDetail;
          this.showDialog = true;
      } 
}
