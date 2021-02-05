import { Component, OnInit, Input } from '@angular/core';
import { AvalonRole } from '../../../models/avalon/roles';


export interface IAvalonRoleInfoComponentParameters {
  //request: IPlayerSessionGame
  //getPlayersFromCache: (request: IGetPlayersFromCacheRequest) => Promise<IGooseGamesPlayer[]>;
  //getCardsFromCache: (request: ICardsRequest) => Promise<ILetterCard[]>;
  //clue: IClue;
  //highlightColour: string;
  //showEmojis: boolean;
  role: AvalonRole;
  onClick: (role: AvalonRole) => void;
  selectable: boolean;
  showDescription: boolean;
  showDrunkViability: boolean;
  showMyopiaViability: boolean;
}

@Component({
  selector: 'avalon-role-info',
  templateUrl: './role-info.component.html',
  styleUrls: ['./role-info.component.scss']
})
export class AvalonRoleInfoComponent implements OnInit {

  @Input() parameters: IAvalonRoleInfoComponentParameters;

  constructor() { }

  ngOnInit(): void {
  }

  onClick = (event: any) => {
    if (this.parameters.selectable) {
      this.parameters.onClick(this.parameters.role);
    }
  }
}
