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
        public GameRoleEnum RoleEnum { get; set; }
        public GameRoleEnum? DrunkRoleEnum { get; set; }
        public AvalonRoleBase Role { get; set; }
        public int SeatNumber { get; set; }
    }
}
