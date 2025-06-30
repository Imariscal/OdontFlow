
export interface StationWorkSummaryViewModel {
  productividad: number;
  totalTrabajos: number;
  trabajosEnEspera: number;
  trabajosEnProceso: number;
  trabajosTerminados: number;
  trabajosBloqueados: number;
  trabajosRechazados: number;
  trabajosEnTiempo: number;
  trabajosConAlarma: number;
  trabajosConRetraso: number;
  trabajosTerminadosHoy: number;
  trabajosComprometidosHoy: number;
  porEstacion: StationWorkByStationViewModel[];
  porEmpleado: StationWorkByEmployeeViewModel[];
}

export interface StationWorkByStationViewModel {
    stationId: string;
    stationName: string;
    totalTrabajos: number;
    trabajosEnEspera: number;
    trabajosEnProceso: number;
    trabajosTerminados: number;
    trabajosBloqueados: number;
    trabajosRechazados: number;
    trabajosEnTiempo: number;
    trabajosConAlarma: number;
    trabajosConRetraso: number;
    trabajosTerminadosHoy: number;
    trabajosComprometidosHoy: number;
    order: number;
    cumplimiento : number;
    productividad : number;
  }

  export interface StationWorkByEmployeeViewModel {
    employeeId: string;
    employeeName: string;
  
    totalTrabajos: number;
    trabajosEnEspera: number;
    trabajosEnProceso: number;
    trabajosTerminados: number;
    trabajosBloqueados: number;
    trabajosRechazados: number;
  
    trabajosEnTiempo: number;
    trabajosConAlarma: number;
    trabajosConRetraso: number;
  
    trabajosTerminadosHoy: number;
    trabajosComprometidosHoy: number;
    cumplimiento : number;
    productividad : number;

  }
  