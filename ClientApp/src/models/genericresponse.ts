export class GenericResponse<T> {
  success: boolean;
  errorCode: string;
  data: T;
}
