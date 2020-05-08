export enum JustOneWordList {
  JustOne = 1,
  Codenames = 2,
  CodenamesDuet = 3,
  CodenamesDeepUndercover = 4
}


export class WordListCheckboxListItem {
  Name: string;
  Checked: boolean;
  WordList: JustOneWordList;
}
