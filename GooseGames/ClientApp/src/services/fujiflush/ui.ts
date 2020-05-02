class CardNumberCssResolver {
  ResolveCardClass(cardNumber: number) {

    return `fuji-card-${cardNumber}`;
  }

  ResolveCardClass2(cardNumber: number) {

    return `fuji-card-${cardNumber} : true`;
  }
}

export const CardNumberCss = new CardNumberCssResolver();
