export class PlayerSecretRoleResponseBase {
  secretRole: SecretRole;
  mayorName: string;
  mayorPlayerId: string;
}


export class PlayerSecretRoleResponse extends PlayerSecretRoleResponseBase {
  knowledgeAboutOtherPlayers: OtherPlayerSecretRoleResponse[];
}

export class OtherPlayerSecretRoleResponse {
  secretRole: SecretRole;
  playerName: string;
  playerId: string;
}

export class SecretWordResponse extends PlayerSecretRoleResponseBase {
  secretWord: string;
}

export class RoundOutcomePlayerInformation {
  name: string;
  id: string;
  secretRole: SecretRole;
  isMayor: boolean;
  wasVoted: boolean;
}

export class PlayerRoundInformation {
  name: string;
  id: string;
  active: boolean;
  responses: PlayerResponse[];
  secretRole: SecretRole;
  isVoted: boolean;
  isMayor: boolean;
  isHidden: boolean;
}

export class PlayerResponse
{
  playerId: string;
  responseType: PlayerResponseType
}

export enum PlayerResponseType {
  Tick = 1,
  Cross = 2,
  QuestionMark = 3,
  SoClose = 4,
  WayOff = 5,
  Correct = 6
}

export enum SecretRole {
  Villager = 1,
  Seer = 2,
  Werewolf = 3
}

export function GetSecretRoleDescription(secretRole: SecretRole): string {
  if (secretRole === SecretRole.Villager) {
    return "Villager";
  }
  else if (secretRole === SecretRole.Seer) {
    return "Seer";
  }
  else if (secretRole === SecretRole.Werewolf) {
    return "Werewolf";
  }
}
