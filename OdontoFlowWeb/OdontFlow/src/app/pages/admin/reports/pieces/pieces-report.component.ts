import { Component, OnInit, ViewChild, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { forkJoin } from 'rxjs';
import { PrimengModule } from '../../../../shared/primeng.module';
import { ReportService } from '../../../../core/services/report.service';
import { GetOrdersByAdvancedFilterQuery } from '../../../../core/model/get-orders-by-advanced-filter.model';
import { OrderModalComponent } from '../../order/modal/order-modal.component';
import { WorkedPiecesReportViewModel, WorkedPieceViewModel } from '../../../../core/model/worked-piece-view.model';
import { SelectItemGroup, TreeNode } from 'primeng/api'; // <-- agrega esto
import { TreeTable, TreeTableModule } from 'primeng/treetable';
import { ProductService } from '../../../../core/services/product.service';
import { MultiSelectModule } from 'primeng/multiselect';
import { ChartModule } from 'primeng/chart';

@Component({
  standalone: true,
  templateUrl: './pieces-report.component.html',
  imports: [CommonModule, FormsModule, PrimengModule, TreeTableModule, ChartModule,  OrderModalComponent, MultiSelectModule]
})
export class PiecesReportComponent implements OnInit {
  advancedFiltersVisible = false;

  pieces: WorkedPieceViewModel[] = [];
  selectedPieces: WorkedPieceViewModel[] = [];
  treeData: TreeNode[] = [];
  donutChartData: any;
  barChartData: any;
  donutOptions: any;
  barOptions: any;

  labSummary: any[] = [];
  summaryColumns: { field: string; header: string }[] = [];

  @ViewChild('dt') treeTable!: TreeTable; 

  showDialog = false;
  orderEntity: any = null;
  showProductColumns = true;

  groupedProducts: SelectItemGroup[] = [];

  
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

  private reportService = inject(ReportService);
  private productService = inject(ProductService);

  ngOnInit() {
    this.loadPieces();
    this.loadProducts();

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
              return `${value} (${percentage}%)`;
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
 
  
  
  scrollHeight: string = '65vh'; 

  ngAfterViewInit() {
    const offset = 310; // píxeles a restar por header, toolbar, etc.
    const vh = window.innerHeight;
    this.scrollHeight = `${vh - offset}px`;
  }
  

  private loadPieces() {
    this.reportService.getWorkPiecesByAdvancedFilter(this.filters).subscribe({
      next: (res : any) => {
        if (res && res.payload?.items?.length > 0) {

          const result :WorkedPiecesReportViewModel = res.payload.items[0];
          this.pieces = result.items;  
  
          // Donut Chart (Top 5)
          this.donutChartData = {
            labels: result.topProducts.map((p :any) => p.productName),
            datasets: [{
              data: result.topProducts.map((p :any) => p.pieces),
              backgroundColor: ['#FF6384', '#36A2EB', '#FFCE56', '#4BC0C0', '#9966FF']
            }]
          };
  
          // Bar Chart (All Products)
          this.barChartData = {
            labels: result.allProducts.map((p :any) => p.productName),
            datasets: [{
              label: 'Piezas trabajadas',
              data: result.allProducts.map((p :any) => p.pieces),
              backgroundColor: '#42A5F5'
            }]
          };

           // 🔥 Aquí agregas el procesamiento del consolidado
          this.loadLabSummary(result.consolidadoPorLaboratorista);
  
        } else {
          this.treeData = [];
          this.donutChartData = null;
          this.barChartData = null;
        }
      },
      error: (err) => {
        this.treeData = [];
        this.donutChartData = null;
        this.barChartData = null;
        console.error('Error al cargar piezas trabajadas', err);
      }
    });
  }


  private loadLabSummary(labData: any[]) {
    if (!labData || labData.length === 0) {
      this.labSummary = [];
      this.summaryColumns = [];
      return;
    }
  
    this.labSummary = labData.map(item => ({
      laboratorista: item.laboratorista,
      ordenes: item.ordenes,
      productos: item.productos
    }));
  
    const uniqueProducts = new Set<string>();
    labData.forEach(item => {
      Object.keys(item.productos).forEach(prod => uniqueProducts.add(prod));
    });
  
    this.summaryColumns = [
      { field: 'laboratorista', header: 'Laboratorista' },
      { field: 'ordenes', header: 'Órdenes' },
      ...Array.from(uniqueProducts).map(prod => ({
        field: prod,
        header: prod
      }))
    ];
  }
  
 

  
  private loadProducts() {
    forkJoin([
      this.productService.getByCategory(1),
      this.productService.getByCategory(2)
    ]).subscribe(([gammaProducts, zirconiaProducts]) => {
 
      this.groupedProducts = [
        {
          label: 'GAMMA',
          value: '1',
          items: (gammaProducts.payload || []).map((p: any) => ({
            label: p.name,
            value: p.id
          }))
        },
        {
          label: 'ZIRCONIA',
          value: '2',
          items: (zirconiaProducts.payload  || []).map((p: any) => ({
            label: p.name,
            value: p.id
          }))
        }
      ];
    });
  }
  
  
  search() {
    this.filters.page = 1;
    this.loadPieces();
  }

  clearFilters() {
    this.filters = { page: 1, pageSize: 50 };
    this.loadPieces();
  }

  exportCSV() {
   // this.dt.exportCSV();
  }

  openOrderModal(orderDetail: any) {
    this.orderEntity = orderDetail;
    this.showDialog = true;
  }

  getTeethAsString(teeth: string[] = []): string {
    return teeth.length > 0 ? teeth.join(', ') : '';
  }

  isProductLevel(rowNode: any): boolean {
    return rowNode.level >= 2;  
  }

  expandAllRows() {
    this.treeData.forEach(node => {
      this.expandRecursive(node, true);
    });
  }
  
  collapseAllRows() {
    this.treeData.forEach(node => {
      this.expandRecursive(node, false);
    });
  }
  
  private expandRecursive(node: TreeNode, isExpand: boolean) {
    node.expanded = isExpand;
    if (node.children) {
      node.children.forEach(child => {
        this.expandRecursive(child, isExpand);
      });
    }
  }
 
}
