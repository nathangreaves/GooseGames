import _ from "lodash";

export enum AvalonRoleEnum {
  LoyalServantOfArthur = -2,
  MinionOfMordred = -1,
  Merlin = 0,
  Percival = 1,
  Assassin = 2,
  Morgana = 3,
  Mordred = 4,
  Oberon = 5,
  AssassinPlus = 6,
  Coroner = 7,
  Witchdoctor = 8,
  Instagawain = 9,
  MerlinsApprentice = 10,
  Yvain = 11,
  Cassia = 12,
  Guinevere = 13,
  Arthur = 14,
  Gambler = 15,
  Macy = 16,
  Sonny = 17,
  Matchmaker = 18,
  Cook = 19,
  BraveSirRobin = 20,
  SirHector = 21,
  Visionary = 22,
  Myopia = 23,
  Drunk = 24,
  Sage = 25,
  Karenevere = 26,
  VoodooDoodooDoer = 27,
  Armless = 28,
  BlindAndArmless = 29,
  BraveSirRobin2 = 30
}

export interface IAvalonRole {
  roleEnum: AvalonRoleEnum;
  roleWeight: number;
  good: boolean;
}


export interface IAvalonRoleDetail {
  roleEnum: AvalonRoleEnum;
  name: string;
  description: string;
}
export class AvalonRole implements IAvalonRole, IAvalonRoleDetail {
  roleEnum: AvalonRoleEnum;
  roleWeight: number;
  good: boolean;
  name: string;
  description: string;
  selected: boolean;
}

const RoleDetails: IAvalonRoleDetail[] = [
  {
    roleEnum: AvalonRoleEnum.LoyalServantOfArthur,
    name: "Loyal Servent of Arthur",
    description: "Knows nothing"
  },
  {
    roleEnum: AvalonRoleEnum.MinionOfMordred,
    name: "Minion of Mordred",
    description: "Knows evil"
  },
  {
    roleEnum: AvalonRoleEnum.Merlin,
    name: "Merlin",
    description: "Knows evil"
  },
  {
    roleEnum: AvalonRoleEnum.Percival,
    name: "Percival",
    description: "Sees Merlin (and possibly Morgana)"
  },
  {
    roleEnum: AvalonRoleEnum.Assassin,
    name: "Assassin",
    description: "Evil steals victory if good win but Assassin can name Merlin at game end"
  },
  {
    roleEnum: AvalonRoleEnum.Morgana,
    name: "Morgana",
    description: "Appears as Merlin to Percival"
  },
  {
    roleEnum: AvalonRoleEnum.Mordred,
    name: "Mordred",
    description: "Unknown to Merlin"
  },
  {
    roleEnum: AvalonRoleEnum.Oberon,
    name: "Oberon",
    description: "Unknown to evil, knows no evil"
  }
];

export const GetAvalonRoleDetail = (role: AvalonRoleEnum) => {
  var result = _.find(RoleDetails, r => r.roleEnum === role);

  return result ?? {
    roleEnum: role,
    name: AvalonRoleEnum[role],
    description: "error not found"
  };
}
