using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Common
{
    public interface IHasLastUpdatedUtc
    {
        DateTime LastUpdatedUtc { get; set; }
    }
}
