﻿using Adis.Dm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Bll.Interfaces
{
    public interface IWorkObjectSectionService
    {
        public Task<WorkObjectSection> AddWorkObjectSectionAsync(WorkObjectSection workObjectSection);
    }
}
