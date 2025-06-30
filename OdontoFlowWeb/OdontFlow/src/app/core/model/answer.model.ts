export interface Answer<T> {
  success: boolean;
  message?: string;
  payload: T;
}