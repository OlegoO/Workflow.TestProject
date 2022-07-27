﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Events;

namespace Workflow.TestProject.BusinessForms
{
    public class BusinessFormSubmitedEvent: DomainEvent
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Email { get; set; }

        public string CompanyName { get; set; }

        public string Phone { get; set; }

        public bool TermConditonsAccepted { get; set; }
    }
}
