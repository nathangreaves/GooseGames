import { Component, Input, OnInit } from "@angular/core";

export interface ITristateSwitchHandler {  
  SwitchOn(): void;
  SwitchOff(): void;
  SwitchUnselected(): void;


  ReadOnly: boolean;
  DefaultState: boolean;

  AllowUnselected: boolean;

  GroupName: string;
}

@Component({
  selector: 'tristate-switch',
  templateUrl: './tristate-switch.component.html',
  styleUrls: ['./tristate-switch.component.css']
})
export class TristateSwitch implements OnInit {

  @Input() TristateSwitchHandler: ITristateSwitchHandler;
  ReadOnly: boolean;
  DefaultState: boolean;
  AllowUnselected: boolean;

  Checked: boolean;
    groupId: string;

  static groupIdIncrement = 1;

  ngOnInit(): void {

    this.ReadOnly = this.TristateSwitchHandler.ReadOnly;
    this.DefaultState = this.TristateSwitchHandler.DefaultState;
    this.AllowUnselected = this.TristateSwitchHandler.AllowUnselected;

    this.Checked = this.DefaultState;    

    this.groupId = this.TristateSwitchHandler.GroupName + TristateSwitch.groupIdIncrement++;
  }

  OnClicked() {
    this.TristateSwitchHandler.SwitchOn();
    this.Checked = true;
  }

  OffClicked() {
    this.TristateSwitchHandler.SwitchOff();
    this.Checked = false;
  }

  UnselectedClicked() {
    this.TristateSwitchHandler.SwitchUnselected();
    this.Checked = undefined;
  }
}

//[LabelOn] = "Valid"
//[LabelOff] = "Invalid"
//[UnselectedLabel] = ""
//[ReadOnly] = "clue.responseAutoInvalid"
//[DefaultState] = "clue.responseAutoInvalid ? false : undefined"
