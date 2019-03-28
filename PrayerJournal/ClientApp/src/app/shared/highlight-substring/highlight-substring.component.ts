import { Component, OnInit, Input } from '@angular/core';

interface Terminus {
  index: number,
  isStart: boolean
}

interface StringSegment {
  content: string,
  isHighlighted: boolean
}

@Component({
  selector: 'app-highlight-substring',
  templateUrl: './highlight-substring.component.html',
  styleUrls: ['./highlight-substring.component.scss']
})
export class HighlightSubstringComponent implements OnInit {
  @Input() content: string = "";
  @Input() substrings: string[] = [];

  constructor() { }

  ngOnInit() {
  }

  get separatedString(): StringSegment[] {
    let highlightTermini: Terminus[] = [];

    if (this.content === null || this.content === undefined || this.content === "") {
      return [];
    }

    this.substrings.forEach(s => {
      if (s.length > 0)
        this.getHighlightTermini(this.content, s)
          .forEach(t => highlightTermini.push(t))
    });

    let sortedTermini = highlightTermini.sort((a, b) => a.index - b.index);

    let culledTermini = this.cullTermini(sortedTermini);

    return this.separateString(this.content, culledTermini);
  }

  private getHighlightTermini(content: string, searchString: string): Terminus[] {
    let searchedTo = 0;
    let termini: Terminus[] = [];

    do {
      let start = content.toUpperCase().indexOf(searchString.toUpperCase(), searchedTo);
      if (start >= 0) {
        termini.push({ index: start, isStart: true });
        termini.push({ index: start + searchString.length, isStart: false });
        searchedTo += searchString.length;
      }
      else
        return termini;
    } while (true);
  }

  cullTermini(termini: Terminus[]): Terminus[] {
    let culledTermini: Terminus[] = [];
    let i = 0;

    if (termini.length > 0) {
      do {
        culledTermini.push(termini[i]);
        do {
          ++i;
        } while (termini[i].isStart);
        do {
          ++i;
        } while (i < termini.length ? !termini[i].isStart : false)
        culledTermini.push(termini[i - 1])
      } while (i < termini.length);
    }

    return culledTermini;
  }

  separateString(content: string, culledTermini: Terminus[]): any {
    let separatedString: StringSegment[] = [];

    if (culledTermini.length === 0) {
      separatedString.push({ content: content, isHighlighted: false });
      return separatedString;
    }

    if (culledTermini[0].index > 0)
      separatedString.push({ content: content.substr(0, culledTermini[0].index), isHighlighted: false });

    let i = 0;
    let isHighlighted = true;

    for (i = 0; i < culledTermini.length; i++) {
      if (i < culledTermini.length - 1)
        separatedString.push({ content: content.substring(culledTermini[i].index, culledTermini[i + 1].index), isHighlighted: isHighlighted });
      else
        separatedString.push({ content: content.substring(culledTermini[i].index), isHighlighted: isHighlighted });

      isHighlighted = !isHighlighted;
    }

    if (separatedString[i - 1].content.length === 0)
      separatedString.pop();

    return separatedString;
  }
}
