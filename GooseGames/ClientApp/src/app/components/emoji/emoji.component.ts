import { Component, OnInit, Input } from '@angular/core';
import 'twemoji';

declare var twemoji: any;

@Component({
  selector: 'gg-emoji',
  templateUrl: './emoji.component.html',
  styleUrls: ['./emoji.component.scss']
})
export class EmojiComponent implements OnInit {

  @Input() emoji: string;

  constructor() { }

  parseEmoji(emoji: string) {
    if (emoji) {
      return twemoji.parse(emoji);
    }
    return "&nbsp;";
  }

  ngOnInit(): void {
  }
}
