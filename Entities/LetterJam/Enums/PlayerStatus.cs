using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.LetterJam.Enums
{
    public class PlayerStatusId
    {
        public Guid Id { get; set; }

        internal static PlayerStatusId Construct(string guid)
        {
            return new PlayerStatusId
            {
                Id = new Guid(guid)
            };
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
            //       
            // See the full list of guidelines at
            //   http://go.microsoft.com/fwlink/?LinkID=85237  
            // and also the guidance for operator== at
            //   http://go.microsoft.com/fwlink/?LinkId=85238
            //

            if (obj == null)
            {
                return false;
            }

            var objAsType = obj as PlayerStatusId;
            if (objAsType == null)
            {
                return false;
            }

            return Equals(Id, objAsType.Id);
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

    }

    public static class PlayerStatus
    {

        public static readonly PlayerStatusId InLobby = PlayerStatusId.Construct("4744BE42-71B4-4027-A36B-013DC1979773");
        public static readonly PlayerStatusId ConstructingWord = PlayerStatusId.Construct("A0465731-9BD5-4042-A19A-304F987AEBDB");
        public static readonly PlayerStatusId WaitingForFirstRound = PlayerStatusId.Construct("28A8674D-025E-484B-A5D0-58ED0AEBC923");
        public static readonly PlayerStatusId ProposingClues = PlayerStatusId.Construct("F56B427C-4357-4D41-BBFB-BA2C6A5182A5");

        private static readonly Dictionary<PlayerStatusId, string> s_Instances = new Dictionary<PlayerStatusId, string>
        {
            { InLobby, nameof(InLobby) },
            { ConstructingWord, nameof(ConstructingWord) },
            { WaitingForFirstRound, nameof(WaitingForFirstRound) },
            { ProposingClues, nameof(ProposingClues) },
        };
        public static string GetDescription(PlayerStatusId guid)
        {
            return s_Instances[guid];
        }

        public static string TryGetDescription(PlayerStatusId guid)
        {
            try
            {
                return GetDescription(guid);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }

//
//
//
//
//PlayerStatusId.Construct("B3A55203-5F59-4C4F-8830-7244974727BF");
//PlayerStatusId.Construct("76B04091-EF39-4339-8C92-7C04A87249FF");
//PlayerStatusId.Construct("31AABF51-845B-458D-9FD6-DFCE674F6BEF");
//PlayerStatusId.Construct("94832875-2F14-48F5-8943-D09348CBF1E6");
//PlayerStatusId.Construct("3E5C01C8-312B-4C84-8DB2-3E2A218C8325");
//PlayerStatusId.Construct("F7222075-AD4D-465F-9A08-787CD08C84E1");
//PlayerStatusId.Construct("DFE3F08F-CB19-42B8-AC87-C34995DA4F51");
//PlayerStatusId.Construct("B9273966-6AE4-44FC-A85B-8EFE58AD85D1");
//PlayerStatusId.Construct("296C2D10-E1DF-463D-961E-89DFD1D01856");
//PlayerStatusId.Construct("DDFFB2E1-D493-40DE-AC86-731D9EB38E0C");
//PlayerStatusId.Construct("5C10B26F-2B70-4719-BDFF-D544342FCB99");
//PlayerStatusId.Construct("743BDB16-A5A6-4E88-8FD9-0E51335B5B6D");
//PlayerStatusId.Construct("A76888EC-DF57-40A4-9205-6389B556BC2A");
//PlayerStatusId.Construct("B104B1A4-9E1E-466A-9B80-008DEB5D8664");
//PlayerStatusId.Construct("C0FBCE81-207F-4DE6-A953-1D9F66AA9279");
//PlayerStatusId.Construct("9566FE53-76E7-4F81-A295-9052D7C03CA8");
}
