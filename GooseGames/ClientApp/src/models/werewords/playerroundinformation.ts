export class PlayerSecretRoleResponse {
  secretRole: SecretRole;
  mayorName: string;
  mayorPlayerId: string;
  knowledgeAboutOtherPlayers: OtherPlayerSecretRoleResponse[];
}

export class OtherPlayerSecretRoleResponse {
  secretRole: SecretRole;
  playerName: string;
  playerId: string;
}


export enum SecretRole {
  Villager = 1,
  Seer = 2,
  Werewolf = 3
}
