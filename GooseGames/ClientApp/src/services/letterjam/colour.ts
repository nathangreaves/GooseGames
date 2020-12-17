export const GetColourFromLetterIndex = (letterIndex: number) => {
  switch (letterIndex) {
    case 1:
      return "#f03320";
    case 2:
      return "#eba640";
    case 3:
      return "#f3e13c";
    case 4:
      return "#95df1b";
    case 5:
      return "#169b1b";
    case 6:
      return "#0264b4";
    case 7:
      return "#9a0f6e";
    case 8:
      return "#302834";
    default:
      return null;
  }
}

export const StyleLetterCardWithColour = (colour: string) => {
  if (colour) {
    return {
      'border-color': colour,
      'box-shadow': `1px 1px 2px 0px ${colour}`
    }
  }
  return {};
}
