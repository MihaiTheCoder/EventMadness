﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventProcessing.Core.CommandCreation
{
    public interface IoCResolver
    {
        object Get();
    }
}
