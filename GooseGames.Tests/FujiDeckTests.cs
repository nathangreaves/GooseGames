using FluentAssertions;
using GooseGames.Services.Fuji;
using NUnit.Framework;
using System;
using System.Linq;

namespace GooseGames.Tests
{
    [TestFixture]
    public class FujiDeckTests
    {
        [Test]

        public void CreateDeck_90CardsTotal()
        {
            var sessionId = Guid.NewGuid();

            var deck = DeckService.CreateNewDeck(sessionId);

            deck.Count().Should().Be(90);
        }

        [Test]

        public void CreateDeck_QuantitiesCorrect()
        {
            var sessionId = Guid.NewGuid();

            var deck = DeckService.CreateNewDeck(sessionId);

            var dist = DeckService.DeckDistributions;

            foreach (var d in dist)
            {
                deck.Where(card => card.FaceValue == d.Key).Count().Should().Be(d.Value);
            }
        }


    }
}
