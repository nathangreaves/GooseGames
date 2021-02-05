using Enums.Avalon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Models.Avalon.Roles.Types
{
    public class MerlinsApprentice : GoodRoleBase
    {
        public override GameRoleEnum RoleEnum => GameRoleEnum.MerlinsApprentice;
        public override bool ViableForDrunkToMimic => true;

        //TODO: Defo some unit tests for this.
        public override List<PlayerIntel> GeneratePlayerIntel(Player currentPlayer, List<Player> players, List<AvalonRoleBase> allRoles)
        {

            var mySeatNumber = players.Single(x => x.PlayerId == currentPlayer.PlayerId).SeatNumber;

            var nearestClockwiseEvilPlayer = players.Where(x => x.SeatNumber > mySeatNumber).OrderBy(x => x.SeatNumber).ThenBy(x => x.ActualRole.AppearsEvilToMerlin).FirstOrDefault()
                ?? players.Where(x => x.SeatNumber < mySeatNumber).OrderBy(x => x.SeatNumber).ThenBy(x => x.ActualRole.AppearsEvilToMerlin).FirstOrDefault();

            var nearestAntiClockwiseEvilPlayer = players.Where(x => x.SeatNumber < mySeatNumber).OrderByDescending(x => x.SeatNumber).ThenBy(x => x.ActualRole.AppearsEvilToMerlin).FirstOrDefault()
                ?? players.Where(x => x.SeatNumber > mySeatNumber).OrderByDescending(x => x.SeatNumber).ThenBy(x => x.ActualRole.AppearsEvilToMerlin).FirstOrDefault();

            var nearestClockwiseEvilPlayerSeatNumber = nearestClockwiseEvilPlayer.SeatNumber;
            var nearestClockwiseNumberOfSeats = nearestClockwiseEvilPlayer.SeatNumber > mySeatNumber ? nearestClockwiseEvilPlayer.SeatNumber - mySeatNumber : (nearestClockwiseEvilPlayer.SeatNumber + players.Count) - mySeatNumber;

            var nearestAntiClockwiseEvilPlayerSeatNumber = nearestAntiClockwiseEvilPlayer.SeatNumber;
            var nearestAntiClockwiseNumberOfSeats = nearestClockwiseEvilPlayer.SeatNumber < mySeatNumber ? mySeatNumber - nearestClockwiseEvilPlayer.SeatNumber : (mySeatNumber + players.Count) - nearestClockwiseEvilPlayer.SeatNumber;

            return new List<PlayerIntel>
            {
                new PlayerIntel
                {
                    IntelNumber = Math.Min(nearestClockwiseNumberOfSeats, nearestAntiClockwiseNumberOfSeats),
                    IntelType = IntelTypeEnum.NumberOfSeats,
                    PlayerId = currentPlayer.PlayerId
                }
            };
        }


        internal override List<PlayerIntel> GenerateDrunkIntel(Player currentPlayer, List<Player> players, List<AvalonRoleBase> allRoles)
        {
            var numberOfGoodPlayers = players.Where(x => x.ActualRole is GoodRoleBase).Count();
            var upperBound = 3;
            if (numberOfGoodPlayers <= 4)
            {
                upperBound = 3;
            }
            else if (numberOfGoodPlayers <= 6)
            {
                upperBound = 4;
            } 
            else if (numberOfGoodPlayers > 6)
            {
                upperBound = 5;
            }

            return new List<PlayerIntel>
            {
                new PlayerIntel
                {
                    IntelNumber = s_Random.Next(1, upperBound),
                    IntelType = IntelTypeEnum.NumberOfSeats,
                    PlayerId = currentPlayer.PlayerId
                }
            };
        }
        public override short GetRoleWeight(int numberOfPlayers, IEnumerable<AvalonRoleBase> rolesInPlay, IEnumerable<AvalonRoleBase> allRoles)
        {
            var yvainInPlay = allRoles.Any(x => x is Yvain);

            short weight = 1;

            if (yvainInPlay)
            {
                weight -= 1; //Knowledge is incomplete
            }

            if (numberOfPlayers == 5 || numberOfPlayers == 6)
            {
                weight += 2;
            }
            if (numberOfPlayers == 7 || numberOfPlayers == 8 || numberOfPlayers == 9)
            {
                weight += 1;
            }
            return weight;
        }
    }
}
