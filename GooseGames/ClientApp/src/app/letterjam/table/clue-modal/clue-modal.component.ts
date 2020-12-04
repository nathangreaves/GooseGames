import { Component, Input, TemplateRef } from '@angular/core';
import { ILetterJamClueComponentParameters } from '../clue/clue.component';

export interface ILetterJamClueComponentTemplates {
  buttonsTemplate: TemplateRef<any>;
  clueInfoTemplate: TemplateRef<any>;
}

@Component({
  selector: 'letterjam-clue-modal',
  templateUrl: './clue-modal.component.html',
  styleUrls: ['../../common/letterjam.common.scss',
    './clue-modal.component.scss']
})
export class LetterJamProposedClueModalComponent {

  @Input() title: string;
  @Input() parameters: ILetterJamClueComponentParameters;
  @Input() templates: ILetterJamClueComponentTemplates;
  @Input() dismiss: () => void;

  constructor() { }
}
