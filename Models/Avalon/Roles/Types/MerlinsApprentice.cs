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

        //TODO: Defo some unit tests for this.
        public override List<PlayerIntel> GeneratePlayerIntel(Guid currentPlayerId, List<Player> players)
        {

            var mySeatNumber = players.Single(x => x.PlayerId == currentPlayerId).SeatNumber;

            var nearestClockwiseEvilPlayer = players.Where(x => x.SeatNumber > mySeatNumber).OrderBy(x => x.SeatNumber).ThenBy(x => x.Role.AppearsEvilToMerlin).FirstOrDefault()
                ?? players.Where(x => x.SeatNumber < mySeatNumber).OrderBy(x => x.SeatNumber).ThenBy(x => x.Role.AppearsEvilToMerlin).FirstOrDefault();

            var nearestAntiClockwiseEvilPlayer = players.Where(x => x.SeatNumber < mySeatNumber).OrderByDescending(x => x.SeatNumber).ThenBy(x => x.Role.AppearsEvilToMerlin).FirstOrDefault()
                ?? players.Where(x => x.SeatNumber > mySeatNumber).OrderByDescending(x => x.SeatNumber).ThenBy(x => x.Role.AppearsEvilToMerlin).FirstOrDefault();

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
                    PlayerId = currentPlayerId
                }
            };
        }

        public override short GetRoleWeight(int numberOfPlayers)
        {
            if (numberOfPlayers == 5 || numberOfPlayers == 6)
            {
                return 3;
            }
            if (numberOfPlayers == 7 || numberOfPlayers == 8 || numberOfPlayers == 9)
            {
                return 2;
            }
            return 1;
        }
    }
}
