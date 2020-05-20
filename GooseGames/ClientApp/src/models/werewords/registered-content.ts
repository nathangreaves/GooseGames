import { WerewordsContent, WerewordsContentEnum } from "./content";
import { WerewordsNightSecretRoleComponent } from "../../app/werewords/night-secret-role.component";
import { WerewordsNightSecretWordComponent } from "../../app/werewords/night-secret-word.component";

export const RegisteredContent: WerewordsContent[] = [
  new WerewordsContent(WerewordsContentEnum.NightSecretRole, WerewordsNightSecretRoleComponent, null),
  new WerewordsContent(WerewordsContentEnum.NightSecretWord, WerewordsNightSecretWordComponent, null),
];
