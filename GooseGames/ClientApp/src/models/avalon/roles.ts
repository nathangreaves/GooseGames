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
  },
  {
    roleEnum: AvalonRoleEnum.AssassinPlus,
    name: "Assassin+",
    description: "Evil steals victory if good win but Assassin can name either Merlin, or 2 other good player's roles, at game end"
  },
  {
    roleEnum: AvalonRoleEnum.Coroner,
    name: "Coroner",
    description: "Once per game, after mission cards are revealed, investigate any 1 mission and find out exactly how many evil players were on it"
  },
  {
    roleEnum: AvalonRoleEnum.Witchdoctor,
    name: "Witchdoctor",
    description: "May play fail cards. If other fail cards are present, the mission instead succeeds. If theirs is the only fail card, the mission fails, even if it’s a mission that would require 2 fail cards"
  },
  {
    roleEnum: AvalonRoleEnum.Instagawain,
    name: "Instagawain",
    description: "Find out the current tally for/against before they cast their vote, which counts as 2 votes"
  },
  {
    roleEnum: AvalonRoleEnum.MerlinsApprentice,
    name: "Merlin's Apprentice",
    description: "At start of the game finds out how many spaces away their nearest evil neighbour is seated. Yvain and/or Mordred appear as evil/good respectively in terms of how many spaces away an evil person is"
  },
  {
    roleEnum: AvalonRoleEnum.Yvain,
    name: "Yvain",
    description: "Appears evil to the evil players, Merlin and Merlin’s apprentice"
  },
  {
    roleEnum: AvalonRoleEnum.Cassia,
    name: "Cassia",
    description: "Chooses 1 player each vote phase to be protected from the Sage. At the start of the game, find out 1 player is truly good"
  },
  {
    roleEnum: AvalonRoleEnum.Guinevere,
    name: "Guinevere",
    description: "At the start of the game, find out 1 player is truly good"
  },
  {
    roleEnum: AvalonRoleEnum.Arthur,
    name: "Arthur",
    description: "Once per game just before a new leader is crowned, choose the next mission leader. Arthur can’t pick themselves. Mission leader proceeds clockwise from the new player"
  },
  {
    roleEnum: AvalonRoleEnum.Gambler,
    name: "Gambler",
    description: "Shares main win condition with the good team, but this character loses the game if the game does not reach the 5th round"
  },
  {
    roleEnum: AvalonRoleEnum.Macy,
    name: "Macy",
    description: "Sees two people and knows at least one is truly good"
  },
  {
    roleEnum: AvalonRoleEnum.Sonny,
    name: "Sonny",
    description: "Sees two people and knows at least one is evil"
  },
  {
    roleEnum: AvalonRoleEnum.Matchmaker,
    name: "Matchmaker",
    description: "Knows that two people are on the same team, but not which team that is"
  },
  {
    roleEnum: AvalonRoleEnum.Cook,
    name: "Cook",
    description: "Knows that two people are on differing teams, but not which is which"
  },
  {
    roleEnum: AvalonRoleEnum.BraveSirRobin,
    name: "Brave Sir Robin",
    description: "Once per round can optionally pick a player who is on a mission. Does not learn which card that person played, but gets to choose to invert it"
  },
  {
    roleEnum: AvalonRoleEnum.SirHector,
    name: "Sir Hector",
    description: "Once per game may change the team proposed by the mission leader after the vote happens by swapping a player out"
  },
  {
    roleEnum: AvalonRoleEnum.Visionary,
    name: "Visionary",
    description: "Once per game can look at the success/fail card played by a person on a mission"
  },
  {
    roleEnum: AvalonRoleEnum.Myopia,
    name: "Myopia",
    description: "After every mission, can learn whether it was as simple as it looked, or whether character abilities affected the outcomes"
  },
  {
    roleEnum: AvalonRoleEnum.Drunk,
    name: "Drunk",
    description: "Believes that they are a different good character that receives information. The information they think they know/receive is actually random nonsense "
  },
  {
    roleEnum: AvalonRoleEnum.Sage,
    name: "Sage",
    description: "During the voting phase, picks a player and an animal. That player can only make that animal's sound until the next mission is voted on (whether successfully or otherwise)"
  },
  {
    roleEnum: AvalonRoleEnum.Karenevere,
    name: "Karenevere",
    description: "Picks a player before voting and finds out how they voted before making their own choice"
  },
  {
    roleEnum: AvalonRoleEnum.VoodooDoodooDoer,
    name: "Voodoo Doodoo Doer",
    description: "Once per game (after mission cards are revealed), pick a player. If that player goes on the following mission, the card they play is automatically replaced with a Fail card. All players find out that someone has been voodone. Cannot use ability for final round"
  },
  {
    roleEnum: AvalonRoleEnum.Armless,
    name: "Armless",
    description: "Shares main win condition with the rest of the evil team but cannot play fail cards"
  },
  {
    roleEnum: AvalonRoleEnum.BlindAndArmless,
    name: "BlindAndArmless",
    description: "Shares main win condition with the rest of the evil team but cannot play fail cards. Also does not know they are Armless"
  },
  {
    roleEnum: AvalonRoleEnum.BraveSirRobin2,
    name: "Brave Sir Robin v2",
    description: "At start of game, sees 1 evil player (or Yvain), who also knows they have been seen by Brave Sir Robin. Once per round can optionally pick a player who is on a mission. Does not learn which card that person played, but gets to choose to invert it"
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
