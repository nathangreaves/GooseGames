import { IGooseGamesPlayer } from "../player";
import { AvalonRoleEnum, IAvalonRole } from "./roles";

export interface IAvalonPlayer {
  playerId: string;
  role: IAvalonRole;
  playerIntel: IAvalonPlayerIntel[];
}
export interface IAvalonPlayerIntel {
  intelType: AvalonIntelType;
  intelPlayerId: string | null;
  intelNumber: number | null;
  roleKnowsYou: AvalonRoleEnum | null;
}

export class AvalonPlayer implements IAvalonPlayer {
  playerId: string;
  role: IAvalonRole;
  player: IGooseGamesPlayer;
  playerIntel: IAvalonPlayerIntel[];
}

export class AvalonPlayerIntel implements IAvalonPlayerIntel {
  intelType: AvalonIntelType;
  intelPlayerId: string | null;
  intelPlayer: IGooseGamesPlayer | null;
  intelNumber: number | null;
  roleKnowsYou: AvalonRoleEnum | null;
  roleName: string | null;
}

export enum AvalonIntelType {
  AppearsEvil = 1,
  NumberOfSeats = 2,
  RoleKnowsYou = 3,
  AppearsGood = 4,
  ContextDependant = 5,
  AppearsAsMerlin = 6,
  DefinitelyEvil = 7,
  DefinitelyGood = 8,
}
