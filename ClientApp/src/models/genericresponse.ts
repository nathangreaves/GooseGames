export class GenericResponseBase {
  success: boolean;
  errorCode: string;
}

export class GenericResponse<T> extends GenericResponseBase {
  data: T;
}
