using Enums.Avalon;
using Models.Avalon.Roles;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Avalon
{
    public class Player
    {
        public Guid PlayerId { get; set; }
        public GameRoleEnum ActualRoleEnum { get; set; }
        public GameRoleEnum AssumedRoleEnum { get; set; }
        public AvalonRoleBase ActualRole { get; set; }
        public AvalonRoleBase AssumedRole { get; set; }
        public int SeatNumber { get; set; }
    }
}
