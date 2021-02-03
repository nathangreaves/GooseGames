import { Type } from '@angular/core';
import { Directive, ViewContainerRef } from '@angular/core';
import { GenericResponseBase } from '../../../models/genericresponse';
import { AvalonPlayerStatus } from '../../../models/avalon/content';

@Directive({
  selector: '[avalon-content]',
})
export class AvalonContentDirective {
  constructor(public viewContainerRef: ViewContainerRef) { }
}

export class AvalonContent {
  constructor(public Key: AvalonPlayerStatus, public component: Type<any>) { }
}

export type AvalonRouter = (status: AvalonPlayerStatus, validated: boolean) => void;
export type SetSessionData = (sessionId: string, playerId: string, gameId: string) => void;
export type ReadSessionData = (gameIdentifier: string) => boolean;
export type GenericResponseHandler<T> = (data: T) => Promise<GenericResponseBase> | GenericResponseBase;
export type GenericResponseBaseHandler = () => Promise<GenericResponseBase> | GenericResponseBase;
