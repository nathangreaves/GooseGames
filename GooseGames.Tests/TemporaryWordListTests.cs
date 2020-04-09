using FluentAssertions;
using GooseGames.Services.JustOne;
using NUnit.Framework;
using System;
using System.Linq;

namespace GooseGames.Tests
{
    public class TemporaryWordListTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test_WordListContainsNoDuplicates()
        {
            var distinctWords = TemporaryWordsList.Words.Select(w => w.ToLower()).Distinct().Count();

            distinctWords.Should().Be(TemporaryWordsList.Words.Count, BuildDuplicateList());
        }

        private string BuildDuplicateList()
        {
            var distinctWords = TemporaryWordsList.Words.Select(w => w.ToLower()).GroupBy(x => x).Where(x => x.Count() > 1).Select(x => x.Key);
            if (!distinctWords.Any())
            {
                return string.Empty;
            }
            return distinctWords.Aggregate((s, s2) => s + Environment.NewLine + s2);
        }

        [Test]
        public void Test_NumberOfWordsGiven_EqualNumberOfWordsReturned()
        {
            const int numberOfWords = 10;
            var words = TemporaryWordsList.GetWords(numberOfWords);

            words.Count.Should().Be(10);
        }

        [Test]
        public void Test_NumberOfWordsGivenGreaterThanMax_ExceptionThrown()
        {
            ArgumentOutOfRangeException expectedException = null;
            try
            {
                var words = TemporaryWordsList.GetWords(TemporaryWordsList.Words.Count + 1);
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
            var numberOfWords = TemporaryWordsList.Words.Count;
            var words = TemporaryWordsList.GetWords(numberOfWords);

            words.Count.Should().Be(numberOfWords);
        }
    }
}