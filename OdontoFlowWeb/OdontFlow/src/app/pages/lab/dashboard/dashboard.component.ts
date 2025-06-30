import { Component, OnInit, effect, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ChartModule } from 'primeng/chart';
import { CardModule } from 'primeng/card';
import { LabService } from '../../../core/services/lab.service';
import { MessageService } from 'primeng/api';
import { StationWorkByStationViewModel, StationWorkSummaryViewModel } from '../../../core/model/station-work-summary.model';
import { StationWorkSignalRService } from '../../../core/signalr/station-work-signalr.service';
import { Router } from '@angular/router';

@Component({
  standalone: true,
  templateUrl: './dashboard.component.html',
  imports: [CommonModule, ChartModule, CardModule],
})
export class LabDashboardComponent implements OnInit {

  service = inject(LabService);
  summary!: StationWorkSummaryViewModel;
  stationChartData: any;
  donutChartOptions: any;
  barChartOptions: any;
  barChartDataGeneral: any;
  barChartDataByStation: any;
  router = inject(Router);

  
  private messageService = inject(MessageService);
  private signalR = inject(StationWorkSignalRService);

  constructor() {
    effect(() => {
      if (this.signalR.trigger()) {
        this.loadEnities();
        this.signalR.trigger.set(false);
      }
    });
  }

  ngOnInit(): void {
    this.loadEnities();

    this.donutChartOptions = {
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
              return `${context.label}: ${value} (${percentage}%)`;
            }
          }
        }
      },
      responsive: true,
      maintainAspectRatio: false
    };

    this.barChartOptions = {
      responsive: true,
      plugins: {
        legend: {
          position: 'top'
        },
        tooltip: {
          callbacks: {
            label: function (context: any) {
              return `${context.dataset.label}: ${context.raw}`;
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

  private loadEnities() {
    this.service.get().subscribe({
      next: (res: any) => {
        if (res.success) {
          const result = res.payload as StationWorkSummaryViewModel;
          result.porEstacion.sort((a, b) => a.order - b.order);
          this.summary = result;
       

          this.barChartDataGeneral = {
            labels: ['Comprometidos Hoy', 'Terminados Hoy'],
            datasets: [
              {
                label: 'General',
                backgroundColor: ['#42A5F5', '#66BB6A'],
                data: [
                  result.trabajosComprometidosHoy,
                  result.trabajosTerminadosHoy
                ]
              }
            ]
          };

          this.barChartDataByStation = {
            labels: result.porEstacion.map(e => e.stationName),
            datasets: [
              {
                label: 'Comprometidos Hoy',
                backgroundColor: '#42A5F5',
                data: result.porEstacion.map(e => e.trabajosComprometidosHoy)
              },
              {
                label: 'Terminados Hoy',
                backgroundColor: '#66BB6A',
                data: result.porEstacion.map(e => e.trabajosTerminadosHoy)
              }
            ]
          };

        } else {
          if (res.errors?.length > 0) {
            this.showError(res.error?.errors?.Error[0] ?? 'No se pudieron cargar la lista de clientes.');
          }
        }
      },
      error: err => this.showError(err.message)
    });
  }

  buildDonutData(station: StationWorkByStationViewModel) {
    return {
      labels: ['En Tiempo', 'Con Alarma', 'Con Retraso', 'Bloqueados'],
      datasets: [
        {
          data: [
            station.trabajosEnTiempo,
            station.trabajosConAlarma,
            station.trabajosConRetraso,
            station.trabajosBloqueados
          ],
          backgroundColor: ['#4CAF50', '#FFC107', '#F44336', '#9E9E9E'],
          hoverBackgroundColor: ['#388E3C', '#FFA000', '#D32F2F','#9E9E9E']
        }
      ]
    };
  }

  onStationChartClick(stationId: string) {
    this.router.navigate(['/pages/station', stationId]);
  }


  private showError(detail: string) {
    this.messageService.add({ severity: 'error', summary: 'Error', detail, life: 3000 });
  }

} 
