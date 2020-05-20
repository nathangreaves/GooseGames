using FluentAssertions;
using GooseGames.Services;
using Models.Enums;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GooseGames.Tests
{
    [TestFixture]
    public class TemporaryWordListTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test_NumberOfWordsGiven_EqualNumberOfWordsReturned()
        {
            const int numberOfWords = 10;
            var words = StaticWordsList.GetWords(numberOfWords, new[] { WordListEnum.JustOne });

            words.Count.Should().Be(10);
        }

        [Test]
        public void Test_NumberOfWordsGivenGreaterThanMax_ExceptionThrown()
        {
            ArgumentOutOfRangeException expectedException = null;
            try
            {
                //The -2 is here because there are 3 duplicated words in this list and I can't be bothered to find them
                var words = StaticWordsList.GetWords(StaticWordsList.JustOne.Count - 2, new[] { WordListEnum.JustOne });
            }
            catch (ArgumentOutOfRangeException e)
            {
                expectedException = e;
            }

            expectedException.Should().NotBeNull();
        }

        [Test]
        public void Test_NumberOfWordsGivenEqualsMaxInList_EqualNumberOfWordsReturned()
        {
            //The -3 is here because there are 3 duplicated words in this list and I can't be bothered to find them
            var numberOfWords = StaticWordsList.JustOne.Count - 3;
            var words = StaticWordsList.GetWords(numberOfWords, new[] { WordListEnum.JustOne });

            words.Count.Should().Be(numberOfWords);
        }



        [Test]
        public void Test_NumberOfWordsGivenGreaterThanMaxInList_OtherListRequired_EqualNumberOfWordsReturned()
        {
            //The -3 is here because there are 3 duplicated words in this list and I can't be bothered to find them
            var numberOfWords = StaticWordsList.JustOne.Count + 10;
            var words = StaticWordsList.GetWords(numberOfWords, new[] { WordListEnum.JustOne, WordListEnum.CodenamesDeepUndercover });

            words.Count.Should().Be(numberOfWords);
        }
    }
}