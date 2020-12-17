import { Type } from '@angular/core';
import { Directive, ViewContainerRef } from '@angular/core';
import { LetterJamPlayerStatus } from "../../../models/letterjam/content";
import { GenericResponseBase } from '../../../models/genericresponse';

@Directive({
  selector: '[letterjam-content]',
})
export class LetterJamContentDirective {
  constructor(public viewContainerRef: ViewContainerRef) { }
}

export class LetterJamContent {
  constructor(public Key: LetterJamPlayerStatus, public component: Type<any>) { }
}

export type LetterJamRouter = (status: LetterJamPlayerStatus, validated: boolean) => void;
export type SetSessionData = (sessionId: string, playerId: string, gameId: string) => void;
export type ReadSessionData = (gameIdentifier: string) => boolean;
export type GenericResponseHandler<T> = (data: T) => Promise<GenericResponseBase> | GenericResponseBase;
export type GenericResponseBaseHandler = () => Promise<GenericResponseBase> | GenericResponseBase;
