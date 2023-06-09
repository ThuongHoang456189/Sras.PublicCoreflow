using Sras.PublicCoreflow.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Identity;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public interface IPlaceHolderRepository
    {
        string GetDataFromPlaceholder(string placeHolderName, Conference? conference, RecipientInforForEmail recipient, Submission? submission, IdentityUser sender);
    }
}
