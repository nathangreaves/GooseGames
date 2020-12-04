import { LetterJamPlayerStatus } from "../../../models/letterjam/content";
import { LetterJamContent } from "./content";
import { LetterJamLobbyComponent } from "../lobby/lobby.component";
import { LetterJamSubmitWordComponent } from "../submit-word/submit-word.component";
import { LetterJamWaitingForFirstRoundComponent } from "../waiting-for-first-round/waiting-for-first-round.component";
import { LetterJamTableComponent } from "../table/table.component";


export const RegisteredContent: LetterJamContent[] = [
  new LetterJamContent(LetterJamPlayerStatus.InLobby, LetterJamLobbyComponent),
  new LetterJamContent(LetterJamPlayerStatus.ConstructingWord, LetterJamSubmitWordComponent),
  new LetterJamContent(LetterJamPlayerStatus.WaitingForFirstRound, LetterJamWaitingForFirstRoundComponent),
  new LetterJamContent(LetterJamPlayerStatus.ProposingClues, LetterJamTableComponent),
  new LetterJamContent(LetterJamPlayerStatus.ReceivedClue, LetterJamTableComponent),
  new LetterJamContent(LetterJamPlayerStatus.ReadyForNextRound, LetterJamTableComponent)
];
