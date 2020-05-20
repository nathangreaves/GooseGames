import { Component, OnInit, ViewChild, ComponentFactoryResolver } from '@angular/core';
import * as _ from 'lodash';
import { IWerewordsComponent, WerewordsContent, WerewordsContentDirective, WerewordsContentEnum } from '../../models/werewords/content';
import { RegisteredContent } from '../../models/werewords/registered-content';

@Component({
  selector: 'app-werewords-component',
  templateUrl: './werewords.component.html'
})
export class WerewordsComponent implements OnInit {

  @ViewChild(WerewordsContentDirective, { static: true }) contentHost: WerewordsContentDirective;
  ComponentSet: boolean;

  ngOnInit(): void {

    //this.setContent(contentComponent);

    this.route(WerewordsContentEnum.NightSecretRole);

  }
  
  ErrorMessage: string;

  constructor(private componentFactoryResolver: ComponentFactoryResolver) {

  }

  route = (status: WerewordsContentEnum) => {
    var content = _.find(RegisteredContent, c => c.Key === status);

    this.setContent(content);
  }

  setContent(werewordsContent: WerewordsContent) {

    const viewContainerRef = this.contentHost.viewContainerRef;
    viewContainerRef.clear();

    if (werewordsContent == null || werewordsContent == undefined) {
      this.ComponentSet = false;
    }
    else {
      const componentFactory = werewordsContent ? this.componentFactoryResolver.resolveComponentFactory(werewordsContent.component) : null;

      if (componentFactory) {

        this.ComponentSet = true;

        const componentRef = viewContainerRef.createComponent(componentFactory);
        (<IWerewordsComponent>componentRef.instance).router = this.route;
      }
    }


  }

  HandleGenericError() {
    this.ErrorMessage = "An Unknown Error Occurred";
  }
}
