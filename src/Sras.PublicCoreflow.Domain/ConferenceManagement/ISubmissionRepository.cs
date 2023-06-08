﻿using Sras.PublicCoreflow.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public interface ISubmissionRepository
    {
        Task<object> GetNumberOfSubmission(Guid trackId);
        Task<object> GetNumOfSubmissionAndEmailWithAllAuthor(SubmissionWithEmailRequest request);
        Task<object> GetNumOfSubmissionAndEmailWithPrimaryContactAuthor(SubmissionWithEmailRequest request);
        Task<IEnumerable<object>> GetSubmissionAsync();
    }
}