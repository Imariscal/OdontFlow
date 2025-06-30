import { Component, Input } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-order-print',
  standalone: true,
  templateUrl: './order-print.component.html',
  styleUrls: ['./order-print.component.scss']
})
export class OrderPrintComponent {
  @Input() orderData: any;  

  constructor(private router: Router) {
    const nav = this.router.getCurrentNavigation();
    this.orderData = nav?.extras.state?.['orderData'];
  }

  print(): void {
    window.print();
  }
}
