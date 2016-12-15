﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models;

namespace TLCGen.Messaging.Messages
{
    public class SelectedConflictGroepChangedMessage
    {
        public RoBuGroverConflictGroepModel OldGroep { get; private set; }
        public RoBuGroverConflictGroepModel NewGroep { get; private set; }

        public SelectedConflictGroepChangedMessage(RoBuGroverConflictGroepModel newgroep, RoBuGroverConflictGroepModel oldgroep)
        {
            OldGroep = oldgroep;
            NewGroep = newgroep;
        }
    }
}