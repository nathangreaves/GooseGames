import { Type } from '@angular/core';
import { Directive, ViewContainerRef } from '@angular/core';

@Directive({
  selector: '[werewords-content]',
})
export class WerewordsContentDirective {
  constructor(public viewContainerRef: ViewContainerRef) { }
}

export class WerewordsContent {
  constructor(public Key: WerewordsContentEnum, public component: Type<any>, public router: WerewordsRouter) { }
}

export enum WerewordsContentEnum {
  NightSecretRole,
  NightSecretWord
}

type WerewordsRouter = (status: WerewordsContentEnum) => void;

export interface IWerewordsComponent {

  router: WerewordsRouter
  //To hook this up, inherit from this interface, create a new enum value in WerewordsContentEnum
  //Then register your new component in module entryComponents in app.module.ts
  //Then add it to the collection RegisteredContent above
}


export class WerewordsComponentBase implements IWerewordsComponent {
  ErrorMessage: string;
  Loading: boolean;
  router: WerewordsRouter

  Route(status: WerewordsContentEnum) {
    this.router(status);
  }

  HandleGenericError(err: any) {
    console.error(err);
    this.ErrorMessage = "Unexpected Error";
  }
}
