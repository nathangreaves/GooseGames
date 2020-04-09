using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Common
{
    public interface IHasGuidId
    {
        Guid Id { get; set; }
    }
}
