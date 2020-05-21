import { WerewordsContent, WerewordsPlayerStatus } from "./content";
import { WerewordsNightSecretRoleComponent } from "../../app/werewords/night/night-secret-role.component";
import { WerewordsNightSecretWordComponent } from "../../app/werewords/night/night-secret-word.component";
import { WerewordsNightMayorSecretWordComponent } from "../../app/werewords/night/mayor-secret-word";

export const RegisteredContent: WerewordsContent[] = [
  new WerewordsContent(WerewordsPlayerStatus.NightRevealSecretRole, WerewordsNightSecretRoleComponent),
  new WerewordsContent(WerewordsPlayerStatus.NightWaitingForMayor, WerewordsNightSecretRoleComponent),
  new WerewordsContent(WerewordsPlayerStatus.NightSecretWord, WerewordsNightSecretWordComponent),
  new WerewordsContent(WerewordsPlayerStatus.NightMayorPickSecretWord, WerewordsNightMayorSecretWordComponent),
];
