import { GenericResponse, GenericResponseBase } from "../../../models/genericresponse";
import { GenericResponseHandler, GenericResponseBaseHandler } from "../../werewords/scaffolding/content";

export class TableComponentBase {

  Loading: boolean;
  ErrorMessage: string;

  HandleGenericResponse<T>(genericResponse: GenericResponse<T>, handleSuccess: GenericResponseHandler<T>) {
    if (genericResponse.success) {
      return handleSuccess(genericResponse.data);
    }
    else {
      this.SetErrorMessage(genericResponse.errorCode);
    }
    return genericResponse;
  }

  HandleGenericResponseBase(genericResponse: GenericResponseBase, handleSuccess: GenericResponseBaseHandler) {
    if (genericResponse.success) {
      return handleSuccess();
    }
    else {
      this.SetErrorMessage(genericResponse.errorCode);
    }
    return genericResponse;
  }

  SetErrorMessage = (err: string) => {
    this.ErrorMessage = err;
  }

  HandleGenericError = (err: any) => {
    console.error(err);
    this.SetErrorMessage("Unexpected Error");
  }
}
