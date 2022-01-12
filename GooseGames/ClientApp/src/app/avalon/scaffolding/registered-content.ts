import { AvalonPlayerStatus } from "../../../models/avalon/content";
import { AvalonContent } from "./content";
import { AvalonLobbyComponent } from "../lobby/lobby.component";
import { AvalonTableComponent } from "../table/table.component";


export const RegisteredContent: AvalonContent[] = [
  new AvalonContent(AvalonPlayerStatus.InLobby, AvalonLobbyComponent),
  new AvalonContent(AvalonPlayerStatus.InGame, AvalonTableComponent),
];
