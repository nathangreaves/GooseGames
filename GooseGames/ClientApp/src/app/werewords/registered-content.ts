import { WerewordsNightSecretRoleComponent } from "../../app/werewords/night/secret-role";
import { WerewordsNightSecretWordComponent } from "../../app/werewords/night/secret-word";
import { WerewordsNightMayorSecretWordComponent } from "../../app/werewords/night/mayor-secret-word";
import { WerewordsNightWakingComponent } from "../../app/werewords/night/waking";
import { WerewordsDayComponent } from "../../app/werewords/day/day";
import { WerewordsDayOutcomeComponent } from "../../app/werewords/day/outcome";
import { WerewordsNewPlayerDetailsComponent } from "../../app/werewords/lobby/newplayer";
import { WerewordsLobbyComponent } from "../../app/werewords/lobby/lobby";
import { WerewordsPlayerStatus } from "../../models/werewords/content";
import { WerewordsContent } from "./scaffolding/content";

export const RegisteredContent: WerewordsContent[] = [
  new WerewordsContent(WerewordsPlayerStatus.New, WerewordsNewPlayerDetailsComponent),
  new WerewordsContent(WerewordsPlayerStatus.InLobby, WerewordsLobbyComponent),

  new WerewordsContent(WerewordsPlayerStatus.NightRevealSecretRole, WerewordsNightSecretRoleComponent),
  new WerewordsContent(WerewordsPlayerStatus.NightWaitingForMayor, WerewordsNightSecretRoleComponent),
  new WerewordsContent(WerewordsPlayerStatus.NightSecretWord, WerewordsNightSecretWordComponent),
  new WerewordsContent(WerewordsPlayerStatus.NightMayorPickSecretWord, WerewordsNightMayorSecretWordComponent),
  new WerewordsContent(WerewordsPlayerStatus.NightWaitingToWake, WerewordsNightWakingComponent),
  new WerewordsContent(WerewordsPlayerStatus.DayActive, WerewordsDayComponent),
  new WerewordsContent(WerewordsPlayerStatus.DayPassive, WerewordsDayComponent),
  new WerewordsContent(WerewordsPlayerStatus.DayMayor, WerewordsDayComponent),
  new WerewordsContent(WerewordsPlayerStatus.DayVotingOnSeer, WerewordsDayComponent),
  new WerewordsContent(WerewordsPlayerStatus.DayVotingOnWerewolves, WerewordsDayComponent),
  new WerewordsContent(WerewordsPlayerStatus.DayOutcome, WerewordsDayOutcomeComponent),
];
