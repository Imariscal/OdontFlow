import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SpinnerService } from '../../core/services/spinner.service';
import { ProgressSpinnerModule } from 'primeng/progressspinner';

@Component({
  selector: 'app-spinner',
  standalone: true,
  imports: [CommonModule, ProgressSpinnerModule],
  template: `
    <div class="spinner-overlay" *ngIf="spinnerService.loading$ | async">
      <p-progressSpinner styleClass="custom-spinner" strokeWidth="4" animationDuration=".5s"></p-progressSpinner>
    </div>
  `,
  styles: [`
    .spinner-overlay {
      position: fixed;
      top: 0;
      left: 0;
      z-index: 9999;
      width: 100vw;
      height: 100vh;
      background: rgba(255, 255, 255, 0.6);
      display: flex;
      align-items: center;
      justify-content: center;
    }
    .custom-spinner {
      width: 60px;
      height: 60px;
    }
  `]
})
export class AppSpinnerComponent {
  constructor(public spinnerService: SpinnerService) {}
}
