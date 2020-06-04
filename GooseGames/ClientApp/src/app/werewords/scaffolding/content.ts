import { Type } from '@angular/core';
import { Directive, ViewContainerRef } from '@angular/core';
import { WerewordsPlayerStatus } from "../../../models/werewords/content";
import { GenericResponseBase } from '../../../models/genericresponse';

@Directive({
  selector: '[werewords-content]',
})
export class WerewordsContentDirective {
  constructor(public viewContainerRef: ViewContainerRef) { }
}

export class WerewordsContent {
  constructor(public Key: WerewordsPlayerStatus, public component: Type<any>) { }
}

export type WerewordsRouter = (status: WerewordsPlayerStatus, validated: boolean) => void;
export type SetSessionData = (sessionId: string, playerId: string, gameId: string) => void;
export type ReadSessionData = (gameId: string) => boolean;
export type GenericResponseHandler<T> = (data: T) => Promise<GenericResponseBase>;
export type GenericResponseBaseHandler = () => Promise<GenericResponseBase>;
