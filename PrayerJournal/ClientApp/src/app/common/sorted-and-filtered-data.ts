
export class SortedAndFilteredData<T> {
  private _list: T[];
  private _data: T[];
  private _filterStrings: string[];
  private _sortByKeys: string[];
  private _filterKeys: string[];

  constructor(data: T[] = [], filterStrings: string[] = [], filterKeys: string[] = [], sortByKeys: string[] = []) {
    this._data = data;
    this._filterStrings = filterStrings;
    this._sortByKeys = sortByKeys;
    this._filterKeys = filterKeys;
  }

  get data() {
    return this._data;
  }

  set data(data: T[]) {
    this._data = data;
    this.refreshList();
  }

  set filterStrings(filterStrings: string[]) {
    this._filterStrings = filterStrings;
    this.refreshList();
  }

  set filterKeys(keys: string[]) {
    this._filterKeys = keys;
    this.refreshList();
  }

  set sortByKeys(keys: string[]) {
    console.log(keys);
    this._sortByKeys = keys;
    this.refreshList();
  }

  get list() {
    return this._list;
  }

  remove(item: T): number {
    let dataIndex = this._data.indexOf(item);
    let listIndex = this._list.indexOf(item);

    this._data.splice(dataIndex, 1);
    this._list.splice(listIndex, 1);

    return listIndex;
  }

  add(item: T, listIndex: number = -1) {
    this._data.push(item);

    if (listIndex === -1 || listIndex > this._list.length)
      this.refreshList();
    else
      this._list.splice(listIndex, 0, item);
  }

  private refreshList(){
    let tempList: T[] = [];

    tempList = this.doFilter(); 

    this._list = this.doSort(tempList);
  }

  private doFilter(): T[] {
    let list = this._data.slice(0);

    for(let filterText of this._filterStrings)
      list = list.filter(item => {
        if (!filterText)
          return true;

        for (let filterKey of this._filterKeys)
          if (this.stringContains((item[filterKey] as unknown) as string, filterText))
            return true;

        return false;
      });
    return list;
  }

  private doSort(list: T[]) {
    if (this._sortByKeys.length > 0)
      list = list.sort((itemA, itemB) => {
        let i = 0
        let returnValue = 0
        do {
          let key = this._sortByKeys[i];
          let aString = (itemA[key]) ? itemA[key] : "";
          let bString = (itemB[key]) ? itemB[key] : "";

          if (aString > bString)
            returnValue = 1;
          if (aString < bString)
            returnValue = -1;
          i++

        } while (returnValue === 0 && i < this._sortByKeys.length)
        return returnValue;
      });
    return list;
  }

  private stringContains(text: string, search: string) {
    if (text && search)
      return text.toUpperCase().indexOf(search.toUpperCase()) !== -1;

    return false;
  }
}
