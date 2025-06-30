export interface PagedResult<T> { 
    items: T[];
    totalCount: number;
    page: number;
    pageSize: number;
    summary? : DebtSummaryViewModel 
  }

  export interface DebtSummaryViewModel {
  totalOrders: number;
  totalAmount: number;
  maxSingleDebt: number;
  maxDaysInDebt: number;
  avgDaysInDebt: number;
}